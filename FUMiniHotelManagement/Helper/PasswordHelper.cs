using System.Windows;
using System.Windows.Controls;

namespace FUMiniHotelManagement.Helper
{
    public static class PasswordHelper
    {
        // Định nghĩa Attached Property cho phép bind Password
        public static readonly DependencyProperty BoundPassword =
            DependencyProperty.RegisterAttached("BoundPassword",
                typeof(string),
                typeof(PasswordHelper),
                new FrameworkPropertyMetadata(string.Empty, OnBoundPasswordChanged));

        public static readonly DependencyProperty BindPassword =
            DependencyProperty.RegisterAttached("BindPassword",
                typeof(bool),
                typeof(PasswordHelper),
                new PropertyMetadata(false, OnBindPasswordChanged));

        private static readonly DependencyProperty UpdatingPassword =
            DependencyProperty.RegisterAttached("UpdatingPassword",
                typeof(bool),
                typeof(PasswordHelper),
                new PropertyMetadata(false));

        public static string GetBoundPassword(DependencyObject d)
            => (string)d.GetValue(BoundPassword);

        public static void SetBoundPassword(DependencyObject d, string value)
            => d.SetValue(BoundPassword, value);

        public static bool GetBindPassword(DependencyObject d)
            => (bool)d.GetValue(BindPassword);

        public static void SetBindPassword(DependencyObject d, bool value)
            => d.SetValue(BindPassword, value);

        private static bool GetUpdatingPassword(DependencyObject d)
            => (bool)d.GetValue(UpdatingPassword);

        private static void SetUpdatingPassword(DependencyObject d, bool value)
            => d.SetValue(UpdatingPassword, value);

        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                passwordBox.PasswordChanged -= HandlePasswordChanged;

                if (!(bool)GetUpdatingPassword(passwordBox))
                    passwordBox.Password = (string)e.NewValue ?? string.Empty;

                passwordBox.PasswordChanged += HandlePasswordChanged;
            }
        }

        private static void OnBindPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            if (dp is PasswordBox passwordBox)
            {
                bool wasBound = (bool)(e.OldValue);
                bool needToBind = (bool)(e.NewValue);

                if (wasBound)
                    passwordBox.PasswordChanged -= HandlePasswordChanged;

                if (needToBind)
                    passwordBox.PasswordChanged += HandlePasswordChanged;
            }
        }

        private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                SetUpdatingPassword(passwordBox, true);
                SetBoundPassword(passwordBox, passwordBox.Password);
                SetUpdatingPassword(passwordBox, false);
            }
        }
    }
}
