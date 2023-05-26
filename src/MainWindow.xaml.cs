using OpenTK.Mathematics;
using OpenTK.Wpf;
using System;
using System.Linq;
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
			SetupOpenTK();
			_viewModel = new MainViewModel();
			DataContext = _viewModel;
			_viewModel.PropertyChanged += (s, e) =>
			{
				OpenTkControl.RenderContinuously = _viewModel.IsRunning;
				if (e.PropertyName == nameof(MainViewModel.ShowMenu))
				{
					if (_viewModel.ShowMenu)
					{
						menuTray.Height = double.NaN; // NaN to reset the height (auto height)
					}
					else
					{
						menuTray.Height = 0.0;
					}
				}
				OpenTkControl.InvalidateVisual();
			};
			_viewModel.Camera.PropertyChanged += (s, e) => OpenTkControl.InvalidateVisual();
			InputManager.Current.EnterMenuMode += Current_EnterMenuMode;
			InputManager.Current.LeaveMenuMode += Current_LeaveMenuMode;

			Persist.Configure(this, _viewModel);
			var args = Environment.GetCommandLineArgs().Skip(1);
			if (args.Any())
			{
				_viewModel.CurrentFile = args.First();
			}
		}

		private void Current_LeaveMenuMode(object? sender, EventArgs e)
		{
			if (!_viewModel.ShowMenu)
			{
				menuTray.Height = 0; // 0 to hide the menu
			}
		}

		private void Current_EnterMenuMode(object? sender, EventArgs e)
		{
			menuTray.Height = double.NaN;
		}

		private void SetupOpenTK()
		{
			GLWpfControlSettings settings = new()
			{
				MajorVersion = 4,
				MinorVersion = 5,
				GraphicsProfile = OpenTK.Windowing.Common.ContextProfile.Compatability,
				GraphicsContextFlags = OpenTK.Windowing.Common.ContextFlags.Default,
				RenderContinuously = false,
				UseDeviceDpi = true,

			};
			OpenTkControl.Start(settings);
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
			OpenTkControl.InvalidateVisual();
		}

		private void OpenTkControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			_viewModel.Resize(OpenTkControl.FrameBufferWidth, OpenTkControl.FrameBufferHeight);
		}
		private void Window_DragEnter(object sender, DragEventArgs dragInfo)
		{
			var validFormat = dragInfo.Data.GetDataPresent(DataFormats.FileDrop, true);
			dragInfo.Effects = validFormat ? DragDropEffects.All : DragDropEffects.None;
		}

		private void Window_Drop(object sender, DragEventArgs dragInfo)
		{
			string[] fileNames = (string[])dragInfo.Data.GetData(DataFormats.FileDrop, true);
			if (fileNames is null) return;
			foreach (string fileName in fileNames)
			{
				_viewModel.CurrentFile = fileName;
			}
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.IsRepeat) return;
			switch (e.Key)
			{
				case Key.Escape: Close(); break;
				case Key.Space: _viewModel.IsRunning = !_viewModel.IsRunning; break;
			}
			_viewModel.StartMovement(e.Key);
			OpenTkControl.InvalidateVisual();
		}

		private void Window_KeyUp(object sender, KeyEventArgs e)
		{
			//switch (e.Key)
			//{
			//	case Key.System:
			//		if(!InputManager.Current.IsInMenuMode)
			//			menuTray.Height = 0 == menuTray.Height ? double.NaN : 0;
			//		break;
			//}
			_viewModel.StopMovement(e.Key);
			OpenTkControl.InvalidateVisual();
		}

		private void ResetCameraButton_Click(object sender, RoutedEventArgs e)
		{
			_viewModel.Camera.Heading = 0f;
			_viewModel.Camera.Tilt = 0f;
			_viewModel.Camera.Position = Vector3.Zero;
			OpenTkControl.InvalidateVisual();
		}
	}
}
