namespace DX12GameProgramming
{
    internal class Program
    {
       
        static void Main(string[] args)
        {
            using (ShapesModelApp app = new ShapesModelApp())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
