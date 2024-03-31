using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml.Linq;

namespace LoLChatViewer.UI.Animations.Conversion
{
    public class ElementScrollConverter
    {
        private FrameworkElement scrollElement, parentElement;
        private double scrollPos, posLimitUp, scrollSteps;
        private TranslateTransform currentPos;
        private const double posLimitDown = 0;
        private ScrollDirection selectedDirection;

        public enum ScrollDirection
        {
            Vertical,
            Horizontal
        }

        /// <summary>
        /// Takes an element and makes it scrollable with a PreviewMouseWheel event along with specified parameters, allowing it to be moved with the mouse wheel.
        /// Also adds animations when the element reaches its limits and adjusts its position in case the element is moved by other means.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="scrollDirection"></param>
        /// <param name="upLimit"></param>
        /// <param name="downLimit"></param>
        /// <param name="steps"></param>
        /// <returns>scrollElement</returns>
        public void Convert(FrameworkElement element, ScrollDirection scrollDirection, double steps = 15)
        {
            parentElement = element.Parent as FrameworkElement;
            scrollElement = element;
            scrollSteps = steps;
            selectedDirection = scrollDirection;
            currentPos = scrollElement.RenderTransform as TranslateTransform;
            
            if (currentPos == null)
            {
                currentPos = new TranslateTransform(0, 0);
                scrollElement.RenderTransform = currentPos;
            }

            if (selectedDirection == ScrollDirection.Vertical)
            {
                scrollPos = currentPos.Y;
            }
            else
            {
                scrollPos = currentPos.X;
            }

            double offset = element.Height - parentElement.ActualHeight;

            Debug.WriteLine(element.Height + " " + parentElement.Height);

            if (offset > 0)
            {
                posLimitUp = offset;
            }
            else
            {
                posLimitUp = 0;
            }

            element.PreviewMouseWheel += Scroll;
            parentElement.SizeChanged += Parent_SizeChanged;
        }

        private void Parent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double offset = scrollElement.Height - parentElement.Height;

            if (offset > 0)
            {
                posLimitUp = offset;
            }
            else
            {
                posLimitUp = 0;
            }
        }

        public void RemoveEvents(FrameworkElement element)
        {
            element.PreviewMouseWheel -= Scroll;
        }

        // This moves the element up or down depending on which way the mouse wheel is turned. The element can exceed the limits a little bit,
        // but after 100 milliseconds if the element is exceeded, it pushes it back, creating a rubber-band effect that prevents the element from going beyond the limits.
        private async void Scroll(object sender, MouseWheelEventArgs e)
        {
            CancellationTokenSource cancelation = new CancellationTokenSource();
            Task wait = Task.Delay(100, cancelation.Token);

            currentPos = scrollElement.RenderTransform as TranslateTransform;

            if (e.Delta < 0)
            {
                scrollPos -= scrollSteps;
            }
            else
            {
                scrollPos += scrollSteps;
            }

            if (selectedDirection == ScrollDirection.Vertical)
            {
                Animate.Position(scrollElement, new TranslateTransform(currentPos.X, scrollPos), new QuadraticEase { EasingMode = EasingMode.EaseOut }, 40, 0);
            }
            else
            {
                Animate.Position(scrollElement, new TranslateTransform(scrollPos, currentPos.Y), new QuadraticEase { EasingMode = EasingMode.EaseOut }, 40, 0);
            }

            try
            {
                await Task.Delay(180);
                if (scrollPos > posLimitDown)
                {
                    scrollPos = posLimitDown;
                }
                else if (scrollPos < posLimitUp)
                {
                    scrollPos = posLimitUp;
                }

                if (selectedDirection == ScrollDirection.Vertical)
                {
                    Animate.Position(scrollElement, new TranslateTransform(currentPos.X, scrollPos), new QuadraticEase { EasingMode = EasingMode.EaseOut }, 200, 0);
                }
                else
                {
                    Animate.Position(scrollElement, new TranslateTransform(scrollPos, currentPos.Y), new QuadraticEase { EasingMode = EasingMode.EaseOut }, 200, 0);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}
