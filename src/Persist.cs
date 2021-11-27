using Jot;
#if !DEBUG
using Jot.Storage;
#endif
using System.Windows;

namespace ShaderForm2
{
	internal static class Persist
	{
#if DEBUG
		public static Tracker Tracker { get; } = new();
#else
		public static Tracker Tracker { get; } = new(new JsonFileStore("./"));
#endif

		internal static void Configure(Window window, MainViewModel mainViewModel)
		{
			_ = Tracker.Configure<Window>().Id(w => "Window", SystemParameters.PrimaryScreenWidth)
				.Property(w => w.Left)
				.Property(w => w.Top)
				.Property(w => w.Width)
				.Property(w => w.Height)
				.WhenPersistingProperty((wnd, property) => property.Cancel = WindowState.Normal != wnd.WindowState)
				.PersistOn(nameof(Window.Closing));
			Tracker.Track(window);

			_ = Tracker.Configure<MainViewModel>().Id(vm => nameof(MainViewModel))
				.Property(vm => vm.CurrentFile, "")
				.Property(vm => vm.RecentlyUsed)
				.PersistOn(nameof(Window.Closing), window);
			Tracker.Track(mainViewModel);
			
			//window.Closing += (_, args) =>
			//{

			//};
		}
	}
}
