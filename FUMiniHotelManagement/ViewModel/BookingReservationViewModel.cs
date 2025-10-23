using FUMiniHotelManagement.BLL.Services;
using FUMiniHotelManagement.DAL.Entities;
using FUMiniHotelManagement.Helper;
using FUMiniHotelManagement.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows; // for CommandManager fallback (optional)
using System.Windows.Input;

namespace FUMiniHotelManagement.ViewModel
{
    public class BookingReservationViewModel : INotifyPropertyChanged
    {
        private readonly BookingReservationService _bookingService;
        private readonly CustomerService _customerService;
        private readonly RoomService _roomService;



        public ObservableCollection<BookingReservation> Reservations { get; } = new ObservableCollection<BookingReservation>();

        private BookingReservation? _selectedReservation;
        public BookingReservation? SelectedReservation
        {
            get => _selectedReservation;
            set
            {
                if (_selectedReservation == value) return;
                _selectedReservation = value;
                OnPropertyChanged(nameof(SelectedReservation));

                // IMPORTANT: Notify command can-execute changed so UI updates IsEnabled
                RaiseCancelCommandCanExecuteChanged();
            }
        }

        private DateTime? _filterStart;
        public DateTime? FilterStart
        {
            get => _filterStart;
            set
            {
                _filterStart = value;
                OnPropertyChanged(nameof(FilterStart));
            }
        }

        private DateTime? _filterEnd;
        public DateTime? FilterEnd
        {
            get => _filterEnd;
            set
            {
                _filterEnd = value;
                OnPropertyChanged(nameof(FilterEnd));
            }
        }

        // Commands
        public ICommand CreateReservationCommand { get; }
        public ICommand ShowCreateDialogCommand { get; }
        public ICommand CancelReservationCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ApplyDateFilterCommand { get; }
        public ICommand OpenAvailabilityCommand { get; }
        public ICommand EditDetailCommand { get; }
        public ICommand DeleteDetailCommand { get; }

        public BookingReservationViewModel(BookingReservationService bookingService, CustomerService customerService, RoomService roomService)
        {
            _bookingService = bookingService ?? throw new ArgumentNullException(nameof(bookingService));
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService)); // <-- Khởi tạo
            _roomService = roomService ?? throw new ArgumentNullException(nameof(roomService));           // <-- Khởi tạo


            // Default filter
            FilterStart = DateTime.Today.AddMonths(-1);
            FilterEnd = DateTime.Today.AddMonths(1);

            CreateReservationCommand = new RelayCommand(_ => CreateReservation(), _ => true);
            ShowCreateDialogCommand = new RelayCommand(_ => ShowCreateDialog(), _ => true);

            // <--- Important: set CanExecute to check SelectedReservation != null
            CancelReservationCommand = new RelayCommand(
                _ => CancelSelectedReservation(),
                _ => SelectedReservation != null
            );

            RefreshCommand = new RelayCommand(_ => Refresh(), _ => true);
            ApplyDateFilterCommand = new RelayCommand(_ => ApplyDateFilter(), _ => FilterStart.HasValue && FilterEnd.HasValue && FilterEnd >= FilterStart);
            OpenAvailabilityCommand = new RelayCommand(_ => OpenAvailability(), _ => true);

            EditDetailCommand = new RelayCommand(_ => EditDetail(), _ => SelectedReservation?.BookingDetails?.Any() == true);
            DeleteDetailCommand = new RelayCommand(_ => DeleteDetail(), _ => SelectedReservation?.BookingDetails?.Any() == true);

            // initial load
            ApplyDateFilter();
        }

        #region Actions

        private void Refresh()
        {
            if (FilterStart.HasValue && FilterEnd.HasValue)
            {
                LoadReservationsBetween(FilterStart.Value, FilterEnd.Value);
            }
            else
            {
                LoadReservationsBetween(DateTime.Today.AddYears(-1), DateTime.Today.AddYears(1));
            }
        }

        private void ApplyDateFilter()
        {


            if (!FilterStart.HasValue || !FilterEnd.HasValue) return;
            if (FilterEnd < FilterStart) return;

            LoadReservationsBetween(FilterStart.Value, FilterEnd.Value);
        }



        private void LoadReservationsBetween(DateTime start, DateTime end)
        {
            var dStart = DateOnly.FromDateTime(start);
            var dEnd = DateOnly.FromDateTime(end);

            Reservations.Clear();

            IEnumerable<BookingReservation> items;

            if (SessionManager.IsAdmin)
            {
                // ADMIN: Lấy tất cả đặt phòng
                items = _bookingService.GetReservationsBetween(dStart, dEnd) ?? Enumerable.Empty<BookingReservation>();
            }
            else if (SessionManager.IsCustomer && SessionManager.CurrentUser != null)
            {
                // CUSTOMER: Chỉ lấy đặt phòng của chính họ
                items = _bookingService.GetReservationsByCustomer(SessionManager.CurrentUser.CustomerId, dStart, dEnd)
                    ?? Enumerable.Empty<BookingReservation>();
            }
            else
            {
                // Chưa đăng nhập hoặc không có quyền
                items = Enumerable.Empty<BookingReservation>();
            }

            foreach (var r in items.OrderByDescending(r => r.BookingDate)) // Nên sắp xếp theo ngày
                Reservations.Add(r);

            SelectedReservation = Reservations.FirstOrDefault();
            OnPropertyChanged(nameof(Reservations));

            Debug.WriteLine("Loaded items: " + (items?.Count() ?? 0));
        }

        private void CancelSelectedReservation()
        {
            if (SelectedReservation == null) return;

            try
            {
                _bookingService.CancelReservation(SelectedReservation.BookingReservationId);
                Reservations.Remove(SelectedReservation);
                SelectedReservation = Reservations.FirstOrDefault();
            }
            catch (Exception)
            {
                // handle or rethrow; consider showing a message to user
                throw;
            }
        }

        private void CreateReservation()
        {
            ShowCreateDialog();
        }

        private void ShowCreateDialog()
        {
            // 1. Khởi tạo ViewModel cho cửa sổ Tạo mới
            var createVm = new BookingCreateViewModel(_bookingService, _customerService, _roomService);

            // 2. Khởi tạo View/Window
            var createReservationWindow = new BookingCreateWindow
            {
                DataContext = createVm,
                Owner = Application.Current.MainWindow // Đặt cửa sổ chính làm Owner để giữ nó ở phía trên
            };

            // 3. Hiển thị dưới dạng Dialog (modal)
            bool? result = createReservationWindow.ShowDialog();

            // 4. Kiểm tra kết quả và tải lại dữ liệu
            if (result == true)
            {
                // Tải lại danh sách đặt phòng để hiển thị bản ghi mới được tạo
                Refresh();
            }
        }

        private void OpenAvailability()
        {
            DateOnly start, end;

            if (SelectedReservation != null && SelectedReservation.BookingDetails.Any())
            {
                start = SelectedReservation.BookingDetails.First().StartDate;
                end = SelectedReservation.BookingDetails.First().EndDate;
            }
            else if (FilterStart.HasValue && FilterEnd.HasValue)
            {
                start = DateOnly.FromDateTime(FilterStart.Value);
                end = DateOnly.FromDateTime(FilterEnd.Value);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn khoảng thời gian để xem phòng trống.", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var availableRooms = _bookingService.GetAvailableRooms(start, end);
            if (!availableRooms.Any())
            {
                MessageBox.Show("Không có phòng trống trong khoảng thời gian này.", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new AvailableRoomsWindow(availableRooms)
            {
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();
        }


        private void EditDetail()
        {
            if (SelectedReservation?.BookingDetails?.Any() != true) return;

            var firstDetail = SelectedReservation.BookingDetails.First();
            firstDetail.ActualPrice += 100000; // ví dụ
            OnPropertyChanged(nameof(SelectedReservation));
        }

        private void DeleteDetail()
        {
            if (SelectedReservation?.BookingDetails?.Any() != true) return;

            var firstDetail = SelectedReservation.BookingDetails.FirstOrDefault();
            if (firstDetail != null)
            {
                SelectedReservation.BookingDetails.Remove(firstDetail);
            }
            OnPropertyChanged(nameof(SelectedReservation));
        }

        #endregion

        #region Helpers

        // Try to raise CanExecuteChanged on the RelayCommand if method exists,
        // otherwise fallback to CommandManager.InvalidateRequerySuggested()
        private void RaiseCancelCommandCanExecuteChanged()
        {
            if (CancelReservationCommand is RelayCommand rc)
            {
                // common RelayCommand implementations have this method
                rc.RaiseCanExecuteChanged();
            }
            else
            {
                // fallback: ask WPF to requery (works for RoutedCommand/CommandManager scenarios)
                CommandManager.InvalidateRequerySuggested();
            }
        }

        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
