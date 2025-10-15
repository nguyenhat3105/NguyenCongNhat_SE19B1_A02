using FUMiniHotelManagement.BLL.Services;
using FUMiniHotelManagement.Helper;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace FUMiniHotelManagement.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly CustomerService _customerService;
        private readonly RoomService _roomService;

        // Các ViewModel con
        public CustomerViewModel CustomerVM { get; }
        public RoomViewModel RoomVM { get; }

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

        public MainViewModel(CustomerService customerService, RoomService roomService)
        {
            _customerService = customerService;
            _roomService = roomService;

            CustomerVM = new CustomerViewModel(_customerService);
            RoomVM = new RoomViewModel(_roomService);



            ShowCustomerViewCommand = new RelayCommand(_ => CurrentView = CustomerVM);
            ShowRoomViewCommand = new RelayCommand(_ => CurrentView = RoomVM);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
