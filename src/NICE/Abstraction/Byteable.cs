namespace NICE.Abstraction
{
    public interface Byteable<T>
    {
        T FromBytes(byte[] bytes);
        byte[] ToBytes();
    }
}