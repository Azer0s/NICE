
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

        protected readonly Dictionary<string, byte[]> MACTable = new Dictionary<string, byte[]>();
        
        protected Action<EthernetFrame, EthernetPort> OnReceive;
        
        protected Device(Action<EthernetFrame, EthernetPort> onReceive)
        {
            OnReceive = onReceive;
        }

        public EthernetPort this[string name]
        {
            get
            {
                if (_ports.ContainsKey(name))
                {
                    return _ports[name];
                }
                
                _ports[name] = new EthernetPort(port => { MACTable[name] = port.MACAddress; }, port => MACTable.Remove(name));

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