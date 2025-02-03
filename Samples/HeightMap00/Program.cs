namespace DX12GameProgramming
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (HeightMapApp app = new HeightMapApp())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
