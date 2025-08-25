using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using NovelEditor.Data;
using NovelEditor.Data.Models;
using NovelEditor.UserControls.NovelControl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;


namespace NovelEditor.Pages;

public sealed partial class NovelDisplayPage : Page
{
    private readonly MongoDbHelper _dbHelper;

    public NovelDisplayPage()
    {
        this.InitializeComponent();
        _dbHelper = new MongoDbHelper();
        this.Loaded += NovelDisplayPage_Loaded;
    }

    private async void NovelDisplayPage_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadNovelsAsync();
    }

    /// <summary>
    /// 从数据库异步加载小说并更新 UI。
    /// </summary>
    private async Task LoadNovelsAsync()
    {
        var novels = await _dbHelper.GetAllNovelsAsync();

        if (novels != null && novels.Any())
        {
            NovelsListView.Items.Clear();
            foreach (var novel in novels)
            {
                // 将 NovelTitle 的赋值加上 null 合并运算符，确保不会赋值为 null
                var novelControl = new NovelItemControl
                {
                    NovelId = novel.NovelId,
                    NovelTitle = novel.NovelTitle ?? string.Empty,
                    CreatedDate = novel.CreatedAt ?? string.Empty,
                    UpdatedDate = novel.UpdatedAt ?? string.Empty
                };
                novelControl.OnDelete += NovelControl_OnDelete;
                NovelsListView.Items.Add(novelControl);
            }
            NovelsListView.Visibility = Visibility.Visible;
            NoNovelsPanel.Visibility = Visibility.Collapsed;
        }
        else
        {
            NovelsListView.Visibility = Visibility.Collapsed;
            NoNovelsPanel.Visibility = Visibility.Visible;
        }
    }

    /// <summary>
    /// 处理创建新小说的按钮点击事件。
    /// </summary>
    private async void CreateNovelButton_Click(object sender, RoutedEventArgs e)
    {
        // 简单创建一个示例小说
        var newNovel = new Novel
        {
            NovelTitle = $"新小说 {DateTime.Now:yyyyMMddHHmmss}",
            CreatedAt = DateTime.UtcNow.ToString("o"),
            UpdatedAt = DateTime.UtcNow.ToString("o")
        };
        await _dbHelper.CreateNovelAsync(newNovel);
        await LoadNovelsAsync(); // 重新加载列表
    }

    private async void NovelControl_OnDelete(object? sender, string? novelId)
    {
        if (novelId is not null)
        {
            await _dbHelper.DeleteNovelAsync(novelId);
            await LoadNovelsAsync(); // 重新加载列表
        }
    }
}
