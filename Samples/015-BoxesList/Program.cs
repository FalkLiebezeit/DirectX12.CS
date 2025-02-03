using System;

namespace DX12GameProgramming
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (BoxesList app = new BoxesList())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
