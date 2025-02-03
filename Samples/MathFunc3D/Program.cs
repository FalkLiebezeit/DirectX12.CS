namespace DX12GameProgramming
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (MathFunc3DApp app = new MathFunc3DApp())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
