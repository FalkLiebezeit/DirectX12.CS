namespace DX12GameProgramming
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (Gauss3DApp app = new Gauss3DApp())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
