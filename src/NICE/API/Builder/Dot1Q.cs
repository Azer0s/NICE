using System;
using System.Collections.Generic;
using System.Linq;
using NICE.API.Abstraction;
using NICE.Foundation;

namespace NICE.API.Builder
{
    public class Dot1Q : Payloadable, IProtocol
    {
        public Option<ushort> EtherType;
        public Option<bool> Flag;
        public Option<byte> Priority;
        public Option<ushort> VlanId;

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

            var vlanId = VlanId.Get();

            firstByte.Set(3, vlanId.Get(11));
            firstByte.Set(2, vlanId.Get(10));
            firstByte.Set(1, vlanId.Get(9));
            firstByte.Set(0, vlanId.Get(8));

            bytes.Add(firstByte);
            bytes.Add((byte) vlanId);

            bytes.AddRange(BitConverter.GetBytes(EtherType.Get()).Reverse());

            return bytes.ToArray();
        }

        public static Wrapped<Dot1Q> operator - (Dot1Q dot1Q)
        {
            return new Wrapped<Dot1Q> {Value = dot1Q};
        }

        public static Ethernet operator | (Ethernet frame, Dot1Q dot1Q)
        {
            frame.EtherType = (ushort) 0x8100;
            frame.Payload = dot1Q;
            return frame;
        }
    }
}