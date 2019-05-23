using System.Collections.Generic;
using System.Linq;
using NICE.Abstraction;
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

namespace NICE.Layer2.STP
{
    public class SNAPFrame : IByteable<SNAPFrame>
    {
        public byte[] SNAP { get; set; } //5 bytes
        public byte[] Data { get; set; }

        public static SNAPFrame FromBytes(byte[] bytes)
        {
            var snapBytes = bytes.ToList();
            var snap = new SNAPFrame
            {
                SNAP = snapBytes.GetRange(0, 5).ToArray(),
                Data = snapBytes.GetRange(5, snapBytes.Count - 5).ToArray()
            };

            return snap;
        }
        
        SNAPFrame IByteable<SNAPFrame>.FromBytes(byte[] bytes)
        {
            return FromBytes(bytes);
        }

        public byte[] ToBytes()
        {
            var bytes = new List<byte>();
            bytes.AddRange(SNAP);
            bytes.AddRange(Data);

            return bytes.ToArray();
        }
    }
}