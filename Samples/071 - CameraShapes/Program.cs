using System;

namespace DX12GameProgramming
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (CameraShapesApp app = new CameraShapesApp())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
