using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ALE.Controls.Filtering
{
    public class FilterCondition
    {
        public string PropertyName { get; set; }
        public FilterOperator Operator { get; set; }
        public string Value { get; set; } = "";

        public bool Evaluate(object item, Type itemType)
        {
            if (item == null || string.IsNullOrEmpty(PropertyName)) return true;

            var prop = itemType?.GetProperty(PropertyName);
            if (prop == null) return true;

            var rawValue = prop.GetValue(item);
            string itemStr = rawValue?.ToString() ?? "";

            // Unary operators
            if (Operator == FilterOperator.IsNull) return rawValue == null || string.IsNullOrEmpty(itemStr);
            if (Operator == FilterOperator.IsNotNull) return rawValue != null && !string.IsNullOrEmpty(itemStr);

            // Helpers
            double? itemNum = double.TryParse(itemStr, out double n1) ? n1 : null;
            DateTime? itemDate = DateTime.TryParse(itemStr, out DateTime d1) ? d1 : null;

            switch (Operator)
            {
                case FilterOperator.Equals:
                    if (itemNum.HasValue && double.TryParse(Value, out double vNumEq)) return itemNum == vNumEq;
                    if (itemDate.HasValue && DateTime.TryParse(Value, out DateTime vDateEq)) return itemDate.Value.Date == vDateEq.Date;
                    return itemStr.Equals(Value, StringComparison.OrdinalIgnoreCase);

                case FilterOperator.NotEquals:
                    if (itemNum.HasValue && double.TryParse(Value, out double vNumNe)) return itemNum != vNumNe;
                    if (itemDate.HasValue && DateTime.TryParse(Value, out DateTime vDateNe)) return itemDate.Value.Date != vDateNe.Date;
                    return !itemStr.Equals(Value, StringComparison.OrdinalIgnoreCase);

                case FilterOperator.GreaterThan:
                case FilterOperator.GreaterThanOrEqual:
                case FilterOperator.LessThan:
                case FilterOperator.LessThanOrEqual:
                    if (itemNum.HasValue && double.TryParse(Value, out double vNum))
                    {
                        return Operator switch
                        {
                            FilterOperator.GreaterThan => itemNum > vNum,
                            FilterOperator.GreaterThanOrEqual => itemNum >= vNum,
                            FilterOperator.LessThan => itemNum < vNum,
                            FilterOperator.LessThanOrEqual => itemNum <= vNum,
                            _ => false
                        };
                    }
                    if (itemDate.HasValue && DateTime.TryParse(Value, out DateTime vDate))
                    {
                        return Operator switch
                        {
                            FilterOperator.GreaterThan => itemDate > vDate,
                            FilterOperator.GreaterThanOrEqual => itemDate >= vDate,
                            FilterOperator.LessThan => itemDate < vDate,
                            FilterOperator.LessThanOrEqual => itemDate <= vDate,
                            _ => false
                        };
                    }
                    int cmp = string.Compare(itemStr, Value, StringComparison.OrdinalIgnoreCase);
                    return Operator switch
                    {
                        FilterOperator.GreaterThan => cmp > 0,
                        FilterOperator.GreaterThanOrEqual => cmp >= 0,
                        FilterOperator.LessThan => cmp < 0,
                        FilterOperator.LessThanOrEqual => cmp <= 0,
                        _ => false
                    };

                case FilterOperator.Contains:
                    return itemStr.IndexOf(Value, StringComparison.OrdinalIgnoreCase) >= 0;
                case FilterOperator.NotContains:
                    return itemStr.IndexOf(Value, StringComparison.OrdinalIgnoreCase) < 0;

                case FilterOperator.StartsWith:
                    return itemStr.StartsWith(Value, StringComparison.OrdinalIgnoreCase);
                case FilterOperator.EndsWith:
                    return itemStr.EndsWith(Value, StringComparison.OrdinalIgnoreCase);

                case FilterOperator.Like:
                case FilterOperator.NotLike:
                    string pattern = "^" + Regex.Escape(Value ?? "").Replace(@"%", ".*").Replace(@"_", ".") + "$";
                    bool matches = Regex.IsMatch(itemStr, pattern, RegexOptions.IgnoreCase);
                    return Operator == FilterOperator.Like ? matches : !matches;

                case FilterOperator.Between:
                case FilterOperator.NotBetween:
                    var parts = (Value ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (parts.Length != 2) return false;
                    if (double.TryParse(parts[0], out double min) && double.TryParse(parts[1], out double max))
                    {
                        bool inRange = itemNum.HasValue && itemNum >= min && itemNum <= max;
                        return Operator == FilterOperator.Between ? inRange : !inRange;
                    }
                    return false;

                case FilterOperator.In:
                case FilterOperator.NotIn:
                    var vals = (Value ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    bool found = vals.Contains(itemStr, StringComparer.OrdinalIgnoreCase);
                    return Operator == FilterOperator.In ? found : !found;

                default:
                    return true;
            }
        }

        public string ToDisplayString(string columnHeader)
        {
            string opStr = OperatorToString(Operator);
            string valPart = Operator is FilterOperator.IsNull or FilterOperator.IsNotNull ? "" : $"'{Value}'";
            return $"[{columnHeader}] {opStr} {valPart}".TrimEnd();
        }

        public static string OperatorToString(FilterOperator op)
        {
            return op switch
            {
                FilterOperator.Equals => "=",
                FilterOperator.NotEquals => "<>",
                FilterOperator.Contains => "Contains",
                FilterOperator.NotContains => "Not Contains",
                FilterOperator.StartsWith => "Starts With",
                FilterOperator.EndsWith => "Ends With",
                FilterOperator.GreaterThan => ">",
                FilterOperator.GreaterThanOrEqual => ">=",
                FilterOperator.LessThan => "<",
                FilterOperator.LessThanOrEqual => "<=",
                FilterOperator.IsNull => "Is Null",
                FilterOperator.IsNotNull => "Is Not Null",
                FilterOperator.Like => "Like",
                FilterOperator.NotLike => "Not Like",
                FilterOperator.Between => "Between",
                FilterOperator.NotBetween => "Not Between",
                FilterOperator.In => "In",
                FilterOperator.NotIn => "Not In",
                _ => op.ToString()
            };
        }

        public static FilterOperator[] AllOperators => Enum.GetValues<FilterOperator>();
    }
}