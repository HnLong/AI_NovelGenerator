using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

namespace NovelEditor.UserControls.NovelControl;

public sealed partial class NovelItemControl : UserControl
{
    // 依赖属性，用于数据绑定
    public static readonly DependencyProperty NovelTitleProperty =
        DependencyProperty.Register("NovelTitle", typeof(string), typeof(NovelItemControl), new PropertyMetadata(null));

    public static readonly DependencyProperty CreatedDateProperty =
        DependencyProperty.Register("CreatedDate", typeof(string), typeof(NovelItemControl), new PropertyMetadata(null));

    public static readonly DependencyProperty UpdatedDateProperty =
        DependencyProperty.Register("UpdatedDate", typeof(string), typeof(NovelItemControl), new PropertyMetadata(null));

    public string? NovelId { get; set; }

    public string NovelTitle
    {
        get { return (string)GetValue(NovelTitleProperty); }
        set { SetValue(NovelTitleProperty, value); }
    }

    public string CreatedDate
    {
        get { return (string)GetValue(CreatedDateProperty); }
        set { SetValue(CreatedDateProperty, value); }
    }

    public string UpdatedDate
    {
        get { return (string)GetValue(UpdatedDateProperty); }
        set { SetValue(UpdatedDateProperty, value); }
    }

    // 删除事件，通知父页面
    public event EventHandler<string?>? OnDelete;

    public NovelItemControl()
    {
        this.InitializeComponent();
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        // 触发删除事件，并传递小说 ID
        OnDelete?.Invoke(this, NovelId);
    }
}
