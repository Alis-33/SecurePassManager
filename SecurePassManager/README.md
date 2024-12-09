# SecurePassManager

SecurePassManager is a multi-user password management application developed using .NET MAUI. It provides a secure way to store and manage passwords across different platforms.

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
- Cross-platform compatibility (Windows, macOS)
- Encrypted local storage of passwords
- User-friendly interface for managing passwords

## Security Model

SecurePassManager employs the following security model:

1. **User Authentication**: Each user has a unique username and master password. The master password is used for authentication into the application.

2. **Password Storage**: All passwords are stored in an encrypted format using AES-256 encryption in CBC mode with PKCS7 padding.

3. **Key Handling**: The application uses a stored encryption key and salt for data protection. These are generated during installation and stored in separate files.

4. **Local Storage**: All data is stored locally on the device in three main files:
    - users.json: Contains encrypted user data
    - passwords.json: stores the encrypted password entries
    - encryption.key: Contains the encryption key
    - encryption.salt: Contains the salt value

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

1. Set the startup project to `SecurePassManager`
2. Select your target platform (Windows or macOS)
3. Click the "Run" button or press F5

### From Command Line:

To run on Windows:
```bash
dotnet run --project SecurePassManager
```

To run on macOS:
```bash
dotnet run --project SecurePassManager -f net8.0-maccatalyst
```

## Screenshots

### Home Page
![Alt text](https://i.imgur.com/vDBp03r.png "Home Page")
### Saved Password
![Alt text](https://i.imgur.com/eSYiwqD.png "Home Page")

## Security Discussion

### Threat Model

The current implementation provides protection against:

1. **Basic Unauthorized Access**: Password data is encrypted rather than stored in plaintext.
2. **Network-based attackers**: As the app uses local storage, it's not vulnerable to remote network attacks.
3. **Casual system access**: The application requires authentication to access stored passwords.

### Cryptographic Decisions

1. **AES-256 for Encryption**: The application employs AES-256 encryption as implemented through .NET cryptographic libraries. This choice was made due to AES-256's standing as a NIST-approved standard and its proven track record in providing strong security while maintaining reasonable performance across different platforms. The implementation uses the standard 128-bit block size alongside the 256-bit key length.

2. **CBC Mode**: Cipher Block Chaining mode was selected as the encryption mode because it ensures each encryption block depends on the previous one. This dependency makes the encryption more robust by preventing pattern recognition in the encrypted data, as identical plaintext blocks will encrypt to different ciphertext blocks. The implementation includes PKCS7 padding to handle the last block appropriately.

3. **PBKDF2 for Key Operations**: The application uses PBKDF2 in two distinct ways. In the encryption service, it runs with 100,000 iterations to derive encryption keys from the stored master key. The master password service uses a higher iteration count of 210,000 for hashing user passwords. This dual implementation reflects the different security needs of key derivation versus password hashing, though both make brute-force attempts computationally expensive.

4. **Salt Usage and Storage**: The current implementation maintains two separate salting mechanisms. For password encryption, a single salt is stored in encryption.salt and used across all encryption operations. For user authentication, each user has their own unique salt stored alongside their password hash. While this provides some protection against rainbow table attacks, storing the encryption salt separately from the encrypted data introduces security risks.

5. **Key Management**: The application generates and stores its main encryption key in a separate file during first launch. This key, combined with the stored salt, serves as the basis for all password encryption operations through PBKDF2 key derivation. While this approach simplifies the implementation and provides consistent encryption across sessions, storing the key separately makes the encrypted data vulnerable if an attacker gains access to all application files.

These cryptographic choices reflect standard security practices in terms of algorithm selection, but the current implementation of key and salt storage could be improved to better protect against physical access to the application's files.
### Security Considerations

1. **Local Storage**: By storing data locally, the attack surface is reduced compared to cloud-based solutions. However, this means users are responsible for their own backups.

2. **Master Password**: While a password strength meter is implemented to encourage strong passwords, the master password is currently used only for authentication and not for data encryption.

3. **Memory Protection**: The application uses standard .NET memory management. Like other managed code environments, implementing perfect memory protection for sensitive data remains challenging.
### Implementation Limitations

1. **Key Storage**: The current implementation stores encryption keys and salt in separate files, which makes the encrypted data vulnerable if an attacker gains access to all local files.
   
   1.1. Instead of storing encryption keys in files, we could derive them from the user's master password. Each user's data would be encrypted with a unique key derived from their master password
2. **Authentication vs Encryption**: While the master password is used for authentication, it is not used for encrypting the stored passwords.

3. **Physical Access**: The current implementation doesn't protect against physical access to the device files.

## Limitations and Future Improvements

1. **Lack of Remote Sync**: Currently, the app doesn't support syncing across devices. This could be added in the future with end-to-end encryption.

2. **No Two-Factor Authentication**: Adding 2FA would provide an extra layer of security.

3. **Limited Password Sharing**: The current version doesn't support secure password sharing between users.

4. **Audit Logging**: Implementing a secure audit log could help users track access to their passwords.

5. **Secure Backup**: While users can manually back up the encrypted database file, a more user-friendly, secure backup solution could be implemented.

6. **Key Handling Improvements**: Future versions should implement master password-based key derivation instead of storing encryption keys.

This application represents a basic password management solution with encryption. While it provides protection against casual access, significant security improvements would be needed for production use, particularly in the handling of encryption keys.
