using NICE.API;
using NICE.API.Abstraction;
using NICE.API.Builder;
using NICE.Hardware;
using NICE.Layer3;

namespace NICE.Layer2
{
    public class EthernetFrame
    {
        private readonly Ethernet _apiEthernet;

        public EthernetFrame(EthernetPort dst, EthernetPort src, Layer3Packet layer3Packet)
        {
            _apiEthernet = Generators.Ethernet(dst, src);
            _apiEthernet = layer3Packet.Merge(_apiEthernet);
        }

        public EthernetFrame(EthernetPort dst, EthernetPort src, Option<ushort> vlan, Layer3Packet layer3Packet)
        {
            _apiEthernet = Generators.Ethernet(dst, src) | Generators.Dot1Q(vlan);
            _apiEthernet = layer3Packet.Merge(_apiEthernet);
        }

        public static implicit operator Ethernet(EthernetFrame ethernet)
        {
            return ethernet._apiEthernet;
        }
    }
}