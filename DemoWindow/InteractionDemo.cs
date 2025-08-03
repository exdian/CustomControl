using CustomControlLibrary;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace DemoWindow
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private bool _canThreeContinuous;
        private bool _liked;
        private bool _coined;
        private bool _stared;
        private readonly Random _random = new();
        public bool CanThreeContinuous
        {
            get => _canThreeContinuous;
            set
            {
                _canThreeContinuous = value;
                OnPropertyChanged(nameof(CanThreeContinuous));
            }
        }
        public bool Liked
        {
            get => _liked;
            set
            {
                _liked = value;
                OnPropertyChanged(nameof(Liked));
            }
        }
        public bool Coined
        {
            get => _coined;
            set
            {
                _coined = value;
                OnPropertyChanged(nameof(Coined));
            }
        }
        public bool Stared
        {
            get => _stared;
            set
            {
                _stared = value;
                OnPropertyChanged(nameof(Stared));
            }
        }
        public bool CanButtonBigger { get; set; }
        public bool CanButtonRotate { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;
        private void SetDataContext()
        {
            DataContext = this;
        }
        private void SlideToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not SlideToggleButton button)
                return;
            if (CanButtonBigger)
            {
                button.Width += 4;
                button.Height += 2;
                button.TrackCornerRadius = new(button.Height / 2);
            }
            else
            {
                var width = button.Width;
                var height = button.Height;
                width -= 4;
                height -= 2;
                button.Width = width < 60 ? 60 : width;
                button.Height = height < 30 ? 30 : height;
                button.TrackCornerRadius = new(button.Height / 2);
            }
            if (button.RenderTransform is RotateTransform rotateTransform)
            {
                var angle = rotateTransform.Angle;
                if (CanButtonRotate)
                {
                    rotateTransform.Angle = (angle + 2) % 360;
                }
                else
                {
                    angle -= 2;
                    rotateTransform.Angle = angle < 0 ? 0 : angle;
                }
                rotateTransform.CenterX = button.Width / 2;
                rotateTransform.CenterY = button.Height / 2;
            }
        }
        private void SlideToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            txtTop.Text = "对";
        }
        private void SlideToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            txtTop.Text = "不对";
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (!(CanThreeContinuous && Liked && Coined && Stared))
                return;
            switch (propertyName)
            {
                default:
                    break;
                case nameof(Liked):
                    if (_random.Next(0, 99) > 49)
                        Coined = false;
                    else
                        Stared = false;
                    break;
                case nameof(Coined):
                    if (_random.Next(0, 99) > 49)
                        Liked = false;
                    else
                        Stared = false;
                    break;
                case nameof(Stared):
                    if (_random.Next(0, 99) > 49)
                        Liked = false;
                    else
                        Coined = false;
                    break;
            }
        }
    }
    internal class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool isTrue)
                return DependencyProperty.UnsetValue;
            if (isTrue)
                return Visibility.Visible;
            else
                return Visibility.Hidden;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    internal class BoolToFillConverter : IValueConverter
    {
        public static readonly Color LikedColor = Color.FromRgb(234, 122, 153);
        public static readonly Color CoinedColor = Color.FromRgb(74, 172, 231);
        public static readonly Color StaredColor = Color.FromRgb(249, 217, 96);
        private static readonly Brush UnsetBrush = new SolidColorBrush(Color.FromRgb(0x3C, 0x3C, 0x3C));
        private static readonly Brush LikedBrush = new SolidColorBrush(LikedColor);
        private static readonly Brush CoinedBrush = new SolidColorBrush(CoinedColor);
        private static readonly Brush StaredBrush = new SolidColorBrush(StaredColor);
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool isTrue || !isTrue || parameter is not BrushTarget target)
                return UnsetBrush;
            return target switch
            {
                BrushTarget.Like => LikedBrush,
                BrushTarget.Coin => CoinedBrush,
                BrushTarget.Star => StaredBrush,
                _ => UnsetBrush,
            };
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    internal enum BrushTarget
    {
        Like, Coin, Star
    }
}
