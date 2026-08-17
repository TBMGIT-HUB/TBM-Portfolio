namespace GameOfLife_C_
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int width = 81;
            int height = 31;
            Console.SetWindowSize(Math.Min(width + 2, Console.LargestWindowWidth),
                       Math.Min(height + 5, Console.LargestWindowHeight));
            Console.SetBufferSize(width + 2, height + 5);

            GameOfLife game = new GameOfLife(width, height);

            Console.WriteLine("Le jeu tourne indéfiniment.");
            Console.WriteLine("Appuyez sur Entrée pour arrêter...\n");

            int totalGenerations = game.RunUntilEnter();

            Console.Clear();
            Console.WriteLine($"Simulation arrêtée après {totalGenerations} générations.");
        }
    }
}
