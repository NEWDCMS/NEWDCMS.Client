using System.ComponentModel;

namespace Wesley.BitImageEditor.EditorPage
{
    public class BaseNotifier : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
