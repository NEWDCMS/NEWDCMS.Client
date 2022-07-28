using Wesley.Client.Models.Products;
using System.Collections.Generic;


namespace Wesley.Client.Models.Media
{
    /// <summary>
    /// 用于表示图片
    /// </summary>
    public partial class Picture : EntityBase
    {

        private IList<ProductPicture> _productPictures;
        public byte[] PictureBinary { get; set; }

        public string MimeType { get; set; }

        public string SeoFilename { get; set; }

        public bool IsNew { get; set; }


        public IList<ProductPicture> ProductPictures
        {
            get { return _productPictures ?? (_productPictures = new List<ProductPicture>()); }
            protected set { _productPictures = value; }
        }

    }



    /// <summary>
    /// 用于表示商品图片
    /// </summary>
    public partial class ProductPicture : EntityBase
    {

        public int ProductId { get; set; }

        public int PictureId { get; set; }


        public int DisplayOrder { get; set; }


        public virtual Picture Picture { get; set; }

        public virtual ProductModel Product { get; set; }
    }

}
