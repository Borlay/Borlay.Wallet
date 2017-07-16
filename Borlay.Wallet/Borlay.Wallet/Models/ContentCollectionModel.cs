﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet.Models
{

    public abstract class ContentCollectionModel : ModelBase
    {
        public ContentCollectionModel(IEnumerable<object> collection, params ButtonModel[] buttons)
        {
            this.ContentItemsSource = collection;
            this.ActionItems = new ObservableCollection<ButtonModel>(buttons);
        }

        public ObservableCollection<ButtonModel> ActionItems { get; private set; }
        public IEnumerable<object> ContentItemsSource { get; private set; }
    }


    public class ContentCollectionModel<T> : ContentCollectionModel where T : class
    {
        public ContentCollectionModel(ObservableCollection<T> contentItems, params ButtonModel[] buttons)
            : base(contentItems, buttons)
        {
            this.ContentItems = contentItems;
        }

        public ContentCollectionModel(params ButtonModel[] buttons)
            : this(new ObservableCollection<T>(), buttons)
        {
        }

        public ObservableCollection<T> ContentItems { get; private set; }
    }
}
