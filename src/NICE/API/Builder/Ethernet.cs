using System;
using System.Collections.Generic;
using System.Linq;
using NICE.API.Abstraction;
using NICE.Foundation;
using NICE.Protocols.Ethernet;

namespace NICE.API.Builder
{
    public class Ethernet : Payloadable, IProtocol
    {
        public Option<byte[]> Src;
        public Option<byte[]> Dst;

        private Option<ushort> _etherType;
        public Option<ushort> EtherType
        {
            get => _etherType;
            set
            {
                if (Payload is Dot1Q dot1Q)
                {
                    dot1Q.EtherType = value;
                }
                else
                {
                    _etherType = value;
                }
            }
        }

        public byte[] ToBytes()
        {
            if (!Src.IsSet())
            {
                throw new Exception("Src address is unset!");
            }
            
            if (!Dst.IsSet())
            {
                throw new Exception("Dst address is unset!");
            }
            
            if (!EtherType.IsSet())
            {
                throw new Exception("EtherType is unset!");
            }
            
            var list = new List<byte>();
            list.AddRange(Dst.Get());
            list.AddRange(Src.Get());
            list.AddRange(BitConverter.GetBytes(EtherType.Get()).Reverse());
            list.AddRange(Payload.ToBytes());
            
            var fcs = Util.GetFCS(list.ToArray());
                        
            list.AddRange(fcs);
            
            return list.ToArray();
        }

        public EthernetFrame ToEthernetFrame()
        {
            var preamble = new EthernetFrame().Preamble;
            var bytes = preamble.ToList();
            bytes.Add(0xAB);
            bytes.AddRange(ToBytes());
            
            return EthernetFrame.FromBytes(bytes.ToArray());
        }

        public static implicit operator EthernetFrame(Ethernet ethernet)
        {
            return ethernet.ToEthernetFrame();
        }
    }
}