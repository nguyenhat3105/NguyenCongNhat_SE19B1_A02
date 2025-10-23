using FUMiniHotelManagement.BLL.Services;
using FUMiniHotelManagement.DAL.Repositories;
using FUMiniHotelManagement.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FUMiniHotelManagement
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            var customerRepository = new CustomerRepository();

            // 2. KHỞI TẠO TẦNG BLL (Services) với Repository
            var customerService = new CustomerService();

            // SỬA LỖI CS1503: Truyền CustomerRepository vào AuthenticationService
            var authService = new AuthenticationService(customerRepository);

            var roomService = new RoomService();
            var bookingService = new BookingReservationService();

            var viewModel = new LoginViewModel(
                customerService,
                // THÊM: Truyền AuthenticationService vào vị trí thứ 2
                authService,
                // Truyền Func<string> vào vị trí thứ 3
                () => PasswordBox.Password,
                // Truyền Action<Customer> vào vị trí thứ 4
                customer =>
                {
                    var main = new MainWindow(customerService, roomService, bookingService, authService);
                    main.Show();
                    this.Close();
                });

            DataContext = viewModel;

        }
    }
}
