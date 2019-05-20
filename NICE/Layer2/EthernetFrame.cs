using System;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

namespace NICE.Layer2
{
    public class EthernetMessage
    {
        public byte[] Preamble { get; set; } = BitConverter.GetBytes(0b10101010_10101010_10101010_10101010_10101010_10101010_10101010);
        public byte[] SFD { get; set; } = BitConverter.GetBytes(0b10101011);

        public byte[] Dst { get; }
        public byte[] Src { get; }
        
        public byte[] Tag { get; }
        public byte[] Length { get; }
        
        public byte[] DSAP { get; }
        public byte[] SSAP { get; }
        public byte[] Control { get; }
        public byte[] Data { get; }

        public byte[] FCS { get; }

        public EthernetMessage(byte[] dst, byte[] src, byte[] tag, byte[] length, byte[] dsap, byte[] ssap, byte[] control, byte[] data, byte[] fcs)
        {
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
            
            if (tag.Length != 4)
            {
                throw new Exception("Invalid tag length!");
            }
            Tag = tag;
            
            if (length.Length != 2)
            {
                throw new Exception("Invalid length length!");
            }
            Length = length;
            
            if (dsap.Length != 1)
            {
                throw new Exception("Invalid DSAP length!");
            }
            DSAP = dsap;
            
            if (ssap.Length != 1)
            {
                throw new Exception("Invalid SSAP length!");
            }
            SSAP = ssap;
            
            if (control.Length != 1)
            {
                throw new Exception("Invalid control length!");
            }
            Control = control;
            
            if (!(data.Length <= 1522 && data.Length >= 68))
            {
                throw new Exception("Invalid data length!");
            }
            Data = data;
            
            if (data.Length != 4)
            {
                throw new Exception("Invalid FCS length!");
            }
            FCS = fcs;
        }
    }
}