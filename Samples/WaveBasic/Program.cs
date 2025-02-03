namespace DX12GameProgramming
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (WaveApp app = new WaveApp())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
