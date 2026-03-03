using System;

namespace ALE.Controls.Filtering
{
    public class FilterNode
    {
        public FilterCondition Condition { get; set; }
        public FilterGroup Group { get; set; }
        public bool IsGroup => Group != null;

        public bool Evaluate(object item, Type itemType)
        {
            if (IsGroup) return Group.Evaluate(item, itemType);
            return Condition?.Evaluate(item, itemType) ?? true;
        }

        public static FilterNode FromCondition(FilterCondition c) => new() { Condition = c };
        public static FilterNode FromGroup(FilterGroup g) => new() { Group = g };
    }
}