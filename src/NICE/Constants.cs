using NICE.Hardware;
// ReSharper disable InconsistentNaming

namespace NICE
{
    public static class Constants
    {
        public static readonly byte[] ETHERNET_BROADCAST_ADDRESS = {0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
        public static readonly EthernetPort ETHERNET_BROADCAST_PORT = EthernetPort.CreateMock(ETHERNET_BROADCAST_ADDRESS, "BROADCAST", bytes => { });
    }
}