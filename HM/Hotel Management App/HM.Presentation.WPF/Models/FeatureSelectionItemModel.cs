using HM.Domain.Rooms.Value_Objects;

namespace HM.Presentation.WPF.Models;

public class FeatureSelectionItemModel
{
    public Feautre Value { get; init; } 
    public string Name { get; private set; }
    public bool IsSelected { get; set; }
    

    public FeatureSelectionItemModel(Feautre value, bool isSelected)
    {
        Value = value;
        Name = value.ToString();
        IsSelected = isSelected;
    }
}