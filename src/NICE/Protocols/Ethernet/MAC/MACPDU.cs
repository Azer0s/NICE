using System;
using System.Collections.Generic;
using System.Linq;
using NICE.Abstraction;
using NICE.Foundation;

namespace NICE.Protocols.Ethernet.MAC
{
    public class MACPDU : IByteable<MACPDU>
    {
        public MACHeader Header;
        public IMACCompatible Payload;
        public byte[] FCS;


        public static MACPDU FromBytes(byte[] bytes)
        {
            var byteList = bytes.ToList();

            var bytesToCheck = byteList.GetRange(0, byteList.Count - 4);
            var fcs = Util.GetFCS(bytesToCheck.ToArray());
            
            var bytesFcs = byteList.GetRange(byteList.Count - 4, 4);

            if (!fcs.SequenceEqual(bytesFcs))
            {
                throw new Exception("FCS must match!");
            }
            
            var header = MACHeader.FromBytes(bytes.Take(14).ToArray());
            var payload = byteList.GetRange(14, byteList.Count - 18).ToArray();
            var protocol = MACRegistry.GetProtocol(header.EtherType);

            var frame = new MACPDU
            {
                Header = header,
                Payload = protocol.FromBytes(payload),
                FCS = fcs
            };

            return frame;
        }

        MACPDU IByteable<MACPDU>.FromBytes(byte[] bytes)
        {
            return FromBytes(bytes);
        }

        public byte[] ToBytes()
        {
            var list = new List<byte>();
            list.AddRange(Header.ToBytes());
            list.AddRange(Payload.ToBytes());
            list.AddRange(FCS);
            
            return list.ToArray();
        }
    }
}