using System;
using ConsoleApp46.World;

namespace ConsoleApp46.Models
{
    public class Person
    {
        public int MaxHp { get; set; }
        public int Hp { get; set; }
        public int Strength { get; set; }
        public int Coin { get; set; }
        public string NamePerson { get; set; }
        public int Warmth { get; set; }

        public Person(int hp = 100, string name = "АУФ")
        {
            NamePerson = name;
            Hp = hp;
            MaxHp = hp;
            Strength = 3;
            Coin = 0;
            Warmth = Map.MaxWarmth;
        }

        public void GetCharacter()
        {
            Map.WriteLinePadded($"Имя героя     = {NamePerson}");
            Map.WriteLinePadded($"Здоровье      = {Hp} / {MaxHp}");
            Map.WriteLinePadded($"Сила          = {Strength}");
            Map.WriteLinePadded($"Монеты        = {Coin}");
            Map.WriteLinePadded($"Уровень мира  = {Map.LevelWorld}");
        }
    }
}