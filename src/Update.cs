using ShaderForm2.WPFTools;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Zenseless.Patterns.Property;

namespace ShaderForm2;

internal class Update : NotifyPropertyChanged
{
	public Update()
	{
		AutoUpdateViaGitHubRelease.Update update = new();
		update.PropertyChanged += (s, a) => Available = update.Available;
		Assembly assembly = Assembly.GetExecutingAssembly();
		_ = update.CheckDownloadNewVersionAsync("danielScherzer", "ShaderForm2", assembly.GetName().Version, Path.GetTempPath());

		void UpdateAndClose()
		{
			_ = update.StartInstall(Path.GetDirectoryName(assembly.Location));
			Application app = Application.Current;
			app.Shutdown();
		}

		CommandUpdate = new DelegateCommand(_ => UpdateAndClose(), _ => Available);
		CommandClose = new DelegateCommand(_ => Application.Current.Shutdown(), _ => true);
	}

	public bool Available { get => _available; private set => Set(ref _available, value, _ => CommandManager.InvalidateRequerySuggested()); }
	public ICommand CommandUpdate { get; }
	public ICommand CommandClose { get; }

	private bool _available;
}
