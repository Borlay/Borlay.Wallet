using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet.Models
{
    public abstract class CollectionModel : ModelBase
    {
        public CollectionModel(IEnumerable<object> collectionSource)
        {
            this.CollectionSource = collectionSource;
        }

        public IEnumerable<object> CollectionSource { get; private set; }
    }

    public class CollectionModel<T> : CollectionModel where T : class
    {
        public CollectionModel(ObservableCollection<T> collection)
            : base(collection)
        {
            this.Collection = collection;
        }

        public CollectionModel()
            : this(new ObservableCollection<T>())
        {
        }

        public ObservableCollection<T> Collection { get; private set; }
    }
}
