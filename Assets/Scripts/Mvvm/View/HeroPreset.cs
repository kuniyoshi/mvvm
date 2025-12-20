using System;
using System.Diagnostics;
using Mvvm.Model;

namespace Mvvm.View
{
    [Serializable]
    public struct HeroPreset
    {
        public string Name;
        public int Hp;
        public int Attack;
        public Position Position;

        public readonly Hero ToHero()
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(Name), "HeroPreset の Name は必須です。");
            Debug.Assert(Hp > 0, "HeroPreset の Hp は 1 以上で設定してください。");
            Debug.Assert(Attack > 0, "HeroPreset の Attack は 1 以上で設定してください。");
            return new Hero(Name, Hp, Attack, Position);
        }
    }
}
