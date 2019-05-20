
// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Collections.Generic;
using NICE.Layer2;

namespace NICE.Hardware.Abstraction
{
    public abstract class Device
    {
        private readonly Dictionary<string, EthernetPort> _ports = new Dictionary<string, EthernetPort>();

        protected Action<EthernetFrame, EthernetPort> _onReceive;

        protected Device(Action<EthernetFrame, EthernetPort> onReceive)
        {
            _onReceive = onReceive;
        }

        public EthernetPort this[string name]
        {
            get
            {
                if (_ports.ContainsKey(name))
                {
                    return _ports[name];
                }
                
                _ports[name] = new EthernetPort();

                _ports[name].OnReceive(frame => _onReceive(frame, _ports[name]));
                
                return _ports[name];
            }
        }
    }
}