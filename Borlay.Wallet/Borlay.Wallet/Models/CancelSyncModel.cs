﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Borlay.Wallet.Models
{
    public class CancelSyncModel : DefaultSyncModel
    {
        protected readonly CancellationTokenSource cts;
        protected readonly TaskCompletionSource<bool> tcs;

        public CancelSyncModel()
        {
            this.cts = new CancellationTokenSource();
            this.tcs = new TaskCompletionSource<bool>();
            cts.Token.Register(() => tcs.SetResult(true));
            this.CancelCommand = new ActionCommand(o => cts.Cancel());
        }

        public virtual Task WaitAsync()
        {
            return tcs.Task;
        }

        public CancellationToken Token => cts.Token;
        public ICommand CancelCommand { get; private set; }
    }
}
