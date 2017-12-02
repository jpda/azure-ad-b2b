# azure-ad-b2b
Simple app for inviting users via Azure AD B2B, who can then invite their own tenant's users.

# Dependencies
- Needs Azure Table Storage - Windows users can use the Storage Emulator locally
- Expects an 'normal' Azure AD tenant - e.g., not B2C
- Requires the 'Allow inviting guest users' scope from the Microsoft Graph, _and_ admin consent

## Walkthrough laters