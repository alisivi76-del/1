using System;
using System.Text;
using ConsoleApp46.Models;
using ConsoleApp46.Save;
using ConsoleApp46.World;

namespace ConsoleApp46
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            // Один раз пытаемся выставить окно, чтобы кадр влез без скролла.
            try
            {
                int w = Math.Min(120, Console.LargestWindowWidth);
                int h = Math.Min(45, Console.LargestWindowHeight);
                if (Console.BufferWidth < w || Console.BufferHeight < h)
                    Console.SetBufferSize(Math.Max(Console.BufferWidth, w), Math.Max(Console.BufferHeight, h));
                if (Console.WindowWidth < w || Console.WindowHeight < h)
                    Console.SetWindowSize(w, h);
            }
            catch { /* если не получилось — просто идём дальше */ }

            Console.CursorVisible = false;

            Map.BiomeType biome = Map.SelectBiome();

            string[,] map = new string[25, 25];
            Person hero = new Person(100, "АУФ");

            Map.GenerateMap(map, biome);
            bool exit = false;

            while (!exit && hero.Hp > 0)
            {
                Console.SetCursorPosition(0, 0);

                Map.GetMap(map, biome);
                hero.GetCharacter();
                Map.WriteLinePadded($"Биом: {GetBiomeName(biome)}");

                if (biome == Map.BiomeType.Mountains)
                {
                    int lineLen = "Тепло: ".Length +
                                  $"{hero.Warmth} / {Map.MaxWarmth}".Length;

                    Console.Write("Тепло: ");
                    Console.ForegroundColor = hero.Warmth > 30 ? ConsoleColor.Yellow : ConsoleColor.Red;
                    Console.Write($"{hero.Warmth} / {Map.MaxWarmth}");
                    Console.ResetColor();

                    if (hero.Warmth <= 0)
                    {
                        string warn = $"  << ВЫ ЗАМЕРЗАЕТЕ! (-{Map.ColdDamage} HP за шаг)";
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write(warn);
                        Console.ResetColor();
                        lineLen += warn.Length;
                    }
                    int pad = Console.WindowWidth - 1 - lineLen;
                    if (pad > 0) Console.Write(new string(' ', pad));
                    Console.WriteLine();
                }
                else
                {
                    // Пустая строка вместо «тепла» — чтобы высота кадра
                    // была одинаковой во всех биомах.
                    Map.WriteLinePadded("");
                }

                Map.WriteLinePadded("Стрелки — движение, F5 — сохранить, Esc — выход.");

                ConsoleKey key = Console.ReadKey(true).Key;
                int dr = 0, dc = 0;

                switch (key)
                {
                    case ConsoleKey.UpArrow: dr = -1; break;
                    case ConsoleKey.DownArrow: dr = 1; break;
                    case ConsoleKey.LeftArrow: dc = -1; break;
                    case ConsoleKey.RightArrow: dc = 1; break;
                    case ConsoleKey.F5:
                        SaveManager.Save(hero, biome);
                        // выведем сообщение ниже кадра и подождём
                        Map.ClearLines(34, 1);
                        Console.SetCursorPosition(0, 34);
                        Map.WriteLinePadded("Игра сохранена. Нажмите любую клавишу...");
                        Console.ReadKey(true);
                        Map.ClearLines(34, 1);
                        continue;
                    case ConsoleKey.Escape:
                        SaveManager.Save(hero, biome);
                        exit = true;
                        continue;
                    default: continue;
                }

                int levelBefore = Map.LevelWorld;
                bool canMove = Map.GetIvent(hero, map, dr, dc);

                if (hero.Hp <= 0) break;
                if (!canMove) continue;

                if (dr == -1) Map.UpArray(map);
                else if (dr == 1) Map.DownArray(map);
                else if (dc == -1) Map.LeftArray(map);
                else if (dc == 1) Map.RightArray(map);

                Map.Win(map);
                Map.SpawnHealIfNeeded(map);

                Map.ApplyBiomeEffects(hero, biome);
                if (hero.Hp <= 0) break;

                if (Map.LevelWorld != levelBefore)
                {
                    biome = Map.SelectBiome();
                    hero.Warmth = Map.MaxWarmth;
                    Map.GenerateMap(map, biome);
                }
            }

            if (hero.Hp <= 0)
            {
                Console.SetCursorPosition(0, 0);
                Console.Clear();
                Console.WriteLine("Лелик погиб. Игра окончена.");
                Console.ReadKey(true);
            }
        }

        static string GetBiomeName(Map.BiomeType b)
        {
            switch (b)
            {
                case Map.BiomeType.Forest: return "Лес (туман)";
                case Map.BiomeType.Mountains: return "Горы (холод)";
                case Map.BiomeType.Rooms: return "Комнаты (ловушки и золото)";
                default: return b.ToString();
            }
        }
    }
}