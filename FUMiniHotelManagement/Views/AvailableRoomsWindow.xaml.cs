using System.Collections.ObjectModel;
using System.Windows;
using FUMiniHotelManagement.DAL.Entities;

namespace FUMiniHotelManagement.Views
{
    public partial class AvailableRoomsWindow : Window
    {
        public ObservableCollection<RoomInformation> Rooms { get; set; }

        public AvailableRoomsWindow(IEnumerable<RoomInformation> rooms)
        {
            InitializeComponent();
            Rooms = new ObservableCollection<RoomInformation>(rooms);
            DataContext = this;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
