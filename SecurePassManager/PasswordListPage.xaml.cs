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

    public PasswordListPage(PasswordStorageService passwordStorageService, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _passwordStorageService = passwordStorageService;
        _serviceProvider = serviceProvider;
        
        
        Debug.WriteLine("PasswordListPage constructor called");
    }

    public void SetUser(string userId)
    {
        Debug.WriteLine($"SetUser called with userId: {userId}");

        _currentUserId = userId;
        LoadPasswords();
    }

    private void LoadPasswords()
    {
        Debug.WriteLine($"LoadPasswords called for userId: {_currentUserId}");
        var passwords = _passwordStorageService.GetAllPasswords(_currentUserId);
        Debug.WriteLine($"Number of passwords loaded: {passwords.Count()}");
        PasswordCollectionView.ItemsSource = passwords;
    }

    private async void OnAddPasswordClicked(object sender, EventArgs e)
    {
        var passwordEntryPage = _serviceProvider.GetRequiredService<PasswordEntryPage>();
        passwordEntryPage.SetUser(_currentUserId);
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
        passwordEntryPage.SetUser(_currentUserId);
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