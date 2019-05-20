using NICE.Hardware.Abstraction;

namespace NICE.Hardware
{
    public class EthernetSwitch : Device
    {
        public EthernetSwitch() : base(null)
        {
            OnReceive = (frame, port) =>
            {
                //TODO: If an untagged frame comes in, tag it
                //TODO: Send the frame to the corresponding port of the dst mac
                //TODO: I think this is ARP (?)
                //TODO: Like...we gotta find out which MAC belongs to which switch port
                //TODO: But what if a device disconnects tho??
            };
        }
    }
}