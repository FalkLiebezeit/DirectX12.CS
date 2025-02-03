using System;

namespace DX12GameProgramming
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (ShapesWired app = new ShapesWired())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
