using System;

namespace DX12GameProgramming
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (ShapesList app = new ShapesList())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
