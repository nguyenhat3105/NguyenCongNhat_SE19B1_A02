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

        public CustomerViewModel(CustomerService customerService)
        {
            _customerService = customerService;
            Customers = new ObservableCollection<Customer>();
            LoadCustomers();

            SearchCommand = new RelayCommand(_ => Search());
            RefreshCommand = new RelayCommand(_ => LoadCustomers());
            DeleteCommand = new RelayCommand(_ => DeleteSelected(), _ => SelectedCustomer != null);
            SaveCommand = new RelayCommand(SaveChanges);
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
            }
        }

        private Customer _selectedCustomer;
        public Customer SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                _selectedCustomer = value;
                OnPropertyChanged(nameof(SelectedCustomer));
            }
        }

        public ObservableCollection<Customer> Customers { get; set; }

        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }

        // 🟢 Load toàn bộ danh sách khách hàng
        private void LoadCustomers()
        {
            Customers.Clear();
            var list = _customerService.GetAll();
            foreach (var c in list)
                Customers.Add(c);
        }

        // 🟡 Tìm kiếm khách hàng
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

        // 🔴 Xóa khách hàng
        private void DeleteSelected()
        {
            if (SelectedCustomer == null)
            {
                MessageBox.Show("Vui lòng chọn khách hàng để xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                $"Bạn có chắc muốn xóa khách hàng '{SelectedCustomer.CustomerFullName}'?",
                "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirm == MessageBoxResult.Yes)
            {
                _customerService.Delete(SelectedCustomer.CustomerId);
                LoadCustomers();
            }
        }

        // 💾 Lưu thay đổi
        private void SaveChanges(object obj)
        {
            try
            {
                foreach (var customer in Customers)
                {
                    _customerService.Update(customer);
                }
                MessageBox.Show("Đã lưu các thay đổi!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
