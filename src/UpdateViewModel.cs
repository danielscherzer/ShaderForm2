using AutoUpdateViaGitHubRelease;
using ShaderForm2.WPFTools;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Zenseless.Patterns;

namespace ShaderForm2
{
	internal class UpdateViewModel : NotifyPropertyChanged
	{
		public UpdateViewModel()
		{
			Update update = new();
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
		}

		public bool Available { get => _available; private set => Set(ref _available, value, _ => CommandManager.InvalidateRequerySuggested()); }
		public ICommand CommandUpdate { get; }

		private bool _available;
	}
}
