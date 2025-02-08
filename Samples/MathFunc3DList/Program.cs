using System;

namespace DX12GameProgramming
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (MathFunc3DList app = new MathFunc3DList())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
