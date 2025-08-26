using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;

namespace NovelEditor.UserControls.NovelControl
{
    public sealed partial class CreateNovelControl : UserControl
    {
        // 定义一个公共事件，当卡片被点击时触发
        public event EventHandler? Clicked;

        public CreateNovelControl()
        {
            this.InitializeComponent();
        }

        private void RootGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // 触发 Clicked 事件
            Clicked?.Invoke(this, EventArgs.Empty);
        }

        private void RootGrid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "PointerOver", true);
        }

        private void RootGrid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", true);
        }
    }
}
