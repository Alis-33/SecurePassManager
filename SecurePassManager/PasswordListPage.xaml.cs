using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using SecurePassManager.Models;
using SecurePassManager.Services;

namespace SecurePassManager;

public partial class PasswordListPage : ContentPage
{
    private readonly PasswordStorageService _passwordStorageService;
    private readonly IServiceProvider _serviceProvider;
    private string _currentUserId;
    private string _masterPassword; // Store the master password

    public PasswordListPage(PasswordStorageService passwordStorageService, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _passwordStorageService = passwordStorageService;
        _serviceProvider = serviceProvider;

        Debug.WriteLine("PasswordListPage constructor called");
    }

    public void Initialize(string userId, string masterPassword)
    {
        Debug.WriteLine($"Initialize called with userId: {userId} and masterPassword provided");

        _currentUserId = userId;
        _masterPassword = masterPassword; // Set the master password
        LoadPasswords();
    }

    private void LoadPasswords()
    {
        Debug.WriteLine($"LoadPasswords called for userId: {_currentUserId}");
        try
        {
            var passwords = _passwordStorageService.GetAllPasswords(_currentUserId, _masterPassword);
            Debug.WriteLine($"Number of passwords loaded: {passwords.Count()}");
            PasswordCollectionView.ItemsSource = passwords;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading passwords: {ex.Message}");
            DisplayAlert("Error", "Failed to load passwords. Please check your master password.", "OK");
        }
    }

    private async void OnAddPasswordClicked(object sender, EventArgs e)
    {
        var passwordEntryPage = _serviceProvider.GetRequiredService<PasswordEntryPage>();
        passwordEntryPage.Initialize(_currentUserId, _masterPassword); // Pass user ID and master password
        await Navigation.PushAsync(passwordEntryPage);
    }

    private async void OnPasswordSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is PasswordEntry selectedPassword)
        {
            // Deselect the item
            ((CollectionView)sender).SelectedItem = null;

            await NavigateToPasswordEntryPage(selectedPassword);
        }
    }

    private async void OnPasswordTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is PasswordEntry tappedPassword)
        {
            await NavigateToPasswordEntryPage(tappedPassword);
        }
    }

    private async Task NavigateToPasswordEntryPage(PasswordEntry password)
    {
        var passwordEntryPage = _serviceProvider.GetRequiredService<PasswordEntryPage>();
        passwordEntryPage.Initialize(_currentUserId, _masterPassword); // Pass user ID and master password
        passwordEntryPage.SetPasswordEntry(password);
        await Navigation.PushAsync(passwordEntryPage);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Debug.WriteLine("PasswordListPage OnAppearing called");
        LoadPasswords();
    }
}
