using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using Zenseless.Patterns;

namespace ShaderForm2
{
	internal class MainViewModel : NotifyPropertyChanged
	{
		public MainViewModel()
		{
			LoadCommand = new TypedDelegateCommand<string>(path => CurrentFile = path);
		}

		public ICommand LoadCommand { private set; get; }

		public ShaderViewModel ShaderViewModel { get => _shaderViewModel; set => Set(ref _shaderViewModel, value); }

		public string CurrentFile
		{
			get => ShaderViewModel.FilePath; set
			{
				if (value == ShaderViewModel.FilePath) return; // no change
				ShaderViewModel = new ShaderViewModel(value);
				//TODO: remove Application.Current.Dispatcher in VM
				Application.Current.Dispatcher.Invoke(() => RecentlyUsed.Insert(0, value));
				IEnumerable<string> distinct = RecentlyUsed.Distinct();
				RecentlyUsed = new ObservableCollection<string>(distinct);
				_fileChangeSubscription?.Dispose();
				_fileChangeSubscription = TrackedFileObservable.DelayedLoad(value).Subscribe(fileName => CurrentFile = value);
				RaisePropertyChanged();
			}
		}

		public ObservableCollection<string> RecentlyUsed { get => _recentlyUsed; set => Set(ref _recentlyUsed, value/*, coll => BindingOperations.EnableCollectionSynchronization(coll, _lockObj)*/); }

		internal void Render(float frameTime)
		{
			ShaderViewModel?.Render(frameTime);
		}

		internal void Resize(int frameBufferWidth, int frameBufferHeight) => ShaderViewModel?.Resize(frameBufferWidth, frameBufferHeight);

		private IDisposable? _fileChangeSubscription;
		private ObservableCollection<string> _recentlyUsed = new();
		private ShaderViewModel _shaderViewModel = new();
	}
}