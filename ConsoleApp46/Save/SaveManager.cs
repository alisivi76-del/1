using System;
using System.IO;
using System.Xml.Serialization;
using ConsoleApp46.Models;
using ConsoleApp46.World;

namespace ConsoleApp46.Save
{
    public static class SaveManager
    {
        private const string SavePath = "savegame.xml";

        public static string FullPath => Path.GetFullPath(SavePath);

        public static bool Exists()
        {
            return File.Exists(SavePath);
        }

        public static void Save(Person hero, Map.BiomeType biome)
        {
            GameData data = new GameData
            {
                HeroName = hero.NamePerson,
                MaxHp = hero.MaxHp,
                Hp = hero.Hp,
                Strength = hero.Strength,
                Coin = hero.Coin,
                LevelWorld = Map.LevelWorld,
                Biome = biome
            };

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(GameData));
                using (FileStream stream = new FileStream(SavePath, FileMode.Create))
                {
                    serializer.Serialize(stream, data);
                }
                Console.WriteLine($"Игра сохранена: {FullPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения: {ex.Message}");
            }
        }

        public static bool TryLoad(out Person hero, out Map.BiomeType biome)
        {
            hero = null;
            biome = Map.BiomeType.Forest;

            if (!Exists())
            {
                Console.WriteLine($"Сохранение не найдено: {FullPath}");
                return false;
            }

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(GameData));
                using (FileStream stream = new FileStream(SavePath, FileMode.Open))
                {
                    GameData data = (GameData)serializer.Deserialize(stream);
                    if (data == null) return false;

                    hero = new Person(data.MaxHp, data.HeroName);
                    hero.Hp = data.Hp;
                    hero.MaxHp = data.MaxHp;
                    hero.Strength = data.Strength;
                    hero.Coin = data.Coin;
                    Map.LevelWorld = data.LevelWorld;
                    biome = data.Biome;

                    Console.WriteLine($"Загружено: {FullPath}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки ({FullPath}): {ex.Message}");
                return false;
            }
        }
    }
}