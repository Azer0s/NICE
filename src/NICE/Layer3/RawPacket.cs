using NICE.API.Builder;
using static NICE.API.Generators;
namespace NICE.Layer3
{
    public class RawPacket : Layer3Packet
    {
        private byte[] _bytes;
        
        public RawPacket(byte[] bytes)
        {
            _bytes = bytes;
        }
        
        public Ethernet Merge(Ethernet ethernet)
        {
            return ethernet | RawPacket(_bytes);
        }
    }
}