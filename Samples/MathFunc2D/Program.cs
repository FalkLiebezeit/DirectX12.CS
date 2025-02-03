namespace DX12GameProgramming
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (MathFunc2DApp app = new MathFunc2DApp())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
