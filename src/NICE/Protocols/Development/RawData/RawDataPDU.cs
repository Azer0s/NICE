using NICE.Protocols.Ethernet.MAC;

namespace NICE.Protocols.Development.RawData
{
    public class RawDataPDU : IMACCompatible
    {
        public byte[] Bytes;
        public ushort EtherType => 0x88b5;

        public IMACCompatible FromBytes(byte[] bytes)
        {
            return new RawDataPDU {Bytes = bytes};
        }

        public byte[] ToBytes()
        {
            return Bytes;
        }
    }
}