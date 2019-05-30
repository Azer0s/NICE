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

        public MACPDU Data;

        EthernetFrame IByteable<EthernetFrame>.FromBytes(byte[] bytes)
        {
            return FromBytes(bytes);
        }

        public byte[] ToBytes()
        {
            var list = new List<byte>();
            list.AddRange(Preamble);
            list.Add(SFD);
            list.AddRange(Data.ToBytes());
            
            return list.ToArray();
        }
        
        public static EthernetFrame FromBytes(byte[] bytes)
        {
            var macpdu = MACPDU.FromBytes(bytes.Skip(8).ToArray());

            var frame = new EthernetFrame
            {
                Data = macpdu
            };
            return frame;
        }
    }
}
