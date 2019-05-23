using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using NICE.Foundation;
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

        private readonly Dictionary<byte[] /*VLAN*/, Dictionary<byte[] /*MAC Address*/, string /*Port*/>> MACTable = new Dictionary<byte[], Dictionary<byte[], string>> ();
        private readonly Dictionary<string, SwitchPortInfo> _switchPortInfos = new Dictionary<string, SwitchPortInfo>();
        
        public EthernetSwitch(string name) : base(name,null)
        {            
            Log.Info(Hostname, "Initializing switch...");
            
            OnReceive = (frame, port) =>
            {
                if (frame.Type.ToUshort() <= 1500 )
                {
                    //Ok...so apparently, an Ethernet 2 frame is constructed the same way a normal Ethernet Frame is
                    //The only difference is in the Type field
                    //In Ethernet 2 this is 2 bytes which, in decimal, is >= 1536
                    //And in Ethernet <= 1500
                    //We also expect this frame to be untagged
                    //https://networkengineering.stackexchange.com/questions/5300/what-is-the-difference-between-ethernet-ii-and-802-3-ethernet
                    
                    //TODO: Handle STP BPDU
                    return;
                }
                
                //If an untagged frame comes in, tag it
                if (!frame.IsTagged)
                {
                    frame.IsTagged = true;
                    frame.Tag = _switchPortInfos[port.Name].Vlan ?? Vlan.Get(1);
                    frame.FCS = Util.GetFCS(frame);
                }

                lock (MACTable)
                {
                    if (!(MACTable.Any(a => a.Key.SequenceEqual(frame.Tag)) 
                          && MACTable
                              .Where(a => a.Key.SequenceEqual(frame.Tag))
                              .Select(a => a.Value).FirstOr(new Dictionary<byte[], string>())
                              .Any(a => a.Key.SequenceEqual(frame.Src))))
                    {
                        Log.Warn(Hostname, $"Unknown MAC Address {frame.Src.ToMACAddressString()} for VLAN {frame.Tag.ToMACAddressString()}");
                        Log.Debug(Hostname, $"Adding MAC Address {frame.Src.ToMACAddressString()} to MAC Address table for VLAN {frame.Tag.ToMACAddressString()}...");

                        if (!MACTable.Any(a => a.Key.SequenceEqual(frame.Tag)))
                        {
                            MACTable[frame.Tag] = new Dictionary<byte[], string>();
                        }

                        var id = MACTable.Where(a => a.Key.SequenceEqual(frame.Tag)).Select(a => a.Key).First();
                        MACTable[id].Add(frame.Src, port.Name);
                    }
                }

                var dstPort = string.Empty;
                if (!frame.Dst.SequenceEqual(Constants.ETHERNET_BROADCAST_ADDRESS))
                {
                    dstPort = MACTable.Where(a => a.Key.SequenceEqual(frame.Tag) && a.Value.Any(b => b.Key.SequenceEqual(frame.Dst))).Select(a => a.Value.Where(b => b.Key.SequenceEqual(frame.Dst)).Select(b => b.Value).FirstOr(null)).FirstOr(null);
                }
                                 
                //Flooding
                if (string.IsNullOrEmpty(dstPort))
                {
                    //Send to all ports except for source port
                    //Send to all access ports in the same VLAN
                    //Send to all trunk ports
                    var dstPorts = _switchPortInfos.Where(a => a.Key != port.Name && (a.Value.Vlan.SequenceEqual(frame.Tag) || a.Value.Mode == AccessMode.TRUNK)).Select(a => a.Key);

                    if (!dstPorts.Any())
                    {
                        Log.Error(Hostname, "Can't send a frame to any possible port (Possible VLAN mismatch)! Dropping!");
                        return;
                    }
                    
                    foreach (var s in dstPorts)
                    {
                        base[s].Send(frame, true);
                    }
                }
                else
                {
                    base[dstPort].Send(frame, true);
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
                Log.Debug(Hostname, $"Accessing VLAN {vlan.ToMACAddressString()} on port {port}");
            }
            else
            {
                vlan = Vlan.Get(1);
            }
            
            _switchPortInfos[port] = new SwitchPortInfo {Mode = mode, Vlan = vlan};
        }
    }
}