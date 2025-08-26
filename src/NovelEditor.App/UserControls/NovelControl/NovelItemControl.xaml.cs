using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media.Imaging;
using NovelEditor.Data.Models;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace NovelEditor.UserControls.NovelControl
{
    public sealed partial class NovelItemControl : UserControl
    {
        // 依赖属性，用于接收 Novel 对象
        public static readonly DependencyProperty NovelProperty =
            DependencyProperty.Register("Novel", typeof(Novel), typeof(NovelItemControl), new PropertyMetadata(null, OnNovelChanged));

        public Novel Novel
        {
            get => (Novel)GetValue(NovelProperty);
            set => SetValue(NovelProperty, value);
        }

        // 事件，用于通知父页面进行删除或封面更新操作
        public event EventHandler<string?>? OnDelete;
        public event EventHandler? OnCoverChanged;

        public NovelItemControl()
        {
            this.InitializeComponent();
            this.PointerEntered += (s, e) => VisualStateManager.GoToState(this, "PointerOver", true);
            this.PointerExited += (s, e) => VisualStateManager.GoToState(this, "Normal", true);
        }

        private static void OnNovelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NovelItemControl control && e.NewValue is Novel novel)
            {
                control.UpdateUI(novel);
            }
        }

        private void UpdateUI(Novel novel)
        {
            if (!string.IsNullOrEmpty(novel.CoverImagePath) && File.Exists(novel.CoverImagePath))
            {
                CoverImageBrush.ImageSource = new BitmapImage(new Uri(novel.CoverImagePath));
                PlaceholderGrid.Visibility = Visibility.Collapsed;
                CoverImageGrid.Visibility = Visibility.Visible;
            }
            else
            {
                PlaceholderGrid.Visibility = Visibility.Visible;
                CoverImageGrid.Visibility = Visibility.Collapsed;
            }

            UpdateDateTextBlock.Text = $"更新于 {FormatDate(novel.UpdatedAt)}";
        }

        private string FormatDate(string dateString)
        {
            if (DateTime.TryParse(dateString, null, DateTimeStyles.RoundtripKind, out var date))
            {
                var localDate = date.ToLocalTime();
                var today = DateTime.Today;
                if (localDate.Date == today) return $"今天 {localDate:HH:mm}";
                if (localDate.Date == today.AddDays(-1)) return $"昨天 {localDate:HH:mm}";
                return localDate.ToString("yyyy-MM-dd");
            }
            return "未知日期";
        }

        private async void ChangeCoverButton_Click(object sender, RoutedEventArgs e)
        {
            var file = await PickImageAsync();
            if (file != null && Novel?.NovelId != null)
            {
                var newPath = await SaveCoverImageAsync(file);
                if (!string.IsNullOrEmpty(newPath))
                {
                    Novel.CoverImagePath = newPath;
                    UpdateUI(Novel);
                    OnCoverChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// 打开文件选择器让用户选择图片
        /// </summary>
        private async Task<StorageFile?> PickImageAsync()
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            if (this.XamlRoot != null)
            {
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.window);
                InitializeWithWindow.Initialize(picker, hwnd);
            }
            else
            {
                return null;
            }

            return await picker.PickSingleFileAsync();
        }

        private async Task<string?> SaveCoverImageAsync(StorageFile file)
        {
            try
            {
                var localFolder = ApplicationData.Current.LocalFolder;
                var coversFolder = await localFolder.CreateFolderAsync("Covers", CreationCollisionOption.OpenIfExists);
                var newFileName = $"{Novel.NovelId}{file.FileType}";
                var newFile = await file.CopyAsync(coversFolder, newFileName, NameCollisionOption.ReplaceExisting);
                return newFile.Path;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void ConfirmDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var flyout = (Flyout)DeleteButton.Flyout;
            flyout.Hide();
            OnDelete?.Invoke(this, Novel.NovelId);
        }

        private void CancelDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var flyout = (Flyout)DeleteButton.Flyout;
            flyout.Hide();
        }
    }
}