using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PI_Replication_Tool.Core
{
    internal class RelayCommand : ICommand
    {
        private Action<Object> execute;
        private Func<Object, bool> canExecute; 

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
        }
    }
}
