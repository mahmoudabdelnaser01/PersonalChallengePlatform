# PersonalChallengePlatform

A web application for tracking personal challenges, progress, and achievements. Built with ASP.NET Core (.NET 8), Entity Framework Core, and SQL Server.

## 🚀 Features

- User registration and authentication
- Create, edit, and delete personal challenges
- Track daily progress for each challenge
- Leaderboard and user statistics
- Profile picture upload and user settings
- Category management for challenges
- Responsive and modern UI (Bootstrap, Font Awesome)
- Multi-language support (English/Arabic)
- Secure password management and user claims


## 🛠️ Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- Visual Studio 2022+ 

### Setup Instructions

1. **Clone the repository:**
   ```bash
   git clone <your-repo-url>
   cd PersonalChallengePlatform
   ```
2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```
3. **Configure the database:**
   - Edit `appsettings.json` and set your SQL Server connection string.
4. **Apply migrations and update the database:**
   ```bash
   dotnet ef database update
   ```
5. **Run the application:**
   ```bash
   dotnet run --project PersonalChallengePlatform/PersonalChallengePlatform.csproj
   ```
   The app will be available at [http://localhost:5000](http://localhost:5000) (or the port specified in `Program.cs`).

## 👤 User Management

- Register a new user via the Register page.
- The first name and last name are required and will be shown in the top bar.
- Profile pictures can be uploaded from the Settings page. A default avatar is used if none is uploaded.
- User claims (such as FirstName) are automatically added on registration and login.

## 🗄️ Database & Migrations

- Uses Entity Framework Core with code-first migrations.
- To add a new migration:
  ```bash
  dotnet ef migrations add <MigrationName>
  dotnet ef database update
  ```

## 🖼️ Profile Pictures

- Uploaded profile pictures are stored in `wwwroot/uploads/profiles`.
- Default avatar is served from `wwwroot/images/default-avatar.jpg` (or `.svg`).

## 🏆 Leaderboard

- The leaderboard ranks users by points and completed challenges.
- User avatars and names are displayed for a personalized experience.

## 🌐 Multi-language Support

- The platform supports both English and Arabic for a wider audience.

## 🐞 Troubleshooting

- If you see 404 errors for avatar images, ensure the default avatar file exists in `wwwroot/images`.
- If the user's first name does not appear, log out and log in again to refresh claims.
- For database issues, ensure your connection string is correct and SQL Server is running.

## 🤝 Contributing

Contributions are welcome! Please open an issue or submit a pull request.

## 📄 License

This project is licensed under the MIT License.

---

**Made with ❤️ by Mahmoud Abdel Nasser** 