namespace NICE.Abstraction
{
    public interface IByteable<out T>
    {
        T FromBytes(byte[] bytes);
        byte[] ToBytes();
    }
}