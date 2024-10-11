using System;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using SecurePassManager.Models;
using SecurePassManager.Services;

namespace SecurePassManager;

public partial class PasswordEntryPage : ContentPage
{
    private readonly PasswordStorageService _passwordStorageService;
    private readonly PasswordGeneratorService _passwordGeneratorService;
    private readonly ValidationService _validationService;
    private readonly SecurityLogger _securityLogger;
    private readonly PasswordStrengthService _passwordStrengthService;
    private PasswordEntry _currentPassword;
    private string _currentUserId;

    public PasswordEntryPage(PasswordStorageService passwordStorageService, 
        PasswordGeneratorService passwordGeneratorService, 
        ValidationService validationService, 
        SecurityLogger securityLogger,
        PasswordStrengthService passwordStrengthService)
    {
        InitializeComponent();
        _passwordStorageService = passwordStorageService;
        _passwordGeneratorService = passwordGeneratorService;
        _validationService = validationService;
        _securityLogger = securityLogger;
        _passwordStrengthService = passwordStrengthService;
    }

    public void SetUser(string userId)
    {
        _currentUserId = userId;
    }

    public void SetPasswordEntry(PasswordEntry passwordEntry)
    {
        _currentPassword = passwordEntry;
        if (_currentPassword != null)
        {
            TitleEntry.Text = _currentPassword.Title;
            UsernameEntry.Text = _currentPassword.Username;
            PasswordEntry.Text = _currentPassword.Password;
            WebsiteEntry.Text = _currentPassword.Website;
            DeleteButton.IsVisible = true;
            UpdatePasswordStrength(_currentPassword.Password);
        }
    }

    private void OnPasswordTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdatePasswordStrength(e.NewTextValue);
    }

    private void UpdatePasswordStrength(string password)
    {
        var strength = _passwordStrengthService.CalculatePasswordStrength(password);
        PasswordStrengthMeter.Progress = ((int)strength + 1) / 5.0;
        PasswordStrengthLabel.Text = $"Password Strength: {strength}";

        switch (strength)
        {
            case PasswordStrengthService.StrengthLevel.VeryWeak:
            case PasswordStrengthService.StrengthLevel.Weak:
                PasswordStrengthMeter.ProgressColor = Colors.Red;
                break;
            case PasswordStrengthService.StrengthLevel.Medium:
                PasswordStrengthMeter.ProgressColor = Colors.Orange;
                break;
            case PasswordStrengthService.StrengthLevel.Strong:
                PasswordStrengthMeter.ProgressColor = Colors.YellowGreen;
                break;
            case PasswordStrengthService.StrengthLevel.VeryStrong:
                PasswordStrengthMeter.ProgressColor = Colors.Green;
                break;
        }
    }

    private void OnGeneratePasswordClicked(object sender, EventArgs e)
    {
        var generatedPassword = _passwordGeneratorService.GeneratePassword();
        PasswordEntry.Text = generatedPassword;
        UpdatePasswordStrength(generatedPassword);
        _securityLogger.LogSecurityEvent("Password generated");
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleEntry.Text))
        {
            await DisplayAlert("Error", "Title is required", "OK");
            _securityLogger.LogSecurityWarning("Attempt to save password with empty title");
            return;
        }

        if (!_validationService.IsValidPassword(PasswordEntry.Text, out string errorMessage))
        {
            await DisplayAlert("Error", errorMessage, "OK");
            _securityLogger.LogSecurityWarning($"Attempt to save invalid password: {errorMessage}");
            return;
        }

        if (!string.IsNullOrWhiteSpace(WebsiteEntry.Text) && !_validationService.IsValidUrl(WebsiteEntry.Text))
        {
            await DisplayAlert("Error", "Invalid URL format", "OK");
            _securityLogger.LogSecurityWarning("Attempt to save password with invalid URL");
            return;
        }

        if (_currentPassword == null)
        {
            _currentPassword = new PasswordEntry
            {
                Title = TitleEntry.Text,
                Username = UsernameEntry.Text,
                Password = PasswordEntry.Text,
                Website = WebsiteEntry.Text
            };
            _passwordStorageService.AddPassword(_currentUserId, _currentPassword);
            _securityLogger.LogPasswordCreated(UsernameEntry.Text);
        }
        else
        {
            _currentPassword.Title = TitleEntry.Text;
            _currentPassword.Username = UsernameEntry.Text;
            _currentPassword.Password = PasswordEntry.Text;
            _currentPassword.Website = WebsiteEntry.Text;
            _passwordStorageService.UpdatePassword(_currentUserId, _currentPassword);
            _securityLogger.LogPasswordUpdated(UsernameEntry.Text);
        }

        await Navigation.PopAsync();
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (_currentPassword != null)
        {
            bool answer = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this password?", "Yes", "No");
            if (answer)
            {
                _passwordStorageService.DeletePassword(_currentUserId, _currentPassword.Id);
                _securityLogger.LogPasswordDeleted(_currentPassword.Username);
                await Navigation.PopAsync();
            }
        }
    }

    private void OnTogglePasswordVisibility(object sender, EventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
        ((Button)sender).Text = PasswordEntry.IsPassword ? "👁" : "🙈";
    }

    private async void OnCopyPassword(object sender, EventArgs e)
    {
        await Clipboard.SetTextAsync(PasswordEntry.Text);
        await DisplayAlert("Success", "Password copied to clipboard", "OK");
        _securityLogger.LogSecurityEvent("Password copied to clipboard");
    }
}