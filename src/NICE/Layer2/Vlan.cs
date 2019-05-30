using NICE.API.Abstraction;

namespace NICE.Layer2
{
    public abstract class Vlan
    {
        public static void Register(int nr, string name)
        {
            API.Vlan.Register(nr, name);
        }

        public static Option<ushort> Get(int nr)
        {
            return API.Vlan.Get(nr);
        }
    }
}