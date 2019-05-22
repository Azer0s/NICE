// ReSharper disable InconsistentNaming
namespace NICE.Layer4
{
    /// <summary>
    /// http://www.tcpipguide.com/free/t_TCPChecksumCalculationandtheTCPPseudoHeader-2.htm
    /// </summary>
    public class IPPseudoHeader
    {
        public uint SourceAddress; //4 byte
        public uint DestinationAddress; //4 byte
        public byte Reserved = 0x0;
        public byte Protocol; // bit 0
        public byte[] Length; //2 byte
    }
}