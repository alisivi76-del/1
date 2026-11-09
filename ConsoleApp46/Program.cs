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

            char[,] map = new char[21, 21];
            Person hero = new Person(100, "Герой");

            Map.Array(map);

            Console.Clear();
            Map.GetMap(map);
            Person.GetCharacter(hero);

            bool exit = false;

            while (!exit && hero.HP > 0)
            {
                Console.WriteLine("Стрелки — движение, Esc — выход.");
                ConsoleKey key = Console.ReadKey(true).Key;

                int dr = 0;
                int dc = 0;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        dr = -1;
                        break;
                    case ConsoleKey.DownArrow:
                        dr = 1;
                        break;
                    case ConsoleKey.LeftArrow:
                        dc = -1;
                        break;
                    case ConsoleKey.RightArrow:
                        dc = 1;
                        break;
                    case ConsoleKey.Escape:
                        exit = true;
                        continue;
                    default:
                        continue;
                }

                // Проверяем событие в клетке, куда пытается пойти герой
                bool canMove = Map.GetIvent(hero, map, dr, dc);

                if (hero.HP <= 0)
                {
                    Console.WriteLine("Герой погиб. Игра окончена.");
                    break;
                }

                // Если впереди стена — не двигаемся
                if (!canMove)
                {
                    Map.GetMap(map);
                    Person.GetCharacter(hero);
                    continue;
                }

                // Сдвигаем карту в зависимости от направления
                if (dr == -1)
                {
                    Map.UpArray(map);
                }
                else if (dr == 1)
                {
                    Map.DownArray(map);
                }
                else if (dc == -1)
                {
                    Map.LeftArray(map);
                }
                else if (dc == 1)
                {
                    Map.RightArray(map);
                }

                // Проверяем текущую клетку героя.
                // Это нужно, например, для выхода '0', который появляется после Win.
                int levelBefore = Map.levelWorld;
                Map.GetIvent(hero, map, 0, 0);

                if (Map.levelWorld != levelBefore)
                {
                    Map.GetMap(map);
                }

                Person.GetCharacter(hero);
            }

            if (hero.HP <= 0)
            {
                Console.WriteLine("Поражение. Нажмите любую клавишу...");
                Console.ReadKey(true);
            }
        }
    }
}