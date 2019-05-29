using System;
using System.Collections.Generic;
using System.Linq;
using NICE.Abstraction;
using NICE.Foundation;
using NICE.Protocols.Ethernet.Dot1Q;
using NICE.Protocols.Ethernet.MAC;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

namespace NICE.Protocols.Ethernet
{
    /// <summary>
    /// IEEE 802.3
    /// https://networkengineering.stackexchange.com/questions/5300/what-is-the-difference-between-ethernet-ii-and-802-3-ethernet
    /// </summary>
    public class EthernetFrame : IByteable<EthernetFrame>
    {
        public byte[] Preamble { get; } = {0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA};
        public byte SFD { get; } = BitConverter.GetBytes(0b10101011)[0];

        public MACHeader MacHeader;
        public IMACCompatible Payload;
        public byte[] FCS;

        EthernetFrame IByteable<EthernetFrame>.FromBytes(byte[] bytes)
        {
            return FromBytes(bytes);
        }

        public byte[] ToBytes()
        {
            var list = new List<byte>();
            list.AddRange(Preamble);
            list.Add(SFD);
            list.AddRange(MacHeader.ToBytes());
            list.AddRange(Payload.ToBytes());
            list.AddRange(FCS);
            
            return list.ToArray();
        }
        
        public static EthernetFrame FromBytes(byte[] bytes)
        {
            var byteList = bytes.ToList();
            var fcs = Util.GetFCS(byteList.GetRange(0, bytes.Length - 4).ToArray());
            var bytesFcs = byteList.GetRange(byteList.Count - 4, 4);

            if (!fcs.SequenceEqual(bytesFcs))
            {
                throw new Exception("FCS must match!");
            }
            
            var header = MACHeader.FromBytes(bytes.Take(14).ToArray());
            var payload = byteList.GetRange(14, byteList.Count - 18).ToArray();
            var protocol = MACRegistry.GetProtocol(header.EtherType);

            var frame = new EthernetFrame
            {
                MacHeader = header,
                Payload = protocol.FromBytes(payload),
                FCS = fcs
            };

            return frame;
        }
    }
}
