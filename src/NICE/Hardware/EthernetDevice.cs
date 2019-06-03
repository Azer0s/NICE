using NICE.Foundation;
using NICE.Hardware.Abstraction;

// ReSharper disable UnusedVariable

namespace NICE.Hardware
{
    public class EthernetDevice : Device
    {
        public EthernetDevice(string name) : base(name, null)
        {
            Log.Info(Hostname, "Initializing end device...");
            OnReceive = (frame, port) =>
            {
                if (frame.Data.Header.Dst == Constants.ETHERNET_BROADCAST_ADDRESS.Get())
                {
                    //TODO: Handle broadcast
                    Log.Debug(Hostname, $"Received Ethernet broadcast frame from {frame.Data.Header.Src}");
                }

                if (frame.Data.Header.Dst != port.MACAddress)
                {
                    return;
                }

                Log.Debug(Hostname, $"Received Ethernet frame from {frame.Data.Header.Src}");

                //TODO: Handle all the layer 3 protocols
            };
        }
    }
}