using NICE.Hardware.Abstraction;

namespace NICE.Hardware
{
    public class EthernetDevice : Device
    {
        public EthernetDevice() : base(null)
        {
            _onReceive = (frame, port) =>
            {
                //TODO: Handle all the layer 3 protocols
            };
        }
    }
}