using Mvvm.Model;

namespace Mvvm.ViewModel
{
    public sealed class PartySlotViewModel
    {
        public int Index { get; }
        public Hero? Hero { get; private set; }

        public PartySlotViewModel(int index, Hero? hero)
        {
            Index = index;
            Hero = hero;
        }

        public string Title => $"枠 {Index + 1}";

        public string Name => Hero?.Name ?? "未設定";

        public string HpText => Hero != null ? $"HP {Hero.Hp}" : "-";

        public string AttackText => Hero != null ? $"ATK {Hero.Attack}" : "-";

        public string PositionText => Hero?.Position.ToString() ?? "-";

        internal void Update(Hero? hero)
        {
            Hero = hero;
        }
    }
}
