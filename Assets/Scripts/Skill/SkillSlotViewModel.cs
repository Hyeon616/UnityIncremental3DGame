using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;

public class SkillSlotViewModel : INotifyPropertyChanged
{
    private readonly GameLogic _gameLogic;
    private ObservableCollection<SkillSlotModel> _skillSlots;
    private int _selectedSlotIndex = -1;

    public ObservableCollection<SkillSlotModel> SkillSlots
    {
        get => _skillSlots;
        set
        {
            _skillSlots = value;
            OnPropertyChanged();
        }
    }

    public int SelectedSlotIndex
    {
        get => _selectedSlotIndex;
        set
        {
            _selectedSlotIndex = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SelectedSkillDescription));
        }
    }

    public string SelectedSkillDescription
    {
        get
        {
            if (_selectedSlotIndex >= 0 && _selectedSlotIndex < _skillSlots.Count)
            {
                var skillSlot = _skillSlots[_selectedSlotIndex];
                return skillSlot.is_empty ? "Empty slot" : skillSlot.playerSkill.skill.description;
            }
            return string.Empty;
        }
    }

    public SkillSlotViewModel(GameLogic gameLogic)
    {
        _gameLogic = gameLogic;
        _skillSlots = new ObservableCollection<SkillSlotModel>();
        _gameLogic.PropertyChanged += OnGameLogicPropertyChanged;
    }

    private void OnGameLogicPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GameLogic.PlayerSkills))
        {
            UpdateSkillSlots();
        }
    }

    private void UpdateSkillSlots()
    {
        SkillSlots.Clear();
        foreach (var playerSkill in _gameLogic.PlayerSkills)
        {
            SkillSlots.Add(new SkillSlotModel(playerSkill));
        }

        while (SkillSlots.Count < 16)
        {
            SkillSlots.Add(new SkillSlotModel(null));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}