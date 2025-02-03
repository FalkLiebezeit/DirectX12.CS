using System;

namespace DX12GameProgramming
{
    internal class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            using (wiredFountainApp app = new wiredFountainApp())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
