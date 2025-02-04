using System;

namespace DX12GameProgramming
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (ShapesBasics app = new ShapesBasics())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
