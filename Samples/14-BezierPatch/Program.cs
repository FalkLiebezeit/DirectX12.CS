using System;

namespace DX12GameProgramming
{
    internal class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            using (BezierPatchApp app = new BezierPatchApp())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
