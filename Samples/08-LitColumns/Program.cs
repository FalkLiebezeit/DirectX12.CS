namespace DX12GameProgramming
{
    internal class Program
    {
       
        static void Main(string[] args)
        {
            using (LitColumnsApp app = new LitColumnsApp())
            {
                app.Initialize();
                app.Run();
            }
        }
    }
}
