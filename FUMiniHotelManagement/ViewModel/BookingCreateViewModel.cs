using FUMiniHotelManagement.BLL.Services;
using FUMiniHotelManagement.DAL.Entities;
using FUMiniHotelManagement.Helper;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace FUMiniHotelManagement.ViewModel
{
    public class BookingCreateViewModel : INotifyPropertyChanged
    {
        private readonly BookingReservationService _bookingService;
        private readonly CustomerService _customerService;
        private readonly RoomService _roomService;

        public ObservableCollection<Customer> Customers { get; } = new ObservableCollection<Customer>();
        public ObservableCollection<RoomInformation> Rooms { get; } = new ObservableCollection<RoomInformation>();
        public ObservableCollection<RoomInformation> SelectedRooms { get; } = new ObservableCollection<RoomInformation>();

        private Customer? _selectedCustomer;

        private BookingReservation? _selectedReservation;
        //public Customer? SelectedCustomer
        //{
        //    get => _selectedCustomer;
        //    set { _selectedCustomer = value; OnPropertyChanged(nameof(SelectedCustomer)); UpdateCanSave(); }
        //}


        // Thay đổi SelectedCustomer để chỉ đọc (read-only)
        public new Customer? SelectedCustomer
        {
            get => _selectedCustomer;
            set { _selectedCustomer = value; OnPropertyChanged(nameof(SelectedCustomer)); UpdateCanSave(); }
        }

        private RoomInformation? _selectedRoomSingular;
        public RoomInformation? SelectedRoomSingular
        {
            get => _selectedRoomSingular;
            set
            {
                _selectedRoomSingular = value;
                OnPropertyChanged(nameof(SelectedRoomSingular));
                SelectedRooms.Clear();
                if (value != null) SelectedRooms.Add(value);
            }
        }

        private DateTime _checkInDate = DateTime.Today;
        public DateTime CheckInDate
        {
            get => _checkInDate;
            set
            {
                _checkInDate = value;
                OnPropertyChanged(nameof(CheckInDate));
                LoadAvailableRooms();
                RecalculateTotal();
                UpdateCanSave();
            }
        }

        private DateTime _checkOutDate = DateTime.Today.AddDays(1);
        public DateTime CheckOutDate
        {
            get => _checkOutDate;
            set
            {
                _checkOutDate = value;
                OnPropertyChanged(nameof(CheckOutDate));
                LoadAvailableRooms();
                RecalculateTotal();
                UpdateCanSave();
            }
        }

        private decimal _totalPrice;
        public decimal TotalPrice
        {
            get => _totalPrice;
            set { _totalPrice = value; OnPropertyChanged(nameof(TotalPrice)); }
        }

        private string _validationMessage = string.Empty;
        public string ValidationMessage
        {
            get => _validationMessage;
            set { _validationMessage = value; OnPropertyChanged(nameof(ValidationMessage)); }
        }

        public ICommand SaveCommand { get; }

        public event Action<bool?>? RequestClose;
        public event PropertyChangedEventHandler? PropertyChanged;

        public BookingCreateViewModel(
            BookingReservationService bookingService,
            CustomerService customerService,
            RoomService roomService)
        {
            _bookingService = bookingService;
            _customerService = customerService;
            _roomService = roomService;

            // Load dữ liệu khách hàng
            //foreach (var c in _customerService.GetAll())
            //    Customers.Add(c);

            if (SessionManager.IsCustomer && SessionManager.CurrentUser != null)
            {
                // Lấy thông tin chi tiết (nếu cần thiết, nếu không dùng luôn SessionManager.CurrentUser)
                // Tuy nhiên, vì SessionManager.CurrentUser đã được thiết lập, ta dùng trực tiếp.
                SelectedCustomer = SessionManager.CurrentUser;
            }
            else if (SessionManager.IsAdmin)
            {
                // Nếu là Admin, vẫn có thể muốn giữ lại tính năng chọn khách hàng.
                // Nếu bạn muốn hạn chế Admin, giữ nguyên logic cũ.
                // NẾU MUỐN ADMIN CHỌN: Cần tái cấu trúc code để tải khách hàng chỉ khi là Admin.
                foreach (var c in _customerService.GetAll())
                    Customers.Add(c);
            }

            // Ban đầu hiển thị phòng trống hôm nay
            LoadAvailableRooms();

            SelectedRooms.CollectionChanged += (s, e) => { RecalculateTotal(); UpdateCanSave(); };
            SaveCommand = new RelayCommand(_ => Save(), _ => CanSave);
            RecalculateTotal();
        }

        private bool _canSave;
        public bool CanSave
        {
            get => _canSave;
            private set { _canSave = value; OnPropertyChanged(nameof(CanSave)); CommandManager.InvalidateRequerySuggested(); }
        }

        private void UpdateCanSave()
        {
            CanSave = SelectedCustomer != null && SelectedRooms.Any() && CheckOutDate > CheckInDate;
        }

        private void RecalculateTotal()
        {
            if (!SelectedRooms.Any())
            {
                TotalPrice = 0;
                return;
            }

            var days = (CheckOutDate - CheckInDate).Days;
            if (days < 1) days = 1;
            TotalPrice = SelectedRooms.Sum(r => r.RoomPricePerDay * days);
        }

        // 🔹 Dùng hàm có sẵn trong service để lấy phòng hợp lệ
        private void LoadAvailableRooms()
        {
            Rooms.Clear();

            var start = DateOnly.FromDateTime(CheckInDate);
            var end = DateOnly.FromDateTime(CheckOutDate);

            var availableRooms = _bookingService.GetAvailableRooms(start, end);
            foreach (var room in availableRooms)
                Rooms.Add(room);
        }

        private void Save()
        {
            ValidationMessage = string.Empty;

            if (!CanSave)
            {
                ValidationMessage = "Vui lòng chọn khách hàng, phòng và ngày hợp lệ.";
                return;
            }

            try
            {
                var bookingDate = DateOnly.FromDateTime(DateTime.Now);
                var rooms = SelectedRooms
                    .Select(r => (r.RoomId, DateOnly.FromDateTime(CheckInDate), DateOnly.FromDateTime(CheckOutDate)))
                    .ToList();

                // Vì phương thức service là đồng bộ, chúng ta gọi nó trực tiếp.
                _bookingService.CreateReservation(SelectedCustomer!.CustomerId, bookingDate, rooms);

                RequestClose?.Invoke(true);
            }
            catch (Exception ex)
            {
                // Cân nhắc ghi lại exception đầy đủ (ex)
                ValidationMessage = "Lỗi khi tạo đặt phòng: " + ex.Message;
            }
        }

        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
