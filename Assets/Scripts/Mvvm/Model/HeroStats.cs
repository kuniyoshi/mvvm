namespace Mvvm.Model
{
    public readonly struct HeroStats
    {
        public int Hp { get; }
        public int Attack { get; }

        public HeroStats(int hp, int attack)
        {
            Hp = hp;
            Attack = attack;
        }
    }
}
