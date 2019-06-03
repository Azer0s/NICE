using System;
using System.Collections.Generic;
using System.Linq;
using NICE.API.Abstraction;
using NICE.Foundation;
using NICE.Hardware.Abstraction;
using NICE.Layer2;
using NICE.Protocols.Ethernet.Dot1Q;
using NICE.Protocols.Ethernet.MAC;

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

        private readonly Dictionary<string, SwitchPortInfo> _switchPortInfos = new Dictionary<string, SwitchPortInfo>();

        private readonly Dictionary<ushort /*VLAN*/, Dictionary<MACAddress /*MAC Address*/, string /*Port*/>> MACTable = new Dictionary<ushort, Dictionary<MACAddress, string>>();

        public EthernetSwitch(string name) : base(name, null)
        {
            Log.Info(Hostname, "Initializing switch...");

            OnReceive = (frame, port) =>
            {
                if (frame.Data.Header.EtherType <= 1500)
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
                if (!(frame.Data.Payload is Dot1QPDU))
                {
                    var type = frame.Data.Header.EtherType;
                    var payload = frame.Data.Payload;

                    var dot1q = new Dot1QPDU
                    {
                        Header = new Dot1QHeader
                        {
                            Type = type,
                            VlanID = _switchPortInfos[port.Name].Vlan == 0
                                ? Vlan.Get(1).Get()
                                : _switchPortInfos[port.Name].Vlan
                        },
                        Payload = payload
                    };

                    frame.Data.Payload = dot1q;
                    frame.Data.Header.EtherType = 0x8100;

                    frame.Data.FCS = Util.GetFCS(frame);
                }

                lock (MACTable)
                {
                    if (!(MACTable.Any(a => a.Key == ((Dot1QPDU) frame.Data.Payload).Header.VlanID)
                          && MACTable
                              .Where(a => a.Key == ((Dot1QPDU) frame.Data.Payload).Header.VlanID)
                              .Select(a => a.Value).FirstOr(new Dictionary<MACAddress, string>())
                              .Any(a => a.Key == frame.Data.Header.Src)))
                    {
                        Log.Warn(Hostname, $"Unknown MAC Address {frame.Data.Header.Src} for VLAN {((Dot1QPDU) frame.Data.Payload).Header.VlanID.ToMACAddressString()}");
                        Log.Debug(Hostname, $"Adding MAC Address {frame.Data.Header.Src} to MAC Address table for VLAN {((Dot1QPDU) frame.Data.Payload).Header.VlanID.ToMACAddressString()}...");

                        if (MACTable.All(a => a.Key != ((Dot1QPDU) frame.Data.Payload).Header.VlanID))
                        {
                            MACTable[((Dot1QPDU) frame.Data.Payload).Header.VlanID] = new Dictionary<MACAddress, string>();
                        }

                        var id = MACTable.Where(a => a.Key == ((Dot1QPDU) frame.Data.Payload).Header.VlanID).Select(a => a.Key).First();
                        MACTable[id].Add(frame.Data.Header.Src, port.Name);
                    }
                }

                var dstPort = string.Empty;
                if (frame.Data.Header.Dst != Constants.ETHERNET_BROADCAST_ADDRESS.Get())
                {
                    dstPort = MACTable.Where(a => a.Key == ((Dot1QPDU) frame.Data.Payload).Header.VlanID && a.Value.Any(b => b.Key == frame.Data.Header.Dst)).Select(a => a.Value.Where(b => b.Key == frame.Data.Header.Dst).Select(b => b.Value).FirstOr(null)).FirstOr(null);
                }

                //Flooding
                if (string.IsNullOrEmpty(dstPort))
                {
                    //Send to all ports except for source port
                    //Send to all access ports in the same VLAN
                    //Send to all trunk ports
                    var dstPorts = _switchPortInfos.Where(a => a.Key != port.Name && (a.Value.Vlan == ((Dot1QPDU) frame.Data.Payload).Header.VlanID || a.Value.Mode == AccessMode.TRUNK)).Select(a => a.Key).ToList();

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
            
            PostConstruct();
        }

        public new EthernetPort this[string name]
        {
            get
            {
                if (!_switchPortInfos.ContainsKey(name))
                {
                    _switchPortInfos[name] = new SwitchPortInfo {Mode = AccessMode.ACCESS, Vlan = Vlan.Get(1).Get()};
                }

                return base[name];
            }
        }

        public void SetPort(string port, AccessMode mode, Option<ushort> vlan)
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
                Log.Debug(Hostname, $"Accessing VLAN {vlan.Get()} on port {port}");
            }
            else
            {
                vlan = Vlan.Get(1);
            }

            _switchPortInfos[port] = new SwitchPortInfo {Mode = mode, Vlan = vlan.Get()};
        }

        private struct SwitchPortInfo
        {
            public AccessMode Mode;
            public ushort Vlan;
        }
    }
}