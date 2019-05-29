using System;
using System.Collections.Generic;
using System.Linq;

namespace NICE.Protocols.Ethernet.MAC
{
    public static class MACRegistry
    {
        private static readonly Dictionary<ushort, IMACCompatible> ProtocolMapping = new Dictionary<ushort, IMACCompatible>();

        static MACRegistry()
        {
            var interfaceType = typeof(IMACCompatible);
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()))
            {
                if (!interfaceType.IsAssignableFrom(type)) continue;
                if (type == interfaceType) continue;

                var protocol = (IMACCompatible) Activator.CreateInstance(type);
                var etherType = protocol.EtherType;

                ProtocolMapping.Add(etherType, protocol);
            }
        }

        public static IMACCompatible GetProtocol(ushort type)
        {
            if (type <= 1500) return ProtocolMapping[0];

            if (type >= 0x0600) return ProtocolMapping[type];

            throw new Exception();
        }
    }
}