using System;

namespace DX12GameProgramming
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (ShapesMoving app = new ShapesMoving())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
