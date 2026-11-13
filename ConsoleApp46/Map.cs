using System;
using System.Collections.Generic;
using ConsoleApp46.Models;

namespace ConsoleApp46.World
{
    public class EnemyTemplate
    {
        public string Symbol { get; }
        public string Name { get; }
        public int Hp { get; }
        public int Strength { get; }
        public int MinReward { get; }
        public int MaxReward { get; }
        public ConsoleColor Color { get; }

        public EnemyTemplate(string symbol, string name, int hp, int strength,
                             int minReward, int maxReward, ConsoleColor color)
        {
            Symbol = symbol; Name = name; Hp = hp; Strength = strength;
            MinReward = minReward; MaxReward = maxReward; Color = color;
        }
    }

    public class Map
    {
        public static int LevelWorld = 1;
        private static readonly Random Rnd = new Random();

        public const string PlayerChar = "😒";
        public const string TreeChar = "🌲";
        public const string HealChar = "💗";
        public const string MountainChar = "▬";
        public const string EmptyChar = "··";
        public const string ExitChar = "🚪";

        public const string FireChar = "🔥";
        public const string TrapChar = "💥";
        public const string CoinChar = "💰";
        public const string FogChar = "▒▒";

        public const int HealAmount = 25;

        public const int MaxWarmth = 100;
        public const int WarmthPerStep = 3;
        public const int ColdDamage = 5;
        public const int ForestVision = 5;

        // Сколько строк занимает «рамка» игры (карта + инфо-панель).
        // Нужно, чтобы понимать, где начинается область под сообщения боя.
        public const int FrameHeight = 34;

        public enum BiomeType { Forest, Mountains, Rooms }

        public static readonly Dictionary<string, EnemyTemplate> Enemies =
            new Dictionary<string, EnemyTemplate>
        {
            { "🐺", new EnemyTemplate("🐺", "Волк",    20, 3,  5, 15, ConsoleColor.Green) },
            { "🐗", new EnemyTemplate("🐗", "Кабан",   45, 2, 10, 25, ConsoleColor.DarkGreen) },
            { "🐍", new EnemyTemplate("🐍", "Змея",    12, 6,  8, 20, ConsoleColor.Yellow) },
            { "👹", new EnemyTemplate("👹", "Огр",     50, 6, 15, 30, ConsoleColor.DarkRed) },
            { "🧊", new EnemyTemplate("🧊", "Йети",    35, 8, 15, 35, ConsoleColor.Cyan) },
            { "🗿", new EnemyTemplate("🗿", "Голем",   70, 5, 20, 40, ConsoleColor.Gray) },
            { "💀", new EnemyTemplate("💀", "Скелет",  22, 4,  8, 18, ConsoleColor.White) },
            { "🧟", new EnemyTemplate("🧟", "Зомби",   38, 3, 10, 20, ConsoleColor.DarkYellow) },
            { "👻", new EnemyTemplate("👻", "Призрак", 16, 7, 12, 25, ConsoleColor.Magenta) },
        };

        public static bool IsEnemy(string c) => Enemies.ContainsKey(c);
        private static string PickEnemy(params string[] opts) => opts[Rnd.Next(opts.Length)];

        // ===== Новые вспомогательные методы =====

        /// <summary>Пишет строку и добивает её пробелами до ширины окна (без переноса на последнюю колонку).</summary>
        public static void WriteLinePadded(string s)
        {
            Console.Write(s);
            int pad = Console.WindowWidth - 1 - s.Length;
            if (pad > 0) Console.Write(new string(' ', pad));
            Console.WriteLine();
        }

        /// <summary>Затирает count строк начиная со startRow пробелами.</summary>
        public static void ClearLines(int startRow, int count)
        {
            if (startRow >= Console.WindowHeight) return;
            string blank = new string(' ', Console.WindowWidth - 1);
            for (int i = 0; i < count; i++)
            {
                int r = startRow + i;
                if (r >= Console.WindowHeight) break;
                Console.SetCursorPosition(0, r);
                Console.Write(blank);
            }
        }

        // ===== Меню выбора биома (оставлено как есть) =====
        public static BiomeType SelectBiome()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Выберите биом для генерации карты:");
                Console.WriteLine("1. Леса     — густой туман, враги: 🐺 🐗 🐍");
                Console.WriteLine("2. Горы     — холод, нужны костры 🔥, враги: 👹 🧊 🗿");
                Console.WriteLine("3. Комнаты  — ловушки 💥 и золото 💰, враги: 💀 🧟 👻");
                Console.Write("Ваш выбор: ");

                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    switch (choice)
                    {
                        case 1: return BiomeType.Forest;
                        case 2: return BiomeType.Mountains;
                        case 3: return BiomeType.Rooms;
                    }
                }
                Console.WriteLine("Некорректный ввод. Нажмите любую клавишу...");
                Console.ReadKey(true);
            }
        }

        public static void GenerateMap(string[,] mas, BiomeType biome)
        {
            int rows = mas.GetLength(0), cols = mas.GetLength(1);
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    mas[i, j] = EmptyChar;

            switch (biome)
            {
                case BiomeType.Forest: GenerateForest(mas); break;
                case BiomeType.Mountains: GenerateMountains(mas); break;
                case BiomeType.Rooms: GenerateRooms(mas); break;
            }

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    if (mas[i, j] == EmptyChar && Rnd.Next(100) < 4)
                        mas[i, j] = HealChar;

            int cr = rows / 2, cc = cols / 2;
            mas[cr, cc] = PlayerChar;
            for (int di = -1; di <= 1; di++)
                for (int dj = -1; dj <= 1; dj++)
                {
                    if (di == 0 && dj == 0) continue;
                    int r = cr + di, c = cc + dj;
                    if (r < 0 || r >= rows || c < 0 || c >= cols) continue;
                    string ch = mas[r, c];
                    if (ch == MountainChar || ch == TreeChar || ch == TrapChar || IsEnemy(ch))
                        mas[r, c] = EmptyChar;
                }
        }

        private static void GenerateForest(string[,] mas)
        {
            int rows = mas.GetLength(0), cols = mas.GetLength(1);
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    if (Rnd.Next(100) < 6) mas[i, j] = TreeChar;

            for (int k = 0; k < 10; k++)
            {
                int r = Rnd.Next(rows), c = Rnd.Next(cols);
                bool horiz = Rnd.Next(2) == 0;
                int len = Rnd.Next(3, 8);
                for (int t = 0; t < len; t++)
                {
                    int rr = horiz ? r : r + t;
                    int ccc = horiz ? c + t : c;
                    if (rr >= 0 && rr < rows && ccc >= 0 && ccc < cols)
                        mas[rr, ccc] = TreeChar;
                }
            }

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    if (mas[i, j] == EmptyChar && Rnd.Next(100) < 3)
                        mas[i, j] = PickEnemy("🐺", "🐗", "🐍");
        }

        private static void GenerateMountains(string[,] mas)
        {
            int rows = mas.GetLength(0), cols = mas.GetLength(1);
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    if (Rnd.Next(100) < 25) mas[i, j] = MountainChar;

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                {
                    if (mas[i, j] != EmptyChar) continue;
                    int c = Rnd.Next(100);
                    if (c < 4) mas[i, j] = PickEnemy("👹", "🧊", "🗿");
                    else if (c < 9) mas[i, j] = FireChar;
                }
        }

        private static void GenerateRooms(string[,] mas)
        {
            int rows = mas.GetLength(0), cols = mas.GetLength(1);

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    if (i % 5 == 0 || j % 5 == 0 || i == rows - 1 || j == cols - 1)
                        mas[i, j] = MountainChar;

            for (int i = 5; i < rows - 1; i += 5)
                for (int k = 0; k < 5; k++)
                {
                    int doorCol = k * 5 + 2;
                    if (doorCol < cols - 1) mas[i, doorCol] = EmptyChar;
                }

            for (int j = 5; j < cols - 1; j += 5)
                for (int k = 0; k < 5; k++)
                {
                    int doorRow = k * 5 + 2;
                    if (doorRow < rows - 1) mas[doorRow, j] = EmptyChar;
                }

            for (int i = 5; i < rows - 1; i += 5)
                for (int j = 1; j < cols - 1; j++)
                    if (j % 5 != 0 && Rnd.Next(100) < 30) mas[i, j] = EmptyChar;

            for (int j = 5; j < cols - 1; j += 5)
                for (int i = 1; i < rows - 1; i++)
                    if (i % 5 != 0 && Rnd.Next(100) < 30) mas[i, j] = EmptyChar;

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                {
                    if (mas[i, j] != EmptyChar) continue;
                    int c = Rnd.Next(100);
                    if (c < 3) mas[i, j] = PickEnemy("💀", "🧟", "👻");
                    else if (c < 6) mas[i, j] = TrapChar;
                    else if (c < 8) mas[i, j] = CoinChar;
                }
        }

        // ==== Рендер карты с дополнением строк пробелами ====
        public static void GetMap(string[,] mas, BiomeType biome)
        {
            int rows = mas.GetLength(0), cols = mas.GetLength(1);
            int cr = (rows - 1) / 2, cc = (cols - 1) / 2;
            int vision = (biome == BiomeType.Forest) ? ForestVision : -1;
            int windowWidth = Console.WindowWidth;

            for (int i = 0; i < rows; i++)
            {
                int visual = 0;

                for (int j = 0; j < cols; j++)
                {
                    bool visible = vision < 0 ||
                        Math.Max(Math.Abs(i - cr), Math.Abs(j - cc)) <= vision;

                    if (!visible)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(FogChar);
                        Console.ResetColor();
                        visual += 2;
                        continue;
                    }

                    string cell = mas[i, j];

                    if (Enemies.TryGetValue(cell, out EnemyTemplate tpl))
                    {
                        Console.ForegroundColor = tpl.Color;
                    }
                    else
                    {
                        switch (cell)
                        {
                            case PlayerChar: Console.ForegroundColor = ConsoleColor.Cyan; break;
                            case TreeChar: Console.ForegroundColor = ConsoleColor.Red; break;
                            case HealChar: Console.ForegroundColor = ConsoleColor.Magenta; break;
                            case MountainChar: Console.ForegroundColor = ConsoleColor.Gray; break;
                            case ExitChar: Console.ForegroundColor = ConsoleColor.Blue; break;
                            case FireChar: Console.ForegroundColor = ConsoleColor.DarkYellow; break;
                            case TrapChar: Console.ForegroundColor = ConsoleColor.DarkRed; break;
                            case CoinChar: Console.ForegroundColor = ConsoleColor.Yellow; break;
                        }
                    }
                    Console.Write(cell);
                    Console.ResetColor();

                    // MountainChar — 1 колонка, все остальные — 2 колонки на экране.
                    visual += (cell == MountainChar) ? 1 : 2;
                }

                int pad = windowWidth - 1 - visual;
                if (pad > 0) Console.Write(new string(' ', pad));
                Console.WriteLine();
            }

            WriteLinePadded(""); // пустая строка после карты
        }

        public static void UpArray(string[,] mas)
        {
            string[] temp = new string[mas.GetLength(1)];
            for (int i = mas.GetLength(0) - 1; i >= 0; i--)
                for (int j = 0; j < mas.GetLength(1); j++)
                {
                    if (i == mas.GetLength(0) - 1) temp[j] = mas[i, j];
                    else if (i == 0) mas[i, j] = temp[j];
                    if (i != 0) mas[i, j] = mas[i - 1, j];
                    if (i == (mas.GetLength(0) - 1) / 2 && j == (mas.GetLength(1) - 1) / 2) mas[i, j] = PlayerChar;
                    if (i == (mas.GetLength(0) - 1) / 2 && j == (mas.GetLength(1) - 1) / 2) mas[i + 1, j] = EmptyChar;
                }
        }

        public static void DownArray(string[,] mas)
        {
            string[] temp = new string[mas.GetLength(1)];
            for (int i = 0; i < mas.GetLength(0); i++)
                for (int j = 0; j < mas.GetLength(1); j++)
                {
                    if (i == 0) temp[j] = mas[i, j];
                    else if (i == mas.GetLength(0) - 1) mas[i, j] = temp[j];
                    if (i != mas.GetLength(0) - 1) mas[i, j] = mas[i + 1, j];
                    if (i == (mas.GetLength(0) - 1) / 2 && j == (mas.GetLength(1) - 1) / 2) mas[i, j] = PlayerChar;
                    if (i == (mas.GetLength(0) - 1) / 2 && j == (mas.GetLength(1) - 1) / 2) mas[i - 1, j] = EmptyChar;
                }
        }

        public static void LeftArray(string[,] mas)
        {
            string[] temp = new string[mas.GetLength(0)];
            for (int i = 0; i < mas.GetLength(0); i++)
                for (int j = mas.GetLength(1) - 1; j >= 0; j--)
                {
                    if (j == mas.GetLength(1) - 1) temp[i] = mas[i, j];
                    else if (j == 0) mas[i, j] = temp[i];
                    if (j != 0) mas[i, j] = mas[i, j - 1];
                    if (i == (mas.GetLength(0) - 1) / 2 && j == (mas.GetLength(1) - 1) / 2) mas[i, j] = PlayerChar;
                    if (i == (mas.GetLength(0) - 1) / 2 && j == (mas.GetLength(1) - 1) / 2) mas[i, j + 1] = EmptyChar;
                }
        }

        public static void RightArray(string[,] mas)
        {
            string[] temp = new string[mas.GetLength(0)];
            for (int i = 0; i < mas.GetLength(0); i++)
                for (int j = 0; j < mas.GetLength(1); j++)
                {
                    if (j == 0) temp[i] = mas[i, j];
                    else if (j == mas.GetLength(1) - 1) mas[i, j] = temp[i];
                    if (j != mas.GetLength(1) - 1) mas[i, j] = mas[i, j + 1];
                    if (i == (mas.GetLength(0) - 1) / 2 && j == (mas.GetLength(1) - 1) / 2) mas[i, j] = PlayerChar;
                    if (i == (mas.GetLength(0) - 1) / 2 && j == (mas.GetLength(1) - 1) / 2) mas[i, j - 1] = EmptyChar;
                }
        }

        public static bool Win(string[,] mas)
        {
            for (int i = 0; i < mas.GetLength(0); i++)
                for (int j = 0; j < mas.GetLength(1); j++)
                    if (IsEnemy(mas[i, j]) || mas[i, j] == ExitChar)
                        return false;

            mas[10, 10] = ExitChar;
            return true;
        }

        // ==== Бой: рисуем результат в фикс-области под кадром и стираем после клавиши ====
        private static void Batle(Person hero, EnemyTemplate tpl)
        {
            int enemyHp = tpl.Hp + (LevelWorld - 1) * 6;
            int enemyStr = tpl.Strength + (LevelWorld - 1) * 2;

            while (enemyHp > 0 && hero.Hp > 0)
            {
                enemyHp -= Rnd.Next(5, 10) + hero.Strength;
                if (enemyHp <= 0) break;
                hero.Hp -= Rnd.Next(0, 5) + enemyStr;
            }

            // Сообщения боя выводим на строку FrameHeight (сразу под кадром).
            int battleRow = FrameHeight;
            int maxRows = Console.WindowHeight - 1 - battleRow;
            int clearRows = Math.Min(4, Math.Max(1, maxRows));

            ClearLines(battleRow, clearRows);
            Console.SetCursorPosition(0, battleRow);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Бой с {tpl.Symbol} {tpl.Name}!  ");
            Console.ResetColor();

            if (enemyHp <= 0)
            {
                int reward = Rnd.Next(tpl.MinReward, tpl.MaxReward + 1);
                hero.Coin += reward;
                bool strUp = Rnd.Next(100) < 25;
                if (strUp) hero.Strength++;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"Победа! +{reward} монет");
                if (strUp) Console.Write(", +1 сила");
                Console.WriteLine("  ");
                Console.ResetColor();
            }
            else
            {
                int lost = hero.Coin / 5;
                hero.Coin -= lost;
                if (hero.Hp < 0) hero.Hp = 0;
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"Поражение! -{lost} монет  ");
                Console.ResetColor();
            }

            Console.ReadKey(true);
            ClearLines(battleRow, clearRows);
            Console.SetCursorPosition(0, 0);
        }

        public static bool GetIvent(Person hero, string[,] mas, int a = 0, int b = 0)
        {
            int centerRow = ((mas.GetLength(0) - 1) / 2) + a;
            int centerCol = ((mas.GetLength(1) - 1) / 2) + b;
            string key = mas[centerRow, centerCol];

            if (Enemies.TryGetValue(key, out EnemyTemplate tpl))
            {
                Batle(hero, tpl);
                mas[centerRow, centerCol] = EmptyChar;
                return true;
            }

            switch (key)
            {
                case HealChar:
                    hero.Hp += HealAmount;
                    if (hero.Hp > hero.MaxHp) hero.Hp = hero.MaxHp;
                    mas[centerRow, centerCol] = EmptyChar;
                    break;

                case FireChar:
                    hero.Warmth = MaxWarmth;
                    break;

                case TrapChar:
                    hero.Hp -= 10;
                    mas[centerRow, centerCol] = EmptyChar;
                    break;

                case CoinChar:
                    hero.Coin += Rnd.Next(5, 20);
                    mas[centerRow, centerCol] = EmptyChar;
                    break;

                case ExitChar:
                    LevelWorld++;
                    break;

                case TreeChar:
                case MountainChar:
                    return false;
            }
            return true;
        }

        public static void ApplyBiomeEffects(Person hero, BiomeType biome)
        {
            if (biome != BiomeType.Mountains) return;

            hero.Warmth -= WarmthPerStep;
            if (hero.Warmth < 0) hero.Warmth = 0;
            if (hero.Warmth <= 0) hero.Hp -= ColdDamage;
        }

        public static void SpawnHealIfNeeded(string[,] mas)
        {
            int healCount = 0;
            for (int i = 0; i < mas.GetLength(0); i++)
                for (int j = 0; j < mas.GetLength(1); j++)
                    if (mas[i, j] == HealChar) healCount++;

            if (healCount < 10)
            {
                for (int tries = 0; tries < 200; tries++)
                {
                    int r = Rnd.Next(mas.GetLength(0));
                    int c = Rnd.Next(mas.GetLength(1));
                    if (mas[r, c] == EmptyChar) { mas[r, c] = HealChar; break; }
                }
            }
        }
    }
}