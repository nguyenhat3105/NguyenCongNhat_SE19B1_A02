using FUMiniHotelManagement.BLL.Services;
using FUMiniHotelManagement.DAL.Entities;
using FUMiniHotelManagement.Helper;
using System;
using System.Windows;
using System.Windows.Input;

namespace FUMiniHotelManagement.ViewModel
{
    public class LoginViewModel : ObservableObject
    {
        private readonly CustomerService _customerService;
        private readonly AuthenticationService _authService;
        private readonly Func<string> _getPassword;
        private readonly Action<Customer>? _onLoginSuccess;
        

        public LoginViewModel(CustomerService customerService ,AuthenticationService authService, Func<string> getPassword, Action<Customer>? onLoginSuccess = null)
        {
            _customerService = customerService;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _getPassword = getPassword ?? throw new ArgumentNullException(nameof(getPassword));
            _onLoginSuccess = onLoginSuccess;

            LoginCommand = new RelayCommand(ExecuteLogin, CanLogin);
        }

        // ---------------------- PROPERTIES ----------------------
        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value))
                    CommandManager.InvalidateRequerySuggested();
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (SetProperty(ref _isBusy, value))
                    CommandManager.InvalidateRequerySuggested();
            }
        }

        // ---------------------- COMMANDS ----------------------
        public ICommand LoginCommand { get; }

        private bool CanLogin(object? parameter) =>
            !IsBusy && !string.IsNullOrWhiteSpace(Email);

        private void ExecuteLogin(object? parameter)
        {
            try
            {
                IsBusy = true;

                var email = Email?.Trim();
                var password = _getPassword();

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ email và mật khẩu.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Dùng AuthenticationService để đăng nhập
                var customer = _authService.Login(email, password);

                if (customer == null)
                {
                    MessageBox.Show("Sai email hoặc mật khẩu!", "Đăng nhập thất bại", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // ✅ Lưu user vào session
                SessionManager.SetCurrentUser(customer);

                MessageBox.Show($"Xin chào {customer.CustomerFullName}!", "Đăng nhập thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                // Callback mở MainWindow
                _onLoginSuccess?.Invoke(customer);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đăng nhập: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
