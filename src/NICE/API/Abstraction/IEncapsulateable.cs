namespace NICE.API.Abstraction
{
    public interface IEncapsulateable<T>
    {
        void Encapsulate(ref T frame);
    }
}