using System.Windows;
using System.Windows.Threading;

namespace MrInsight;

public partial class App : Application
{
	protected override void OnStartup(StartupEventArgs e)
	{
		DispatcherUnhandledException += OnDispatcherUnhandledException;
		TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
		AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;

		AppLog.Write($"起動しました: {AppEnvironment.ProjectPath}");
		DatabaseInitializer.Initialize();
		base.OnStartup(e);
	}

	private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
	{
		AppLog.Write(e.Exception);
		e.Handled = true;
		MessageBox.Show($"{e.Exception.Message}{Environment.NewLine}{Environment.NewLine}詳しくは画面下のログを見てください。", "MrInsight", MessageBoxButton.OK, MessageBoxImage.Error);
	}

	private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
	{
		AppLog.Write(e.Exception);
		e.SetObserved();
	}

	private void OnDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		if (e.ExceptionObject is Exception exception)
		{
			AppLog.Write(exception);
		}
	}
}
