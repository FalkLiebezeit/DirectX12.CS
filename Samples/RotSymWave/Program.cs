using System;

namespace DX12GameProgramming
{
    internal class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            using (RotSymWaveApp app = new RotSymWaveApp())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
