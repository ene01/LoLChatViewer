using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using LoLChatViewer.UI.Animations;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace LoLChatViewer.UI.Panels
{
    public class TitleElements
    {
        // Easing usado en las animaciones.
        public QuadraticEase quadratic = new()
        {
            EasingMode = EasingMode.EaseOut
        };

        private const int buttonHeight = 30, buttonWidth = 30, spacing = 37, miniSpacing = 8;

        // Label fijo que se usa dentro del titleGrid.
        public TextBlock titleTextBlock = new()
        {
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Left,
            Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)),
            Foreground = new SolidColorBrush(Color.FromArgb(180, 255, 255, 255)),
            Margin = new Thickness(15, 10, 0, 0),
            FontSize = 15,
        };

        public Grid customButtonGridOne = new()
        {
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Right,
            Background = new SolidColorBrush(Color.FromArgb(50, 255, 255, 255)),
            Height = buttonHeight,
            Width = buttonWidth,
            Margin = new Thickness(0, 6, miniSpacing, 0),
        };

        public Grid customButtonGridTwo = new()
        {
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Right,
            Background = new SolidColorBrush(Color.FromArgb(50, 255, 255, 255)),
            Height = buttonHeight,
            Width = buttonWidth,
            Margin = new Thickness(0, 6, spacing + miniSpacing, 0),
        };

        public Grid customButtonGridThree = new()
        {
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Right,
            Background = new SolidColorBrush(Color.FromArgb(50, 255, 255, 255)),
            Height = buttonHeight,
            Width = buttonWidth,
            Margin = new Thickness(0, 6, (spacing * 2) + miniSpacing, 0),
        };

        // Grid delgado posicionado arriba que solo muestra texto.
        private Grid titleHandler = new()
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            IsHitTestVisible = true,
        };

        public void ShowIn(Panel element, string startingTitle)
        {
            titleTextBlock.Text = startingTitle;

            // Añadir labels.
            titleHandler.Children.Add(titleTextBlock);
            titleHandler.Children.Add(customButtonGridOne);
            titleHandler.Children.Add(customButtonGridTwo);
            titleHandler.Children.Add(customButtonGridThree);

            customButtonGridOne.MouseEnter += HoverAnimationEnter;
            customButtonGridTwo.MouseEnter += HoverAnimationEnter;
            customButtonGridThree.MouseEnter += HoverAnimationEnter;

            customButtonGridOne.MouseLeave += HoverAnimationLeave;
            customButtonGridTwo.MouseLeave += HoverAnimationLeave;
            customButtonGridThree.MouseLeave += HoverAnimationLeave;

            element.Children.Clear();
            element.Children.Add(titleHandler);
        }

        // Evento que se ejecuta cuando el mouse sale del contenedor
        void HoverAnimationLeave(object sender, MouseEventArgs e)
        {
            Grid currentElement = (Grid)sender;

            Animate.Color(currentElement, Animate.ColorProperty.Background, Color.FromArgb(50, 255, 255, 255), quadratic, 250, 0);
        }

        // Evento que se ejecuta cuando el mouse entra al contenedor
        void HoverAnimationEnter(object sender, MouseEventArgs e)
        {
            Grid currentElement = (Grid)sender;

            Animate.Color(currentElement, Animate.ColorProperty.Background, Color.FromArgb(180, 255, 255, 255), quadratic, 250, 0);
        }
    }
}
