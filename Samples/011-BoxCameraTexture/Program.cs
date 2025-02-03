using System;

namespace DX12GameProgramming
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (BoxCameraApp app = new BoxCameraApp())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
