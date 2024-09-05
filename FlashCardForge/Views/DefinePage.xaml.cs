using System.Text.RegularExpressions;
using FlashCardForge.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System;
using Windows.UI.Core;

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
        LanguageMode.SelectedItem = LanguageMode.Items[0];

        keyWordTextBox.Loaded += (s, e) =>
        {
            ViewModel.SetKeyWordTextBoxSelectAllCommand(KeyWordTextBox_SelectAll);
        };
    }

    private void KeyWordTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if((Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down && e.Key == VirtualKey.Enter)
        {
            if (ViewModel.AddToDeckCommand.CanExecute(null))
            {
                ViewModel.AddToDeckCommand.Execute(null);
                return;
            }
        }
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            if (ViewModel.LookupCommand.CanExecute(null))
            {
                ViewModel.LookupCommand.Execute(null);
                return;
            }
        }
    }

    private void KeyWordTextBox_SelectAll()
    {
        keyWordTextBox.SelectAll();
        keyWordTextBox.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
    }

    private void LanguageMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var comboBox = sender as ComboBox;
        var selectedItem = comboBox.SelectedItem as ComboBoxItem;

        if (selectedItem != null)
        {
            var selectedContent = selectedItem.Content.ToString();
            ViewModel.SetExtractionService(selectedContent);
        }
    }
}
