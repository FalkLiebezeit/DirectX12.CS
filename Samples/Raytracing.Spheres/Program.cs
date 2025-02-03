using System;

namespace DX12GameProgramming
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (RaytracingSphereApp app = new RaytracingSphereApp())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
