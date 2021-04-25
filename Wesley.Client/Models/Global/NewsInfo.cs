using System;
using System.Runtime.Serialization;
using System.Windows.Input;

namespace Wesley.Client.Models
{

    [Serializable]
    public partial class NewsInfoModel : Base
    {

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string PicturePath { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Content { get; set; }
        [DataMember]
        public string Navigation { get; set; }
        [DataMember]
        public DateTime CreateDate { get; set; }
        [DataMember]
        public string Short { get; set; }
        [DataMember]
        public string Full { get; set; }
        [DataMember]
        public int NewsCategoryId { get; set; }
        public string NewsCategoryName { get; set; }
        [DataMember]
        public string MetaKeywords { get; set; }
        [DataMember]
        public string MetaDescription { get; set; }
        [DataMember]
        public string MetaTitle { get; set; }

        public ICommand PrimaryCommand { get; set; }

        public int ViewCount { get; set; }
        public int HeartCount { get; set; }
        public string ViewCountStr { get; set; }
    }
}
