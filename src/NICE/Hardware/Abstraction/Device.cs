
// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Collections;
using System.Collections.Generic;
using NICE.Foundation;
using NICE.Layer2;
// ReSharper disable InconsistentNaming

namespace NICE.Hardware.Abstraction
{
    public abstract class Device : IEnumerable<(string name, EthernetPort port)>
    {
        private readonly Dictionary<string, EthernetPort> _ports = new Dictionary<string, EthernetPort>();

        public readonly string Hostname;
        public bool PowerOn = true;
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

            if (Global.Devices.ContainsKey(Hostname))
            {
                throw new Exception("A device with the same hostname already exists!");
            }
            Global.Devices[Hostname] = this;
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
                    if (!PowerOn)
                    {
                        Log.Error(Hostname, "This device is turned off and can't receive frames!");
                        return;
                    }
                    
                    var frame = EthernetFrame.FromBytes(bytes);
                    OnReceive(frame, _ports[name]);
                });
                
                return _ports[name];
            }
        }

        public IEnumerator<(string name, EthernetPort port)> GetEnumerator()
        {
            foreach (var (key, value) in _ports)
            {
                yield return (key, value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}