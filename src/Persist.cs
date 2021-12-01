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

		internal static void Configure(MainWindow window, MainViewModel mainViewModel)
		{
			//_ = Tracker.Configure<MainWindow>().Id(w => nameof(MainWindow), SystemParameters.PrimaryScreenWidth)
			//	.WhenPersistingProperty((wnd, property) => property.Cancel = WindowState.Normal != wnd.WindowState)
			//Tracker.Track(window);

			_ = Tracker.Configure<MainViewModel>().Id(vm => nameof(MainViewModel))
				.Property(vm => vm.CurrentFile, "")
				.Property(vm => vm.RecentlyUsed)
				.Property(vm => vm.TopMost)
				.PersistOn(nameof(Window.Closing), window);
			Tracker.Track(mainViewModel);

			PersistentSettings settings = new();

			settings.AddProperty("left", () => window.Left, value => window.Left = value);
			settings.AddProperty("top", () => window.Top, value => window.Top = value);
			settings.AddProperty("Height", () => window.Height, value => window.Height = value);
			settings.AddProperty("Width", () => window.Width, value => window.Width = value);
			GridLengthConverter converter = new();
			settings.AddProperty("LeftColumn", () => converter.ConvertToString(window.LeftColumn.Width), value => window.LeftColumn.Width = (GridLength)converter.ConvertFromString(value));
			settings.AddProperty("RightColumn", () => converter.ConvertToString(window.RightColumn.Width), value => window.RightColumn.Width = (GridLength)converter.ConvertFromString(value));

			//settings.AddProperty(() => mainViewModel.CurrentFile);

			settings.Load();
			window.Closing += (_, __) =>
			{
				settings.Store();
			};
		}
	}
}
