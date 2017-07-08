using System;
using System.Windows.Input;

namespace Borlay.Wallet.Models
{
    public class ActionCommand : ICommand
    {
        public event EventHandler CanExecuteChanged = (s, a) => { };

        private readonly Action action;
        private bool canExecute = true;


        public ActionCommand(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            this.action = action;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute;
        }

        public void Execute(object parameter)
        {
            action();
        }

        public void SetCanExecute(bool canExecute)
        {
            if (this.canExecute != canExecute)
            {
                this.canExecute = canExecute;
                CanExecuteChanged(this, new EventArgs());
            }
        }

    }
}
