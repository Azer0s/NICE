using System;
using System.Collections.Generic;
using System.Linq;
using NICE.Abstraction;
using NICE.Foundation;

// ReSharper disable PossibleNullReferenceException

namespace NICE.Protocols.Ethernet.MAC
{
    public class MACAddress : IByteable<MACAddress>
    {
        public byte[] Id;
        public byte[] Oui;

        MACAddress IByteable<MACAddress>.FromBytes(byte[] bytes)
        {
            return FromBytes(bytes);
        }

        public byte[] ToBytes()
        {
            var list = new List<byte>();
            list.AddRange(Oui);
            list.AddRange(Id);
            return list.ToArray();
        }

        public override string ToString()
        {
            return ToBytes().ToMACAddressString();
        }

        public static MACAddress FromBytes(byte[] bytes)
        {
            var mac = new MACAddress();

            if (bytes.Length != 6) throw new ArgumentException("MAC Address must be 6 bytes long!", nameof(bytes));

            mac.Oui = bytes.Take(3).ToArray();
            mac.Id = bytes.Skip(3).Take(3).ToArray();

            return mac;
        }

        public static bool operator ==(MACAddress a, MACAddress b)
        {
            return a.ToBytes().SequenceEqual(b.ToBytes());
        }

        public static bool operator ==(MACAddress a, byte[] b)
        {
            return a.ToBytes().SequenceEqual(b ?? throw new ArgumentNullException(nameof(b)));
        }

        public static bool operator !=(MACAddress a, byte[] b)
        {
            return !(a == b);
        }

        public static bool operator !=(MACAddress a, MACAddress b)
        {
            return !(a == b);
        }
    }
}