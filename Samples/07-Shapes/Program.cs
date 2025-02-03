using System;

namespace DX12GameProgramming
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (ShapesApp app = new ShapesApp())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
