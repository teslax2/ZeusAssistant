using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ZeusAssistant.Model
{
    class CommandBinding : ICommand
    {
        private Action _action;
        private Func<bool> _canExecute;
        public event EventHandler CanExecuteChanged;

        public CommandBinding(Action action, Func<bool> CanExecute)
        {
            _action = action;
            _canExecute = CanExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute();
        }

        public void Execute(object parameter)
        {
            _action();
        }
    }
}
