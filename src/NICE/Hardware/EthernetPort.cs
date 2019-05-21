using System;
using System.Linq;
using System.Threading.Tasks;
using NICE.Hardware.Abstraction;
using NICE.Layer2;
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

namespace NICE.Hardware
{
    public class EthernetPort
    {
        public byte[] MACAddress { get; private set; }
        public readonly string Name;
        private Action<byte[]> _onReceiveAction;
        private EthernetPort _connectedTo;
        private readonly Action<EthernetPort> _onConnectAction;
        private readonly Action<EthernetPort> _onDisconnectAction;
        private readonly Device _device;

        public static EthernetPort CreateMock(byte[] mac, string name, Action<byte[]> onReceive)
        {
            if (mac.Length != 6)
            {
                throw new Exception("Invalid MAC Address length!");
            }
            
            return new EthernetPort(name, null,port => {}, port => {}){MACAddress = mac, _onReceiveAction = onReceive};
        }
        
        public EthernetPort(string name, Device device, Action<EthernetPort> onConnect, Action<EthernetPort> onDisconnect)
        {
            _onConnectAction = onConnect;
            _onDisconnectAction = onDisconnect;
            _device = device;
            Name = name;
        }
        
        public void Init()
        {
            var vendorBytes = new byte[]{0x10,0x14,0x20};
            var guid = Guid.NewGuid().ToByteArray();
            MACAddress = new[] {vendorBytes[0], vendorBytes[1], vendorBytes[2], guid[0], guid[1], guid[2]};
            Log.Trace(_device.Hostname, $"Initialized port {Name} with MAC Address {string.Join(":", MACAddress.Select(a => a.ToString("X2")))}");
        }

        public void ConnectTo(EthernetPort ethernetPort)
        {
            Log.Info(_device.Hostname, $"Connecting {Name} to {ethernetPort.Name} on {ethernetPort._device.Hostname}");
            _connectedTo = ethernetPort;
            ethernetPort._connectedTo = this;
            _onConnectAction(ethernetPort);
            ethernetPort._onConnectAction(this);
        }

        public void Disconnect()
        {
            Log.Info(_device.Hostname, $"Disconnecting {Name} from {_connectedTo.Name} on {_connectedTo._device.Hostname}");
            _onDisconnectAction(_connectedTo);
            _connectedTo._onDisconnectAction(this);
            _connectedTo._connectedTo = null;
            _connectedTo = null;
        }

        public void Receive(byte[] bytes)
        {
            _onReceiveAction(bytes);
        }

        public void Send(byte[] bytes)
        {
            _connectedTo.Receive(bytes);
        }

        public void Send(EthernetFrame frame)
        {
            Send(frame.GetBytes());
        }

        public void OnReceive(Action<byte[]> action)
        {
            _onReceiveAction = action;
        }
    }
}