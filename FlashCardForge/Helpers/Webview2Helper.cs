using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

namespace FlashCardForge.Helpers;
internal static class Webview2Helper
{
    public static readonly DependencyProperty BindableHtmlProperty =
        DependencyProperty.RegisterAttached(
            "BindableHtml",
            typeof(string),
            typeof(Webview2Helper),
            new PropertyMetadata(string.Empty, OnBindableHtmlChanged));

    public static string GetBindableHtml(DependencyObject obj)
    {
        return (string)obj.GetValue(BindableHtmlProperty);
    }

    public static void SetBindableHtml(DependencyObject obj, string value)
    {
        obj.SetValue(BindableHtmlProperty, value);
    }

    private static async void OnBindableHtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WebView2 webView)
        {
            var html = e.NewValue as string;
            if (!string.IsNullOrEmpty(html))
            {
                await webView.EnsureCoreWebView2Async();
                webView.NavigateToString(html);
            }
        }
    }
}