using System;
using System.Collections.Generic;

namespace _hexEffect.Scripts
{
    public class PathBuilder
    {

        private Random random;
        public PathBuilder()
        {
            
            
        }
  
        private bool TryPlaceWord(char[,] grid, string word, int startX, int startY)
        {
            // Find the possible directions for the word
            List<(int dx, int dy)> directions = GetPossibleDirections(grid, startX, startY, word.Length);

            // If there are no possible directions, return false
            if (directions.Count == 0)
            {
                return false;
            }

            // Randomly choose a direction for the word
            (int dx, int dy) direction = directions[random.Next(directions.Count)];

            // Place the word in the chosen direction
            int endX = startX + direction.dx * (word.Length - 1);
            int endY = startY + direction.dy * (word.Length - 1);
            for (int i = 0; i < word.Length; i++)
            {
                int x = startX + direction.dx * i;
                int y = startY + direction.dy * i;
                grid[x, y] = word[i];
            }

            // Return true to indicate success
            return true;
        }
        private static List<(int dx, int dy)> GetPossibleDirections(char[,] grid, int startX, int startY, int length)
        {
            List<(int dx, int dy)> directions = new List<(int dx, int dy)>();
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                    {
                        continue;
                    }
                    int endX = startX + dx * (length - 1);
                    int endY = startY + dy * (length - 1);
                    if (endX >= 0 && endX < grid.GetLength(0) && endY >= 0 && endY < grid.GetLength(1))
                    {
                        bool valid = true;
                        for (int i = 0; i < length; i++)
                        {
                            int x = startX + dx * i;
                            int y = startY + dy * i;
                            if (grid[x, y] != '\0')
                            {
                                valid = false;
                                break;
                            }
                        }
                        if (valid)
                        {
                            directions.Add((dx, dy));
                        }
                    }
                }
            }
            return directions;
        }

    }
}