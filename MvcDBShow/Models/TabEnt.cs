using System.Collections.Generic;

namespace MvcDBShow.Models
{
    public class TabEnt
    {
        public string TabName { get; set; }
        /// <summary>
        /// 表的备注
        /// </summary>
        public string TabRemarks { get; set; }
        public List<ColEnt> TabCol { get; set; }
    }

    public class ColEnt
    {
        public string ColName { get; set; }
        public string ColType { get; set; }
        public string ColRemarks { get; set; }
    }
}
