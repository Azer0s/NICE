using System.Linq;
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
                if (!frame.Dst.SequenceEqual(port.MACAddress))
                {
                    return;
                }
                
                var type = Util.GetEtherTypeFromBytes(frame.Type);
                //TODO: Handle all the layer 3 protocols
            };
        }
    }
}