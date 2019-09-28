using NICE.Protocols.Ethernet.MAC;

namespace NICE.Protocols.IP
{
    // ReSharper disable once InconsistentNaming
    public class IPv4PDU : IMACCompatible
    {
        public ushort EtherType => 0x0800;

        public IMACCompatible FromBytes(byte[] bytes)
        {
            throw new System.NotImplementedException();
        }

        public byte[] ToBytes()
        {
            throw new System.NotImplementedException();
        }
    }
}