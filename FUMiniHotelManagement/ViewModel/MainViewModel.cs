using FUMiniHotelManagement.BLL.Services;
using FUMiniHotelManagement.DAL.Repositories;
using FUMiniHotelManagement.Helper;
using System.ComponentModel;
using System.Windows;
using System;
using System.Windows.Input;

namespace FUMiniHotelManagement.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly CustomerService _customerService;
        private readonly RoomService _roomService;
        private readonly BookingReservationService _bookingService;

        public bool IsAdmin => SessionManager.IsAdmin;
        public bool IsCustomer => SessionManager.IsCustomer;

        // Các ViewModel con
        public CustomerViewModel CustomerVM { get; }
        public CustomerProfileViewModel CustomerProfileVM { get; }
        public RoomViewModel RoomVM { get; }
        public BookingReservationViewModel BookingVM { get; }

        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        public ICommand ShowCustomerViewCommand { get; }
        public ICommand ShowRoomViewCommand { get; }
        public ICommand ShowBookingViewCommand { get; }

        public ICommand LogoutCommand { get; }

        private readonly AuthenticationService _authService;
        public Action? RequestClose { get; set; }

        public MainViewModel(CustomerService customerService, RoomService roomService, BookingReservationService bookingService, AuthenticationService authenticationService)
        {
            _customerService = customerService;
            _roomService = roomService;
            _bookingService = bookingService;

            _authService = authenticationService;

            // 1. ĐĂNG KÝ SỰ KIỆN SESSION MANAGER
            SessionManager.SessionChanged += OnSessionChanged;

            

            CustomerVM = new CustomerViewModel(_customerService);
            CustomerProfileVM = new CustomerProfileViewModel(_customerService);
            RoomVM = new RoomViewModel(_roomService);
            BookingVM = new BookingReservationViewModel(_bookingService, _customerService, _roomService);

      

            if (IsAdmin)
            {
                // Admin thường vào View quản lý đặt phòng
                CurrentView = BookingVM;
            }
            else if (IsCustomer)
            {
                // Customer thường vào View đặt phòng hoặc Profile của họ
                // Giả sử BookingVM là View ưu tiên (để đặt phòng)
                CurrentView = BookingVM;
            }

            ShowCustomerViewCommand = new RelayCommand(_ =>
            {
                if (IsAdmin)
                    CurrentView = CustomerVM; // Admin thấy Management
                else if (IsCustomer)
                    CurrentView = CustomerProfileVM; // Customer thấy Profile
            });

            ShowRoomViewCommand = new RelayCommand(_ => CurrentView = RoomVM);
            ShowBookingViewCommand = new RelayCommand(_ => CurrentView = BookingVM);

            LogoutCommand = new RelayCommand(_ => Logout());
        }


        private void Logout()
        {
            var result = MessageBox.Show("Bạn có chắc muốn đăng xuất không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            _authService.Logout();

            

            // Mở lại LoginWindow
            Application.Current.Dispatcher.Invoke(() =>
            {
                var loginWindow = new FUMiniHotelManagement.LoginWindow();
                loginWindow.Show();



            });

            RequestClose?.Invoke();
        }

        private void OnSessionChanged()
        {
            // Cập nhật các thuộc tính phân quyền
            OnPropertyChanged(nameof(IsAdmin));
            OnPropertyChanged(nameof(IsCustomer));

            // ... (Có thể cần cập nhật thêm logic UI tại đây nếu cần)
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
