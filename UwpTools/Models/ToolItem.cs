using Microsoft.UI.Xaml.Controls;

namespace UwpTools.Models
{
    public class ToolItem
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public Symbol Icon { get; set; }
        public required string PageType { get; set; }
    }
} 