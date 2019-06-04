using NICE.API.Abstraction;

namespace NICE.API.Builder
{
    public class RawPacket : IProtocol
    {
        public byte[] Bytes;

        public byte[] ToBytes()
        {
            return Bytes;
        }

        public static Wrapped<RawPacket> operator - (RawPacket rawPacket)
        {
            return new Wrapped<RawPacket> {Value = rawPacket};
        }
        
        public static Ethernet operator | (Ethernet frame, RawPacket packet)
        {
            frame.Payload = packet;
            frame.EtherType = Option<ushort>.Of((ushort) 0x88b5);
            return frame;
        }
    }
}