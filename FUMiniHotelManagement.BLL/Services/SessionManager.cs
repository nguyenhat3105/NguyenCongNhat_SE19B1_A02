using System;
using FUMiniHotelManagement.DAL.Entities;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace FUMiniHotelManagement.Helper
{
    public static class SessionManager
    {
        private static readonly IConfigurationRoot _config;

        static SessionManager()
        {
            // Tự động đọc file appsettings.json khi chương trình khởi động
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _config = builder.Build();
        }

        // Đọc email và password admin từ file
        public static string AdminEmail => _config["AdminSettings:Email"];
        public static string AdminPassword => _config["AdminSettings:Password"];

        // Thuộc tính kiểm tra vai trò
        public static bool IsAdmin => CurrentUser != null && CurrentUser.EmailAddress == AdminEmail;
        public static bool IsCustomer => CurrentUser != null && CurrentUser.EmailAddress != AdminEmail;

        // user hiện tại, null nếu chưa login
        public static Customer? CurrentUser { get; private set; }

        // Event khi user thay đổi (login hoặc logout)
        public static event Action? SessionChanged;

        public static void SetCurrentUser(Customer? user)
        {
            CurrentUser = user;
            SessionChanged?.Invoke();
        }

        public static void Clear()
        {
            if (CurrentUser != null)
            {
                // tránh giữ password trong memory
                CurrentUser.Password = null;
            }
            CurrentUser = null;
            SessionChanged?.Invoke();
        }

        public static bool IsLoggedIn => CurrentUser != null;
    }
}
