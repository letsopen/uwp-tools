using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UwpTools.Models
{
    public class ToolItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Type TargetPage { get; set; }
    }
}