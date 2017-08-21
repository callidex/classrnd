using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq.Expressions;
using System.Windows.Input;

namespace Utilities
{
    public class ReasonDelegateCommand : DelegateCommand
    {
        public ReasonDelegateCommand(Action execute) : base(execute)
        {
        }


        public ReasonDelegateCommand(Action execute, Func<bool> canExecute) : base(execute, canExecute)
        {
        }

        public ReasonDelegateCommand(Action<object> execute, Func<object, bool> canExecute) : base(execute, canExecute)
        {
        }

        public string Reason { get; set; }
    }


    public class DelegateCommand : ICommand
    {
        private Func<object, bool> _canExecute;

        private Action<object> _execute;

        public DelegateCommand(Action execute) : this(execute, null)
        {
        }

        public DelegateCommand(Action<object> execute) : this(execute, null)
        {
        }

        public DelegateCommand(Action execute, Func<bool> canExecute)
            : this(
                execute != null ? p => execute() : (Action<object>)null,
                canExecute != null ? p => canExecute() : (Func<object, bool>)null)
        {
        }

        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            if (execute == null) throw new ArgumentNullException(nameof(execute));

            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            if (!CanExecute(parameter))
                throw new InvalidOperationException(
                    "The command cannot be executed because the canExecute action returned false.");

            _execute(parameter);
        }

        public void ReplaceAction(Action<object> newAction)
        {
            _execute = newAction;
        }

        public void ReplaceExecuteCheck(Func<object, bool> newCheck)
        {
            _canExecute = newCheck;
        }

        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged(EventArgs.Empty);
        }

        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            var handler = CanExecuteChanged;
            handler?.Invoke(this, e);
        }
    }


    public class WatchingDelegateCommand<TViewModel> : ReasonDelegateCommand where TViewModel : INotifyPropertyChanged
    {
        private readonly List<string> _PropertiesToWatch;


        public WatchingDelegateCommand(TViewModel viewModelInstance, Action executedMethod, Func<bool> canExecuteMethod,
            Expression<Func<TViewModel, object>> propertiesToWatch)
            : base(executedMethod, canExecuteMethod)
        {
            _PropertiesToWatch = RegisterPropertiesWatcher(propertiesToWatch);
            viewModelInstance.PropertyChanged += PropertyChangedHandler;
        }


        private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (_PropertiesToWatch.Contains(e.PropertyName))
                OnCanExecuteChanged(e);
        }

        protected List<string> RegisterPropertiesWatcher(Expression<Func<TViewModel, object>> selector)
        {
            var properties = new List<string>();

            LambdaExpression lambda = selector;

            if (lambda.Body is MemberExpression)
            {
                var memberExpression = (MemberExpression)lambda.Body;
                properties.Add(memberExpression.Member.Name);
            }
            else if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;

                properties.Add(((MemberExpression)unaryExpression.Operand).Member.Name);
            }
            else if (lambda.Body.NodeType == ExpressionType.New)
            {
                var newExp = (NewExpression)lambda.Body;
                foreach (var argument in newExp.Arguments)
                {
                    var exp = argument as MemberExpression;
                    if (exp != null)
                    {
                        var mExp = exp;
                        properties.Add(mExp.Member.Name);
                    }
                    else
                    {
                        throw new SyntaxErrorException(
                            "Syntax Error, selector has to be an expression that returns a new object containing a list of properties, e.g.: s => new { s.Property1, s.Property2 }");
                    }
                }
            }
            else
            {
                throw new SyntaxErrorException(
                    "Syntax Error, selector has to be an expression that returns a new object containing a list of properties, e.g.: s => new { s.Property1, s.Property2 }");
            }

            return properties;
        }
    }
}
