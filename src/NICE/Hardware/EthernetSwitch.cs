using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
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

        private readonly Dictionary<byte[], string> MACTable = new Dictionary<byte[], string> ();
        private readonly Dictionary<string, SwitchPortInfo> _switchPortInfos = new Dictionary<string, SwitchPortInfo>();
        
        public EthernetSwitch(string name) : base(name,null)
        {            
            Log.Info(Hostname, "Initializing switch...");
            
            OnReceive = (frame, port) =>
            {
                if (!MACTable.Any(a => a.Key.SequenceEqual(frame.Src)))
                {
                    Log.Warn(Hostname, $"Unknown MAC Address {string.Join(":", frame.Src.Select(a => a.ToString("X2")))}");
                    Log.Debug(Hostname, $"Adding MAC Address {string.Join(":", frame.Src.Select(a => a.ToString("X2")))} to MAC Address table...");
                    MACTable[frame.Src] = port.Name;
                }
                
                //If an untagged frame comes in, tag it
                if (!frame.IsTagged)
                {
                    frame.IsTagged = true;
                    frame.Tag = Vlan.Get(1);
                    
                    frame.FCS = Util.GetFCS(frame);
                }

                var dstPort = string.Empty;
                if (!frame.Dst.SequenceEqual(Constants.ETHERNET_BROADCAST_ADDRESS))
                {
                    dstPort = MACTable.Where(a => a.Key.SequenceEqual(frame.Dst)).Select(a => a.Value).FirstOrDefault();
                }

                //TODO: Check VLANs
                    
                if (string.IsNullOrEmpty(dstPort))
                {
                    //Send to all ports except for source port
                    var dstPorts = _switchPortInfos.Where(a => a.Key != MACTable[frame.Src]).Select(a => a.Key);

                    foreach (var s in dstPorts)
                    {
                        base[s].Send(frame);
                    }
                }
                else
                {
                    base[dstPort].Send(frame);
                }
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
            Log.Info(Hostname, $"Setting port {port} to {mode.ToString().ToLower()} mode");
            if (mode == AccessMode.TRUNK)
            {
                if (vlan != null)
                {
                    throw new Exception("Can't set VLAN for a trunking port!");
                }
            }

            if (vlan != null)
            {
                Log.Debug(Hostname, $"Accessing VLAN {string.Join(":", vlan.Select(a => a.ToString("X2")))} on port {port}");
            }
            
            _switchPortInfos[port] = new SwitchPortInfo {Mode = mode, Vlan = vlan};
        }
    }
}