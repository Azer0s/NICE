using NICE.Protocols.Ethernet.MAC;

namespace NICE.Protocols.Ethernet.Dot1Q
{
    public class Dot1QHeader : IMACCompatible
    {
        public IMACCompatible Payload;
        
        public IMACCompatible FromBytes(byte[] bytes)
        {
            throw new System.NotImplementedException();
        }

        public byte[] ToBytes()
        {
            throw new System.NotImplementedException();
        }

        public ushort EtherType { get; } = 0x8100;
    }
}