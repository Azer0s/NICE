using System;
using System.Threading.Tasks;
using NICE.API.Abstraction;
using NICE.Foundation;
using NICE.Hardware.Abstraction;
using NICE.Layer2;
using NICE.Protocols.Ethernet;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

namespace NICE.Hardware
{
    public class EthernetPort : Stateable
    {
        public byte[] MACAddress { get; private set; }
        public readonly string Name;
        private Action<byte[]> _onReceiveAction;
        private EthernetPort _connectedTo;
        private readonly Action<EthernetPort> _onConnectAction;
        private readonly Action<EthernetPort> _onDisconnectAction;
        private readonly Device _device;
        public ref readonly string ConnectedToHostname => ref _connectedTo._device.Hostname;

        public static EthernetPort CreateMock(byte[] mac, string name, Action<byte[]> onReceive)
        {
            if (mac.Length != 6)
            {
                throw new Exception("Invalid MAC Address length!");
            }
            
            return new EthernetPort(name, null,port => {}, port => {}){MACAddress = mac, _onReceiveAction = onReceive};
        }

        public static implicit operator Option<byte[]>(EthernetPort port)
        {
            var option = new Option<byte[]>();
            option.Set(port.MACAddress);
            return option;
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
            Log.Trace(_device.Hostname, $"Initialized port {Name} with MAC Address {MACAddress.ToMACAddressString()}");
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
            Log.Info(_device.Hostname, $"Disconnecting {Name} from {_connectedTo.Name} on {ConnectedToHostname}");
            _onDisconnectAction(_connectedTo);
            _connectedTo._onDisconnectAction(this);
            _connectedTo._connectedTo = null;
            _connectedTo = null;
        }

        public void Receive(byte[] bytes)
        {
            _onReceiveAction(bytes);
        }

        public void Send(byte[] bytes,  bool sendAsync, bool log = true)
        {
            if (!_device.PowerOn)
            {
                Log.Error(_device.Hostname, "This device is turned off and can't send frames!");
                return;
            }
            
            //TODO: Handle blocked STP ports, handle STP designate ports
            
            Global.Operations++;
            if (log)
            {
                Log.Trace(_device.Hostname, $"Sending bytes from {Name} to {_connectedTo.Name} on {ConnectedToHostname}");
            }

            var action = new Action(() =>
            {
                try
                {
                    _connectedTo.Receive(bytes);
                }
                catch (Exception e)
                {
                    Log.Error(_device.Hostname, e);
                }
                Global.Operations--;
            });

            if (!sendAsync)
            {
                action();
            }
            else
            {
                //KIND OF VERY EXPERIMENTAL
                Task.Run(action);
            }
        }

        public void Send(EthernetFrame frame, bool sendAsync)
        {
            Log.Trace(_device.Hostname, $"Sending Ethernet frame to {frame.MacHeader.Dst} from port {Name}");
            Send(frame.ToBytes(), sendAsync, false);
        }

        public void SendSync(EthernetFrame frame)
        {
            Send(frame, false);
        }

        public void SendAsync(EthernetFrame frame)
        {
            Send(frame, true);
        }

        public void OnReceive(Action<byte[]> action)
        {
            _onReceiveAction = action;
        }
    }
}