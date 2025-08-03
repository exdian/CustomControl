using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CustomControlLibrary
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:CustomControlLibrary"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:CustomControlLibrary;assembly=CustomControlLibrary"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:SlideToggleButton/>
    ///
    /// </summary>
    [TemplatePart(Name = "PART_Track", Type = typeof(Border))]
    [TemplatePart(Name = "PART_Thumb", Type = typeof(Border))]
    public class SlideToggleButton : Thumb, ICommandSource
    {
        static SlideToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SlideToggleButton), new FrameworkPropertyMetadata(typeof(SlideToggleButton)));
        }
        public SlideToggleButton()
        {
            Loaded += OnLoaded;
            DragStarted += OnDragStarted;
            DragCompleted += OnDragCompleted;
            DragDelta += OnDragDelta;
            Unloaded += OnUnloaded;
            SetValue(ThumbTransformKey, new TranslateTransform(0, 0));
        }
        #region 依赖属性
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
            "Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SlideToggleButton));
        public event RoutedEventHandler Click
        {
            add => AddHandler(ClickEvent, value);
            remove => RemoveHandler(ClickEvent, value);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(SlideToggleButton), new(null, OnCommandChanged));
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(SlideToggleButton));
        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public static readonly DependencyProperty CommandTargetProperty =
            DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(SlideToggleButton));
        public IInputElement CommandTarget
        {
            get => (IInputElement)GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public static readonly RoutedEvent CheckedEvent = EventManager.RegisterRoutedEvent(
            "Checked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SlideToggleButton));
        public event RoutedEventHandler Checked
        {
            add => AddHandler(CheckedEvent, value);
            remove => RemoveHandler(CheckedEvent, value);
        }

        public static readonly RoutedEvent UncheckedEvent = EventManager.RegisterRoutedEvent(
            "Unchecked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SlideToggleButton));
        public event RoutedEventHandler Unchecked
        {
            add => AddHandler(UncheckedEvent, value);
            remove => RemoveHandler(UncheckedEvent, value);
        }

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(SlideToggleButton), new(false, OnIsCheckedChanged));
        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        private static readonly DependencyPropertyKey ThumbTransformKey =
            DependencyProperty.RegisterReadOnly("ThumbTransform", typeof(TranslateTransform), typeof(SlideToggleButton), new(defaultValue: null));
        public static readonly DependencyProperty ThumbTransformProperty = ThumbTransformKey.DependencyProperty;
        public TranslateTransform ThumbTransform
        {
            get => (TranslateTransform)GetValue(ThumbTransformProperty);
        }

        public static readonly DependencyProperty TrackCornerRadiusProperty =
            DependencyProperty.Register("TrackCornerRadius", typeof(CornerRadius), typeof(SlideToggleButton));
        public CornerRadius TrackCornerRadius
        {
            get => (CornerRadius)GetValue(TrackCornerRadiusProperty);
            set => SetValue(TrackCornerRadiusProperty, value);
        }

        public static readonly DependencyProperty CheckedTrackColorProperty =
            DependencyProperty.Register("CheckedTrackColor", typeof(Color), typeof(SlideToggleButton), new(defaultValue: Colors.Green));
        public Color CheckedTrackColor
        {
            get => (Color)GetValue(CheckedTrackColorProperty);
            set => SetValue(CheckedTrackColorProperty, value);
        }

        public static readonly DependencyProperty UncheckedTrackColorProperty =
            DependencyProperty.Register("UncheckedTrackColor", typeof(Color), typeof(SlideToggleButton), new(defaultValue: Colors.Gray));
        public Color UncheckedTrackColor
        {
            get => (Color)GetValue(UncheckedTrackColorProperty);
            set => SetValue(UncheckedTrackColorProperty, value);
        }

        public static readonly DependencyProperty ThumbColorProperty =
            DependencyProperty.Register("ThumbColor", typeof(Color), typeof(SlideToggleButton), new(defaultValue: Colors.White));
        public Color ThumbColor
        {
            get => (Color)GetValue(ThumbColorProperty);
            set => SetValue(ThumbColorProperty, value);
        }
        #endregion
        private Border? PART_Track;
        private Border? PART_Thumb;
        private bool _isDragging;
        private Point _dragStartPoint;
        private double _dragOffset = 0;
        private double _dragDistanceMax;
        public static SlideToggleButtonHelper Helper { get; } = new();
        // 方法按照可能的执行顺序来排序
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_Track = GetTemplateChild("PART_Track") as Border;
            PART_Thumb = GetTemplateChild("PART_Thumb") as Border;
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (PART_Track != null)
            {
                if (PART_Track.Background is SolidColorBrush brush)
                    brush.Color = IsChecked ? CheckedTrackColor : UncheckedTrackColor;
            }
            if (PART_Thumb != null)
            {
                if (PART_Thumb.Child is Shape shape)
                    shape.Fill = new SolidColorBrush(ThumbColor);
                else if (PART_Thumb.Child is Control control)
                    control.Background = new SolidColorBrush(ThumbColor);
                else if (PART_Thumb.Background is SolidColorBrush brush)
                    brush.Color = ThumbColor;
            }
            if (IsChecked)
                UpdateVisualCheckState();
        }
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            Helper.ThumbColor = ThumbColor;
        }
        private void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            _isDragging = true;
            _dragStartPoint = Mouse.GetPosition(this);
            VisualStateManager.GoToState(this, "Dragging", true);
            UpdateThumbTranslateTransform();
            _dragDistanceMax = Width - Height;
        }
        /// <summary>
        /// 清除动画对属性的控制权，使后续本地赋值生效
        /// </summary>
        private void UpdateThumbTranslateTransform()
        {
            if (PART_Thumb == null)
                return;
            _dragOffset = ThumbTransform.X;
            ThumbTransform.BeginAnimation(TranslateTransform.XProperty, null);
            ThumbTransform.X = _dragOffset;
        }
        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (PART_Thumb == null)
                return;
            var horizontalDistance = e.HorizontalChange + _dragOffset;
            ThumbTransform.X = Math.Max(0, Math.Min(_dragDistanceMax, horizontalDistance));
        }
        private void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (!_isDragging)
                return;
            // 拖拽距离小于阈值则触发点击，往正确的方向拖拽一定距离后触发滑动切换
            var oldValue = IsChecked;
            var endPoint = Mouse.GetPosition(this);
            double deltaX = endPoint.X - _dragStartPoint.X;
            double deltaY = endPoint.Y - _dragStartPoint.Y;
            double distance = deltaX * deltaX + deltaY * deltaY;
            if (distance < 5 * 5)
                OnClick();
            else if (ThumbTransform.X > (Width - Height) / 2)
                IsChecked = true;
            else
                IsChecked = false;
            if (oldValue == IsChecked)
                UpdateVisualCheckState();
            _isDragging = false;
        }
        protected virtual void OnClick()
        {
            if (RaiseClickEvent())
                return;
            IsChecked = !IsChecked;
            ExecuteCommand();
        }
        private bool RaiseClickEvent()
        {
            var args = new RoutedEventArgs(ClickEvent);
            RaiseEvent(args);
            return args.Handled;
        }
        private void ExecuteCommand()
        {
            if (Command == null)
                return;
            if (Command is RoutedCommand routedCommand)
            {
                var target = CommandTarget ?? this;
                if (routedCommand.CanExecute(CommandParameter, target))
                    routedCommand.Execute(CommandParameter, target);
            }
            else if (Command.CanExecute(CommandParameter))
            {
                Command.Execute(CommandParameter);
            }
        }
        private void UpdateVisualCheckState()
        {
            VisualStateManager.GoToState(this, "Reset", true); // 防止因重复值而动作失效
            if (IsChecked)
            {
                Helper.ThumbEndPoint = Width - Height;
                Helper.CheckedTrackColor = CheckedTrackColor;
                VisualStateManager.GoToState(this, "Checked", true);
            }
            else
            {
                Helper.UncheckedTrackColor = UncheckedTrackColor;
                VisualStateManager.GoToState(this, "Unchecked", true);
            }
        }
        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (SlideToggleButton)d;
            control.UpdateVisualCheckState();
            if ((bool)e.NewValue)
                control.CheckedAction();
            else
                control.UncheckedAction();
            control.OnToggle();
        }
        private void CheckedAction()
        {
            var args = new RoutedEventArgs(CheckedEvent);
            OnChecked(args);
        }
        private void UncheckedAction()
        {
            var args = new RoutedEventArgs(UncheckedEvent);
            OnUnchecked(args);
        }
        protected virtual void OnChecked(RoutedEventArgs e)
        {
            RaiseEvent(e);
        }
        protected virtual void OnUnchecked(RoutedEventArgs e)
        {
            RaiseEvent(e);
        }
        protected virtual void OnToggle() { }
        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (SlideToggleButton)d;
            control.HookCommand(e.OldValue as ICommand, e.NewValue as ICommand);
        }
        private void HookCommand(ICommand? oldCommand, ICommand? newCommand)
        {
            // 移除旧命令的监听
            if (oldCommand != null)
            {
                CanExecuteChangedEventManager.RemoveHandler(oldCommand, OnCanExecuteChanged);
            }
            // 添加新命令的监听
            if (newCommand != null)
            {
                CanExecuteChangedEventManager.AddHandler(newCommand, OnCanExecuteChanged);
            }
            UpdateCanExecuteState(); // 初始状态更新
        }
        private void UpdateCanExecuteState()
        {
            if (Command == null)
            {
                IsEnabled = true; // 没有命令时默认启用
                return;
            }
            if (Command is RoutedCommand routedCommand)
            {
                var target = CommandTarget ?? this;
                IsEnabled = routedCommand.CanExecute(CommandParameter, target);
            }
            else
            {
                IsEnabled = Command.CanExecute(CommandParameter);
            }
        }
        private void OnCanExecuteChanged(object? sender, EventArgs e)
        {
            UpdateCanExecuteState();
        }
        private void OnUnloaded(object s, RoutedEventArgs e)
        {
            // 控件卸载时移除事件监听
            if (Command != null)
            {
                CanExecuteChangedEventManager.RemoveHandler(Command, OnCanExecuteChanged);
            }
        }
        public class SlideToggleButtonHelper : INotifyPropertyChanged
        {
            private double _thumbEndPoint;
            private Color _checkedTrackColor;
            private Color _uncheckedTrackColor;
            private Color _thumbColor;
            public double ThumbEndPoint
            {
                get => _thumbEndPoint;
                set
                {
                    _thumbEndPoint = value;
                    OnPropertyChanged(nameof(ThumbEndPoint));
                }
            }
            public Color CheckedTrackColor
            {
                get => _checkedTrackColor;
                set
                {
                    _checkedTrackColor = value;
                    OnPropertyChanged(nameof(CheckedTrackColor));
                }
            }
            public Color UncheckedTrackColor
            {
                get => _uncheckedTrackColor;
                set
                {
                    _uncheckedTrackColor = value;
                    OnPropertyChanged(nameof(UncheckedTrackColor));
                }
            }
            public Color ThumbColor
            {
                get => _thumbColor;
                set
                {
                    _thumbColor = value;
                    OnPropertyChanged(nameof(ThumbColor));
                }
            }
            public event PropertyChangedEventHandler? PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
               => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    internal class HeightBindingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not double a)
                return 0;
            if (parameter is not ConvertParameter p)
                return a;
            var divisor = (int)p;
            if (divisor > 1)
                return a / divisor;
            else
                return a;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    [Flags]
    internal enum ConvertParameter
    {
        B0 = 1, B1 = 2, B2 = 4, B3 = 8
    }
}