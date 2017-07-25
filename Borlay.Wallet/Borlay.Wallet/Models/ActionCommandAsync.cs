using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Borlay.Wallet.Models
{
    public interface IActionCommand : ICommand
    {
        event Action<IActionCommand, bool> ExecutingChanged;
        void SetCanExecute(bool canExecute);
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

            try
            {
                await action(parameter);
            }
            catch(OperationCanceledException)
            {
                // do nothing
            }
            catch(Exception e)
            {
                // todo handle in better way
                MessageBox.Show(e.Message);
            }
            finally
            {
                isExecuting = false;
                ExecutingChanged(this, false);

                RaiseCanExecuteChanged();
            }
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
