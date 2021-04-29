using System;
using System.Linq;
using System.Text;
using AquariumSimulator.Model;


namespace AquariumSimulator.View
{
    public class CliView : IView
    {
        public void DrawAquarium(Aquarium aquarium)
        {
            const int posInCell = 3;
            const char filler = '.';
            int width = (int) aquarium.Width;
            int height = (int) aquarium.Height;
            string cell = "|" + new string(filler, posInCell);
            string fieldRow = string.Concat(Enumerable.Repeat(cell, width)) + "|\n"; 
            var field = new StringBuilder(string.Concat(Enumerable.Repeat(fieldRow, height)));
            
            // Fill field (string)
            foreach (var terrain in aquarium.Terrains)
            {
                int start = terrain.Location.Y * fieldRow.Length + terrain.Location.X * cell.Length + 1;
                field.Replace(filler, 'X', start, posInCell);
            }
            foreach (var seaweed in aquarium.Seaweeds)
            {
                int start = seaweed.Location.Y * fieldRow.Length + seaweed.Location.X * cell.Length + 1;
                field.Replace(filler, 'S', start, 1);
            }
            foreach (var herbivore in aquarium.Herbivores)
            {
                int start = herbivore.Location.Y * fieldRow.Length + herbivore.Location.X * cell.Length + 2;
                field.Replace(filler, 'H', start, 1);
            }
            foreach (var predator in aquarium.Predators)
            {
                int start = predator.Location.Y * fieldRow.Length + predator.Location.X * cell.Length + 3;
                field.Replace(filler, 'P', start, 1);
            }
            
            
            Console.Clear();
            Console.Write("Stage:\t" + aquarium.Stage + "\n\n");
            Console.Write(field);
            Console.WriteLine("\nObjects:\t\t" + aquarium.NumObjects);
            Console.WriteLine("Predators fishes:\t" + aquarium.NumPredators);
            Console.WriteLine("Herbivorous fishes:\t" + aquarium.NumHerbivores);
            Console.WriteLine("Seaweeds:\t\t" + aquarium.NumSeaweeds);
            Console.WriteLine("Terrains:\t\t" + aquarium.NumStones);
        }
    }
}
