using HM.Domain.Rooms.Value_Objects;

namespace HM.Presentation.WPF.Models;

public class FeatureSelectionItemModel
{
    public FeatureSelectionItemModel(Feature value, bool isSelected)
    {
        Value = value;
        Name = value.ToString();
        IsSelected = isSelected;
    }

    public Feature Value { get; init; }
    public string Name { get; private set; }
    public bool IsSelected { get; set; }
}