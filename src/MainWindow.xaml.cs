using OpenTK.Wpf;
using System;
using System.Windows;
using System.Windows.Input;

namespace ShaderForm2
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly MainViewModel _viewModel;

		public MainWindow()
		{
			InitializeComponent();
			GLWpfControlSettings settings = new()
			{
				MajorVersion = 4,
				MinorVersion = 5,
				GraphicsProfile = OpenTK.Windowing.Common.ContextProfile.Compatability,
			};
			OpenTkControl.Start(settings);

			_viewModel = new MainViewModel();
			DataContext = _viewModel;
			Persist.Configure(this, _viewModel);
		}

		private void OpenTkControl_OnRender(TimeSpan delta)
		{
			_viewModel.Render((float)delta.TotalSeconds);
		}

		private void OpenTkControl_MouseDown(object sender, MouseButtonEventArgs e)
		{

		}

		private void OpenTkControl_MouseMove(object sender, MouseEventArgs e)
		{
			//OpenTkControl.InvalidateVisual();
			//var position = e.GetPosition(OpenTkControl);
			//Debug.WriteLine(position);
		}

		private void OpenTkControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			_viewModel.Resize(OpenTkControl.FrameBufferWidth, OpenTkControl.FrameBufferHeight);
		}

		private void OpenTkControl_KeyDown(object sender, KeyEventArgs e)
		{

		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape) window.Close();
		}

		private void Window_DragOver(object sender, DragEventArgs dragInfo)
		{
			dragInfo.Effects = DragDropEffects.Link;
			dragInfo.Handled = true;
		}

		private void Window_Drop(object sender, DragEventArgs dragInfo)
		{
			string[] fileNames = (string[])dragInfo.Data.GetData(DataFormats.FileDrop);
			foreach (string fileName in fileNames)
			{
				_viewModel.CurrentFile = fileName;
			}
		}
	}
}
