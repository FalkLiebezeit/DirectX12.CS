using System;

namespace DX12GameProgramming
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (movGaussGridApp app = new movGaussGridApp())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
