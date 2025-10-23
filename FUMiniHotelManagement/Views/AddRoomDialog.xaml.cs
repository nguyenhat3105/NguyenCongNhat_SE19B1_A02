using FUMiniHotelManagement.DAL;
using FUMiniHotelManagement.DAL.Entities;
using FUMiniHotelManagement.DAL.Repositories;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FUMiniHotelManagement.Views
{
    public partial class AddRoomDialog : Window
    {
        public RoomInformation NewRoom { get; private set; }

        public AddRoomDialog()
        {
            InitializeComponent();
            LoadRoomTypes();
        }

        private void LoadRoomTypes()
        {
            var context = new FuminiHotelManagementContext();
            cbRoomType.ItemsSource = context.RoomTypes.ToList();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedType = cbRoomType.SelectedItem as RoomType;
                if (selectedType == null)
                {
                    MessageBox.Show("Vui lòng chọn loại phòng!");
                    return;
                }

                // Xác định trạng thái phòng (Available = 0, Occupied = 1, Maintenance = 2)
                byte status = 0;
                string statusText = ((ComboBoxItem)cbStatus.SelectedItem)?.Content?.ToString() ?? "Available";
                switch (statusText)
                {
                    case "Occupied":
                        status = 1;
                        break;
                    case "Maintenance":
                        status = 2;
                        break;
                    default:
                        status = 0;
                        break;
                }

                // Tạo đối tượng RoomInformation hợp lệ với kiểu dữ liệu đúng
                NewRoom = new RoomInformation
                {
                    RoomNumber = txtRoomNumber.Text.Trim(),
                    RoomDetailDescription = txtDescription.Text.Trim(),
                    RoomMaxCapacity = int.TryParse(txtCapacity.Text, out int cap) ? cap : null,
                    RoomPricePerDay = decimal.TryParse(txtPrice.Text, out decimal price) ? price : 0,
                    RoomStatus = status,
                    RoomTypeId = selectedType.RoomTypeId,
                //    RoomType = selectedType
                };

                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nhập dữ liệu: " + ex.Message);
            }
        }


        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
