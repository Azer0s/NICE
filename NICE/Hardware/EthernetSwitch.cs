using System;
using System.Collections.Generic;
using System.Linq;
using NICE.Hardware.Abstraction;
using NICE.Layer2;

// ReSharper disable InconsistentNaming

namespace NICE.Hardware
{
    public class EthernetSwitch : Device
    {
        public enum AccessMode
        {
            TRUNK,
            ACCESS
        }
        private struct SwitchPortInfo
        {
            public AccessMode Mode;
            public byte[] Vlan;
        }
        
        private readonly Dictionary<string, SwitchPortInfo> _switchPortInfos = new Dictionary<string, SwitchPortInfo>();
        
        public EthernetSwitch() : base(null)
        {
            OnReceive = (frame, port) =>
            {
                //If an untagged frame comes in, tag it
                if (!frame.IsTagged)
                {
                    frame.IsTagged = true;
                    frame.Tag = Vlan.Get(1);
                    
                    frame.FCS = Util.GetFCS(frame);
                }
                
                //TODO: Check VLANs
                
                var dstPort = _switchPortInfos.Where(a => MACTable[a.Key].SequenceEqual(frame.Dst)).Select(a => a.Key).FirstOrDefault();
                    
                if (string.IsNullOrEmpty(dstPort))
                {
                    //Send to all trunk ports except for source port
                    var dstPorts = _switchPortInfos.Where(a => !MACTable[a.Key].SequenceEqual(frame.Src) && a.Value.Mode == AccessMode.TRUNK).Select(a => a.Key);

                    foreach (var s in dstPorts)
                    {
                        base[s].Send(frame);
                    }
                }
                else
                {
                    base[dstPort].Send(frame);
                }
                
                //TODO: Send the frame to the corresponding port of the dst mac
                //TODO: Like...we gotta find out which MAC belongs to which switch port
            };
        }
        
        public new EthernetPort this[string name]
        {
            get
            {
                if (!_switchPortInfos.ContainsKey(name))
                {
                    _switchPortInfos[name] = new SwitchPortInfo{Mode = AccessMode.ACCESS, Vlan = Vlan.Get(1)};
                }
                
                return base[name];
            }
        }

        public void SetPort(string port, AccessMode mode, byte[] vlan)
        {
            if (mode == AccessMode.TRUNK)
            {
                if (vlan != null)
                {
                    throw new Exception("Can't set VLAN for a trunking port!");
                }
            }
            
            _switchPortInfos[port] = new SwitchPortInfo {Mode = mode, Vlan = vlan};
        }
    }
}