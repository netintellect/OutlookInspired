using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace TelerikOutlookInspiredApp
{
    public class EmptyDataTemplateBehavior
    {
        private readonly RadGridView grid = null;
        ContentPresenter contentPresenter = new ContentPresenter(); 
        public DataTemplate EmptyDataTemplate { get; set; }

        public EmptyDataTemplateBehavior(RadGridView grid)
        {
            this.grid = grid;
            this.EmptyDataTemplate = Application.Current.FindResource("GridViewEmptyDataTemplate") as DataTemplate;
        }

        public static readonly DependencyProperty IsEnabledProperty
            = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(EmptyDataTemplateBehavior),
                new PropertyMetadata(new PropertyChangedCallback(OnIsEnabledPropertyChanged)));

        public static void SetIsEnabled(DependencyObject dependencyObject, bool enabled)
        {
            dependencyObject.SetValue(IsEnabledProperty, enabled);
        }

        public static bool GetIsEnabled(DependencyObject dependencyObject)
        {
            return (bool) dependencyObject.GetValue(IsEnabledProperty);
        }

        private static void OnIsEnabledPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            RadGridView grid = dependencyObject as RadGridView;
            if (grid != null)
            {
                if ((bool) e.NewValue)
                {
                    var behavior = new EmptyDataTemplateBehavior(grid);
                    behavior.Attach();
                }
            }
        }

        private void Attach()
        {
            if (this.grid != null)
            {
                this.grid.LayoutUpdated += new EventHandler(this.OnGridLayoutUpdated);
            }
        }

        private void OnGridLayoutUpdated(object sender, EventArgs e)
        {
            Grid rootGrid = this.grid.ChildrenOfType<Grid>().FirstOrDefault();

            if (rootGrid != null)
            {
                this.grid.LayoutUpdated -= new EventHandler(this.OnGridLayoutUpdated);
                this.LoadTemplateIntoGridView(this.grid);
                this.grid.Items.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(this.OnItemsCollectionChanged);
                SetVisibility();
            }
        }

        private void OnItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.SetVisibility();
        }

        private void SetVisibility()
        {
            if (this.grid.Items.Count == 0)
            {
                this.contentPresenter.Visibility = Visibility.Visible;
            }
            else
            {
                this.contentPresenter.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadTemplateIntoGridView(RadGridView gridView)
        {

            contentPresenter.IsHitTestVisible = false;
            contentPresenter.DataContext = this;
            contentPresenter.ContentTemplate = this.EmptyDataTemplate;
            Grid rootGrid = gridView.ChildrenOfType<Grid>().FirstOrDefault();

            contentPresenter.SetValue(Grid.RowProperty, 2);
            contentPresenter.SetValue(Grid.RowSpanProperty, 2);
            contentPresenter.SetValue(Grid.ColumnSpanProperty, 2);
            contentPresenter.SetValue(Border.MarginProperty, new Thickness(0, 27, 0, 0));
            rootGrid.Children.Add(contentPresenter);
        }
    }
}
