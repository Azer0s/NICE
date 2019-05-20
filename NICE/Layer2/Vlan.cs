using System;
using System.Collections.Generic;

namespace NICE.Layer2
{
    public static class Vlan
    {
        private static List<(int nr, string name)> _vlans = new List<(int, string)>();
        
        public static void Register(int nr, string name)
        {
            if (_vlans.Exists(tuple => tuple.nr == nr && tuple.name == name))
            {
                throw new Exception($"VLAN {nr} already exists!");
            }
            
            _vlans.Add((nr, name));
        }

        public static byte[] Get(int nr)
        {
            if (!_vlans.Exists(tuple => tuple.nr == nr))
            {
                throw new Exception($"VLAN {nr} does not exist!");
            }
            
            byte b0 = (byte)nr,
                b1 = (byte)(nr>>8);

            return new []{(byte)(b1 & 0b0001_1111), b0};
        }
    }
}