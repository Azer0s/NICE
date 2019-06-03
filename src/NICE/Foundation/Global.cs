using System.Collections.Generic;
using System.Threading.Tasks;
using NICE.Hardware.Abstraction;

namespace NICE.Foundation
{
    public static class Global
    {
        public static int Operations = 0;
        public static Dictionary<string, Device> Devices { get; } = new Dictionary<string, Device>();

        public static async Task WaitForOperationsFinished()
        {
            await Task.Run(() =>
            {
                while (Operations != 0)
                {
                }
            });
        }
    }
}