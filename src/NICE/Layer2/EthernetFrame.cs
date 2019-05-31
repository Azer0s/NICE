using NICE.API.Abstraction;
using NICE.Hardware;
using NICE.Layer3;

namespace NICE.Layer2
{
    public class EthernetFrame
    {
        private readonly API.Builder.Ethernet _apiEthernet;

        public EthernetFrame(EthernetPort dst, EthernetPort src, Layer3Packet layer3Packet)
        {
            _apiEthernet = API.Generators.Ethernet(dst, src);
            _apiEthernet = layer3Packet.Merge(_apiEthernet);
        }
        
        public EthernetFrame(EthernetPort dst, EthernetPort src, Option<ushort> vlan, Layer3Packet layer3Packet)
        {
            _apiEthernet = API.Generators.Ethernet(dst, src) | API.Generators.Dot1Q(vlan);
            _apiEthernet = layer3Packet.Merge(_apiEthernet);
        }

        public static implicit operator API.Builder.Ethernet(EthernetFrame ethernet)
        {
            return ethernet._apiEthernet;
        }
    }
}