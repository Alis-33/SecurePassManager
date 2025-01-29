# SecurePassManager

SecurePassManager is a multi-user password management application developed using .NET MAUI. It provides a **secure** and **cross-platform** way to store and manage passwords.

## Table of Contents

*   [Features](#features)
*   [Security Model](#security-model)
*   [Setup Instructions](#setup-instructions)
*   [Running the Application](#running-the-application)
*   [Screenshots](#screenshots)
*   [Security Discussion](#security-discussion)
*   [Limitations and Future Improvements](#limitations-and-future-improvements)

## Features

*   Multi-user support with individually encrypted password storage
*   PBKDF2-based encryption key derivation
*   AES-256 encryption for password storage
*   Random IV per encryption operation for added security
*   Secure master password authentication
*   Strong password generation & strength meter
*   Cross-platform compatibility (Windows, macOS)
*   Encrypted local storage of passwords
*   User-friendly interface for managing passwords

## Security Model

*   **Master Password-Based Encryption:** Each user has a unique master password, which is used to derive a strong encryption key using PBKDF2 with 100,000 iterations.
*   **Encrypted Password Storage:** User passwords are encrypted using AES-256 in CBC mode with PKCS7 padding.
*   **Key Management:** No encryption key is stored on disk—keys are derived at runtime from the master password.
*   **Local Storage Security:** Encrypted files are stored in users.json, passwords.dat, and encryption.salt.

## Setup Instructions

### Prerequisites

*   .NET 8.0 SDK or later
*   Visual Studio 2022 or JetBrains Rider (for development)

### Clone the Repository

```
git clone https://github.com/Alis-33/SecurePassManager.git
cd SecurePassManager
```

### Setup the Development Environment

```
dotnet workload install maui
dotnet restore
dotnet build
```

## Running the Application

### From Visual Studio

1.  Set the startup project to `SecurePassManager`
2.  Select your target platform (Windows or macOS)
3.  Click "Run" or press `F5`

### From Command Line

To run on Windows:

```
dotnet run --project SecurePassManager
```

To run on macOS:

```
dotnet run --project SecurePassManager -f net8.0-maccatalyst
```

## Screenshots

**Home Page**

![Home Page](https://i.imgur.com/vDBp03r.png)

**Saved Password**

![Saved Password](https://i.imgur.com/eSYiwqD.png)

## Security Discussion

### Threat Model

*   Passwords are AES-encrypted and cannot be accessed without the master password.
*   Brute force attacks are mitigated with PBKDF2 (100,000 iterations).
*   Random IV ensures pattern analysis protection.

### Cryptographic Decisions

*   **PBKDF2 for Key Derivation:** Master password-derived encryption key using SHA-256 with 100,000 iterations.
*   **AES-256 Encryption:** Each password entry is encrypted separately with CBC mode.
*   **Random IV Generation:** A new IV is generated per encryption operation.
*   **User Credential Protection:** User hashed passwords are encrypted using AES.

## Limitations and Future Improvements

*   No Cloud Syncing - Currently, passwords cannot be synced across devices.
*   No Two-Factor Authentication (2FA) - Adding 2FA would further improve authentication security.
*   No Biometric Authentication - Face ID/Touch ID support is not yet implemented.
*   Secure Backup Needed - Users must manually backup encrypted password files.

## Final Notes

*    No stored keys
*    PBKDF2 key derivation
*    AES-256 with CBC mode & random IV
*    100,000 PBKDF2 iterations to prevent brute-force attacks

This updated security model ensures that even if an attacker gains access to the stored files, they cannot decrypt the passwords without the correct master password.
