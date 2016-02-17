using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace TelerikOutlookInspiredApp.Appearance
{
    public class ThemeResourceExtension : MarkupExtension
    {
        public Resources Resource { get; set; }

        public ThemeResourceExtension()
        {

        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException("serviceProvider");
            }

            string propertyPath;
            if (ThemePalette.TryGetResource(this.Resource, out propertyPath))
            {
                Binding binding = new Binding(propertyPath)
                {
                    Source = ThemePalette.Palette,
                    Converter = new ColorToSolidColorBrushConverter(),
                    Mode = BindingMode.OneWay
                };
                return binding.ProvideValue(serviceProvider);
            }

            return null;
        }
    }
}
