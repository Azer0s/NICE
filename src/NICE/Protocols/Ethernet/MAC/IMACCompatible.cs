using NICE.Abstraction;

// ReSharper disable InconsistentNaming

namespace NICE.Protocols.Ethernet.MAC
{
    public interface IMACCompatible : IByteable<IMACCompatible>
    {
        ushort EtherType { get; }
    }
}