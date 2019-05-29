using System;
using System.Collections.Generic;
using NICE.API.Abstraction;

namespace NICE.API.Builder
{
    public class Dot1Q : IProtocol
    {
        public Option<byte[]> VlanId;
        public Option<byte[]> EtherType;
        public IProtocol Payload;
        
        public static Ethernet operator |(Ethernet frame, Dot1Q dot1Q)
        {
            frame.EtherType = Option<byte[]>.Of(new byte[]{0x81, 0x00});
            frame.Payload = dot1Q;
            return frame;
        }

        public byte[] ToBytes()
        {
            if (!VlanId.IsSet())
            {
                throw new Exception();
            }
            
            if (!EtherType.IsSet())
            {
                throw new Exception();
            }
            
            var list = new List<byte>();
            list.AddRange(VlanId.Get());
            list.AddRange(EtherType.Get());
            list.AddRange(Payload.ToBytes());
            return list.ToArray();
        }
    }
}