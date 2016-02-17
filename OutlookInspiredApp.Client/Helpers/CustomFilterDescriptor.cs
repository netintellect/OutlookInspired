﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;

namespace TelerikOutlookInspiredApp
{
    public class CustomFilterDescriptor : FilterDescriptorBase
    {
        private static readonly ConstantExpression TrueExpression = Expression.Constant(true);
        private readonly CompositeFilterDescriptor compositeFilterDesriptor;
        private string filterValue;

        public CustomFilterDescriptor(IEnumerable<GridViewColumn> columns)
        {
            this.compositeFilterDesriptor = new CompositeFilterDescriptor();
            this.compositeFilterDesriptor.LogicalOperator = FilterCompositionLogicalOperator.Or;

            foreach (GridViewDataColumn column in columns)
            {
                this.compositeFilterDesriptor.FilterDescriptors.Add(this.CreateFilterForColumn(column));
            }
        }

        public string FilterValue
        {
            get
            {
                return this.filterValue;
            }
            set
            {
                if (this.filterValue != value)
                {
                    this.filterValue = value;
                    this.UpdateCompositeFilterValues();
                    this.OnPropertyChanged("FilterValue");
                }
            }
        }

        protected override Expression CreateFilterExpression(ParameterExpression parameterExpression)
        {
            Expression expression = CustomFilterDescriptor.TrueExpression;
            if (!string.IsNullOrEmpty(this.FilterValue))
            {
                try
                {
                    expression = this.compositeFilterDesriptor.CreateFilterExpression(parameterExpression);
                }
                catch
                {
                }
            }

            return expression;
        }

        private IFilterDescriptor CreateFilterForColumn(GridViewDataColumn column)
        {
            var filterOperator = CustomFilterDescriptor.GetFilterOperatorForType(column.DataType);
            return new FilterDescriptor(column.UniqueName, filterOperator, this.filterValue)
            {
                MemberType = column.DataType
            };
        }

        private static FilterOperator GetFilterOperatorForType(Type dataType)
        {
            return dataType == typeof(string) ? FilterOperator.Contains : FilterOperator.IsEqualTo;
        }

        private static object DefaultValue(Type type)
        {
            if (type != null && type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }

        private void UpdateCompositeFilterValues()
        {
            foreach (FilterDescriptor descriptor in this.compositeFilterDesriptor.FilterDescriptors)
            {
                object convertedValue = DefaultValue(descriptor.MemberType);

                try
                {
                    convertedValue = Convert.ChangeType(this.FilterValue, descriptor.MemberType, CultureInfo.InvariantCulture);
                }
                catch
                {
                    convertedValue = FilterDescriptor.UnsetValue;
                }

                if (descriptor.MemberType != null && descriptor.MemberType.IsAssignableFrom(typeof(DateTime)))
                {
                    DateTime date;
                    if (DateTime.TryParse(this.FilterValue, out date))
                    {
                        convertedValue = date;
                    }
                }

                descriptor.Value = convertedValue;
            }
        }
    }
}