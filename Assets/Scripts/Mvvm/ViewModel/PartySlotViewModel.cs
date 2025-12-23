using Mvvm.Model;

namespace Mvvm.ViewModel
{
    public sealed class PartySlotViewModel
    {
        public int Index { get; }
        public Hero? Hero { get; private set; }
        private HeroStats? _stats;

        public PartySlotViewModel(int index, Hero? hero, HeroStats? stats)
        {
            Index = index;
            Hero = hero;
            _stats = stats;
        }

        public string Title => $"枠 {Index + 1}";

        public string Name => Hero?.Name ?? "未設定";

        public string HpText => _stats.HasValue ? $"HP {_stats.Value.Hp}" : "-";

        public string AttackText => _stats.HasValue ? $"ATK {_stats.Value.Attack}" : "-";

        public string PositionText => Hero?.Position.ToString() ?? "-";

        internal void Update(Hero? hero, HeroStats? stats)
        {
            Hero = hero;
            _stats = stats;
        }
    }
}
