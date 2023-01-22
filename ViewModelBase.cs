using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Webolar.Framework
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        protected virtual void SetProperty<T>(ref T member, T val, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(member, val)) return;
            member = val;
            RaisePropertyChanged(propertyName);
        }

        private ICommand _initializeCommand;

        public ICommand InitializeCommand => _initializeCommand ??
                                           (_initializeCommand = new  RelayCommand(()=> Initialize(), ()=> CanInitialize()));
        public virtual void Initialize()
        {
           
        }
        public virtual bool CanInitialize()
        {
            return true;
        }
        public virtual bool CanExecute()
        {
            return true;
        }
    }
}
