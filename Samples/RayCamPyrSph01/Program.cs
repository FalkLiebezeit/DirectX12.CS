using System;

namespace DX12GameProgramming
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (RaytracingCamPyrSpheresApp app = new RaytracingCamPyrSpheresApp())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
