using NICE.API.Abstraction;
using NICE.API.Builder;

namespace NICE.API
{
    public static class Generators
    {
        public static Ethernet Ethernet(Option<byte[]> dst = null, Option<byte[]> src = null, Option<byte[]> etherType = null)
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
                etherType = new Option<byte[]>();
            }
            
            return new Ethernet {Src = src, Dst = dst, EtherType = etherType};
        }

        public static Dot1Q Dot1Q(Option<byte[]> vlanId = null, Option<byte[]> etherType = null)
        {
            if (vlanId == null)
            {
                vlanId = Vlan.Get(0);
            }

            if (etherType == null)
            {
                etherType = Option<byte[]>.Of(new byte[0]);
            }
            
            return new Dot1Q{VlanId = vlanId, EtherType = etherType};
        }

        public static RawPacket RawPacket(byte[] bytes)
        {
            return new RawPacket {Bytes = bytes};
        }
    }
}