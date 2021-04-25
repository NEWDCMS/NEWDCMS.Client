using System;
using System.Collections.Generic;
namespace Wesley.Client.Models.Products
{
    [Serializable]
    public partial class SpecificationAttributeOptionModel : EntityBase
    {
        public int SpecificationAttributeId { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
        public int? ConvertedQuantity { get; set; }
        public string UnitConversion { get; set; }
        public int NumberOfAssociatedProducts { get; set; }
    }


    public class SpecificationModel : EntityBase
    {
        public IList<SpecificationAttributeOptionModel> smallOptions { get; set; } = new List<SpecificationAttributeOptionModel>();
        public IList<SpecificationAttributeOptionModel> strokOptions { get; set; } = new List<SpecificationAttributeOptionModel>();
        public IList<SpecificationAttributeOptionModel> bigOptions { get; set; } = new List<SpecificationAttributeOptionModel>();
    }
}