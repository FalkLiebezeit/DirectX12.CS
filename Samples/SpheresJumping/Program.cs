using System;

namespace DX12GameProgramming
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (SpheresJumping app = new SpheresJumping())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
