using FlashCardForge.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace FlashCardForge.Views;

// TODO: Change the grid as appropriate for your app. Adjust the column definitions on DataGridPage.xaml.
// For more details, see the documentation at https://docs.microsoft.com/windows/communitytoolkit/controls/datagrid.
public sealed partial class BrowsePage : Page
{
    public BrowseViewModel ViewModel
    {
        get;
    }

    public BrowsePage()
    {
        ViewModel = App.GetService<BrowseViewModel>();
        InitializeComponent();
    }
}
