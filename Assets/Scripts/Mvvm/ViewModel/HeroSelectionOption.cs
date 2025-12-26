using Mvvm.Model;

namespace Mvvm.ViewModel
{
    public readonly struct HeroSelectionOption
    {
        public Hero? Hero { get; }
        public bool IsCurrent { get; }

        public HeroSelectionOption(Hero? hero, bool isCurrent)
        {
            Hero = hero;
            IsCurrent = isCurrent;
        }

        public bool IsEmpty => Hero == null;

        public string Name => Hero?.Name ?? "未設定";

        public string StatText => Hero != null ? $"HP {Hero.Hp} / ATK {Hero.Attack}" : "空枠";
    }
}
