using System;
using System.Windows.Input;

namespace ShaderForm2.WPFTools;

//TODO: Use or delete
public class UiCommand : RoutedUICommand
{
	public UiCommand(string text, string name, Type ownerType, InputGestureCollection inputGestures, ICommand command) : base(text, name, ownerType, inputGestures)
	{
		_command = command ?? throw new ArgumentNullException(nameof(command));
	}

	public bool CanExecute(object? parameter) => _command.CanExecute(parameter);

	public void Execute(object? parameter) => _command.Execute(parameter);

	private readonly ICommand _command;
}
