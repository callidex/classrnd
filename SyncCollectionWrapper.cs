using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace Callidex
{
    public class AsyncObservableCollection<T> : ObservableCollection<T>
    {
        private SynchronizationContext _synchronizationContext = SynchronizationContext.Current;

        public AsyncObservableCollection()
        {
        }

        public AsyncObservableCollection(IEnumerable<T> list)
            : base(list)
        {
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (SynchronizationContext.Current == _synchronizationContext)
            {
                // Execute the CollectionChanged event on the current thread
                RaiseCollectionChanged(e);
            }
            else
            {
                // Post the CollectionChanged event on the creator thread
                _synchronizationContext.Post(RaiseCollectionChanged, e);
            }
        }

        private void RaiseCollectionChanged(object param)
        {
            // We are in the creator thread, call the base implementation directly
            base.OnCollectionChanged((NotifyCollectionChangedEventArgs)param);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (SynchronizationContext.Current == _synchronizationContext)
            {
                // Execute the PropertyChanged event on the current thread
                RaisePropertyChanged(e);
            }
            else
            {
                // Post the PropertyChanged event on the creator thread
                _synchronizationContext.Post(RaisePropertyChanged, e);
            }
        }

        private void RaisePropertyChanged(object param)
        {
            // We are in the creator thread, call the base implementation directly
            base.OnPropertyChanged((PropertyChangedEventArgs)param);
        }
    }

    class SyncCollectionWrapper :ObservableCollection<object>
    {
        // Attempt a generic class which handles new items being put in the container

        private List<object> _target; 
        public SyncCollectionWrapper(List<object> target )
        {
            _target = target;
            this.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(SyncCollectionWrapper_CollectionChanged);
        }

        void SyncCollectionWrapper_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // generic synchroniser
            switch(e.Action)
            {
                case NotifyCollectionChangedAction.Add :
                    Console.Out.WriteLine("Add");
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach(object i in e.NewItems )
                    {
                        Console.Out.WriteLine("Remove");
                    }
                    break;
            
            }
        }
    }
}
