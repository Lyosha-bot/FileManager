using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FileManager.UI
{
    public static class Styles
    {
        public static Style Button()
        {
            Style style = new Style(typeof(Button));
            style.Setters.Add(new Setter { Property = Control.MarginProperty, Value = new Thickness(5) });
            style.Setters.Add(new Setter { Property = Control.BackgroundProperty, Value = Brushes.Transparent });
            style.Setters.Add(new Setter { Property = Control.BorderBrushProperty, Value = Brushes.Transparent });
            style.Setters.Add(new Setter { Property = Control.BorderThicknessProperty, Value = new Thickness(1) });
            return style;
        }

        public static Style Wrapper()
        {
            Style style = new Style();
            style.Setters.Add(new Setter { Property = Control.WidthProperty, Value = 100.0 });
            style.Setters.Add(new Setter { Property = Control.HeightProperty, Value = 100.0 });
            return style;
        }

        public static Style Image()
        {
            Style style = new Style();
            style.Setters.Add(new Setter { Property = Control.MaxHeightProperty, Value = 80.0 });
            return style;
        }

        public static Style Name()
        {
            Style style = new Style();
            style.Setters.Add(new Setter { Property = Control.VerticalAlignmentProperty, Value = VerticalAlignment.Center });
            style.Setters.Add(new Setter { Property = Control.HorizontalAlignmentProperty, Value = HorizontalAlignment.Center });
            return style;
        }
    }
}
