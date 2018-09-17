using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PartsReserver.ViewModels
{
    /// <summary>
    /// Базовый класс для ViewModel.
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        protected BaseViewModel()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}