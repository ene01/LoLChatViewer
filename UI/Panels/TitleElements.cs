using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace LoLChatViewer.UI.Panels
{
    public class TitleElements
    {
        // Label fijo que se usa dentro del titleGrid.
        public Label titleLabel = new()
        {
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Left,
            Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)),
            Foreground = new SolidColorBrush(Color.FromArgb(200, 255, 255, 255)),
            Margin = new Thickness(10, 6, 0, 0),
            FontSize = 14,
            Content = "🡶 Estas viendo: ",
        };

        // Grid delgado posicionado arriba que solo muestra texto.
        private Grid titleHandler = new()
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            IsHitTestVisible = true,
        };

        public Grid Show()
        {
            // Añadir labels.
            titleHandler.Children.Add(titleLabel);

            return titleHandler;
        }
    }
}
