using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet.Models
{
    public class BundleItemModel : TransactionItemBaseModel
    {
        private readonly BundleDetailModel bundleDetail;

        public BundleItemModel(params TransactionItemModel[] transactionItems)
        {
            this.bundleDetail = new BundleDetailModel(transactionItems);
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
    }
}
