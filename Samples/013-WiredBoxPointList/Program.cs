namespace DX12GameProgramming
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (WBoxApp app = new WBoxApp())
            {
                app.Initialize();   // ->  WBoxApp.Initialze()  -> base.Initialize()
                app.Run();          //  -> D3DApp.Run();
            }
        }
    }
}
