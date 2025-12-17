using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KISHelper.Common
{
    using System;
    using System.Windows.Input;

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T>? _canExecute;

        public RelayCommand(Action<T> execute, Predicate<T>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter)
        {
            // 处理 null 和类型安全问题
            if (parameter == null)
                return _canExecute?.Invoke(default) ?? true;

            if (parameter is T typedParameter)
                return _canExecute?.Invoke(typedParameter) ?? true;

            return false; // 类型不匹配
        }

        public void Execute(object? parameter)
        {
            if (parameter == null)
            {
                _execute(default);
            }
            else if (parameter is T typedParameter)
            {
                _execute(typedParameter);
            }
            else
            {
                throw new ArgumentException($"参数必须是 {typeof(T).Name} 类型");
            }
        }
    }

    // 非泛型版本（用于无参数命令）
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;
        public void Execute(object? parameter) => _execute();

        public static implicit operator RelayCommand(RelayCommand<AccountBook> v)
        {
            throw new NotImplementedException();
        }
    }


}
