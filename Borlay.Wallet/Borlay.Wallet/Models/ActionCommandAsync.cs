using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Borlay.Wallet.Models
{
    public class ActionCommandAsync : ICommand
    {
        public event EventHandler CanExecuteChanged = (s, a) => { };

        private readonly Func<object, Task> action;
        private bool canExecute = true;
        private bool isExecuting = false;


        public ActionCommandAsync(Func<object, Task> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            this.action = action;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute && !isExecuting;
        }

        public async void Execute(object parameter)
        {
            isExecuting = true;

            RaiseCanExecuteChanged();

            await action(parameter);
            isExecuting = false;

            RaiseCanExecuteChanged();
        }

        public void SetCanExecute(bool canExecute)
        {
            if (this.canExecute != canExecute)
            {
                this.canExecute = canExecute;
                RaiseCanExecuteChanged();
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, new EventArgs());
        }

    }
}
