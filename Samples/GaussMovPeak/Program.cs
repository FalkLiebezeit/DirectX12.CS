using System;

namespace DX12GameProgramming
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (GaussMovPeakApp app = new GaussMovPeakApp())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
