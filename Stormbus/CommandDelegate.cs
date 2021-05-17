using System;
using System.Windows.Input;

namespace Stormbus.UI
{
    public class CommandDelegate : ICommand
    {
        public Predicate<object> CanExecutePredicate;
        public event EventHandler CanExecuteChanged;

        public Action<object> ExecuteHandler;

        public void Execute(object parameter)
        {
            ExecuteHandler?.Invoke(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return CanExecutePredicate?.Invoke(parameter) != false;
        }
    }
}