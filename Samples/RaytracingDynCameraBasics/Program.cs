using System;

namespace DX12GameProgramming
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (RaytracingDynCameraBasicApp app = new RaytracingDynCameraBasicApp())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
