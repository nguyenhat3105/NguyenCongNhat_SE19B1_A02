using FUMiniHotelManagement.Helper;
using FUMiniHotelManagement.BLL.Services;
using FUMiniHotelManagement.DAL.Entities;
using System;
using System.Windows;
using System.Windows.Input;

namespace FUMiniHotelManagement.ViewModel
{
    public class LoginViewModel : ObservableObject
    {
        private readonly ICustomerService _customerService;
        private readonly Func<string> _getPassword;
        private readonly Action<Customer>? _onLoginSuccess;

        public LoginViewModel(ICustomerService customerService, Func<string> getPassword, Action<Customer>? onLoginSuccess = null)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
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
                    CommandManager.InvalidateRequerySuggested(); // cập nhật trạng thái nút login
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

        private bool CanLogin(object? parameter)
        {
            return !IsBusy && !string.IsNullOrWhiteSpace(Email);
        }

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

                // Gọi service để kiểm tra đăng nhập
                var customer = _customerService.Login(email, password);

                if (customer == null)
                {
                    MessageBox.Show("Sai email hoặc mật khẩu!", "Đăng nhập thất bại", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("Đăng nhập thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                // Gọi callback mở cửa sổ chính
                _onLoginSuccess?.Invoke(customer);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi đăng nhập: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
