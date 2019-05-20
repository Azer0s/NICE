using System;
using System.Linq;
using NICE.Layer2;

namespace NICE
{
    public class EthernetPort
    {
        private EthernetDevice _device;
        private EthernetPort _connectedTo;
        public byte[] MACAddress { get; }
        private Action<EthernetFrame, EthernetDevice> _onReceiveAction;

        public void Init(EthernetDevice ethernetDevice)
        {
            var vendorBytes = new byte[]{0x10,0x14,0x20};
            var guid = Guid.NewGuid().ToByteArray();
            MACAddress = new[] {vendorBytes[0], vendorBytes[1], vendorBytes[2], guid[0], guid[1], guid[2]};
            _device = ethernetDevice;
        }

        public void ConnectTo(EthernetPort ethernetPort)
        {
            _connectedTo = ethernetPort;
            ethernetPort._connectedTo = this;
        }

        public void Receive(EthernetFrame frame)
        {
            _onReceiveAction(frame, _device);
        }

        public void Send(EthernetFrame frame)
        {
            _connectedTo.Receive(frame);
        }

        public void OnReceive(Action<EthernetFrame, EthernetDevice> action)
        {
            _onReceiveAction = action;
        }
    }
}