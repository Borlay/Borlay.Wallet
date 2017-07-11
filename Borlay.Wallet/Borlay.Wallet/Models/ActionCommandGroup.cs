using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet.Models
{
    public class ActionCommandGroup
    {
        private readonly IActionCommand[] commands;

        public ActionCommandGroup(params IActionCommand[] commands)
        {
            this.commands = commands.Where(c => c != null).ToArray();

            foreach(var command in this.commands)
            {
                command.ExecutingChanged += (s, a) => SetCanExecute(!a);
            }
        }

        public void SetCanExecute(bool canExecute)
        {
            foreach (var command in commands)
                command.SetCanExecute(canExecute);
        }
    }
}
