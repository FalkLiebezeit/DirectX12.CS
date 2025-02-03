using System;

namespace DX12GameProgramming
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (SpheresJumpingCamera app = new SpheresJumpingCamera())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
