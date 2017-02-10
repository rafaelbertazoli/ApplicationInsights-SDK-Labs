﻿using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

namespace Microsoft.ApplicationInsights.Wcf.Implementation
{
    internal abstract class ClientTelemetryChannelBase
    {
        protected IChannel InnerChannel { get; private set; }
        private IChannelManager channelManager;

        public CommunicationState State
        {
            get { return InnerChannel.State; }
        }

        public abstract EndpointAddress RemoteAddress { get; }

        public event EventHandler Closed;
        public event EventHandler Closing;
        public event EventHandler Faulted;
        public event EventHandler Opened;
        public event EventHandler Opening;

        public ClientTelemetryChannelBase(IChannelManager channelManager, IChannel channel)
        {
            if ( channelManager == null )
            {
                throw new ArgumentNullException(nameof(channelManager));
            }
            if ( channel == null )
            {
                throw new ArgumentNullException(nameof(channel));
            }
            this.channelManager = channelManager;
            this.InnerChannel = channel;
        }

        public void Open()
        {
            HookChannelEvents();
            var telemetry = this.StartOpenTelemetry(nameof(Open));
            try
            {
                InnerChannel.Open();
                this.StopOpenTelemetry(telemetry, null, nameof(Open));
            } catch ( Exception ex )
            {
                this.StopOpenTelemetry(telemetry, ex, nameof(Open));
                throw;
            }
        }

        public void Open(TimeSpan timeout)
        {
            HookChannelEvents();
            var telemetry = this.StartOpenTelemetry(nameof(Open));
            try
            {
                InnerChannel.Open(timeout);
                this.StopOpenTelemetry(telemetry, null, nameof(Open));
            } catch ( Exception ex )
            {
                this.StopOpenTelemetry(telemetry, ex, nameof(Open));
                throw;
            }
        }

        public void Close()
        {
            try
            {
                InnerChannel.Close();
            } finally
            {
                UnhookChannelEvents();
                OnClose();
            }
        }

        public void Close(TimeSpan timeout)
        {
            try
            {
                InnerChannel.Close(timeout);
            } finally
            {
                UnhookChannelEvents();
                OnClose();
            }
        }

        public void Abort()
        {
            try
            {
                InnerChannel.Abort();
            } finally
            {
                UnhookChannelEvents();
                OnClose();
            }
        }

        public T GetProperty<T>() where T : class
        {
            return InnerChannel.GetProperty<T>();
        }

        //
        // Async Methods
        //
        public IAsyncResult BeginOpen(AsyncCallback callback, object state)
        {
            HookChannelEvents();
            var telemetry = StartOpenTelemetry(nameof(BeginOpen));
            try
            {
                var result = InnerChannel.BeginOpen(callback, state);
                return new NestedAsyncResult(result, telemetry);
            } catch ( Exception ex )
            {
                StopOpenTelemetry(telemetry, ex, nameof(BeginOpen));
                throw;
            }
        }

        public IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            HookChannelEvents();
            var telemetry = StartOpenTelemetry(nameof(BeginOpen));
            try
            {
                var result = InnerChannel.BeginOpen(timeout, callback, state);
                return new NestedAsyncResult(result, telemetry);
            } catch ( Exception ex )
            {
                StopOpenTelemetry(telemetry, ex, nameof(BeginOpen));
                throw;
            }
        }

        public void EndOpen(IAsyncResult result)
        {
            NestedAsyncResult nar = (NestedAsyncResult)result;
            try
            {
                InnerChannel.EndOpen(nar.Inner);
                StopOpenTelemetry(nar.Telemetry, null, nameof(EndOpen));
            } catch ( Exception ex )
            {
                StopOpenTelemetry(nar.Telemetry, ex, nameof(EndOpen));
                throw;
            }
        }

        public IAsyncResult BeginClose(AsyncCallback callback, object state)
        {
            return InnerChannel.BeginClose(callback, state);
        }

        public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return InnerChannel.BeginClose(timeout, callback, state);
        }

        public void EndClose(IAsyncResult result)
        {
            try
            {
                InnerChannel.EndClose(result);
            } finally
            {
                UnhookChannelEvents();
                OnClose();
            }
        }


        private void HookChannelEvents()
        {
            InnerChannel.Closed += OnChannelClosed;
            InnerChannel.Closing += OnChannelClosing;
            InnerChannel.Opened += OnChannelOpened;
            InnerChannel.Opening += OnChannelOpening;
            InnerChannel.Faulted += OnChannelFaulted;
        }
        private void UnhookChannelEvents()
        {
            InnerChannel.Closed -= OnChannelClosed;
            InnerChannel.Closing -= OnChannelClosing;
            InnerChannel.Opened -= OnChannelOpened;
            InnerChannel.Opening -= OnChannelOpening;
            InnerChannel.Faulted -= OnChannelFaulted;
        }
        private void OnChannelOpened(object sender, EventArgs e)
        {
            Opened?.Invoke(this, e);
        }
        private void OnChannelOpening(object sender, EventArgs e)
        {
            Opening?.Invoke(this, e);
        }
        private void OnChannelClosed(object sender, EventArgs e)
        {
            Closed?.Invoke(sender, e);
        }
        private void OnChannelClosing(object sender, EventArgs e)
        {
            Closing?.Invoke(sender, e);
        }
        private void OnChannelFaulted(object sender, EventArgs e)
        {
            Faulted?.Invoke(sender, e);
        }


        // telemetry implementation
        private DependencyTelemetry StartOpenTelemetry(String method)
        {
            try
            {
                var telemetry = new DependencyTelemetry();
                telemetry.Start();
                telemetry.Type = DependencyConstants.WcfChannelOpen;
                telemetry.Target = RemoteAddress.Uri.Host;
                telemetry.Name = RemoteAddress.Uri.ToString();
                telemetry.Data = channelManager.ContractType.FullName;
                return telemetry;
            } catch ( Exception ex )
            {
                WcfEventSource.Log.ClientInspectorError(method, ex.ToString());
                return null;
            }
        }
        private void StopOpenTelemetry(DependencyTelemetry telemetry, Exception error, String method)
        {
            if ( telemetry == null )
            {
                return;
            }
            try
            {
                telemetry.Success = error == null;
                telemetry.Stop();
                channelManager.TelemetryClient.TrackDependency(telemetry);
            } catch ( Exception ex )
            {
                WcfEventSource.Log.ClientInspectorError(method, ex.ToString());
            }
        }

        protected DependencyTelemetry StartSendTelemetry(Message request, String method)
        {
            var soapAction = request.Headers.Action;
            ClientOpDescription operation;
            if ( !channelManager.OperationMap.TryLookupByAction(soapAction, out operation) )
            {
                return null;
            }

            try
            {
                var telemetry = new DependencyTelemetry();
                telemetry.Start();
                telemetry.Type = DependencyConstants.WcfClientCall;
                telemetry.Target = RemoteAddress.Uri.Host;
                telemetry.Name = RemoteAddress.Uri.ToString();
                telemetry.Data = channelManager.ContractType.Name + "." + operation.Name;
                telemetry.Properties[DependencyConstants.SoapActionProperty] = soapAction;
                if ( operation.IsOneWay )
                {
                    telemetry.Properties[DependencyConstants.IsOneWayProperty] = Boolean.TrueString;
                }
                return telemetry;
            } catch ( Exception ex )
            {
                WcfEventSource.Log.ClientInspectorError(method, ex.ToString());
                return null;
            }
        }
        protected void StopSendTelemetry(DependencyTelemetry telemetry, Message response, Exception error, String method)
        {
            if ( telemetry == null )
            {
                return;
            }
            try
            {
                if ( error != null )
                {
                    telemetry.Success = false;
                }
                if ( response != null && response.IsFault )
                {
                    telemetry.Success = false;
                }
                telemetry.Stop();
                channelManager.TelemetryClient.TrackDependency(telemetry);
            } catch ( Exception ex )
            {
                WcfEventSource.Log.ClientInspectorError(method, ex.ToString());
            }
        }
        protected bool IsOneWay(DependencyTelemetry telemetry)
        {
            String value;
            if ( telemetry.Properties.TryGetValue(DependencyConstants.IsOneWayProperty, out value) )
            {
                return Boolean.Parse(value);
            }
            return false;
        }

        protected virtual void OnClose()
        {
        }

        protected class NestedAsyncResult : IAsyncResult
        {

            public object AsyncState { get { return Inner.AsyncState; } }

            public WaitHandle AsyncWaitHandle { get { return Inner.AsyncWaitHandle; } }

            public bool CompletedSynchronously { get { return Inner.CompletedSynchronously; } }

            public bool IsCompleted { get { return Inner.IsCompleted; } }

            public IAsyncResult Inner { get; private set; }
            public DependencyTelemetry Telemetry { get; private set; }
            public object OtherState { get; private set; }

            public NestedAsyncResult(IAsyncResult innerResult, DependencyTelemetry telemetry)
                : this(innerResult, telemetry, null)
            {
            }
            public NestedAsyncResult(IAsyncResult innerResult, DependencyTelemetry telemetry, object otherState)
            {
                this.Inner = innerResult;
                this.Telemetry = telemetry;
                this.OtherState = otherState;
            }
        }
    }
}
