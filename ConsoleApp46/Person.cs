using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp46
{
    public class Person
    {
        public int MaxHp { get; set; }
        public int Hp { get; set; }
        public int Strength { get; set; }
        public int Coin { get; set; }
        public string NamePerson { get; set; }

        public Person(int hp = 100, string name = "Враг")
        {
            NamePerson = name;
            Hp = hp;
            MaxHp = hp;
            Strength = 0;
            Coin = 0;
        }

        public void GetCharacter()
        {
            Console.WriteLine($"Имя героя = {NamePerson}");
            Console.WriteLine($"Здоровье = {Hp}");
            Console.WriteLine($"MAX Здоровье = {MaxHp}");
            Console.WriteLine($"Деняк = {Coin}");
            Console.WriteLine($"Уровень мира = {Map.LevelWorld}");
        }

    }
}
