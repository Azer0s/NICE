using NICE.API.Abstraction;
using NICE.API.Builder;

namespace NICE.API
{
    public static class Generators
    {
        public static Ethernet Ethernet(Option<byte[]> dst = null, Option<byte[]> src = null, Option<ushort> etherType = null)
        {
            if (dst == null)
            {
                dst = new Option<byte[]>();
            }

            if (src == null)
            {
                src = new Option<byte[]>();
            }

            if (etherType == null)
            {
                etherType = new Option<ushort>();
            }

            return new Ethernet {Src = src, Dst = dst, EtherType = etherType};
        }

        public static Dot1Q Dot1Q(Option<ushort> vlanId = null, Option<byte> priority = null, Option<bool> flag = null, Option<ushort> etherType = null)
        {
            if (vlanId == null)
            {
                vlanId = new Option<ushort>();
            }

            if (priority == null)
            {
                priority = new Option<byte>();
            }

            if (flag == null)
            {
                flag = new Option<bool>();
            }

            if (etherType == null)
            {
                etherType = new Option<ushort>();
            }

            return new Dot1Q {VlanId = vlanId, Priority = priority, Flag = flag, EtherType = etherType};
        }

        public static RawPacket RawPacket(byte[] bytes)
        {
            return new RawPacket {Bytes = bytes};
        }
    }
}