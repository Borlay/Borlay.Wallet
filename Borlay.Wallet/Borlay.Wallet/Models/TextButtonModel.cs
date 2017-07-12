using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet.Models
{
    public class TextButtonModel : ButtonModel
    {
        public TextButtonModel(Func<ButtonModel, Task> click)
            : base(click)
        {
        }

        public TextButtonModel(Action<ButtonModel> click)
            : base(click)
        {
        }

        public TextButtonModel()
        {
        }
    }
}
