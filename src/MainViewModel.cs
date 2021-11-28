﻿using OpenTK.Mathematics;
using ShaderForm2.WPFTools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using Zenseless.OpenTK;
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

		public ShaderViewModel ShaderViewModel => shaderViewModel;
		public string CurrentFile
		{
			get => ShaderViewModel.FilePath;
			set
			{
				ShaderViewModel.Load(value);
				//TODO: remove Application.Current.Dispatcher in VM
				Application.Current.Dispatcher.Invoke(() => RecentlyUsed.Insert(0, value));
				IEnumerable<string> distinct = RecentlyUsed.Distinct();
				RecentlyUsed = new ObservableCollection<string>(distinct);
				_fileChangeSubscription?.Dispose();
				_fileChangeSubscription = TrackedFileObservable.DelayedLoad(value).ObserveOnDispatcher().Subscribe(fileName => ShaderViewModel.Load(fileName));
				RaisePropertyChanged();
			}
		}

		internal void SetMouse(double x, double y, int button)
		{
			x = x.Clamp(0, ShaderViewModel.Resolution.X - 1.0);
			y = ShaderViewModel.Resolution.Y - 1.0 - y.Clamp(0, ShaderViewModel.Resolution.Y - 1.0);
			ShaderViewModel.Mouse = new Vector3((int)x, (int)y, button);
		}

		public ObservableCollection<string> RecentlyUsed { get => _recentlyUsed; set => Set(ref _recentlyUsed, value/*, coll => BindingOperations.EnableCollectionSynchronization(coll, _lockObj)*/); }
		public bool IsRunning { get; internal set; }

		internal void Render(float frameTime)
		{
			if (IsRunning) ShaderViewModel.Time += frameTime;
			ShaderViewModel.Render();
		}

		internal void Resize(int frameBufferWidth, int frameBufferHeight) => ShaderViewModel.Resolution = new Vector2(frameBufferWidth, frameBufferHeight);

		private IDisposable? _fileChangeSubscription;
		private ObservableCollection<string> _recentlyUsed = new();
		private readonly ShaderViewModel shaderViewModel = new();
	}
}