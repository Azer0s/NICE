using System.Collections.Generic;
using System.Linq;
using NICE.Abstraction;
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable InconsistentNaming

namespace NICE.Layer2.STP
{
    /// <summary>
    /// 802.2 LLC Frame
    /// </summary>
    public class LLCFrame : Byteable<LLCFrame>
    {
        public byte[] LLC { get; set; } //3 byte
        public byte[] Data { get; set; }

        public static LLCFrame FromBytes(byte[] bytes)
        {
            var llcBytes = bytes.ToList();
            var llc = new LLCFrame
            {
                LLC = llcBytes.GetRange(0, 3).ToArray(),
                Data = llcBytes.GetRange(3, llcBytes.Count - 3).ToArray()
            };

            return llc;
        }

        LLCFrame Byteable<LLCFrame>.FromBytes(byte[] bytes)
        {
            return FromBytes(bytes);
        }

        public byte[] ToBytes()
        {
            var bytes = new List<byte>();
            bytes.AddRange(LLC);
            bytes.AddRange(Data);

            return bytes.ToArray();
        }
    }
}