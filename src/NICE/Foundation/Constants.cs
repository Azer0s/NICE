using NICE.API.Abstraction;
using NICE.Hardware;

// ReSharper disable InconsistentNaming

namespace NICE.Foundation
{
    public static class Constants
    {
        public static readonly Option<byte[]> ETHERNET_BROADCAST_ADDRESS = Option<byte[]>.Of(new byte[] {0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF});
        public static readonly EthernetPort ETHERNET_BROADCAST_PORT = EthernetPort.CreateMock(ETHERNET_BROADCAST_ADDRESS.Get(), "BROADCAST", bytes => { });
    }
}