namespace DX12GameProgramming
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (TexColumnsApp app = new TexColumnsApp())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
