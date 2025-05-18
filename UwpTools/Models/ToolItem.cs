using System;
using System.Diagnostics.CodeAnalysis;

namespace UwpTools.Models
{
    public class ToolItem
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Icon { get; set; }
        
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        public required Type PageType { get; set; }
    }
} 