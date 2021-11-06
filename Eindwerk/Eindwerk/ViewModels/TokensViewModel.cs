using System.ComponentModel;
using System.Runtime.CompilerServices;
using Eindwerk.Annotations;
using Eindwerk.Models;

namespace Eindwerk.ViewModels
{
    public class TokensViewModel : INotifyPropertyChanged
    {
        public Tokens Tokens
        {
            get => _tokens;
            set { _tokens = value; OnPropertyChanged(nameof(Tokens)); }
        }

        private Tokens _tokens;
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}