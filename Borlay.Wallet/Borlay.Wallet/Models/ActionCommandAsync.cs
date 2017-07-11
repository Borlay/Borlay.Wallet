using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Borlay.Wallet.Models
{
    public interface IActionCommand : ICommand
    {
        event Action<IActionCommand, bool> ExecutingChanged;

        void SetCanExecute(bool canExecute);
        //void EnabledGroup(bool enabledGroup);
    }

    public class ActionCommandAsync : IActionCommand
    {
        public event EventHandler CanExecuteChanged = (s, a) => { };
        public event Action<IActionCommand, bool> ExecutingChanged = (s, a) => { };

        private readonly Func<object, Task> action;
        private bool canExecute = true;
        private bool isExecuting = false;
        private bool enabledGroup = true;

        public ActionCommandAsync(Func<object, Task> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            this.action = action;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute && enabledGroup && !isExecuting;
        }

        public async void Execute(object parameter)
        {
            isExecuting = true;
            ExecutingChanged(this, true);

            RaiseCanExecuteChanged();

            await action(parameter);
            isExecuting = false;
            ExecutingChanged(this, false);

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

        //public void EnabledGroup(bool enabledGroup)
        //{
        //    if (this.enabledGroup != enabledGroup)
        //    {
        //        this.enabledGroup = enabledGroup;
        //        RaiseCanExecuteChanged();
        //    }
        //}

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, new EventArgs());
        }
    }
}
