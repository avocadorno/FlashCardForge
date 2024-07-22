using FlashCardForge.ViewModels;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace FlashCardForge.Views;

public sealed partial class DefinePage : Page
{
    public DefineViewModel ViewModel
    {
        get;
    }

    public DefinePage()
    {
        ViewModel = App.GetService<DefineViewModel>();
        InitializeComponent();

        dictionaryWebView.Loaded += async (s, e) =>
        {
            await dictionaryWebView.EnsureCoreWebView2Async();
            if (dictionaryWebView.CoreWebView2 != null)
            {
                ViewModel.SetWebView(dictionaryWebView.CoreWebView2);
            }
        };
    }

    private void KeyWordTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            if (ViewModel.LookupCommand.CanExecute(null))
            {
                ViewModel.LookupCommand.Execute(null);
            }
        }
    }
}
