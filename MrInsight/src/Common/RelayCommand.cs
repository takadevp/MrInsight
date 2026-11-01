using System.Windows.Input;

namespace MrInsight;

public sealed class RelayCommand(Action execute, Func<bool> canExecute) : ICommand
{
	public event EventHandler? CanExecuteChanged
	{
		add
		{
			CommandManager.RequerySuggested += value;
		}
		remove
		{
			CommandManager.RequerySuggested -= value;
		}
	}

	public bool CanExecute(object? parameter)
	{
		return canExecute();
	}

	public void Execute(object? parameter)
	{
		execute();
	}

	public void RaiseCanExecuteChanged()
	{
		CommandManager.InvalidateRequerySuggested();
	}
}
