using System;
using System.Windows.Input;

namespace Stormbus.UI
{
    public class WpfCommandDelegate : ICommand
    {
        public Predicate<object> CanExecutePredicate;

        public Action<object> ExecuteHandler;

        public bool CanExecute(object parameter)
        {
            return CanExecutePredicate?.Invoke(parameter) != false;
        }

        public void Execute(object parameter)
        {
            ExecuteHandler?.Invoke(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}