using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.Windows.Controls;

namespace Utilities
{

    /// <summary>
    /// Class NotifyingViewModel  Lowest level of standard viewmodel
    /// </summary>
    public class NotifyingViewModel : ViewModelBase
    {
        protected bool SetPropertyValue<T>(ref T field, T value, [CallerMemberName] string propName = null)
        {
            // Generic type, so no ==
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            try
            {
                if (propName != null) RaisePropertyChanged(propName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }
            return true;
        }
    }
}
