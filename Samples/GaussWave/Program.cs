using System;

namespace DX12GameProgramming
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (GaussWaveApp app = new GaussWaveApp())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
