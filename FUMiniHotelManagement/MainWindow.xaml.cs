using FUMiniHotelManagement.BLL.Services;
using FUMiniHotelManagement.ViewModel;
using FUMiniHotelManagement.Views;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FUMiniHotelManagement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(CustomerService customerService, RoomService roomService)
        {
            InitializeComponent();
            var vm = new MainViewModel(customerService, roomService);
            DataContext = vm;
        }

    }
}