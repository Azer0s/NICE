// ReSharper disable InconsistentNaming
namespace NICE.Layer4
{
    /// <summary>
    /// RFC 793
    /// https://4.bp.blogspot.com/-GJTB-0794v4/WHSMM09kzDI/AAAAAAAAAqw/lAfVqCHjLbkz65rUKIlLnaeZyiuQCoqnACPcBGAYYCw/s1600/tcp_header.png
    /// </summary>
    public class TCPMessage
    {
        public byte[] SourcePort; //2 byte
        public byte[] DestinationPort; //2 byte
        public byte[] SequenceNumber; //4 byte
        public byte[] AcknowledgmentNumber; //4 byte
        public byte DataOffset; //Last 4 bits are 0
        public byte ControlBits;
        public byte[] Window; //2 byte
        public byte[] Checksum; //2 byte
        public byte[] UrgentPointer; //2 byte
        public byte[] Options; //4 byte
        public byte[] Data; //Variable
    }
}