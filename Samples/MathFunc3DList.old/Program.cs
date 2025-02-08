namespace DX12GameProgramming
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (MathFunc3DListApp app = new MathFunc3DListApp())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
