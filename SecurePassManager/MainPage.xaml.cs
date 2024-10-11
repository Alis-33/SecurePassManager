using System;
using SecurePassManager.Services;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;

namespace SecurePassManager;

public partial class MainPage : ContentPage
{
    private readonly MasterPasswordService _masterPasswordService;
    private readonly ValidationService _validationService;
    private readonly SecurityLogger _securityLogger;
    private readonly IServiceProvider _serviceProvider;

    public MainPage(MasterPasswordService masterPasswordService, ValidationService validationService, SecurityLogger securityLogger, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _masterPasswordService = masterPasswordService;
        _validationService = validationService;
        _securityLogger = securityLogger;
        _serviceProvider = serviceProvider;

        UpdatePasswordRequirements(string.Empty);
    }

    private void OnMasterPasswordTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdatePasswordRequirements(e.NewTextValue);
    }

    private void UpdatePasswordRequirements(string password)
    {
        // ... (keep existing code)
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        try
        {
            Debug.WriteLine("Login button clicked");
            string username = UsernameEntry.Text;
            string password = MasterPasswordEntry.Text;
        
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Username and password cannot be empty", "OK");
                _securityLogger.LogLoginAttempt(false, username);
                Debug.WriteLine("Login failed: Empty username or password");
                return;
            }

            Debug.WriteLine($"Attempting to verify password for user: {username}");
            if (_masterPasswordService.VerifyMasterPassword(username, password))
            {
                _securityLogger.LogLoginAttempt(true, username);
                Debug.WriteLine("Login successful, navigating to PasswordListPage");
                var passwordListPage = _serviceProvider.GetRequiredService<PasswordListPage>();
                passwordListPage.SetUser(username);
                await Navigation.PushAsync(passwordListPage);
            }
            else
            {
                _securityLogger.LogLoginAttempt(false, username);
                await DisplayAlert("Error", "Invalid username or password", "OK");
                Debug.WriteLine("Login failed: Invalid username or password");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception during login: {ex}");
            await DisplayAlert("Error", "An unexpected error occurred. Please try again.", "OK");
        }
    }

    private async void OnCreateAccountClicked(object sender, EventArgs e)
    {
        try
        {
            Debug.WriteLine("Create Account button clicked");
            string username = UsernameEntry.Text;
            string password = MasterPasswordEntry.Text;
        
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Username and password cannot be empty", "OK");
                _securityLogger.LogSecurityWarning("Attempt to create account with empty username or password");
                Debug.WriteLine("Account creation failed: Empty username or password");
                return;
            }

            if (!_validationService.IsValidPassword(password, out string errorMessage))
            {
                await DisplayAlert("Error", errorMessage, "OK");
                _securityLogger.LogSecurityWarning($"Attempt to create account with invalid password: {errorMessage}");
                Debug.WriteLine($"Account creation failed: Invalid password - {errorMessage}");
                return;
            }

            Debug.WriteLine($"Attempting to create account for user: {username}");
            var (success, userId) = _masterPasswordService.CreateUser(username, password);
            if (success)
            {
                _securityLogger.LogSecurityEvent($"New account created for user: {username}");
                await DisplayAlert("Success", "Account created successfully", "OK");
                Debug.WriteLine("Account created successfully");
            }
            else
            {
                _securityLogger.LogSecurityWarning($"Failed to create account for user: {username}");
                await DisplayAlert("Error", "Failed to create account. Username might already exist.", "OK");
                Debug.WriteLine("Account creation failed: Username might already exist");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception during account creation: {ex}");
            await DisplayAlert("Error", "An unexpected error occurred. Please try again.", "OK");
        }
    }
}