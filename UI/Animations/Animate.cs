using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Diagnostics;

namespace LoLChatViewer.UI.Animations
{
    internal class Animate
    {
        public enum ColorProperty
        {
            Background,
            Foreground,
            Stroke,
            BorderBrush,
            Fill
        }

        public static void Color(FrameworkElement element, ColorProperty elementProperty, Color newColor, IEasingFunction ease, double time, double delay, Color? fromColor = null)
        {
            Color? currentColor = null;

            ColorProperty selectedProperty = elementProperty;

            if (fromColor != null)
            {
                currentColor = fromColor;
            }
            else if (element is Control)
            {
                if (selectedProperty == ColorProperty.Background)
                {
                    currentColor = ((element as Control).Background as SolidColorBrush)?.Color;
                }
                else if (selectedProperty == ColorProperty.Foreground)
                {
                    currentColor = ((element as Control).Foreground as SolidColorBrush)?.Color;
                }
                else if (selectedProperty == ColorProperty.BorderBrush)
                {
                    currentColor = ((element as Control).BorderBrush as SolidColorBrush)?.Color;
                }
            }
            else if (element is Panel)
            {
                if (selectedProperty == ColorProperty.Background)
                {
                    currentColor = ((element as Panel)?.Background as SolidColorBrush)?.Color;
                }
            }
            else if (element is Shape)
            {
                if (selectedProperty == ColorProperty.Fill)
                {
                    currentColor = ((element as Shape)?.Fill as SolidColorBrush)?.Color;
                }
                else if (selectedProperty == ColorProperty.Stroke)
                {
                    currentColor = ((element as Shape)?.Stroke as SolidColorBrush)?.Color;
                }
            }
            else if (element is TextBlock) // We do a bit of hardcoding 🤏 :tf:
            {
                if (selectedProperty == ColorProperty.Background)
                {
                    currentColor = ((element as TextBlock).Background as SolidColorBrush)?.Color;
                }
                else if (selectedProperty == ColorProperty.Foreground)
                {
                    currentColor = ((element as TextBlock).Foreground as SolidColorBrush)?.Color;
                }
            }

            ColorAnimation colorAnim = new()
            {
                From = currentColor,
                To = newColor,
                Duration = TimeSpan.FromMilliseconds(time),
                BeginTime = TimeSpan.FromMilliseconds(delay),
                EasingFunction = ease
            };

            SolidColorBrush brushAnimColor = new SolidColorBrush();

            if (element is Control)
            {
                Control controlElement = element as Control;

                if (selectedProperty == ColorProperty.Background)
                {
                    if (controlElement != null)
                    {
                        controlElement.Background = brushAnimColor;
                        controlElement.Background.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);
                    }
                }
                else if (selectedProperty == ColorProperty.Foreground)
                {
                    if (controlElement != null)
                    {
                        controlElement.Foreground = brushAnimColor;
                        controlElement.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);
                    }
                }
                else if (selectedProperty == ColorProperty.BorderBrush)
                {
                    if (controlElement != null)
                    {
                        controlElement.BorderBrush = brushAnimColor;
                        controlElement.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);
                    }
                }
            }
            else if (element is Panel)
            {
                Panel panelElement = element as Panel;

                if (selectedProperty == ColorProperty.Background)
                {
                    if (panelElement != null)
                    {
                        panelElement.Background = brushAnimColor;
                        panelElement.Background.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);
                    }
                }
            }
            else if (element is Shape)
            {
                Shape shapeElement = element as Shape;

                if (selectedProperty == ColorProperty.Fill)
                {
                    if (shapeElement != null)
                    {
                        shapeElement.Fill = brushAnimColor;
                        shapeElement.Fill.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);
                    }
                }
                else if (selectedProperty == ColorProperty.Stroke)
                {
                    if (shapeElement != null)
                    {
                        shapeElement.Stroke = brushAnimColor;
                        shapeElement.Stroke.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);
                    }
                }
            }
            else if (element is TextBlock)
            {
                TextBlock textBlockElement = element as TextBlock;

                if (selectedProperty == ColorProperty.Background)
                {
                    if (textBlockElement != null)
                    {
                        textBlockElement.Background = brushAnimColor;
                        textBlockElement.Background.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);
                    }
                }
                else if (selectedProperty == ColorProperty.Foreground)
                {
                    if (textBlockElement != null)
                    {
                        textBlockElement.Foreground = brushAnimColor;
                        textBlockElement.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);
                    }
                }
            }
        }

        public static void Size(FrameworkElement element, double newHeight, double newWidth, IEasingFunction ease, int time, int delay, double fromHeight = 0,  double fromWidth = 0)
        {
            double currentHeight, currentWidth;

            if (fromHeight != 0)
            {
                currentHeight = fromHeight;
            }
            else
            {
                currentHeight = element.Height;
            }

            if (fromWidth != 0)
            {
                currentWidth = fromWidth;
            }
            else
            {
                currentWidth = element.Width;
            }

            DoubleAnimation hAnim = new()
            {
                From = currentHeight,
                To = newHeight,
                Duration = TimeSpan.FromMilliseconds(time),
                BeginTime = TimeSpan.FromMilliseconds(delay),
                EasingFunction = ease
            };

            DoubleAnimation wAnim = new()
            {
                From = currentWidth,
                To = newWidth,
                Duration = TimeSpan.FromMilliseconds(time),
                BeginTime = TimeSpan.FromMilliseconds(delay),
                EasingFunction = ease
            };

            Storyboard sb = new Storyboard();

            sb.Children.Add(hAnim);
            sb.Children.Add(wAnim);

            Storyboard.SetTarget(hAnim, element);
            Storyboard.SetTargetProperty(hAnim, new PropertyPath(FrameworkElement.HeightProperty));

            Storyboard.SetTarget(wAnim, element);
            Storyboard.SetTargetProperty(wAnim, new PropertyPath(FrameworkElement.WidthProperty));

            sb.Begin();
        }

        public static void Position(FrameworkElement element, TranslateTransform newPosition, IEasingFunction ease, int time, int delay, TranslateTransform fromPosition = null)
        {
            TranslateTransform currentPosition;

            if (fromPosition != null)
            {
                currentPosition = fromPosition;
            }
            else
            {
                currentPosition = element.RenderTransform as TranslateTransform;
            }

            if (currentPosition == null)
            {
                currentPosition = new TranslateTransform();
                element.RenderTransform = currentPosition;
            }

            DoubleAnimation xAnim = new()
            {
                From = currentPosition.X,
                To = newPosition.X,
                Duration = TimeSpan.FromMilliseconds(time),
                EasingFunction = ease,
                BeginTime = TimeSpan.FromMilliseconds(delay)
            };

            DoubleAnimation yAnim = new()
            {
                From = currentPosition.Y,
                To = newPosition.Y,
                Duration = TimeSpan.FromMilliseconds(time),
                EasingFunction = ease,
                BeginTime = TimeSpan.FromMilliseconds(delay)
            };

            TranslateTransform elementAnim = new();
            element.RenderTransform = elementAnim;

            elementAnim.BeginAnimation(TranslateTransform.XProperty, xAnim);
            elementAnim.BeginAnimation(TranslateTransform.YProperty, yAnim);
        }

        public static void Opacity(FrameworkElement element, double newOpacity, IEasingFunction ease, int time, int delay, double? fromOpacity = null)
        {
            double? currentOpacity;

            if (fromOpacity != null)
            {
                currentOpacity = fromOpacity;
            }
            else
            {
                currentOpacity = element.Opacity;
            }

            DoubleAnimation oAnim = new()
            {
                From = currentOpacity,
                To = newOpacity,
                EasingFunction = ease,
                BeginTime = TimeSpan.FromMilliseconds(delay),
                Duration = TimeSpan.FromMilliseconds(time)
            };

            Storyboard sb = new();

            sb.Children.Add(oAnim);

            Storyboard.SetTarget(oAnim, element);

            Storyboard.SetTargetProperty(oAnim, new PropertyPath(UIElement.OpacityProperty));

            sb.Begin();
        }

        public void Rotation(FrameworkElement element, double newAngle, Point center, IEasingFunction ease, int time, int delay)
        {
            RotateTransform? currentAngle;

            currentAngle = element.RenderTransform as RotateTransform;

            if (currentAngle == null)
            {
                currentAngle = new RotateTransform();
                element.RenderTransform = currentAngle;
            }

            element.RenderTransformOrigin = center;

            DoubleAnimation rAnim = new DoubleAnimation()
            {
                From = currentAngle.Angle,
                To = newAngle,
                Duration = TimeSpan.FromMilliseconds(time),
                EasingFunction = ease,
                BeginTime = TimeSpan.FromMilliseconds(delay)
            };

            Storyboard sb = new Storyboard();

            sb.Children.Add(rAnim);

            Storyboard.SetTarget(rAnim, element);
            Storyboard.SetTargetProperty(rAnim, new PropertyPath("RenderTransform.Angle"));

            sb.Begin();
        }
    }
}
