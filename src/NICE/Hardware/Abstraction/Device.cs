
// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Collections.Generic;
using NICE.Layer2;
// ReSharper disable InconsistentNaming

namespace NICE.Hardware.Abstraction
{
    public abstract class Device
    {
        private readonly Dictionary<string, EthernetPort> _ports = new Dictionary<string, EthernetPort>();

        public readonly string Hostname;
        protected readonly Func<string, Action<EthernetPort>> OnConnect;
        protected readonly Func<string, Action<EthernetPort>> OnDisconnect;
        
        protected Action<EthernetFrame, EthernetPort> OnReceive;
        
        protected Device(string hostname, Action<EthernetFrame, EthernetPort> onReceive, Func<string, Action<EthernetPort>> onConnect = null, Func<string, Action<EthernetPort>> onDisconnect = null)
        {
            Hostname = hostname;
            OnReceive = onReceive;
            if (onConnect == null)
            {
                OnConnect = s => { return ethernetPort => {}; };
            }
            else
            {
                OnConnect = onConnect;
            }

            if (onDisconnect == null)
            {
                OnDisconnect = s => { return ethernetPort => {}; };
            }
            else
            {
                OnDisconnect = onDisconnect;
            }
        }

        public EthernetPort this[string name]
        {
            get
            {
                if (_ports.ContainsKey(name))
                {
                    return _ports[name];
                }
                
                Log.Debug(Hostname, $"Creating new port {name}...");
                _ports[name] = new EthernetPort(name, this, OnConnect(name), OnDisconnect(name));

                _ports[name].OnReceive(bytes =>
                {
                    var frame = EthernetFrame.FromBytes(bytes);
                    OnReceive(frame, _ports[name]);
                });
                
                return _ports[name];
            }
        }
    }
}