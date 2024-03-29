﻿using OpenTK.Mathematics;
using ShaderForm2.Tools;
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
using Zenseless.Patterns.Property;

namespace ShaderForm2;

internal class MainViewModel : NotifyPropertyChanged
{
	public MainViewModel()
	{
		LoadCommand = new TypedDelegateCommand<string>(path => CurrentFile = path);
		//shaderViewModel.PropertyChanged += (_, __) => RaisePropertyChanged();
	}

	public FirstPersonCamera Camera { get; } = new();

	public ICommand LoadCommand { private set; get; }

	public ShaderViewModel ShaderViewModel => shaderViewModel;

	public string CurrentFile
	{
		get => _currentFile;
		set
		{
			if (string.IsNullOrEmpty(value)) return;
			bool exists = ShaderViewModel.Load(value);
			if (!exists) return;
			//TODO: remove Application.Current.Dispatcher in VM
			Application.Current.Dispatcher.Invoke(() => RecentlyUsed.Insert(0, value));
			IEnumerable<string> distinct = RecentlyUsed.Distinct();
			RecentlyUsed = new ObservableCollection<string>(distinct);
			_fileChangeSubscription?.Dispose();
			_fileChangeSubscription = TrackedFileObservable
				.DelayedLoad(value)
				.ObserveOnDispatcher()
				.Subscribe(fileName => { shaderViewModel.Load(fileName); RaisePropertyChanged(); });
			if (exists)
			{
				Set(ref _currentFile, value);
			}
		}
	}

	public bool IsRunning { get => _isRunning; set => Set(ref _isRunning, value); }
	public bool TopMost { get => _topMost; set => Set(ref _topMost, value); }
	public bool ShowMenu { get => _showMenu; set => Set(ref _showMenu, value); }
	//public bool ShowLog => shaderViewModel.Log != string.Empty; //TODO: notify on log change

	public ObservableCollection<string> RecentlyUsed { get => _recentlyUsed; set => Set(ref _recentlyUsed, value/*, coll => BindingOperations.EnableCollectionSynchronization(coll, _lockObj)*/); }

	internal void SetMouse(double x, double y, int button)
	{
		x = x.Clamp(0, ShaderViewModel.Resolution.X - 1.0);
		y = ShaderViewModel.Resolution.Y - 1.0 - y.Clamp(0, ShaderViewModel.Resolution.Y - 1.0);
		ShaderViewModel.Mouse = new Vector3((int)x, (int)y, button);
		//camera movement
		if (1 == button)
		{
			var diff = ShaderViewModel.Mouse.Xy - lastMouse;
			var delta = Vector2.Divide(diff, ShaderViewModel.Resolution);
			Camera.Heading += 300 * delta.X;
			Camera.Tilt += 300 * delta.Y;
		}
		lastMouse = ShaderViewModel.Mouse.Xy;
	}

	internal void StartMovement(Key key)
	{
		var speed = 1f;
		switch (key)
		{
			case Key.A: movement.X = -speed; break;
			case Key.D: movement.X = speed; break;
			case Key.Q: movement.Y = -speed; break;
			case Key.E: movement.Y = speed; break;
			case Key.W: movement.Z = speed; break;
			case Key.S: movement.Z = -speed; break;
		}
	}

	internal void StopMovement(Key key)
	{
		switch (key)
		{
			case Key.A: movement.X = 0f; break;
			case Key.D: movement.X = 0f; break;
			case Key.Q: movement.Y = 0f; break;
			case Key.E: movement.Y = 0f; break;
			case Key.W: movement.Z = 0f; break;
			case Key.S: movement.Z = 0f; break;
		}
	}

	internal void Render(float frameTime)
	{
		Camera.MoveLocal(movement * frameTime);
		ShaderViewModel.CamPos = Camera.Position;

		ShaderViewModel.CamRot = new Vector3(OpenTK.Mathematics.MathHelper.DegreesToRadians(Camera.Tilt), OpenTK.Mathematics.MathHelper.DegreesToRadians(Camera.Heading), 0f);

		if (IsRunning) ShaderViewModel.Time += frameTime;
		ShaderViewModel.Render();
	}

	internal void Resize(int frameBufferWidth, int frameBufferHeight) => ShaderViewModel.Resolution = new Vector2(frameBufferWidth, frameBufferHeight);

	private IDisposable? _fileChangeSubscription;
	private ObservableCollection<string> _recentlyUsed = new();
	private readonly ShaderViewModel shaderViewModel = new();
	private Vector3 movement;
	private Vector2 lastMouse;
	private bool _topMost;
	private bool _isRunning;
	private string _currentFile = string.Empty;
	private bool _showMenu = true;
	//TODO: private Movement movement = new();
}