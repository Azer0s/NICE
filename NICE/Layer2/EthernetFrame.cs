using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NICE.Hardware;
using NICE.Layer3;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

namespace NICE.Layer2
{
    public class EthernetFrame
    {
        public byte[] Preamble { get; } = {0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA};
        public byte SFD { get; } = BitConverter.GetBytes(0b10101011)[0];

        public byte[] Dst { get; set; }
        public byte[] Src { get; set; }

        public bool IsTagged;
        public byte[] Tag { get; set; }
        public byte[] Type { get; set; }
        public byte[] Data { get; set; }

        public byte[] FCS { get; set; }

        public byte[] GetBytes()
        {
            var bytes = new List<byte>();
            bytes.AddRange(Preamble);
            bytes.Add(SFD);
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

        public EthernetFrame(){}
        
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

        public static EthernetFrame FromBytes(byte[] bytes)
        {
            var ethernetFrame = new EthernetFrame();
            var byteList = bytes.ToList();
            byteList.RemoveRange(0, 8);

            ethernetFrame.Dst = byteList.GetRange(0, 6).ToArray();
            ethernetFrame.Src = byteList.GetRange(6, 6).ToArray();

            var dataStart = 12;
            
            if (byteList[12] == 0x81 && byteList[13] == 0x00)
            {
                //Is tagged
                ethernetFrame.IsTagged = true;
                ethernetFrame.Tag = byteList.GetRange(14, 2).ToArray();
                dataStart = 16;
            }

            ethernetFrame.Type = byteList.GetRange(dataStart, 2).ToArray();
            dataStart += 2;

            ethernetFrame.Data = byteList.GetRange(dataStart, byteList.Count - dataStart - 4).ToArray();

            ethernetFrame.FCS = byteList.GetRange(byteList.Count - 4, 4).ToArray();
            var fcs = Util.GetFCS(ethernetFrame);

            if (!fcs.SequenceEqual(ethernetFrame.FCS))
            {
                throw new Exception("FCS does not match! Ethernet frame invalid!");
            }
            
            return ethernetFrame;
        }
    }
}