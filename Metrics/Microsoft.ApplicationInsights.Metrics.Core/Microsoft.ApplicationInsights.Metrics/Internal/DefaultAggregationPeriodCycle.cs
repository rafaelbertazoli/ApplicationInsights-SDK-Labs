﻿using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.ApplicationInsights.Metrics.Extensibility;

namespace Microsoft.ApplicationInsights.Metrics
{
    internal class DefaultAggregationPeriodCycle
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming Rules", "SA1310: C# Field must not contain an underscore", Justification = "By design: Structured name.")]
        private const int RunningState_NotStarted = 0;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming Rules", "SA1310: C# Field must not contain an underscore", Justification = "By design: Structured name.")]
        private const int RunningState_Running = 1;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming Rules", "SA1310: C# Field must not contain an underscore", Justification = "By design: Structured name.")]
        private const int RunningState_Stopped = 2;

        private readonly Action _workerMethod;

        private readonly MetricAggregationManager _aggregationManager;
        private readonly MetricManager _metricManager;

        private int _runningState;
        private Task _workerTask;

        public DefaultAggregationPeriodCycle(MetricAggregationManager aggregationManager, MetricManager metricManager)
        {
            Util.ValidateNotNull(aggregationManager, nameof(aggregationManager));
            Util.ValidateNotNull(metricManager, nameof(metricManager));

            _workerMethod = this.Run;

            _aggregationManager = aggregationManager;
            _metricManager = metricManager;

            _runningState = RunningState_NotStarted;
            _workerTask = null;
        }

        ~DefaultAggregationPeriodCycle()
        {
            Task fireAndForget = StopAsync();
        }

        public bool Start()
        {
            int prev = Interlocked.CompareExchange(ref _runningState, RunningState_Running, RunningState_NotStarted);

            if (prev != RunningState_NotStarted)
            {
                return false; // Was already running or stopped.
            }

            _workerTask = Task.Run(_workerMethod)
                              .ContinueWith(
                                        (t) => { _workerTask = null; },
                                        TaskContinuationOptions.ExecuteSynchronously);
            return true;
        }

        public Task StopAsync()
        {
            Interlocked.Exchange(ref _runningState, RunningState_Stopped);
            
            // Benign race on being called very soon after start. Will miss a cycle but eventually complete correctly.

            Task workerTask = _workerTask;
            return workerTask ?? Task.FromResult(true);
        }

        public void FetchAndTrackMetrics()
        {
            // We know that GetNextCycleTargetTime(..) tries to snap cycles to 1 second into each minute.
            // But the timer wakes us up *approxumately* at that time. If we are within a few seconds of that time, we will snap exactly to that time.
            // If we are further away, we will just snap to a whole second. That way downstream systems do not need to worry about sub-second resolution.

            DateTimeOffset now = DateTimeOffset.Now;

            if (new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, now.Offset) <= now
                    && new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute, 4, now.Offset) >= now)
            {
                now = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute, 1, now.Offset);
            }
            else
            {
                now = Util.RoundDownToSecond(now);
            }

            AggregationPeriodSummary aggregates = _aggregationManager.StartOrCycleAggregators(
                                                                            MetricAggregationCycleKind.Default,
                                                                            futureFilter: null,
                                                                            tactTimestamp: now);
            if (aggregates != null)
            {
                Task fireAndForget = Task.Run( () => _metricManager.TrackMetricAggregates(aggregates) );
            }
        }

        /// <summary>
        /// We use exactly one background thread for completing aggregators - either once per minute or once per second.
        /// We start this thread right when this manager is created to avoid that potential thread starvation on busy systems affects metrics.
        /// </summary>
        private void Run()
        {
            while (true)
            {
                DateTimeOffset now = DateTimeOffset.Now;
                TimeSpan waitPeriod = GetNextCycleTargetTime(now) - now;

                //Thread.Sleep(waitPeriod);
                Task.Delay(waitPeriod).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter().GetResult();

                int shouldBeRunning = Volatile.Read(ref _runningState);
                if (shouldBeRunning != RunningState_Running)
                {
                    return;
                }

                FetchAndTrackMetrics();
            }
        }

        private static DateTimeOffset GetNextCycleTargetTime(DateTimeOffset periodStart)
        {
            // Next tick: (current time rounded down to MINUTE start) + (1 minute) + (small sub-minute offset).
            // The strategy here is to always "tick" at the same offset within a minute.
            // Due to drift and inprecize timing this may conflict with the aggregation being exactly a minute.
            // In such cases we err on the side of keeping the same offset.
            // This will tend to straighten out the inmterval and to yield consistent timestamps.

            const int targetOffsetFromRebasedCurrentTimeInSecs = (60) + 1;
            const double minPeriodInSecs = 20.0 + 1.0;

            DateTimeOffset target = Util.RoundDownToMinute(periodStart).AddSeconds(targetOffsetFromRebasedCurrentTimeInSecs);

            // If this results in the next period being unreasonably short, we extend that period by 1 minute,
            // resulting in a total period that is somewhat longer than a minute.

            TimeSpan waitPeriod = target - periodStart;
            if (waitPeriod.TotalSeconds < minPeriodInSecs)
            {
                target = target.AddMinutes(1);
            }

            return target;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming Rules", "SA1310: C# Field must not contain an underscore", Justification = "By design: Structured name.")]
        internal static DateTimeOffset GetNextCycleTargetTime_UnitTestAccessor(DateTimeOffset periodStart)
        {
            return GetNextCycleTargetTime(periodStart);
        }
    }
}