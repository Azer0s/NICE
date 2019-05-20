using NICE.Hardware;
using NICE.Layer2;
using NICE.Layer3;

namespace NetEmu
{
    static class Program
    {
        static readonly string ETH01 = "eth0/1";
        static readonly string FA01 = "fa0/1";
        static readonly string FA02 = "fa0/2";
        
        static void Main()
        {
            var dev1 = new EthernetDevice();
            var dev2 = new EthernetDevice();

            Vlan.Register(1, "DEFAULT");

            dev1[ETH01].Init();
            dev2[ETH01].Init();
            dev1[ETH01].ConnectTo(dev2[ETH01]);
            
            dev1[ETH01].Send(new EthernetFrame(dev2[ETH01], dev1[ETH01], 1, EtherType.IPv4, new RawLayer3Packet(new byte[100]), true));
        }
    }
}