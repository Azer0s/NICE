using System;
using System.Collections.Generic;
using NICE.Foundation;

namespace NICE.Layer2
{
    public static class Vlan
    {
        private static readonly List<(int nr, string name)> Vlans = new List<(int, string)>();
        
        public static void Register(int nr, string name)
        {
            if (Vlans.Exists(tuple => tuple.nr == nr && tuple.name == name))
            {
                throw new Exception($"VLAN {nr} already exists!");
            }
            
            Vlans.Add((nr, name));
        }

        public static byte[] Get(int nr)
        {
            if (nr == 0)
            {
                return new byte[2];
            }
            
            if (!Vlans.Exists(tuple => tuple.nr == nr))
            {
                throw new Exception($"VLAN {nr} does not exist!");
            }
            
            byte b0 = (byte)nr,
                b1 = (byte)(nr>>8);

            b1.Set(7, false);
            b1.Set(6, false);
            b1.Set(5, false);
            
            return new []{b1, b0};
        }
    }
}