using System;

namespace DX12GameProgramming
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (RaytracingDynEyeBasicApp app = new RaytracingDynEyeBasicApp())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
