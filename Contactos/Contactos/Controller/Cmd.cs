using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Contactos.Controller
{
    public class Cmd : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        private Action<object> _execute;
        private Predicate<object> _canexecute;

        public Cmd (Action<object>execute, Predicate<object> canexecute)
        {
            this._execute = execute;
            this._canexecute = canexecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canexecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
