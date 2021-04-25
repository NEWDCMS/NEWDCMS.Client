using System;
using Xamarin.Forms;

namespace Wesley.Client.CustomViews
{
    public class CustomCollectionView : CollectionView
    {
        public event EventHandler Loaded;
        public void OnLoaded()
        {
            Loaded?.Invoke(this, new EventArgs());
        }
    }
}