using System;
using System.Threading.Tasks;
using NICE;
using NICE.Foundation;
using NICE.Hardware;
using NICE.Layer2;
using NICE.Layer3;
using NICE.Layer4;

namespace NetEmu
{
    static class Program
    {
        static readonly string ETH01 = "eth0/1";
        static readonly string FA01 = "fa0/1";
        static readonly string FA02 = "fa0/2";
        
        static async Task Main()
        {
            Log.SetLevel(Log.Level.TRACE, Log.Groups.SHOW);
            Vlan.Register(1, "DEFAULT");

            /*
             * PC1 ---  eth0/1
             *       |
             *       | fa0/1
             *      SW1
             *       |  fa0/2
             *       |
             *       | fa0/1
             *      SW2
             *       |  fa0/1
             *       |
             * PC2 --- eth0/1
             */
            
            Log.Group("Initialize devices");
            
            var pc1 = new EthernetDevice("pc1");
            var pc2 = new EthernetDevice("pc2");

            var sw1 = new EthernetSwitch("sw1");
            var sw2 = new EthernetSwitch("sw2");
            
            Log.Group("Initialize ports");
            
            //pc ports
            pc1[ETH01].Init();
            pc2[ETH01].Init();
            
            //Connection from sw1 to pc1
            sw1[FA01].Init();
            
            //Connection from sw2 to pc2
            sw2[FA01].Init();

            //Connection from sw1 to sw2
            sw1[FA02].Init();
            sw2[FA02].Init();
            
            Log.Group("Connect ports");
            
            //Connect the pcs to the switches
            pc1[ETH01].ConnectTo(sw1[FA01]);
            pc2[ETH01].ConnectTo(sw2[FA01]);
            
            //Connect the switches to each other
            sw1[FA02].ConnectTo(sw2[FA02]);
            
            Log.Group("Set switchport modes");
            //Set the ports from pc to switch to access vlan 1
            sw1.SetPort(FA01, EthernetSwitch.AccessMode.ACCESS, Vlan.Get(1));
            sw2.SetPort(FA01, EthernetSwitch.AccessMode.ACCESS, Vlan.Get(1));
            
            //Set the ports from switch to switch to trunk
            sw1.SetPort(FA02, EthernetSwitch.AccessMode.TRUNK, null);
            sw2.SetPort(FA02, EthernetSwitch.AccessMode.TRUNK, null);

            //Log.Group("Current state");
            //Log.PrintState();
            
            //Learn MAC Addresses
            Log.Group("Learn MAC Addresses");
            pc1[ETH01].SendSync(new EthernetFrame(Constants.ETHERNET_BROADCAST_PORT, pc1[ETH01], null, EtherType.IPv4, new RawLayer3Packet(new byte[100]), false));
            //Wait for all sending operations to be finished (you don't HAVE to wait...I just prefer doing so, cause the log is more readable)
            //This is necessary cause even tho you send this frame synchronously, all the connected devices create new tasks for incoming frames 
            await Global.WaitForOperationsFinished();
            
            pc2[ETH01].SendSync(new EthernetFrame(Constants.ETHERNET_BROADCAST_PORT, pc2[ETH01], null, EtherType.IPv4, new RawLayer3Packet(new byte[100]), false));
            await Global.WaitForOperationsFinished();
            
            Log.Group("Send Ethernet frame over learned ports");
            pc1[ETH01].SendAsync(new EthernetFrame(pc2[ETH01], pc1[ETH01], null, EtherType.IPv4, new RawLayer3Packet(new byte[100]), false));
            await Global.WaitForOperationsFinished();
            
            pc1[ETH01].Disconnect();
            pc2[ETH01].Disconnect();

            //Console.ReadKey();
        }
    }
}
