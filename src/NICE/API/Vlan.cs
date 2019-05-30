using System;
using System.Collections.Generic;
using NICE.API.Abstraction;
using NICE.Foundation;

namespace NICE.API
{
    public class Vlan
    {
        private static readonly List<(int nr, string name)> Vlans = new List<(int, string)>();

        static Vlan()
        {
            Register(0, string.Empty);
        }
        
        public static void Register(int nr, string name)
        {
            if (Vlans.Exists(tuple => tuple.nr == nr && tuple.name == name))
            {
                throw new Exception($"VLAN {nr} already exists!");
            }

            Vlans.Add((nr, name));
        }

        public static Option<ushort> Get(int nr)
        {
            if (nr == 0)
            {
                return Option<ushort>.Of((ushort) 0);
            }

            if (!Vlans.Exists(tuple => tuple.nr == nr))
            {
                throw new Exception($"VLAN {nr} does not exist!");
            }

            byte b0 = (byte) nr,
                b1 = (byte) (nr >> 8);

            b1.Set(7, false);
            b1.Set(6, false);
            b1.Set(5, false);

            ushort vlanId = BitConverter.ToUInt16(new[] {b0, b1});

            return Option<byte[]>.Of(vlanId);
        }
    }
}