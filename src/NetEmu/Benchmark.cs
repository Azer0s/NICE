using System;
using System.Timers;
using NICE.API.Builder;
using NICE.Foundation;
using NICE.Hardware;
using NICE.Layer2;
using RawPacket = NICE.Layer3.RawPacket;
using Vlan = NICE.API.Vlan;

namespace NetEmu
{
    public class Benchmark
    {
        
        private static readonly string ETH01 = "eth0/1";

        public static void Main(string[] args)
        {
            Log.SetLevel(Log.Level.FATAL, Log.Groups.HIDE);
            Vlan.Register(1, "DEFAULT");
            Global.SetDeviceAutoStartup(true);
            
            var pc1 = new EthernetDevice("pc1");
            var pc2 = new EthernetDevice("pc2");
            
            pc1[ETH01].Init();
            pc2[ETH01].Init();
            
            pc1[ETH01].ConnectTo(pc2[ETH01]);

            var size = ((Ethernet) new EthernetFrame(Constants.ETHERNET_BROADCAST_PORT, pc1[ETH01], Vlan.Get(1),
                new RawPacket(new byte[100]))).ToEthernetFrame().ToBytes().Length;
            
            var run = true;
            var count = 0;
            
            var timer = new Timer(60_000);
            timer.Elapsed += (a, b) => { run = false; };
            timer.Start();

            while (run)
            {
                pc1[ETH01].SendAsync(new EthernetFrame(Constants.ETHERNET_BROADCAST_PORT, pc1[ETH01], Vlan.Get(1), new RawPacket(new byte[100])));
                count++;
            }
            
            Console.WriteLine($"{(count * size).ToString()} bytes transmitted!");
            Console.WriteLine($"{(count * size * 8/60).ToString()} b/s");
            Console.WriteLine($"{(count * size / 1024 * 8/60).ToString()} Kb/s");
            Console.WriteLine($"{(count * size / 1024 / 1024 * 8/60).ToString()} Mb/s");
            
            Console.ReadKey();
        }
    }
}