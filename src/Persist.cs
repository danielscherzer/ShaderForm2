using Jot;
#if !DEBUG
using Jot.Storage;
#endif
using System.Windows;
using System.Windows.Controls;

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
			GridLengthConverter converter = new();
			_ = Tracker.Configure<MainWindow>().Id(w => nameof(MainWindow), SystemParameters.PrimaryScreenWidth)
				.Property(w => w.Left)
				.Property(w => w.Top)
				.Property(w => w.Width)
				.Property(w => w.Height)
				//.Property(w => w.LeftColumn.Width, "LeftColumn")
				//.Property(w => w.RightColumn)
				//.WhenPersistingProperty((f, p) => 
				//{
				//	if(p.Property.Contains("Column")) p.Value = converter.ConvertToString(((ColumnDefinition)p.Value).Width);
				//})
				//.WhenApplyingProperty((f, p) =>
				//{
				//	if(p.Property.Contains("Column")) ((ColumnDefinition)p.Value).Width = (GridLength)converter.ConvertFromString((string)p.Value);
				//})
				.WhenPersistingProperty((wnd, property) => property.Cancel = WindowState.Normal != wnd.WindowState)
				.PersistOn(nameof(Window.Closing));
			Tracker.Track(window);

			_ = Tracker.Configure<MainViewModel>().Id(vm => nameof(MainViewModel))
				.Property(vm => vm.CurrentFile, "")
				.Property(vm => vm.RecentlyUsed)
				.Property(vm => vm.TopMost)
				.PersistOn(nameof(Window.Closing), window);
			Tracker.Track(mainViewModel);
		}
	}
}
