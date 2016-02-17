using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Telerik.Windows.Controls;

namespace TelerikOutlookInspiredApp.Helpers
{
	public class OrientedGroupHeaderContentTemplateSelector : ScheduleViewDataTemplateSelector
	{
		private DataTemplate horizontalTemplate;
		private DataTemplate verticalTemplate;
		private DataTemplate horizontalResourceTemplate;
		private DataTemplate verticalResourceTemplate;

		public DataTemplate HorizontalTemplate
		{
			get
			{
				return this.horizontalTemplate;
			}
			set
			{
				this.horizontalTemplate = value;
			}
		}

		public DataTemplate VerticalTemplate
		{
			get
			{
				return this.verticalTemplate;
			}
			set
			{
				this.verticalTemplate = value;
			}
		}

		public DataTemplate HorizontalResourceTemplate
		{
			get
			{
				return this.horizontalResourceTemplate;
			}
			set
			{
				this.horizontalResourceTemplate = value;
			}
		}

		public DataTemplate VerticalResourceTemplate
		{
			get
			{
				return this.verticalResourceTemplate;
			}
			set
			{
				this.verticalResourceTemplate = value;
			}
		}

		public override DataTemplate SelectTemplate(object item, DependencyObject container, ViewDefinitionBase activeViewDeifinition)
		{
			CollectionViewGroup cvg = item as CollectionViewGroup;
			if (cvg != null && cvg.Name is IResource)
			{
				if (activeViewDeifinition.GetOrientation() == Orientation.Vertical)
				{
					if (this.HorizontalResourceTemplate != null)
					{
						return this.HorizontalResourceTemplate;
					}
				}
				else
				{
					if (this.VerticalResourceTemplate != null)
					{
						return this.VerticalResourceTemplate;
					}
				}
			}

			if (cvg != null && cvg.Name is DateTime)
			{
				if (activeViewDeifinition.GetOrientation() == Orientation.Vertical)
				{
					if (this.HorizontalTemplate != null)
					{
						return this.HorizontalTemplate;
					}
				}
				else
				{
					if (this.VerticalTemplate != null)
					{
						return this.VerticalTemplate;
					}
				}
			}

			return base.SelectTemplate(item, container, activeViewDeifinition);
		}
	}
}
