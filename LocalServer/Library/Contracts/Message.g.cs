// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Message.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Library.Inputs.Contracts {

  /// <summary>Holder for reflection information generated from Message.proto</summary>
  public static partial class MessageReflection {

    #region Descriptor
    /// <summary>File descriptor for Message.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static MessageReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cg1NZXNzYWdlLnByb3RvEgljb250cmFjdHMaE1NldmVyaXR5TGV2ZWwucHJv",
            "dG8iwwEKB01lc3NhZ2USCwoDdmVyGAEgASgFEg8KB21lc3NhZ2UYAiABKAkS",
            "LwoNc2V2ZXJpdHlMZXZlbBgDIAEoDjIYLmNvbnRyYWN0cy5TZXZlcml0eUxl",
            "dmVsEjYKCnByb3BlcnRpZXMYBCADKAsyIi5jb250cmFjdHMuTWVzc2FnZS5Q",
            "cm9wZXJ0aWVzRW50cnkaMQoPUHJvcGVydGllc0VudHJ5EgsKA2tleRgBIAEo",
            "CRINCgV2YWx1ZRgCIAEoCToCOAFCG6oCGExpYnJhcnkuSW5wdXRzLkNvbnRy",
            "YWN0c2IGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Library.Inputs.Contracts.SeverityLevelReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Library.Inputs.Contracts.Message), global::Library.Inputs.Contracts.Message.Parser, new[]{ "Ver", "Message_", "SeverityLevel", "Properties" }, null, null, new pbr::GeneratedClrTypeInfo[] { null, })
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class Message : pb::IMessage<Message> {
    private static readonly pb::MessageParser<Message> _parser = new pb::MessageParser<Message>(() => new Message());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Message> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Library.Inputs.Contracts.MessageReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Message() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Message(Message other) : this() {
      ver_ = other.ver_;
      message_ = other.message_;
      severityLevel_ = other.severityLevel_;
      properties_ = other.properties_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Message Clone() {
      return new Message(this);
    }

    /// <summary>Field number for the "ver" field.</summary>
    public const int VerFieldNumber = 1;
    private int ver_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Ver {
      get { return ver_; }
      set {
        ver_ = value;
      }
    }

    /// <summary>Field number for the "message" field.</summary>
    public const int Message_FieldNumber = 2;
    private string message_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Message_ {
      get { return message_; }
      set {
        message_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "severityLevel" field.</summary>
    public const int SeverityLevelFieldNumber = 3;
    private global::Library.Inputs.Contracts.SeverityLevel severityLevel_ = 0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Library.Inputs.Contracts.SeverityLevel SeverityLevel {
      get { return severityLevel_; }
      set {
        severityLevel_ = value;
      }
    }

    /// <summary>Field number for the "properties" field.</summary>
    public const int PropertiesFieldNumber = 4;
    private static readonly pbc::MapField<string, string>.Codec _map_properties_codec
        = new pbc::MapField<string, string>.Codec(pb::FieldCodec.ForString(10), pb::FieldCodec.ForString(18), 34);
    private readonly pbc::MapField<string, string> properties_ = new pbc::MapField<string, string>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::MapField<string, string> Properties {
      get { return properties_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Message);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Message other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Ver != other.Ver) return false;
      if (Message_ != other.Message_) return false;
      if (SeverityLevel != other.SeverityLevel) return false;
      if (!Properties.Equals(other.Properties)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Ver != 0) hash ^= Ver.GetHashCode();
      if (Message_.Length != 0) hash ^= Message_.GetHashCode();
      if (SeverityLevel != 0) hash ^= SeverityLevel.GetHashCode();
      hash ^= Properties.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Ver != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(Ver);
      }
      if (Message_.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Message_);
      }
      if (SeverityLevel != 0) {
        output.WriteRawTag(24);
        output.WriteEnum((int) SeverityLevel);
      }
      properties_.WriteTo(output, _map_properties_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Ver != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Ver);
      }
      if (Message_.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Message_);
      }
      if (SeverityLevel != 0) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) SeverityLevel);
      }
      size += properties_.CalculateSize(_map_properties_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Message other) {
      if (other == null) {
        return;
      }
      if (other.Ver != 0) {
        Ver = other.Ver;
      }
      if (other.Message_.Length != 0) {
        Message_ = other.Message_;
      }
      if (other.SeverityLevel != 0) {
        SeverityLevel = other.SeverityLevel;
      }
      properties_.Add(other.properties_);
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            Ver = input.ReadInt32();
            break;
          }
          case 18: {
            Message_ = input.ReadString();
            break;
          }
          case 24: {
            severityLevel_ = (global::Library.Inputs.Contracts.SeverityLevel) input.ReadEnum();
            break;
          }
          case 34: {
            properties_.AddEntriesFrom(input, _map_properties_codec);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code