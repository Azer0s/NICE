using System;
using System.Linq;
using NICE.API.Abstraction;

// ReSharper disable InconsistentNaming

namespace NICE.API
{
    public static class MACAddress
    {
        public static readonly Option<byte[]> Unset = new Option<byte[]>();

        public static Option<byte[]> Of(string address)
        {
            var bytes = address.Split(":").Select(Byte.Parse).ToArray();
            return Option<byte[]>.Of(bytes);
        }
    }
}