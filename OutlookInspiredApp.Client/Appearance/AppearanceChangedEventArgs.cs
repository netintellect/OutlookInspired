using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelerikOutlookInspiredApp.Models.Appearance;

namespace TelerikOutlookInspiredApp.Appearance
{
    public class AppearanceChangedEventArgs : EventArgs
    {
        public Themes Theme { get; private set; }
        public ForegroundColorVariations ColorVariation { get; private set; }

        public AppearanceChangedEventArgs(Themes theme, ForegroundColorVariations colorVariation)
        {
            this.Theme = theme;
            this.ColorVariation = colorVariation;
        }
    }
}
