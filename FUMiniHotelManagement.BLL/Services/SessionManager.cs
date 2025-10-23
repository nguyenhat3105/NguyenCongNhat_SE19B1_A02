using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUMiniHotelManagement.DAL.Entities;

namespace FUMiniHotelManagement.Helper
{
    public static class SessionManager
    {

        private const string AdminEmail = "admin@FUMiniHotelSystem.com"; // <-- Định nghĩa Admin Email

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
