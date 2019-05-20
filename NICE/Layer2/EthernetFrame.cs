using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NICE.Hardware;
using NICE.Layer3;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

namespace NICE.Layer2
{
    public class EthernetFrame
    {
        public byte[] Preamble { get; set; } = BitConverter.GetBytes(0b10101010_10101010_10101010_10101010_10101010_10101010_10101010);
        public byte[] SFD { get; set; } = BitConverter.GetBytes(0b10101011);

        public byte[] Dst { get; }
        public byte[] Src { get; }

        public bool IsTagged;
        public byte[] Tag { get; }        
        public byte[] Type { get; }
        public byte[] Data { get; }

        public byte[] FCS { get; }

        public byte[] GetBytes()
        {
            var bytes = new List<byte>();
            bytes.AddRange(Preamble);
            bytes.AddRange(SFD);
            bytes.AddRange(Dst);
            bytes.AddRange(Src);

            if (IsTagged)
            {
                bytes.Add(0x81);
                bytes.Add(0x00);
                
                bytes.AddRange(Tag);
            }
            
            bytes.AddRange(Type);
            bytes.AddRange(Data);
            bytes.AddRange(FCS);
            return bytes.ToArray();
        }

        public EthernetFrame(EthernetPort dst, EthernetPort src, int? vlanNr, EtherType type, Layer3Packet msg, bool isTagged)
            : this(dst.MACAddress, src.MACAddress, Vlan.Get(vlanNr.GetValueOrDefault()), Util.GetBytesFromEtherType(type), msg.ToBytes(), isTagged) {}
        
        public EthernetFrame(byte[] dst, byte[] src, byte[] tag, byte[] type, byte[] data, bool isTagged)
        {
            IsTagged = isTagged;
            
            if (dst.Length != 6)
            {
                throw new Exception("Invalid DST MAC Address length!");
            }
            Dst = dst;
            
            if (src.Length != 6)
            {
                throw new Exception("Invalid SRC MAC Address length!");
            }
            Src = src;
            
            if (tag.Length != 2)
            {
                throw new Exception("Invalid tag length!");
            }
            Tag = tag;
            
            if (type.Length != 2)
            {
                throw new Exception("Invalid control length!");
            }
            Type = type;
            
            if (!(data.Length <= 1522 && data.Length >= 68))
            {
                throw new Exception("Invalid data length!");
            }
            Data = data;
            
            FCS = new byte[4];
            FCS = Util.GetFCS(this);
        }
    }
}