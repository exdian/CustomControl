using System.Windows;
using System.Windows.Input;
using System.Windows.Shell;

namespace DemoWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            _chrome = WindowChrome.GetWindowChrome(this);
            MinimizeCommand = new OnlyEnableCommand(() => WindowState = WindowState.Minimized);
            MaximizeCommand = new OnlyEnableCommand(Maximize);
            CloseCommand = new OnlyEnableCommand(Close);
            Activated += (s, e) => IsWindowActive = true;
            Deactivated += (s, e) => IsWindowActive = false;
            StateChanged += UpdateWindowBorder;
            Loaded += WindowLoaded;
            _windowName = "演示窗口";
            _maximizeOrRestore = "";
            _windowCaption = "自定义控件";
            SetDataContext();
        }
        private readonly WindowChrome _chrome;
        private WindowState _lastState = WindowState.Normal;
        private string _windowName;
        private string _maximizeOrRestore;
        private string _windowCaption;
        private double _windowCaptionHeight;
        private Thickness _windowMargin;
        private CornerRadius _windowCornerRadius;
        private bool _isWindowActive;
        public string WindowName
        {
            get => _windowName;
            set
            {
                _windowName = value;
                OnPropertyChanged(nameof(WindowName));
            }
        }
        public string MaximizeOrRestore
        {
            get => _maximizeOrRestore;
            set
            {
                _maximizeOrRestore = value;
                OnPropertyChanged(nameof(MaximizeOrRestore));
            }
        }
        public string WindowCaption
        {
            get => _windowCaption;
            set
            {
                _windowCaption = value;
                OnPropertyChanged(nameof(WindowCaption));
            }
        }
        public double WindowCaptionHeight
        {
            get => _windowCaptionHeight;
            set
            {
                _windowCaptionHeight = value;
                OnPropertyChanged(nameof(WindowCaptionHeight));
            }
        }
        public Thickness WindowMargin
        {
            get => _windowMargin;
            set
            {
                _windowMargin = value;
                OnPropertyChanged(nameof(WindowMargin));
            }
        }
        public CornerRadius WindowCornerRadius
        {
            get => _windowCornerRadius;
            set
            {
                _windowCornerRadius = value;
                OnPropertyChanged(nameof(WindowCornerRadius));
            }
        }
        public bool IsWindowActive
        {
            get => _isWindowActive;
            set
            {
                _isWindowActive = value;
                OnPropertyChanged(nameof(IsWindowActive));
            }
        }
        public ICommand MinimizeCommand { get; }
        public ICommand MaximizeCommand { get; }
        public ICommand CloseCommand { get; }
        private void Maximize()
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }
        private void UpdateWindowBorder(object? sender, EventArgs e)
        {
            if (_lastState == WindowState.Normal && WindowState == WindowState.Maximized)
            {
                WindowMargin = new Thickness(7);
                WindowCornerRadius = new CornerRadius(0);
                var captionHeight = mainDockPanel.ActualHeight - _chrome.ResizeBorderThickness.Top + 7;
                WindowCaptionHeight = captionHeight > 0 ? captionHeight : 0;
                MaximizeOrRestore = "";
            }
            else if (_lastState == WindowState.Maximized && WindowState == WindowState.Normal)
            {
                WindowMargin = new Thickness(0);
                WindowCornerRadius = _chrome.CornerRadius;
                var captionHeight = mainDockPanel.ActualHeight - _chrome.ResizeBorderThickness.Top;
                WindowCaptionHeight = captionHeight > 0 ? captionHeight : 0;
                MaximizeOrRestore = "";
            }
            _lastState = WindowState;
        }
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            WindowCornerRadius = _chrome.CornerRadius;
            var captionHeight = mainDockPanel.ActualHeight - _chrome.ResizeBorderThickness.Top;
            WindowCaptionHeight = captionHeight > 0 ? captionHeight : 0;
        }
    }
    internal class OnlyEnableCommand(Action execute) : ICommand
    {
        private readonly Action _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => _execute();
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}