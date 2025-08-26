using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
using WinRT.Interop;

namespace NovelEditor
{
    public partial class App : Application
    {
        public static Window? window { get; private set; }

        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            window = new MainWindow();
            // 从窗口对象获取窗口句柄 (HWND)
            var hWnd = WindowNative.GetWindowHandle(window);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            // 获取 OverlappedPresenter 并调用 Maximize()
            if (appWindow.Presenter is OverlappedPresenter overlappedPresenter)
            {
                overlappedPresenter.Maximize();
            }
            window.Activate();
        }
    }
}
