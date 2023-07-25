using System;
using System.Windows.Input;

namespace ShaderForm2.WPFTools;

public class TypedDelegateCommand<TDataType> : ICommand
{
	private readonly Predicate<TDataType>? _canExecute;
	private readonly Action<TDataType> _execute;

	public event EventHandler? CanExecuteChanged
	{
		add { CommandManager.RequerySuggested += value; }
		remove { CommandManager.RequerySuggested -= value; }
	}

	public TypedDelegateCommand(Action<TDataType> execute, Predicate<TDataType>? canExecute = null)
	{
		_execute = execute;
		_canExecute = canExecute;
	}

	public bool CanExecute(TDataType parameter)
	{
		return _canExecute == null || _canExecute(parameter);
	}

	public void Execute(TDataType parameter) => _execute(parameter);

	public bool CanExecute(object? parameter)
	{
		if (parameter is null) throw new ArgumentNullException(nameof(parameter));
		return CanExecute((TDataType)parameter);
	}

	public void Execute(object? parameter)
	{
		if (parameter is null)
		{
			throw new ArgumentNullException(nameof(parameter));
		}
		else
		{
			Execute((TDataType)parameter);
		}
	}
}
