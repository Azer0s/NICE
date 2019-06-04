using NICE.API.Builder;

namespace NICE.API.Abstraction
{
    public class Wrapped<T>
    {
        public T Value;

        public static Ethernet operator < (Ethernet ethernet, Wrapped<T> value)
        {
            return (dynamic)ethernet | (dynamic)value.Value;
        }

        public static Ethernet operator > (Ethernet ethernet, Wrapped<T> value)
        {
            throw new System.NotImplementedException();
        }
    }
}