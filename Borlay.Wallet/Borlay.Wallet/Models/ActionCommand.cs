using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Borlay.Wallet.Models
{
    public class ActionCommand : IActionCommand
    {
        public event EventHandler CanExecuteChanged = (s, a) => { };
        public event Action<IActionCommand, bool> ExecutingChanged = (s, a) => { };

        private readonly Action<object> action;
        private bool canExecute = true;


        public ActionCommand(Action<object> action)
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
            ExecutingChanged(this, true);
            try
            {
                action(parameter);
            }
            catch (OperationCanceledException)
            {
                // do nothing
            }
            catch (Exception e)
            {
                // todo handle in better way
                MessageBox.Show(e.Message);
            }
            finally
            {
                ExecutingChanged(this, false);
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
