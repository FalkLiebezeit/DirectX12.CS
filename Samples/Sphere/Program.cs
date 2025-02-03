using System;

namespace DX12GameProgramming
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (SimpleSphere app = new SimpleSphere())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
