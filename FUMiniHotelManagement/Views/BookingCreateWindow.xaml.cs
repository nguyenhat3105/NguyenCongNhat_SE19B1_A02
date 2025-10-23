using FUMiniHotelManagement.ViewModel;
using System.Windows;

namespace FUMiniHotelManagement.Views
{
    public partial class BookingCreateWindow : Window
    {
        public BookingCreateWindow()
        {
            InitializeComponent();
            // DataContext sẽ được set từ caller hoặc bạn có thể new trong đây (prefer inject)
        }

        // Caller will set DataContext to BookingCreateViewModel
        protected override void OnSourceInitialized(System.EventArgs e)
        {
            base.OnSourceInitialized(e);
            if (DataContext is BookingCreateViewModel vm)
            {
                vm.RequestClose += Vm_RequestClose;
            }
        }

        private void Vm_RequestClose(bool? dialogResult)
        {
            // Detach to avoid memory leak
            if (DataContext is BookingCreateViewModel vm)
                vm.RequestClose -= Vm_RequestClose;

            this.DialogResult = dialogResult;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
