using System;
using System.Windows.Input;

namespace FUMiniHotelManagement.Helper
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        // backing field for our event handlers
        private event EventHandler? _canExecuteChanged;

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
            => _canExecute == null || _canExecute(parameter);

        public void Execute(object? parameter)
            => _execute(parameter);

        // Implement event so we can both:
        // - allow WPF's CommandManager to subscribe (auto requery)
        // - keep our own backing list so we can invoke directly
        public event EventHandler? CanExecuteChanged
        {
            add
            {
                // add to our backing invocation list
                _canExecuteChanged += value;
                // also register with CommandManager so WPF re-queries automatically
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                _canExecuteChanged -= value;
                CommandManager.RequerySuggested -= value;
            }
        }

        // Raise only our own handlers (and keep behavior predictable)
        public void RaiseCanExecuteChanged()
        {
            _canExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
