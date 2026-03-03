using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ALE.Controls.Grouping
{
    public class GroupInfo
    {
        public object GroupValue { get; set; }
        public string PropertyName { get; set; }
        public bool IsCollapsed { get; set; }
        public int ChildCount { get; set; }
        public int Level { get; set; }
        public string GroupPath { get; set; } // Tracks unique composite path (e.g. "IT|Brisbane")
    }

    public class SafePropertyDescriptor : PropertyDescriptor
    {
        private readonly PropertyDescriptor _base;

        public SafePropertyDescriptor(PropertyDescriptor baseProp) : base(baseProp)
        {
            _base = baseProp;
        }

        public override Type ComponentType => typeof(object);
        public override bool IsReadOnly => _base.IsReadOnly;
        public override Type PropertyType => _base.PropertyType;
        public override bool CanResetValue(object component) => false;
        public override void ResetValue(object component) { }
        public override void SetValue(object component, object value) { _base.SetValue(component, value); }
        public override bool ShouldSerializeValue(object component) => false;

        public override object GetValue(object component)
        {
            if (component is GroupInfo) return null; // Prevents the grid crash
            return _base.GetValue(component);
        }
    }

    public class GroupedBindingList : BindingList<object>, ITypedList
    {
        private readonly Type _originalType;

        public GroupedBindingList(IList<object> list, Type originalType) : base(list)
        {
            _originalType = originalType;
        }

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            var props = TypeDescriptor.GetProperties(_originalType);
            var safeProps = new PropertyDescriptor[props.Count];
            for (int i = 0; i < props.Count; i++)
            {
                safeProps[i] = new SafePropertyDescriptor(props[i]);
            }
            return new PropertyDescriptorCollection(safeProps);
        }

        public string GetListName(PropertyDescriptor[] listAccessors) => "";
    }
}