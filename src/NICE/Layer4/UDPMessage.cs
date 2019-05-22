// ReSharper disable InconsistentNaming
namespace NICE.Layer4
{
    public class UDPMessage
    {
        public byte[] SourcePort; //2 byte
        public byte[] DestinationPort; //2 byte
        public byte[] Length; //2 byte
        public byte[] Checksum; //2 byte
        public byte[] Data; //variable
    }
}