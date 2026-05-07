
using System.Windows.Input;

namespace SystemHealthMonitor.WPF.Commands
{
    internal class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;

        public RelayCommand(Action<object> execute)
        {
            _execute = execute;
        }

        event EventHandler? ICommand.CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
             return true;
        }

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }
    }
}
