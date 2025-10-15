using FUMiniHotelManagement.BLL.Services;
using FUMiniHotelManagement.DAL.Entities;
using FUMiniHotelManagement.Helper;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace FUMiniHotelManagement.ViewModel
{
    public class RoomViewModel : INotifyPropertyChanged
    {
        private readonly RoomService _roomService;

        public RoomViewModel(RoomService roomService)
        {
            _roomService = roomService;
            Rooms = new ObservableCollection<RoomInformation>();
            LoadRooms();

            RefreshCommand = new RelayCommand(_ => LoadRooms());
            DeleteCommand = new RelayCommand(_ => DeleteSelected(), _ => SelectedRoom != null);
            SaveCommand = new RelayCommand(SaveChanges);
        }

        public ObservableCollection<RoomInformation> Rooms { get; set; }

        private RoomInformation _selectedRoom;
        public RoomInformation SelectedRoom
        {
            get => _selectedRoom;
            set
            {
                _selectedRoom = value;
                OnPropertyChanged(nameof(SelectedRoom));
            }
        }

        public ICommand RefreshCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }

        private void LoadRooms()
        {
            Rooms.Clear();
            var list = _roomService.GetAll();
            foreach (var r in list)
                Rooms.Add(r);
        }

        private void DeleteSelected()
        {
            if (SelectedRoom == null)
            {
                MessageBox.Show("Vui lòng chọn phòng để xóa!");
                return;
            }

            var confirm = MessageBox.Show($"Bạn có chắc muốn xóa phòng '{SelectedRoom.RoomNumber}'?",
                "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirm == MessageBoxResult.Yes)
            {
                _roomService.Delete(SelectedRoom);
                LoadRooms();
            }
        }

        private void SaveChanges(object obj)
        {
            foreach (var r in Rooms)
                _roomService.Update(r);

            MessageBox.Show("Đã lưu thay đổi!");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
