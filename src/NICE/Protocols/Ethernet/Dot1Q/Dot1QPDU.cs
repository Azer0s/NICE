using System.Collections.Generic;
using System.Linq;
using NICE.Abstraction;
using NICE.Protocols.Ethernet.MAC;

namespace NICE.Protocols.Ethernet.Dot1Q
{
    public class Dot1QPDU : IMACCompatible
    {
        public ushort EtherType => 0x8100;
        
        public Dot1QHeader Header;
        public IMACCompatible Payload;

        public static Dot1QPDU FromBytes(byte[] bytes)
        {
            var pdu = new Dot1QPDU
            {
                Header = Dot1QHeader.FromBytes(bytes.Take(4).ToArray())
            };

            var protocol = MACRegistry.GetProtocol(pdu.Header.Type);
            pdu.Payload = protocol.FromBytes(bytes.Skip(4).ToArray());

            return pdu;
        }

        IMACCompatible IByteable<IMACCompatible>.FromBytes(byte[] bytes)
        {
            return FromBytes(bytes);
        }

        public byte[] ToBytes()
        {
            var bytes = new List<byte>();
            bytes.AddRange(Header.ToBytes());
            bytes.AddRange(Payload.ToBytes());
            return bytes.ToArray();
        }
    }
}