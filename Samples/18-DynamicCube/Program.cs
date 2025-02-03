using System;

namespace DX12GameProgramming
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (DynamicCubeApp app = new DynamicCubeApp())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
