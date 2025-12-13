using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Mvvm.Model;

namespace Mvvm.ViewModel
{
    public sealed class PartyViewModel
    {
        private readonly Party _party;
        private readonly ReadOnlyCollection<Hero> _availableHeroes;
        private PartyTransaction _transaction;
        private readonly List<PartySlotViewModel> _slots;

        public event Action StateChanged = delegate { };

        public PartyViewModel(Party party, IEnumerable<Hero> heroes)
        {
            _party = party ?? throw new ArgumentNullException(nameof(party));

            if (heroes == null)
            {
                throw new ArgumentNullException(nameof(heroes));
            }

            var heroList = heroes.ToList();
            if (heroList.Count == 0)
            {
                throw new ArgumentException("Hero は 1 人以上定義してください。", nameof(heroes));
            }

            _availableHeroes = heroList.AsReadOnly();
            _transaction = _party.BeginTransaction();
            _slots = Enumerable.Range(0, Party.SlotCount)
                .Select(i => new PartySlotViewModel(i, _transaction.GetHero(i)))
                .ToList();

            RaiseStateChanged();
        }

        public IReadOnlyList<PartySlotViewModel> Slots => _slots;

        public IReadOnlyList<Hero> AvailableHeroes => _availableHeroes;

        public int SelectedSlotIndex { get; private set; } = -1;

        public PartySlotViewModel? SelectedSlot => SelectedSlotIndex >= 0 ? _slots[SelectedSlotIndex] : null;

        public bool HasSelection => SelectedSlotIndex >= 0;

        public bool HasPendingChanges => _transaction.HasChanges;

        public void SelectSlot(int slotIndex)
        {
            Party.ValidateSlotIndex(slotIndex);
            SelectedSlotIndex = slotIndex;
            RaiseStateChanged();
        }

        public void ClearSelection()
        {
            SelectedSlotIndex = -1;
            RaiseStateChanged();
        }

        public IReadOnlyList<HeroSelectionOption> GetHeroOptions(int slotIndex)
        {
            Party.ValidateSlotIndex(slotIndex);
            var current = _transaction.GetHero(slotIndex);
            var options = new List<HeroSelectionOption>
            {
                new HeroSelectionOption(null, current == null)
            };

            foreach (var hero in _availableHeroes)
            {
                if (!_transaction.Contains(hero) || ReferenceEquals(hero, current))
                {
                    options.Add(new HeroSelectionOption(hero, ReferenceEquals(hero, current)));
                }
            }

            return options;
        }

        public void AssignHero(int slotIndex, Hero? hero)
        {
            Party.ValidateSlotIndex(slotIndex);
            _transaction.AssignHero(slotIndex, hero);
            SyncSlotsFromTransaction();
        }

        public void AssignHeroToSelected(Hero? hero)
        {
            if (!HasSelection)
            {
                throw new InvalidOperationException("枠が選択されていません。");
            }

            AssignHero(SelectedSlotIndex, hero);
        }

        public void ConfirmChanges()
        {
            if (!_transaction.HasChanges)
            {
                return;
            }

            _transaction.Commit();
            RestartTransaction();
            SelectedSlotIndex = -1;
            SyncSlotsFromTransaction();
        }

        public void CancelChanges()
        {
            _transaction.Rollback();
            RestartTransaction();
            SelectedSlotIndex = -1;
            SyncSlotsFromTransaction();
        }

        private void RestartTransaction()
        {
            _transaction = _party.BeginTransaction();
        }

        private void SyncSlotsFromTransaction()
        {
            for (var i = 0; i < _slots.Count; i++)
            {
                _slots[i].Update(_transaction.GetHero(i));
            }

            RaiseStateChanged();
        }

        private void RaiseStateChanged()
        {
            StateChanged?.Invoke();
        }
    }
}
