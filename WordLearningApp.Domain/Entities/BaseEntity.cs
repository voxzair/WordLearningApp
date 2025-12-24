using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WordLearningApp.Domain.Entities
{
    public abstract class BaseEntity : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}