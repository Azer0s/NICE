using NICE.API.Builder;

namespace NICE.Layer3
{
    public interface Layer3Packet
    {
        Ethernet Merge(Ethernet ethernet);
    }
}