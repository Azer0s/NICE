using System;

namespace NICE.Layer3
{
    /// <summary>
    /// RFC 791
    /// </summary>
    public class IPv4Packet : Layer3Packet
    {
        public byte Version_IHL; //First 4 bits = Version, last 4 bits = IHL
        public byte TypeOfService;
        public byte[] TotalLength; //2 bytes
        public byte[] Identification; //2 bytes
        public byte[] Flags_FragmentOffset; //First 3 bits = Flags, last 13 bits = Fragment Offset
        public byte TimeToLive;
        public byte Protocol;
        public byte[] HeaderChecksum; //2 bytes
        public uint SourceAddress; //4 bytes
        public uint DestinationAddress; //4 bytes
        public uint[] Options;
        
        public override byte[] ToBytes()
        {
            throw new NotImplementedException();
        }
    }
}