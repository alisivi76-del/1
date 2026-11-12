using ConsoleApp46.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp46.Save
{
    public class GameData
    {
        public string HeroName { get; set; }
        public int MaxHp { get; set; }
        public int Hp { get; set; }
        public int Strength { get; set; }
        public int Coin { get; set; }
        public int LevelWorld { get; set; }
        public Map.BiomeType Biome { get; set; }
    }
}
