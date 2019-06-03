using NICE.Foundation;

namespace NICE.API.Abstraction
{
    public class Option<T>
    {
        private IsSet _set = Foundation.IsSet.Unset;
        private T _value;

        public static Option<T> Of<T>(T input)
        {
            var opt = new Option<T>();
            opt.Set(input);
            return opt;
        }

        public bool IsSet()
        {
            return _set == Foundation.IsSet.Set;
        }

        public void Set(T value)
        {
            _set = Foundation.IsSet.Set;
            _value = value;
        }

        public T Get()
        {
            return _value;
        }

        public T GetOr(T val)
        {
            return IsSet() ? Get() : val;
        }

        public static implicit operator Option<T>(T value)
        {
            return Of(value);
        }
    }
}