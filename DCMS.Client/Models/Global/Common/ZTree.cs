using System.Collections.Generic;

namespace Wesley.Client.Models.Common
{
    public class ZTree
    {
        public double id { get; set; }

        public int? pId { get; set; }

        public string name { get; set; }

        public bool @checked
        {
            get;
            set;
        }
        public bool isParent { get; set; }

        public bool open { get; set; }

        public string value { get; set; }
    }


    public class FancyTree
    {
        public int id { get; set; }
        public string title { get; set; }
        public bool expanded { get; set; }
        public bool folder { get; set; }
        public List<FancyTree> children { get; set; }
    }

    public class FancyTreeExt
    {
        public string title1 { get; set; }
        public string title2 { get; set; }
        public string title3 { get; set; }
        public int id1 { get; set; }
        public int id { get; set; }
        public string title { get; set; }
        public bool expanded { get; set; }
        public bool folder { get; set; }
        public List<FancyTreeExt> children { get; set; }
    }
}
