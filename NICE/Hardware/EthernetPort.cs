using System;
using NICE.Layer2;
// ReSharper disable InconsistentNaming

namespace NICE.Hardware
{
    public class EthernetPort
    {
        private EthernetPort _connectedTo;
        public byte[] MACAddress { get; private set; }
        private Action<EthernetFrame> _onReceiveAction;

        public void Init()
        {
            var vendorBytes = new byte[]{0x10,0x14,0x20};
            var guid = Guid.NewGuid().ToByteArray();
            MACAddress = new[] {vendorBytes[0], vendorBytes[1], vendorBytes[2], guid[0], guid[1], guid[2]};
        }

        public void ConnectTo(EthernetPort ethernetPort)
        {
            _connectedTo = ethernetPort;
            ethernetPort._connectedTo = this;
        }

        public void Receive(EthernetFrame frame)
        {
            _onReceiveAction(frame);
        }

        public void Send(EthernetFrame frame)
        {
            _connectedTo.Receive(frame);
        }

        public void OnReceive(Action<EthernetFrame> action)
        {
            _onReceiveAction = action;
        }
    }
}