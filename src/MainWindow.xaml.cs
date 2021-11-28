using OpenTK.Mathematics;
using OpenTK.Wpf;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

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
				UseDeviceDpi = true,
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

		private void OpenTkControl_Mouse(object sender, MouseEventArgs e)
		{
			DpiScale result = VisualTreeHelper.GetDpi(this);
			Matrix scale = Matrix.Identity;
			scale.Scale(result.DpiScaleX, result.DpiScaleY);
			Point position = scale.Transform(e.GetPosition(OpenTkControl));

			static bool Pressed(MouseButtonState state) => MouseButtonState.Pressed == state;
			int button = Pressed(e.LeftButton) ? 1 : (Pressed(e.RightButton) ? 3 : (Pressed(e.MiddleButton) ? 2 : 0));
			_viewModel.SetMouse(position.X, position.Y, button);
		}

		private void OpenTkControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			_viewModel.Resize(OpenTkControl.FrameBufferWidth, OpenTkControl.FrameBufferHeight);
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

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			_viewModel.IsRunning = !_viewModel.IsRunning;
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Escape: Close(); break;
			}
			_viewModel.StartMovement(e.Key);
		}

		private void Window_KeyUp(object sender, KeyEventArgs e)
		{
			_viewModel.StopMovement(e.Key);
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			_viewModel.Camera.Heading = 0f;
			_viewModel.Camera.Tilt = 0f;
			_viewModel.Camera.Position = Vector3.Zero;
		}
	}
}
