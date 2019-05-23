using System.Linq;
using NICE.Foundation;
using NICE.Hardware.Abstraction;

namespace NICE.Hardware
{
    public class EthernetDevice : Device
    {        
        public EthernetDevice(string name) : base(name,null)
        {
            Log.Info(Hostname, "Initializing end device...");
            OnReceive = (frame, port) =>
            {
                if (frame.Dst.SequenceEqual(Constants.ETHERNET_BROADCAST_ADDRESS))
                {
                    //TODO: Handle broadcast
                    Log.Debug(Hostname, $"Received Ethernet broadcast frame from {frame.Src.ToMACAddressString()}");
                }
                
                if (!frame.Dst.SequenceEqual(port.MACAddress))
                {
                    return;
                }
                
                Log.Debug(Hostname, $"Received Ethernet frame from {frame.Src.ToMACAddressString()}");
                
                var type = Util.GetEtherTypeFromBytes(frame.Type);
                //TODO: Handle all the layer 3 protocols
            };
        }
    }
}