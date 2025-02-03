using System;

namespace DX12GameProgramming
{
    internal class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            using (movPeakApp app = new movPeakApp())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
