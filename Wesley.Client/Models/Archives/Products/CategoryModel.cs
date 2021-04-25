using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;

namespace Wesley.Client.Models.Products
{

    public partial class CategoryModel : EntityBase
    {
        public ReactiveCommand<int, Unit> SelectedCommand { get; set; }
        [Reactive] public bool Selected { get; set; }
        [Reactive] public string Name { get; set; }
        public int ParentId { get; set; }
        [Reactive] public string ParentName { get; set; }
        public SelectList ParentList { get; set; }
        public string PathCode { get; set; }
        public int StatisticalType { get; set; }
        public SelectList StatisticalTypes { get; set; }
        public int Status { get; set; }
        public int OrderNo { get; set; }
        public int BrandId { get; set; }
        [Reactive] public string BrandName { get; set; }
        public bool Published { get; set; }
        public bool Deleted { get; set; }
        public int? PercentageId { get; set; }
    }
}