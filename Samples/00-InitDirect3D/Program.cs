namespace DX12GameProgramming
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (InitDirect3DApp app = new InitDirect3DApp())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
