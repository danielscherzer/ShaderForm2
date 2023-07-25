using System.Windows;
using System.Windows.Controls;
using Zenseless.PersistentSettings;

namespace ShaderForm2;

internal static class Persist
{
	internal static void Configure(MainWindow window, MainViewModel mainViewModel)
	{
		PersistentSettings settings = new();
		settings.AddFromProperty(() => window.Left);
		settings.AddFromProperty(() => window.Top);
		settings.AddFromProperty(() => window.Height);
		settings.AddFromProperty(() => window.Width);
		GridLengthConverter converter = new();
		void TryRead(ColumnDefinition colDef, string value)
		{
			var newValue = converter.ConvertFromString(value);
			if (newValue is GridLength gl) colDef.Width = gl;
		}

		string ToString(GridLength value) => converter.ConvertToString(value) ?? string.Empty;
		settings.AddFromGetterSetter("LeftColumn", () => ToString(window.LeftColumn.Width), value => TryRead(window.LeftColumn, value));
		settings.AddFromGetterSetter("RightColumn", () => ToString(window.RightColumn.Width), value => TryRead(window.RightColumn, value));

		settings.AddFromProperty(() => mainViewModel.CurrentFile);
		settings.AddFromProperty(() => mainViewModel.RecentlyUsed);
		settings.AddFromProperty(() => mainViewModel.TopMost);
		settings.AddFromProperty(() => mainViewModel.ShowMenu);

		settings.Load();
		window.Closing += (_, __) => settings.Store();
	}
}
