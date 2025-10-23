using FUMiniHotelManagement.BLL.Services;
using FUMiniHotelManagement.DAL.Entities;
using FUMiniHotelManagement.Helper;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace FUMiniHotelManagement.ViewModel
{
    public class CustomerViewModel : INotifyPropertyChanged
    {
        private readonly CustomerService _customerService;

        public bool IsAdmin => SessionManager.IsAdmin; // Dùng để ẩn/hiện nút trong View
        public bool IsCustomer => SessionManager.IsCustomer;
        public CustomerViewModel(CustomerService customerService)
        {
            _customerService = customerService;
            Customers = new ObservableCollection<Customer>();
            LoadCustomers();

            SearchCommand = new RelayCommand(_ => Search());
            RefreshCommand = new RelayCommand(_ => LoadCustomers());
            DeleteCommand = new RelayCommand(_ => DeleteSelected(), _ => SelectedCustomer != null);
            SaveCommand = new RelayCommand(_ => SaveCustomer(), _ => EditingCustomer != null);
            AddCommand = new RelayCommand(_ => StartAddCustomer());
        }

        public ObservableCollection<Customer> Customers { get; set; }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(nameof(SearchText)); }
        }

        private Customer _selectedCustomer;
        public Customer SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                _selectedCustomer = value;
                if (value != null)
                {
                    EditingCustomer = new Customer
                    {
                        CustomerId = value.CustomerId,
                        CustomerFullName = value.CustomerFullName,
                        EmailAddress = value.EmailAddress,
                        Telephone = value.Telephone,
                        CustomerBirthday = value.CustomerBirthday,
                        CustomerStatus = value.CustomerStatus,
                        Password = value.Password
                    };
                    IsAddingNew = false;
                }
                OnPropertyChanged(nameof(SelectedCustomer));
            }
        }

        private Customer _editingCustomer;
        public Customer EditingCustomer
        {
            get => _editingCustomer;
            set { _editingCustomer = value; OnPropertyChanged(nameof(EditingCustomer)); }
        }

        private bool _isAddingNew;
        public bool IsAddingNew
        {
            get => _isAddingNew;
            set { _isAddingNew = value; OnPropertyChanged(nameof(IsAddingNew)); }
        }

        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand AddCommand { get; }

        //private void LoadCustomers()
        //{
        //    Customers.Clear();
        //    var list = _customerService.GetAll();
        //    foreach (var c in list)
        //        Customers.Add(c);
        //}


        private void LoadCustomers()
        {
            Customers.Clear();

            if (SessionManager.IsAdmin)
            {
                // ADMIN: Tải tất cả khách hàng
                var list = _customerService.GetAll();
                foreach (var c in list)
                    Customers.Add(c);
            }
            else if (SessionManager.IsCustomer && SessionManager.CurrentUser != null)
            {
                // CUSTOMER: Tải duy nhất profile của họ
                var self = _customerService.GetById(SessionManager.CurrentUser.CustomerId);
                if (self != null)
                {
                    Customers.Add(self);
                    // Cần chọn customer đó và bắt đầu chế độ chỉnh sửa/xem
                    SelectedCustomer = self;
                }
            }
        }

        private void Search()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadCustomers();
                return;
            }

            var filtered = _customerService.GetAll().Where(c =>
                (!string.IsNullOrEmpty(c.CustomerFullName) && c.CustomerFullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(c.EmailAddress) && c.EmailAddress.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(c.Telephone) && c.Telephone.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
            );

            Customers.Clear();
            foreach (var c in filtered)
                Customers.Add(c);
        }

        private void DeleteSelected()
        {
            if (SelectedCustomer == null)
            {
                MessageBox.Show("Vui lòng chọn khách hàng để xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Xóa khách hàng '{SelectedCustomer.CustomerFullName}'?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _customerService.Delete(SelectedCustomer.CustomerId);
                LoadCustomers();
                EditingCustomer = null;
            }
        }

        private void StartAddCustomer()
        {
            IsAddingNew = true;
            EditingCustomer = new Customer
            {
                CustomerBirthday = DateOnly.FromDateTime(DateTime.Now),
                CustomerStatus = 1
            };
        }

        private void SaveCustomer()
        {
            try
            {
                if (EditingCustomer == null) return;

                if (IsAddingNew)
                {
                    _customerService.Add(EditingCustomer);
                    Customers.Add(EditingCustomer);
                    MessageBox.Show("Đã thêm khách hàng mới!", "Thành công");
                }
                else
                {
                    _customerService.Update(EditingCustomer);
                    MessageBox.Show("Đã cập nhật thông tin khách hàng!", "Thành công");
                }

                LoadCustomers();
                IsAddingNew = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu khách hàng: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
