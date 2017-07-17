using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Borlay.Wallet.Models
{
    public class BundleItemModel : TransactionItemBaseModel
    {
        public event Action<BundleItemModel> Rebroadcast = (b) => { };

        private readonly BundleDetailModel bundleDetail;
        private readonly IconButtonModel downButton;
        private readonly IconButtonModel rebroadcastButton;

        public BundleItemModel(params TransactionItemModel[] transactionItems)
        {
            this.bundleDetail = new BundleDetailModel(transactionItems);

            this.ActionItems = new ObservableCollection<ButtonModel>();

            this.downButton = new IconButtonModel((b) => IsSelected = !isSelected, IconType.Down, ColorType.Gray);
            this.rebroadcastButton = new IconButtonModel(b => OnRebroadcast(), IconType.Replace, ColorType.Gray);

            this.ActionItems.Add(new IconButtonModel((b) => OpenInBrowser(), IconType.Browser, ColorType.Gray));
            //this.ActionItems.Add(new IconButtonModel((b) => Clipboard.SetText(base.Hash), IconType.Copy, ColorType.Gray));
            this.ActionItems.Add(downButton);

            
        }

        public void OpenInBrowser()
        {
            // http://node3.tangler.org/search/?kind=address&hash=9E9AGQVGJLBYECABXVQDZJ9NLGGOKNQD9HVMAZPZQMOSXELGRGNE9WNMWUUICFILRSTV9OQVOPRLYGZBIAETGCERGI
            System.Diagnostics.Process.Start($"http://node3.tangler.org/search/?kind=bundle&hash={base.Hash}");
        }

        public void OnRebroadcast()
        {
            Rebroadcast(this);
        }

        public override bool IsConfirmed
        {
            get => base.IsConfirmed;
            set
            {
                base.IsConfirmed = value;
                if (value)
                {
                    this.ActionItems.Remove(rebroadcastButton);
                }
                else
                {
                    if(!this.ActionItems.Any(b => b.Equals(rebroadcastButton)))
                        this.ActionItems.Insert(0, rebroadcastButton);
                }
            }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if (this.isSelected != value)
                {
                    this.isSelected = value;
                    NotifyPropertyChanged();

                    if(isSelected)
                        downButton.SetIcon(IconType.DownFilled, ColorType.Blue);
                    else
                        downButton.SetIcon(IconType.Down, ColorType.Gray);

                    if (value)
                        VisibleBundleDetail = BundleDetail;
                    else
                        VisibleBundleDetail = null;
                }
            }
        }

        public BundleDetailModel BundleDetail => bundleDetail;

        private BundleDetailModel visibleBundleDetail;
        public BundleDetailModel VisibleBundleDetail
        {
            get
            {
                return visibleBundleDetail;
            }
            set
            {
                this.visibleBundleDetail = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<ButtonModel> ActionItems { get; private set; }
    }
}
