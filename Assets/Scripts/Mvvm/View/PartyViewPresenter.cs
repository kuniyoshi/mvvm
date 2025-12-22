using System;
using System.Collections.Generic;
using System.Linq;
using Mvvm.Model;
using Mvvm.ViewModel;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mvvm.View
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class PartyViewPresenter : MonoBehaviour
    {
        [SerializeField] private List<HeroPreset> heroPresets = new();

        [SerializeField] private StyleSheet? styleSheet;

        private UIDocument? _document;
        private PartyViewModel? _viewModel;
        private readonly List<SlotVisual> _slotVisuals = new();
        private Button? _confirmButton;
        private Button? _cancelButton;
        private VisualElement? _heroDialog;
        private ScrollView? _heroList;
        private Label? _heroDialogTitle;
        private Button? _closeDialogButton;

        private void Awake()
        {
            _document = GetComponent<UIDocument>();
            Assertion.AssertNotNull(_document, "UIDocument が見つかりません。");

            if (styleSheet != null)
            {
                _document.rootVisualElement.styleSheets.Add(styleSheet);
            }

            if (heroPresets.Count == 0)
            {
                throw new InvalidOperationException("HeroPreset を 1 件以上設定してください。");
            }

            _viewModel = new PartyViewModel(
                new Party(),
                heroPresets.Select(static preset => preset.ToHero())
                    .ToList()
            );
            _viewModel.StateChanged += RefreshView;

            BindVisualTree();
            RefreshView();
        }

        private void OnDestroy()
        {
            if (_viewModel != null)
            {
                _viewModel.StateChanged -= RefreshView;
            }
        }

        private void BindVisualTree()
        {
            if (_document == null)
            {
                return;
            }

            var root = _document.rootVisualElement;
            _confirmButton = root.Q<Button>("confirm-button");
            _cancelButton = root.Q<Button>("cancel-button");
            _heroDialog = root.Q<VisualElement>("hero-dialog");
            _heroDialogTitle = root.Q<Label>("hero-dialog-title");
            _heroList = root.Q<ScrollView>("hero-list");
            _closeDialogButton = root.Q<Button>("close-dialog-button");

            if (_confirmButton != null)
            {
                _confirmButton.clicked += () => { _viewModel?.ConfirmChanges(); };
            }

            if (_cancelButton != null)
            {
                _cancelButton.clicked += () => { _viewModel?.CancelChanges(); };
            }

            if (_closeDialogButton != null)
            {
                _closeDialogButton.clicked += () => { _viewModel?.ClearSelection(); };
            }

            _slotVisuals.Clear();
            for (var i = 0; i < Party.SlotCount; i++)
            {
                var button = root.Q<Button>($"slot-{i}");
                if (button == null)
                {
                    throw new InvalidOperationException($"slot-{i} が見つかりません。");
                }

                var titleLabel = button.Q<Label>("slot-title");
                var nameLabel = button.Q<Label>("slot-name");
                var statsLabel = button.Q<Label>("slot-stats");
                var positionLabel = button.Q<Label>("slot-position");
                if (titleLabel == null || nameLabel == null || statsLabel == null || positionLabel == null)
                {
                    throw new InvalidOperationException($"slot-{i} のラベル構成が不足しています。");
                }

                var visual = new SlotVisual(
                    i,
                    button,
                    titleLabel,
                    nameLabel,
                    statsLabel,
                    positionLabel);
                _slotVisuals.Add(visual);

                var index = i;
                button.clicked += () => OnSlotClicked(index);
            }
        }

        private void RefreshView()
        {
            if (_viewModel == null)
            {
                return;
            }

            foreach (var slot in _slotVisuals)
            {
                var state = _viewModel.Slots[slot.Index];
                slot.Apply(state);
                slot.MarkSelected(_viewModel.SelectedSlotIndex == slot.Index);
            }

            var hasChanges = _viewModel.HasPendingChanges;
            _confirmButton?.SetEnabled(hasChanges);
            _cancelButton?.SetEnabled(hasChanges);

            if (_viewModel.HasSelection)
            {
                ShowHeroDialog();
            }
            else
            {
                HideHeroDialog();
            }
        }

        private void ShowHeroDialog()
        {
            if (_viewModel == null || _heroDialog == null || _heroList == null)
            {
                return;
            }

            _heroDialog.RemoveFromClassList("hidden");

            var slot = _viewModel.SelectedSlot;
            if (_heroDialogTitle != null && slot != null)
            {
                _heroDialogTitle.text = $"{slot.Title} の Hero";
            }

            PopulateHeroOptions();
        }

        private void HideHeroDialog()
        {
            if (_heroDialog == null || _heroList == null)
            {
                return;
            }

            if (!_heroDialog.ClassListContains("hidden"))
            {
                _heroDialog.AddToClassList("hidden");
            }

            _heroList.Clear();
        }

        private void PopulateHeroOptions()
        {
            if (_viewModel == null || _heroList == null || !_viewModel.HasSelection)
            {
                return;
            }

            _heroList.Clear();
            var slotIndex = _viewModel.SelectedSlotIndex;
            var options = _viewModel.GetHeroOptions(slotIndex);
            foreach (var option in options)
            {
                _heroList.Add(CreateHeroOptionElement(option));
            }
        }

        private VisualElement CreateHeroOptionElement(HeroSelectionOption option)
        {
            var button = new Button
            {
                text = option.Name
            };
            button.AddToClassList("hero-item");

            if (option.IsEmpty)
            {
                button.AddToClassList("hero-item--empty");
            }

            if (option.IsCurrent)
            {
                button.AddToClassList("current");
            }

            var stats = new Label(option.StatText);
            stats.AddToClassList("hero-stats");
            button.Add(stats);

            button.clicked += () =>
            {
                _viewModel?.AssignHeroToSelected(option.Hero);
                _viewModel?.ClearSelection();
            };

            return button;
        }

        private void OnSlotClicked(int slotIndex)
        {
            _viewModel?.SelectSlot(slotIndex);
        }

        private sealed class SlotVisual
        {
            public int Index { get; }
            private readonly Button _button;
            private readonly Label? _titleLabel;
            private readonly Label? _nameLabel;
            private readonly Label? _statsLabel;
            private readonly Label? _positionLabel;

            public SlotVisual(int index, Button button, Label? titleLabel, Label? nameLabel, Label? statsLabel,
                Label? positionLabel)
            {
                Index = index;
                _button = button;
                _titleLabel = titleLabel;
                _nameLabel = nameLabel;
                _statsLabel = statsLabel;
                _positionLabel = positionLabel;
            }

            public void Apply(PartySlotViewModel viewModel)
            {
                if (_titleLabel != null)
                {
                    _titleLabel.text = viewModel.Title;
                }

                if (_nameLabel != null)
                {
                    _nameLabel.text = viewModel.Name;
                }

                if (_statsLabel != null)
                {
                    _statsLabel.text = $"{viewModel.HpText} / {viewModel.AttackText}";
                }

                if (_positionLabel != null)
                {
                    _positionLabel.text = viewModel.PositionText;
                }
            }

            public void MarkSelected(bool selected)
            {
                if (selected)
                {
                    _button.AddToClassList("selected");
                }
                else
                {
                    _button.RemoveFromClassList("selected");
                }
            }
        }
    }
}
