# SecurePassManager

SecurePassManager is a robust, multi-user password management application developed using .NET MAUI. It provides a secure way to store and manage passwords across different platforms.

## Table of Contents

1. [Features](#features)
2. [Security Model](#security-model)
3. [Setup Instructions](#setup-instructions)
4. [Running the Application](#running-the-application)
5. [Screenshots](#screenshots)
6. [Security Discussion](#security-discussion)
7. [Limitations and Future Improvements](#limitations-and-future-improvements)

## Features

- Multi-user support with individual encrypted password storage
- Strong password generation
- Password strength meter
- Cross-platform compatibility (Windows, macOS, iOS, Android)
- Encrypted local storage of passwords
- User-friendly interface for managing passwords

## Security Model

SecurePassManager employs a robust security model to protect user data:

1. **User Authentication**: Each user has a unique username and master password. The master password is never stored directly; instead, it's used to derive a key for encrypting and decrypting the user's passwords.

2. **Password Storage**: All passwords are stored in an encrypted format using AES-256 encryption in CBC mode with PKCS7 padding.

3. **Key Derivation**: PBKDF2 with SHA-256 is used for key derivation, with 100,000 iterations and a unique salt for each user.

4. **Local Storage**: Encrypted data is stored locally on the device, reducing the risk of remote attacks.

5. **Encryption Key Handling**: A unique encryption key is generated for each installation of the app and stored securely in the app's protected storage area.

## Setup Instructions

### Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022 or JetBrains Rider (for development)

### Clone the Repository

```bash
git clone https://github.com/Alis-33/SecurePassManager.git
cd SecurePassManager
```

### Setup the Development Environment

1. Ensure you have the .NET MAUI workload installed:

```bash
dotnet workload install maui
```

2. Open the solution in Visual Studio or Rider.

3. Restore NuGet packages:

```bash
dotnet restore
```

4. Build the solution:

```bash
dotnet build
```

## Running the Application

### From Visual Studio:

1. Set the startup project to `SecurePassManager`.
2. Select your target platform (Windows, macOS, iOS, or Android).
3. Click the "Run" button or press F5.

### From Command Line:

To run on Windows:
```bash
dotnet run --project SecurePassManager
```

To run on macOS:
```bash
dotnet run --project SecurePassManager -f net8.0-maccatalyst
```

For iOS and Android, you'll need to use the respective emulators or connect a physical device.

## Screenshots

### Home Page
![Alt text](https://i.imgur.com/vDBp03r.png "Home Page")
### Saved Password
![Alt text](https://i.imgur.com/eSYiwqD.png "Home Page")

## Security Discussion

### Threat Model

SecurePassManager is designed to protect against several threat actors:

1. **Malicious users with physical access to the device**: The app protects against unauthorized access even if someone gains physical access to the device.
2. **Network-based attackers**: As the app uses local storage, it's not vulnerable to remote network attacks.
3. **Malware on the user's device**: While no app can be completely secure against malware with full system access, our encryption model provides a strong barrier.

### Cryptographic Decisions

1. **AES-256 for Encryption**: We chose AES-256 as it's widely recognized as a secure symmetric encryption algorithm, approved by the US National Institute of Standards and Technology (NIST).

2. **CBC Mode**: Cipher Block Chaining (CBC) mode is used to provide dependence between encrypted blocks, making the encryption more robust against certain types of attacks.

3. **PBKDF2 for Key Derivation**: PBKDF2 is used with 100,000 iterations to derive encryption keys from user passwords. This makes brute-force attacks computationally expensive.

4. **SHA-256**: Used as the hash function in PBKDF2, SHA-256 is collision-resistant and provides a good balance between security and performance.

5. **Unique Salt per User**: Each user has a unique salt, preventing rainbow table attacks and ensuring that even if two users have the same password, their encrypted data will be different.

### Security Considerations

1. **Local Storage**: By storing data locally, we reduce the attack surface compared to cloud-based solutions. However, this means users are responsible for their own backups.

2. **Master Password**: The security of the system heavily relies on the strength of the user's master password. We implement a password strength meter to encourage strong passwords.

3. **Memory Protection**: We've taken care to minimize the time sensitive data (like decrypted passwords) stays in memory, but perfect memory protection is challenging in managed code environments.

## Limitations and Future Improvements

1. **Lack of Remote Sync**: Currently, the app doesn't support syncing across devices. This could be added in the future with end-to-end encryption.

2. **No Two-Factor Authentication**: Adding 2FA would provide an extra layer of security.

3. **Limited Password Sharing**: The current version doesn't support secure password sharing between users.

4. **Audit Logging**: Implementing a secure audit log could help users track access to their passwords.

5. **Secure Backup**: While users can manually back up the encrypted database file, a more user-friendly, secure backup solution could be implemented.

By addressing these limitations in future versions, we can further enhance the security and usability of SecurePassManager.