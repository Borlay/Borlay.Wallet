using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Borlay.Wallet.Models
{
    public class SyncModel : ModelBase, IEnterSync, ISyncView
    {
        private readonly ISyncView view;
        private readonly SemaphoreSlim semaphoreSlim;

        public SyncModel(ISyncView view)
        {
            this.view = view;
            this.semaphoreSlim = new SemaphoreSlim(1);
        }

        public async Task<SyncContent> EnterSync()
        {
            await semaphoreSlim.WaitAsync();

            var oldView = view.SyncView;
            view.SyncView = this;
            var syncContent = new SyncContent(this, () => 
            {
                try
                {
                    view.SyncView = oldView;
                }
                finally
                {
                    semaphoreSlim.Release();
                }
            });
            return syncContent;
        }

        private object syncView;
        public object SyncView
        {
            get
            {
                return syncView;
            }
            set
            {
                syncView = value;
                NotifyPropertyChanged();
            }
        }
    }

    public interface IEnterSync
    {
        Task<SyncContent> EnterSync();
    }

    public class SyncContent : IDisposable
    {
        private readonly Action dispose;
        private readonly ISyncView syncView;

        public SyncContent(ISyncView syncView, Action dispose)
        {
            this.syncView = syncView;
            this.dispose = dispose;
        }

        public void SetView(object view)
        {
            syncView.SyncView = view;
        }

        public void SetText(string text)
        {
            if (syncView is IText t)
                t.Text = text;
        }

        public void Dispose()
        {
            dispose();
        }
    }

    public interface ISyncView
    {
        object SyncView { get; set; }
    }

    public static class SyncExtensions
    {
        public static CancelSyncModel SetCancelView(this SyncContent syncContent, string text)
        {
            var cancelSyncModel = new CancelSyncModel() { Text = text };
            syncContent.SetView(cancelSyncModel);
            return cancelSyncModel;
        }

        public static DefaultSyncModel SetTextView(this SyncContent syncContent, string text)
        {
            var defaultSyncModel = new DefaultSyncModel() { Text = text };
            syncContent.SetView(defaultSyncModel);
            return defaultSyncModel;
        }
    }

}
