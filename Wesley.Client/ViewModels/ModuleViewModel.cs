using Wesley.Client.Enums;
using Wesley.Client.Models;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Windows.Input;


namespace Wesley.Client.ViewModels
{
    public class ModuleViewModel
    {
        public ModuleViewModel(Module module, ICommand onItemTappedCommand)
        {
            Selected = module.Selected;
            Id = module.Id;
            AType = module.AType;
            Count = module.Count;
            Name = module.Name;
            Icon = module.Icon;
            Color = module.Color;
            Navigation = module.Navigation;
            Description = module.Description;
            BillType = module.BillType;
            ChartType = module.ChartType;
            PermissionCodes = new ObservableCollection<AccessGranularityEnum>(module.PermissionCodes);
            OnItemTappedCommand = onItemTappedCommand;
        }

        public ICommand OnItemTappedCommand { get; set; }

        [Reactive] public bool Selected { get; set; }

        [Reactive] public int Id { get; set; }

        [Reactive] public int AType { get; set; }

        [Reactive] public int Count { get; set; }

        [Reactive] public string Name { get; set; }

        [Reactive] public string Icon { get; set; }

        [Reactive] public string Color { get; set; }

        [Reactive] public string Navigation { get; set; }

        [Reactive] public string Description { get; set; }

        [Reactive] public BillTypeEnum BillType { get; set; } = BillTypeEnum.None;

        [Reactive] public ChartTemplate ChartType { get; set; }

        [Reactive] public ObservableCollection<AccessGranularityEnum> PermissionCodes { get; set; } = new ObservableCollection<AccessGranularityEnum>();
    }
}
