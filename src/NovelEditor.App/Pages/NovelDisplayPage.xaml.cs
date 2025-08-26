using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NovelEditor.Data;
using NovelEditor.Data.Models;
using NovelEditor.UserControls.NovelControl;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace NovelEditor.Pages
{
    // 帮助类，用于告诉 GridView 对哪种数据使用哪种界面模板。
    public class NovelTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? NovelTemplate { get; set; }
        public DataTemplate? CreateTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is Novel)
                return NovelTemplate ?? new DataTemplate();
            if (item is CreateNovelControl)
                return CreateTemplate ?? new DataTemplate();
            return new DataTemplate();
        }
    }

    public sealed partial class NovelDisplayPage : Page
    {
        private readonly MongoDbHelper _dbHelper;
        // 使用 ObservableCollection，当列表内容变化时，UI 会自动更新。
        private readonly ObservableCollection<object> _displayItems = new();

        public NovelDisplayPage()
        {
            this.InitializeComponent();
            _dbHelper = new MongoDbHelper();
            this.Loaded += NovelDisplayPage_Loaded;
        }

        private async void NovelDisplayPage_Loaded(object sender, RoutedEventArgs e)
        {
            NovelsGridView.ItemsSource = _displayItems;
            await LoadNovelsAsync();
        }

        private async Task LoadNovelsAsync()
        {
            LoadingRing.IsActive = true;
            NovelsGridView.Visibility = Visibility.Collapsed;
            NoNovelsPanel.Visibility = Visibility.Collapsed;

            _displayItems.Clear();

            try
            {
                // 创建“创建新小说”控件的实例，并在这里直接订阅它的 Clicked 事件
                var createControl = new CreateNovelControl();
                createControl.Clicked += (s, args) => HandleCreateNovel();
                _displayItems.Add(createControl);

                // 从数据库加载小说数据
                var novels = await _dbHelper.GetAllNovelsAsync();
                foreach (var novel in novels)
                {
                    _displayItems.Add(novel);
                }

                // 更新 UI 的可见性
                NovelsGridView.Visibility = Visibility.Visible;
                // 仅当列表中只有“创建”按钮时，才显示空状态提示
                if (_displayItems.Count == 1)
                {
                    NoNovelsPanel.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                ShowInfoBar("错误", $"加载小说失败: {ex.Message}", InfoBarSeverity.Error);
                NovelsGridView.Visibility = Visibility.Visible; // 即使加载失败，也显示创建按钮
                NoNovelsPanel.Visibility = Visibility.Visible;
            }
            finally
            {
                LoadingRing.IsActive = false;
            }
        }

        // 当一个 NovelItemControl 在界面上显示时，此方法被调用
        private void NovelItemControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is NovelItemControl control)
            {
                // 订阅它的删除和封面更新事件
                control.OnDelete += NovelControl_OnDelete;
                control.OnCoverChanged += NovelControl_OnCoverChanged;
            }
        }

        // 当 NovelItemControl 从界面上消失时，取消订阅，以防止内存泄漏
        private void NovelItemControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (sender is NovelItemControl control)
            {
                control.OnDelete -= NovelControl_OnDelete;
                control.OnCoverChanged -= NovelControl_OnCoverChanged;
            }
        }

        public async void HandleCreateNovel()
        {
            var dialog = new ContentDialog
            {
                Title = "创建新小说",
                PrimaryButtonText = "创建",
                CloseButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot
            };

            var stackPanel = new StackPanel { Spacing = 12 };
            var titleBox = new TextBox { Header = "标题", PlaceholderText = "请输入小说标题" };
            var authorBox = new TextBox { Header = "作者", PlaceholderText = "请输入作者名" };
            var genreBox = new TextBox { Header = "类型", PlaceholderText = "例如：科幻、奇幻" };
            var descriptionBox = new TextBox { Header = "简介", PlaceholderText = "简单介绍一下你的故事", Height = 80, TextWrapping = TextWrapping.Wrap, AcceptsReturn = true };
            stackPanel.Children.Add(titleBox);
            stackPanel.Children.Add(authorBox);
            stackPanel.Children.Add(genreBox);
            stackPanel.Children.Add(descriptionBox);
            dialog.Content = stackPanel;

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                if (string.IsNullOrWhiteSpace(titleBox.Text))
                {
                    ShowInfoBar("提示", "小说标题不能为空！", InfoBarSeverity.Warning);
                    return;
                }

                var newNovel = new Novel
                {
                    NovelTitle = titleBox.Text,
                    Author = string.IsNullOrWhiteSpace(authorBox.Text) ? "匿名作者" : authorBox.Text,
                    Genre = string.IsNullOrWhiteSpace(genreBox.Text) ? "未分类" : genreBox.Text,
                    Description = string.IsNullOrWhiteSpace(descriptionBox.Text) ? "暂无简介" : descriptionBox.Text,
                    CreatedAt = DateTime.UtcNow.ToString("o"),
                    UpdatedAt = DateTime.UtcNow.ToString("o")
                };

                await _dbHelper.CreateNovelAsync(newNovel);
                ShowInfoBar("成功", $"小说《{newNovel.NovelTitle}》已创建。", InfoBarSeverity.Success);
                await LoadNovelsAsync();
            }
        }

        private async void NovelControl_OnDelete(object? sender, string? novelId)
        {
            if (novelId is not null)
            {
                await _dbHelper.DeleteNovelAsync(novelId);
                ShowInfoBar("成功", "小说已删除。", InfoBarSeverity.Success);
                await LoadNovelsAsync();
            }
        }

        private async void NovelControl_OnCoverChanged(object? sender, EventArgs e)
        {
            if (sender is NovelItemControl control && control.Novel?.NovelId != null && control.Novel.CoverImagePath != null)
            {
                await _dbHelper.UpdateNovelCoverAsync(control.Novel.NovelId, control.Novel.CoverImagePath);
                ShowInfoBar("成功", "封面已更新。", InfoBarSeverity.Success);
            }
        }

        private void ShowInfoBar(string title, string message, InfoBarSeverity severity)
        {
            PageInfoBar.Title = title;
            PageInfoBar.Message = message;
            PageInfoBar.Severity = severity;
            PageInfoBar.IsOpen = true;
        }
    }
}