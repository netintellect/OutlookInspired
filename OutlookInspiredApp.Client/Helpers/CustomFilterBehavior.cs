using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Telerik.Windows.Controls;

namespace TelerikOutlookInspiredApp
{
    public class CustomFilterBehavior
    {
        private readonly static DispatcherTimer timer;
        private readonly RadGridView gridView;
        private readonly RadWatermarkTextBox textBlock;
        private readonly RadBusyIndicator busyIndicator;
        private CustomFilterDescriptor filterDescriptor;

        static CustomFilterBehavior()
        {
            CustomFilterBehavior.timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1),
            };
        }

        public static readonly DependencyProperty TextBoxProperty =
            DependencyProperty.RegisterAttached("TextBox", typeof(TextBox), typeof(CustomFilterBehavior),
            new PropertyMetadata(new PropertyChangedCallback(OnTextBoxPropertyChanged)));

        public CustomFilterDescriptor FilterDescriptor
        {
            get
            {
                if (this.filterDescriptor == null)
                {
                    this.filterDescriptor = new CustomFilterDescriptor(this.gridView.Columns.OfType<Telerik.Windows.Controls.GridViewColumn>());
                    this.gridView.FilterDescriptors.Add(this.filterDescriptor);
                }
                return this.filterDescriptor;
            }
        }

        public static void SetTextBox(DependencyObject dependencyObject, TextBox tb)
        {
            dependencyObject.SetValue(TextBoxProperty, tb);
        }

        public static TextBox GetTextBox(DependencyObject dependencyObject)
        {
            return (TextBox)dependencyObject.GetValue(TextBoxProperty);
        }

        public static void OnTextBoxPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var gridView = dependencyObject as RadGridView;
            var textBlock = e.NewValue as RadWatermarkTextBox;
            var busyIndicator = gridView.ParentOfType<RadBusyIndicator>();

            if (gridView != null && textBlock != null)
            {
                var behavior = new CustomFilterBehavior(gridView, textBlock, busyIndicator);
            }
        }

        public CustomFilterBehavior(RadGridView gridView, RadWatermarkTextBox textBlock, RadBusyIndicator busyIndicator)
        {
            this.gridView = gridView;
            this.textBlock = textBlock;
            this.busyIndicator = busyIndicator;

            this.textBlock.TextChanged -= this.OnTextBlockTextChanged;
            this.textBlock.TextChanged += this.OnTextBlockTextChanged;
        }

        private void SetStatusBusyIndicator(bool isBusy)
        {
            if (this.busyIndicator != null)
            {
                this.busyIndicator.IsBusy = isBusy;
            }
        }

        private void OnTextBlockTextChanged(object sender, TextChangedEventArgs e)
        {
            if (CustomFilterBehavior.timer != null && CustomFilterBehavior.timer.IsEnabled)
            {
                CustomFilterBehavior.timer.Stop();
                CustomFilterBehavior.timer.Start();
            }
            else
            {
                if (CustomFilterBehavior.timer != null)
                {
                    CustomFilterBehavior.timer.Start();
                    CustomFilterBehavior.timer.Tick += this.OnTimerTick;
                }
            }

            this.SetStatusBusyIndicator(true);
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            CustomFilterBehavior.timer.Stop();
            this.SetStatusBusyIndicator(false);
            this.FilterDescriptor.FilterValue = this.textBlock.Text;
            this.textBlock.Focus();
        }
    }
}