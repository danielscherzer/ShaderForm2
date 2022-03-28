using System.Windows;

namespace ShaderForm2
{
	internal static class Persist
	{
//#if DEBUG
//		public static Tracker Tracker { get; } = new();
//#else
//		public static Tracker Tracker { get; } = new(new JsonFileStore("./"));
//#endif

		internal static void Configure(MainWindow window, MainViewModel mainViewModel)
		{
			//_ = Tracker.Configure<MainWindow>().Id(w => nameof(MainWindow), SystemParameters.PrimaryScreenWidth)
			//	.WhenPersistingProperty((wnd, property) => property.Cancel = WindowState.Normal != wnd.WindowState)
			//Tracker.Track(window);

			PersistentSettings settings = new();

			settings.AddFromProperty(() => window.Left);
			settings.AddFromProperty(() => window.Top);
			settings.AddFromProperty(() => window.Height);
			settings.AddFromProperty(() => window.Width);
			GridLengthConverter converter = new();
			settings.AddFromGetterSetter("LeftColumn", () => converter.ConvertToString(window.LeftColumn.Width), value => window.LeftColumn.Width = (GridLength)converter.ConvertFromString(value));
			settings.AddFromGetterSetter("RightColumn", () => converter.ConvertToString(window.RightColumn.Width), value => window.RightColumn.Width = (GridLength)converter.ConvertFromString(value));
			settings.AddFromGetterSetter("RecentlyUsed", () => mainViewModel.RecentlyUsed, value => mainViewModel.RecentlyUsed = value);

			settings.AddFromProperty(() => mainViewModel.CurrentFile);
			//settings.AddFromProperty(() => mainViewModel.RecentlyUsed);
			settings.AddFromProperty(() => mainViewModel.TopMost);

			settings.Load();
			window.Closing += (_, __) =>
			{
				settings.Store();
			};
		}
	}
}
