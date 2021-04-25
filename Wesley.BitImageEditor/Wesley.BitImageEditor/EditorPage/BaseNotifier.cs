using System.ComponentModel;

namespace Wesley.BitImageEditor.EditorPage
{
    /// <summary>for internal use by <see cref="Wesley.BitImageEditor"/></summary>
    public class BaseNotifier : INotifyPropertyChanged
    {
        /// <summary>for internal use by <see cref="Wesley.BitImageEditor"/></summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
