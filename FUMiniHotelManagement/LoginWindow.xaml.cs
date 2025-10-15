using FUMiniHotelManagement.BLL.Services;
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

            var customerService = new CustomerService();
            var roomService = new RoomService();

            var viewModel = new LoginViewModel(
                customerService,
                () => PasswordBox.Password,
                customer =>
                {
                    var main = new MainWindow(customerService, roomService);
                    main.Show();
                    this.Close();
                });

            DataContext = viewModel;

        }
    }
}
