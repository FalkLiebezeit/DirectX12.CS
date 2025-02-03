using System;

namespace DX12GameProgramming
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (SphereMouseControlled app = new SphereMouseControlled())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
