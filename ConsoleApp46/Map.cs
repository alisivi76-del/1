using System;
using ConsoleApp46.Models;

namespace ConsoleApp46.World
{
    public class Map
    {
        public static int LevelWorld = 1;
        private static readonly Random Rnd = new Random();

        public const char PlayerChar = '@';
        public const char EnemyChar = 'E';
        public const char TreeChar = 'T';
        public const char PortalChar = 'P';
        public const char MountainChar = '#';
        public const char EmptyChar = '.';
        public const char ExitChar = 'X';

        public enum BiomeType
        {
            Forest,
            Mountains,
            Rooms
        }

        public static BiomeType SelectBiome()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Выберите биом для генерации карты:");
                Console.WriteLine("1. Леса (Деревья)");
                Console.WriteLine("2. Горы (Препятствия)");
                Console.WriteLine("3. Комнаты (Стены)");
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

        public static void GenerateMap(char[,] mas, BiomeType biome)
        {
            int rows = mas.GetLength(0);
            int cols = mas.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    mas[i, j] = EmptyChar;
                    int count = Rnd.Next(100);

                    switch (biome)
                    {
                        case BiomeType.Forest:
                            if (count < 10) mas[i, j] = EnemyChar;
                            if (count >= 90) mas[i, j] = TreeChar;
                            if (count >= 20 && count < 30)
                            {
                                int x = i, y = j;
                                for (int t = 0; t < 5; t++)
                                {
                                    mas[x++, y++] = MountainChar;
                                    if (x >= rows || y >= cols) break;
                                }
                            }
                            break;

                        case BiomeType.Mountains:
                            if (count < 5) mas[i, j] = EnemyChar;
                            if (count >= 85) mas[i, j] = TreeChar;
                            if (count >= 30 && count < 45)
                            {
                                int x = i, y = j;
                                for (int t = 0; t < 4; t++)
                                {
                                    mas[x++, y] = MountainChar;
                                    if (x >= rows) break;
                                }
                            }
                            break;

                        case BiomeType.Rooms:
                            if (count < 8) mas[i, j] = EnemyChar;
                            if (count >= 95) mas[i, j] = TreeChar;
                            if (i % 5 == 0 || j % 5 == 0)
                            {
                                if (count < 70) mas[i, j] = MountainChar;
                            }
                            break;
                    }
                }
            }

            mas[rows / 2, cols / 2] = PlayerChar;

            if (LevelWorld > 1)
            {
                mas[rows / 4, cols / 2] = PortalChar;
            }
        }

        public static void GetMap(char[,] mas)
        {
            for (int i = 0; i < mas.GetLength(0); i++)
            {
                for (int j = 0; j < mas.GetLength(1); j++)
                {
                    char cell = mas[i, j];
                    switch (cell)
                    {
                        case PlayerChar:
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write(cell + " ");
                            break;
                        case EnemyChar:
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(cell + " ");
                            break;
                        case TreeChar:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(cell + " ");
                            break;
                        case PortalChar:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write(cell + " ");
                            break;
                        case MountainChar:
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.Write(cell + " ");
                            break;
                        case ExitChar:
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write(cell + " ");
                            break;
                        default:
                            Console.Write(cell + " ");
                            break;
                    }
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public static void UpArray(char[,] mas)
        {
            char[] temp = new char[mas.GetLength(1)];
            for (int i = (mas.GetLength(0) - 1); i >= 0; i--)
            {
                for (int j = 0; j < mas.GetLength(1); j++)
                {
                    if (i == (mas.GetLength(0) - 1)) temp[j] = mas[i, j];
                    else if (i == 0) mas[i, j] = temp[j];
                    if (i != 0) mas[i, j] = mas[i - 1, j];
                    if (i == (mas.GetLength(0) - 1) / 2 && j == (mas.GetLength(1) - 1) / 2)
                        mas[i, j] = PlayerChar;
                    if (i == (mas.GetLength(0) - 1) / 2 && j == (mas.GetLength(1) - 1) / 2)
                        mas[i + 1, j] = EmptyChar;
                }
            }
            GetMap(mas);
            Win(mas);
        }

        public static void DownArray(char[,] mas)
        {
            char[] temp = new char[mas.GetLength(1)];
            for (int i = 0; i < mas.GetLength(0); i++)
            {
                for (int j = 0; j < mas.GetLength(1); j++)
                {
                    if (i == 0) temp[j] = mas[i, j];
                    else if (i == (mas.GetLength(0) - 1)) mas[i, j] = temp[j];
                    if (i != (mas.GetLength(0) - 1)) mas[i, j] = mas[i + 1, j];
                    if (i == (mas.GetLength(0) - 1) / 2 && j == (mas.GetLength(1) - 1) / 2)
                        mas[i, j] = PlayerChar;
                    if (i == (mas.GetLength(0) - 1) / 2 && j == (mas.GetLength(1) - 1) / 2)
                        mas[i - 1, j] = EmptyChar;
                }
            }
            GetMap(mas);
            Win(mas);
        }

        public static void LeftArray(char[,] mas)
        {
            char[] temp = new char[mas.GetLength(0)];
            for (int i = 0; i < mas.GetLength(0); i++)
            {
                for (int j = (mas.GetLength(1) - 1); j >= 0; j--)
                {
                    if (j == (mas.GetLength(1) - 1)) temp[i] = mas[i, j];
                    else if (j == 0) mas[i, j] = temp[i];
                    if (j != 0) mas[i, j] = mas[i, j - 1];

                    if (i == (mas.GetLength(0) - 1) / 2 && j == (mas.GetLength(1) - 1) / 2)
                        mas[i, j] = PlayerChar;
                    if (i == (mas.GetLength(0) - 1) / 2 && j == (mas.GetLength(1) - 1) / 2)
                        mas[i, j + 1] = EmptyChar;
                }
            }
            GetMap(mas);
            Win(mas);
        }

        public static void RightArray(char[,] mas)
        {
            char[] temp = new char[mas.GetLength(0)];
            for (int i = 0; i < mas.GetLength(0); i++)
            {
                for (int j = 0; j < mas.GetLength(1); j++)
                {
                    if (j == 0) temp[i] = mas[i, j];
                    else if (j == (mas.GetLength(1) - 1)) mas[i, j] = temp[i];
                    if (j != (mas.GetLength(1) - 1)) mas[i, j] = mas[i, j + 1];
                    if (i == (mas.GetLength(0) - 1) / 2 && j == (mas.GetLength(1) - 1) / 2)
                        mas[i, j] = PlayerChar;
                    if (i == (mas.GetLength(0) - 1) / 2 && j == (mas.GetLength(1) - 1) / 2)
                        mas[i, j - 1] = EmptyChar;
                }
            }
            GetMap(mas);
            Win(mas);
        }

        public static bool Win(char[,] mas)
        {
            for (int i = 0; i < mas.GetLength(0); i++)
            {
                for (int j = 0; j < mas.GetLength(1); j++)
                {
                    if (mas[i, j] == EnemyChar || mas[i, j] == ExitChar)
                        return false;
                }
            }
            mas[10, 10] = ExitChar;
            return true;
        }

        static void Batle(Person Hero, char[,] mas)
        {
            Person Enemy = new Person(LevelWorld * 10);
            Random rnd = new Random();

            while (Enemy.Hp > 0 && Hero.Hp > 0)
            {
                int Shot = rnd.Next(10);
                Enemy.Hp -= Shot + Hero.Strength;

                if (Enemy.Hp <= 0) break;

                Shot = rnd.Next(10);
                Hero.Hp -= Shot + LevelWorld * 5;
            }

            if (Enemy.Hp <= 0)
            {
                int reward = rnd.Next(10, 50);
                Hero.Coin += reward;
            }
            else if (Hero.Hp <= 0)
            {
                int lostCoins = Hero.Coin / 5;
                Hero.Coin -= lostCoins;
                if (Hero.Hp < 0) Hero.Hp = 0;

                Console.SetCursorPosition(0, 23);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Вы проиграли бой! Герой погиб...      ");
                Console.ResetColor();
            }
        }

        static void Heart(Person Hero, char[,] mas)
        {
            Hero.MaxHp += 10;
            Hero.Hp += Hero.MaxHp / 10;
            if (Hero.Hp > Hero.MaxHp) Hero.Hp = Hero.MaxHp;
        }

        static void Portal(Person Hero, char[,] mas)
        {
            Hero.Hp = Hero.MaxHp;
            for (int i = 0; i < mas.GetLength(0); i++)
            {
                for (int j = 0; j < mas.GetLength(1); j++)
                {
                    if (mas[i, j] == PortalChar) mas[i, j] = EmptyChar;
                }
            }
        }

        static void Forge(Person Hero)
        {
            Console.Clear();
            Console.WriteLine("Выберите действие");
            Console.WriteLine("1. Улучшить силу на 2 (стоит 50 монет)");
            Console.WriteLine("Для выхода нажмите Enter");
            Console.WriteLine($"Оставшиеся деньги: {Hero.Coin}");

            ConsoleKey key;
            while ((key = Console.ReadKey(true).Key) != ConsoleKey.Enter)
            {
                if (key == ConsoleKey.D1 || key == ConsoleKey.NumPad1)
                {
                    if (Hero.Coin >= 50)
                    {
                        Hero.Strength += 2;
                        Hero.Coin -= 50;
                        Console.WriteLine($"Сила увеличена на 2, Текущая сила = {Hero.Strength}");
                        Console.WriteLine($"Оставшиеся деньги: {Hero.Coin}");
                    }
                    else
                    {
                        Console.WriteLine("Недостаточно монет!");
                    }
                }
            }
        }

        public static bool GetIvent(Person hero, char[,] mas, int a = 0, int b = 0)
        {
            int centerRow = ((mas.GetLength(0) - 1) / 2) + a;
            int centerCol = ((mas.GetLength(1) - 1) / 2) + b;
            char key = mas[centerRow, centerCol];

            switch (key)
            {
                case EnemyChar:
                    Batle(hero, mas);
                    mas[centerRow, centerCol] = EmptyChar;
                    break;
                case TreeChar:
                    Heart(hero, mas);
                    mas[centerRow, centerCol] = EmptyChar;
                    break;
                case ExitChar:
                    LevelWorld++;
                    Portal(hero, mas);
                    break;
                case PortalChar:
                    Forge(hero);
                    break;
                case MountainChar:
                    return false;
                default:
                    break;
            }
            return true;
        }
    }
}
