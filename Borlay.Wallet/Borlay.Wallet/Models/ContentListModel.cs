using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet.Models
{
    public abstract class ContentListModel : ModelBase
    {
        public ContentListModel(IEnumerable<object> collection, params ButtonModel[] buttons)
        {
            this.ContentItemsSource = collection;
            this.ActionItems = new ObservableCollection<ButtonModel>(buttons);
        }

        public ObservableCollection<ButtonModel> ActionItems { get; private set; }
        public IEnumerable<object> ContentItemsSource { get; private set; }
    }


    public class ContentListModel<T> : ContentListModel where T : class, new()
    {
        public ContentListModel(ObservableCollection<T> contentItems, params ButtonModel[] buttons)
            : base(contentItems, buttons)
        {
            this.ContentItems = contentItems;
        }

        public ContentListModel(params ButtonModel[] buttons)
            : this(new ObservableCollection<T>(), buttons)
        {
        }

        public ObservableCollection<T> ContentItems { get; private set; }
    }
}
