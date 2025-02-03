namespace DX12GameProgramming
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (WavesApp app = new WavesApp())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
