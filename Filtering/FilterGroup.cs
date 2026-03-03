namespace ALE.Controls.Filtering
{
    public class FilterGroup
    {
        public LogicalOperator Logic { get; set; } = LogicalOperator.And;
        public List<FilterNode> Children { get; set; } = new List<FilterNode>();

        public bool Evaluate(object item, Type itemType)
        {
            if (Children.Count == 0) return true;

            bool any = Children.Any(c => c.Evaluate(item, itemType));
            bool all = Children.All(c => c.Evaluate(item, itemType));

            return Logic switch
            {
                LogicalOperator.And => all,
                LogicalOperator.Or => any,
                LogicalOperator.NotAnd => !all,
                LogicalOperator.NotOr => !any,
                _ => true
            };
        }

        public bool IsEmpty => Children.Count == 0;
    }
}