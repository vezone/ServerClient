using System;
using System.Windows.Input;

namespace ClientUI
{
    class RelayCommand : ICommand
    {
        Action<object> m_Execute;
        Func<object, bool> m_CanExecute;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<object> execute)
            : this(execute, (object parametr) => true)
        {
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            m_Execute = execute;
            m_CanExecute = canExecute;
        }

        public void Execute(object parameter)
        {
            m_Execute(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return m_CanExecute(parameter);
        }

    }
}
