using NICE.API.Abstraction;

namespace NICE.API.Builder
{
    public class RawPacket : IProtocol
    {
        public byte[] Bytes;
        
        public static Ethernet operator |(Ethernet frame, RawPacket packet)
        {
            frame.Payload = packet;
            frame.EtherType = Option<byte[]>.Of(new byte[]{0x88, 0xB5});
            return frame;
        }

        public byte[] ToBytes()
        {
            return Bytes;
        }
    }
}