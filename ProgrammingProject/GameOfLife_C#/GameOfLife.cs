using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife_C_
{
    internal class GameOfLife
    {
        private int width;
        private int height;
        private bool[,] grid;
        private int pixel;

        public GameOfLife(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.grid = new bool[width, height];
            this.pixel = this.width * height;
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            Random random = new Random();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    grid[i, j] = random.Next(5) == 1;
                }
            }
        }

        private void PrintGrid()
        {
            
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    Console.Write(grid[i, j] == true ? '*' : ' ');
                }
                Console.WriteLine();
            }
        }

        public int RunUntilEnter()
        {
            int generation = 0;

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Génération {generation}\nEspace fermée de {pixel} cellules\n");
                Console.WriteLine("Appuyez sur Entrée pour arrêter.\n");

                PrintGrid();

                // Attendre un peu avant la prochaine génération
                System.Threading.Thread.Sleep(1000);

                // Vérifie si une touche a été pressée
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);

                    if (key.Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                }

                UpdateGrid();
                generation++;
            }

            return generation;
        }

        private int CountNeighbors(int x, int y)
        {
            int count = 0;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue; // Skip the cell itself
                    int neighborX = x + i;
                    int neighborY = y + j;
                    if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                    {
                        if (grid[neighborX, neighborY])
                        {
                            count++;
                        }
                    }
                }
            }
            return count;
        }

        private void UpdateGrid()
        {
            bool[,] newGrid = new bool[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int neighbors = CountNeighbors(i, j);

                    if (grid[i, j])
                    {
                        // La cellule survit avec 2 ou 3 voisins
                        newGrid[i, j] = (neighbors == 2 || neighbors == 3);
                    }
                    else
                    {
                        // Une cellule morte naît avec exactement 3 voisins
                        newGrid[i, j] = (neighbors == 3);
                    }
                }
            }

            grid = newGrid;
        }

    }   
}
