using System.Collections.Generic;

namespace Wesley.Client.Models.Products
{

    public partial class SpecificationAttributeListModel
    {
        public SpecificationAttributeListModel()
        {

        }


        public IList<SpecificationAttributeModel> SpecificationAttributes { get; set; }

        public SelectList Stores { get; set; }

        //关键字

        public string SearchKey { get; set; }
    }


    //typeof(SpecificationAttributeValidator))]
    public partial class SpecificationAttributeModel : EntityBase
    {
        public SelectList StoreList { get; set; }
        public string StoreName { get; set; }


        //("规格属性名", "规格属性名

        public string Name { get; set; }

        //("排序", "排序
        public int DisplayOrder { get; set; }
    }

}