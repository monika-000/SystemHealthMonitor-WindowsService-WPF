
using System.ComponentModel;

namespace SystemHealthMonitor.WPF.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void CleanupMemeoryOnWindowClosed<T>(params System.Collections.IList[] collections)
        {
            foreach(var collection in collections)
            {
                collection?.Clear();
            }
        }

        public virtual void LoadData() { }
    }
}
