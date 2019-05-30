using System;
using System.Collections.Generic;
using System.Linq;
using NICE.API.Abstraction;
using NICE.Foundation;

namespace NICE.API.Builder
{
    public class Dot1Q : Payloadable, IProtocol
    {
        public Option<byte> Priority;
        public Option<bool> Flag;
        public Option<ushort> VlanId;
        public Option<ushort> EtherType;
        
        public static Ethernet operator |(Ethernet frame, Dot1Q dot1Q)
        {
            frame.EtherType = (ushort) 0x8100;
            frame.Payload = dot1Q;
            return frame;
        }

        public byte[] ToBytes()
        {
            if (!VlanId.IsSet())
            {
                VlanId.Set(0);
            }

            if (!Priority.IsSet())
            {
                Priority.Set(0);
            }

            if (!Flag.IsSet())
            {
                Flag.Set(false);
            }
            
            if (!EtherType.IsSet())
            {
                throw new Exception();
            }
            
            var bytes = new List<byte>();

            var firstByte = (byte) (Priority.Get() << 5);
            firstByte.Set(4, Flag.Get());

            var vlanID = VlanId.Get();
            
            firstByte.Set(3, vlanID.Get(11));
            firstByte.Set(2, vlanID.Get(10));
            firstByte.Set(1, vlanID.Get(9));
            firstByte.Set(0, vlanID.Get(8));
            
            bytes.Add(firstByte);
            bytes.Add((byte) vlanID);
            
            bytes.AddRange(BitConverter.GetBytes(EtherType.Get()).Reverse());

            return bytes.ToArray();
        }
    }
}