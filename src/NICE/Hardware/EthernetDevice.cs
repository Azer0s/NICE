using NICE.Foundation;
using NICE.Hardware.Abstraction;
// ReSharper disable UnusedVariable

namespace NICE.Hardware
{
    public class EthernetDevice : Device
    {        
        public EthernetDevice(string name) : base(name,null)
        {
            Log.Info(Hostname, "Initializing end device...");
            OnReceive = (frame, port) =>
            {
                if (frame.MacHeader.Dst == Constants.ETHERNET_BROADCAST_ADDRESS.Get())
                {
                    //TODO: Handle broadcast
                    Log.Debug(Hostname, $"Received Ethernet broadcast frame from {frame.MacHeader.Src}");
                }
                
                if (frame.MacHeader.Dst != port.MACAddress)
                {
                    return;
                }
                
                Log.Debug(Hostname, $"Received Ethernet frame from {frame.MacHeader.Src}");
                
                //TODO: Handle all the layer 3 protocols
            };
        }
    }
}