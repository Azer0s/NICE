using NICE.Protocols.Ethernet.MAC;

namespace NICE.Protocols.Development.RawData
{
    public class RawDataPDU : IMACCompatible
    {
        public ushort EtherType => 0x88b5;

        public byte[] Bytes;
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