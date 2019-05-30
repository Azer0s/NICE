using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NICE.Abstraction;
using NICE.Foundation;
using NICE.Protocols.Ethernet.MAC;

namespace NICE.Protocols.Ethernet.Dot1Q
{
    public class Dot1QHeader : IByteable<Dot1QHeader>
    {
        public byte Priority;
        public bool Flag;
        public ushort VlanID;
        public ushort Type;


        public static Dot1QHeader FromBytes(byte[] bytes)
        {
            var header = new Dot1QHeader();

            var firstByte = bytes[0];
            header.Priority.Set(2, firstByte.Get(7));
            header.Priority.Set(1, firstByte.Get(6));
            header.Priority.Set(0, firstByte.Get(5));
            header.Flag = firstByte.Get(4);

            header.VlanID = bytes.Take(2).ToArray().ToUshort();
            header.VlanID.Set(15, false);
            header.VlanID.Set(14, false);
            header.VlanID.Set(13, false);
            header.VlanID.Set(12, false);

            header.Type = bytes.Skip(2).Take(2).ToArray().ToUshort();

            return header;
        }

        Dot1QHeader IByteable<Dot1QHeader>.FromBytes(byte[] bytes)
        {
            return FromBytes(bytes);
        }

        public byte[] ToBytes()
        {
            var bytes = new List<byte>();

            var firstByte = (byte) (Priority << 5);
            firstByte.Set(4, Flag);
            
            firstByte.Set(3, VlanID.Get(11));
            firstByte.Set(2, VlanID.Get(10));
            firstByte.Set(1, VlanID.Get(9));
            firstByte.Set(0, VlanID.Get(8));
            
            bytes.Add(firstByte);
            bytes.Add((byte) VlanID);
            
            bytes.AddRange(BitConverter.GetBytes(Type).Reverse());

            return bytes.ToArray();
        }
    }
}