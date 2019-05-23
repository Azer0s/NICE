using System;
using NICE.Foundation;
using NICE.Hardware;
using NICE.Layer3;
// ReSharper disable InconsistentNaming

namespace NICE.Layer2.STP
{
    public class STPFrame
    {
        /// <summary>
        /// +----+----+------+------+------+------+-----+
        /// | DA | SA | Len  | LLC  | SNAP | Data | FCS |
        /// +----+----+------+------+------+------+-----+
        /// 
        /// DA      Destination MAC Address (6 bytes)
        /// SA      Source MAC Address      (6 bytes)
        /// Len     Length of Data field    (2 bytes: <= 0x05DC or 1500 decimal)
        /// LLC     802.2 LLC Header        (3 bytes)
        /// SNAP                            (5 bytes)
        /// Data    Protocol Data           (46 - 1500 bytes)
        /// FCS     Frame Checksum          (4 bytes)
        /// </summary>
        /// <param name="dst"></param>
        /// <param name="src"></param>
        /// <returns></returns>
        public static EthernetFrame NewFrame(EthernetPort dst, EthernetPort src)
        {
            var frame = new EthernetFrame(dst, src, null, EtherType.NONE, new RawLayer3Packet(new byte[1]), false);
            var llc = new LLCFrame();
            var snap = new SNAPFrame();
            var bpdu = new BPDUFrame();

            snap.Data = bpdu.ToBytes();
            llc.Data = snap.ToBytes();
            frame.Data = llc.ToBytes();

            frame.FCS = Util.GetFCS(frame);
            frame.Type = BitConverter.GetBytes((ushort) frame.ToBytes().Length);
            
            return frame;
        }
    }
}