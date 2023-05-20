using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Webolar.Framework;

public class CommandHandler : ICommand
{
    #region Fields

    private readonly Action _action;
    private readonly Func<bool> _canExecutee;
    private readonly Action<object> _execute;
    private readonly Predicate<object> _canExecute;

    #endregion

    // Fields

    #region Constructors

    public CommandHandler(Action<object> execute)
        : this(execute, null)
    {
    }

    public CommandHandler(Action action, Func<bool> canExecute)
    {
        _action = action;
        _canExecutee = canExecute;
    }

    public CommandHandler(Action<object> execute, Predicate<object> canExecute)
    {
        if (execute == null) throw new ArgumentNullException("execute");
        _execute = execute;
        _canExecute = canExecute;
    }

    #endregion

    // Constructors

    #region ICommand Members

    [DebuggerStepThrough]
    public bool CanExecute(object parameter)
    {
        return _canExecute == null ? true : _canExecute(parameter);
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public void Execute(object parameter)
    {
        _execute(parameter);
    }

    public void CheckAndExecute(object parameter)
    {
        if (CanExecute(parameter)) Execute(parameter);
    }

    #endregion

    // ICommand Members
}