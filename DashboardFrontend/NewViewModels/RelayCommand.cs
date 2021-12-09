using System;
using System.Windows.Input;

namespace DashboardFrontend.NewViewModels
{
    public class RelayCommand : ICommand
    {
        public RelayCommand(Action<object> action, Predicate<object>? canExecute)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            _action = action;
            _canExecute = canExecute;
        }

        public RelayCommand(Action<object> action) : this(action, null)
        {

        }

        public event EventHandler? CanExecuteChanged;

        private readonly Action<object> _action;
        private readonly Predicate<object>? _canExecute;

        public bool CanExecute(object? parameter)
        {
            return _canExecute is null || _canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            _action(parameter);
        }
    }
}
