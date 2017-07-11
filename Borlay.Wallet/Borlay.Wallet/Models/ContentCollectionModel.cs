using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet.Models
{

    public abstract class ContentCollectionModel : ModelBase
    {
        public ContentCollectionModel(IEnumerable<object> collection, params IconButtonModel[] iconButtons)
        {
            this.ContentItemsSource = collection;
            this.ActionItems = new ObservableCollection<IconButtonModel>(iconButtons);
        }

        public ObservableCollection<IconButtonModel> ActionItems { get; private set; }
        public IEnumerable<object> ContentItemsSource { get; private set; }
    }


    public class ContentCollectionModel<T> : ContentCollectionModel where T : class, new()
    {
        public ContentCollectionModel(ObservableCollection<T> contentItems, params IconButtonModel[] iconButtons)
            : base(contentItems, iconButtons)
        {
            this.ContentItems = contentItems;
        }

        public ContentCollectionModel(params IconButtonModel[] iconButtons)
            : this(new ObservableCollection<T>(), iconButtons)
        {
        }

        public ObservableCollection<T> ContentItems { get; private set; }
    }
}
