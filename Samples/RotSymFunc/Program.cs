namespace DX12GameProgramming
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (RotSymFuncApp app = new RotSymFuncApp())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
