namespace NICE.Layer3
{
    public class RawLayer3Packet : Layer3Packet
    {
        private readonly byte[] _bytes;
        
        public RawLayer3Packet(byte[] bytes)
        {
            _bytes = bytes;
        }
        
        public override byte[] ToBytes()
        {
            return _bytes;
        }
    }
}