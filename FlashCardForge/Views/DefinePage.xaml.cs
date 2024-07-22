using FlashCardForge.ViewModels;

using Microsoft.UI.Xaml.Controls;

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
    }
}
