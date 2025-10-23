using FUMiniHotelManagement.BLL.Services;
using FUMiniHotelManagement.DAL.Entities;
using FUMiniHotelManagement.Helper;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows;
using System;

namespace FUMiniHotelManagement.ViewModel
{
    // Kế thừa từ INotifyPropertyChanged
    public class CustomerProfileViewModel : INotifyPropertyChanged
    {
        private readonly CustomerService _customerService;

        private Customer _editingCustomer;
        public Customer EditingCustomer
        {
            get => _editingCustomer;
            set { _editingCustomer = value; OnPropertyChanged(nameof(EditingCustomer)); }
        }

        public ICommand SaveCommand { get; }

        public CustomerProfileViewModel(CustomerService customerService)
        {
            _customerService = customerService;

            LoadProfile();

            // Chỉ cho phép Save khi có thông tin để chỉnh sửa
            SaveCommand = new RelayCommand(_ => SaveProfile(), _ => EditingCustomer != null);
        }

        private void LoadProfile()
        {
            if (!SessionManager.IsLoggedIn || SessionManager.CurrentUser == null)
            {
                // Nên có logic xử lý lỗi nếu không có user (không nên xảy ra)
                return;
            }

            try
            {
                // Lấy bản sao của Customer hiện tại để chỉnh sửa
                var current = SessionManager.CurrentUser;

                // Sử dụng deep copy để tránh thay đổi SessionManager.CurrentUser trực tiếp
                EditingCustomer = new Customer
                {
                    CustomerId = current.CustomerId,
                    CustomerFullName = current.CustomerFullName,
                    EmailAddress = current.EmailAddress,
                    Telephone = current.Telephone,
                    CustomerBirthday = current.CustomerBirthday,
                    CustomerStatus = current.CustomerStatus,
                    Password = current.Password // Lấy password (có thể là hash) để cập nhật
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải hồ sơ: {ex.Message}");
            }
        }

        private void SaveProfile()
        {
            if (EditingCustomer == null) return;

            try
            {
                // ⚠️ Quan trọng: Không cho Customer tự thay đổi CustomerStatus, chỉ cập nhật các trường được phép
                // Có thể phải lấy lại bản gốc từ DB, copy các trường được phép thay đổi, rồi mới Update.

                // Giả định bạn chỉ cho phép họ update FullName, Telephone, Birthday.

                // Lấy bản gốc từ Service
                var original = _customerService.GetById(EditingCustomer.CustomerId);
                if (original == null) throw new Exception("Không tìm thấy hồ sơ gốc.");

                // Chỉ copy các trường được phép chỉnh sửa
                original.CustomerFullName = EditingCustomer.CustomerFullName;
                original.Telephone = EditingCustomer.Telephone;
                original.CustomerBirthday = EditingCustomer.CustomerBirthday;

                // Cập nhật Service
                _customerService.Update(original);

                // Cập nhật lại SessionManager (quan trọng!)
                SessionManager.SetCurrentUser(original);

                MessageBox.Show("Đã cập nhật hồ sơ thành công!", "Thành công");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu hồ sơ: {ex.Message}");
            }
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}