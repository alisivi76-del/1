using System;
using System.Text;

namespace ConsoleApp46
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            char[,] map = new char[25, 25];

            Map.BiomeType selectedBiome = Map.SelectBiome();
            Person hero = new Person(100, "Герой");
            Map.GenerateMap(map, selectedBiome);

            Console.Clear();
            Map.GetMap(map);
            hero.GetCharacter();

            bool exit = false;

            while (!exit && hero.Hp > 0)
            {
                Console.WriteLine("Стрелки — движение, Esc — выход.");
                ConsoleKey key = Console.ReadKey(true).Key;

                int dr = 0;
                int dc = 0;

                switch (key)
                {
                    case ConsoleKey.UpArrow: dr = -1; break;
                    case ConsoleKey.DownArrow: dr = 1; break;
                    case ConsoleKey.LeftArrow: dc = -1; break;
                    case ConsoleKey.RightArrow: dc = 1; break;
                    case ConsoleKey.Escape: exit = true; continue;
                    default: continue;
                }

                bool canMove = Map.GetIvent(hero, map, dr, dc);

                if (hero.Hp <= 0)
                {
                    Console.WriteLine("Герой погиб. Игра окончена.");
                    break;
                }

                if (!canMove)
                {
                    Map.GetMap(map);
                    hero.GetCharacter();
                    continue;
                }

                if (dr == -1) Map.UpArray(map);
                else if (dr == 1) Map.DownArray(map);
                else if (dc == -1) Map.LeftArray(map);
                else if (dc == 1) Map.RightArray(map);

                int levelBefore = Map.LevelWorld;
                Map.GetIvent(hero, map, 0, 0); 

                if (Map.LevelWorld != levelBefore)
                {
                    Map.GenerateMap(map, Map.SelectBiome()); 
                    Console.Clear();
                    Map.GetMap(map);
                }

                hero.GetCharacter();
            }

            if (hero.Hp <= 0)
            {
                Console.WriteLine("Поражение. Нажмите любую клавишу...");
                Console.ReadKey(true);
            }
        }
    }
}