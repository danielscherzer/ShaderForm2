using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.ComponentModel;
using System.IO;
using Zenseless.OpenTK;
using Zenseless.Patterns.Property;
using Zenseless.Resources;

namespace ShaderForm2;

internal class ShaderViewModel : NotifyPropertyChanged
{
	public ShaderViewModel()
	{
		_shaderProgram = LoadFragmentShader(_defaultFragmentSource) ?? throw new ApplicationException("Could not load default shader.");
		ResolveDefaultUniformLocation(_shaderProgram);
	}

	public bool Load(string filePath)
	{
		if (!File.Exists(filePath)) return false;
		var fragmentSource = File.ReadAllText(filePath);
		var dir = Path.GetDirectoryName(filePath) ?? throw new ApplicationException("Shader file path without directory information.");
		fragmentSource = GLSLhelper.Transformation.ExpandIncludes(fragmentSource, fileName => File.ReadAllText(Path.Combine(dir, fileName)));
		try
		{
			var newShaderProgram = LoadFragmentShader(fragmentSource);
			_shaderProgram.Dispose();
			if (newShaderProgram is null)
			{
				_shaderProgram = LoadFragmentShader(_defaultFragmentSource) ?? throw new ApplicationException("Could not load default shader.");
			}
			else
			{
				_shaderProgram = newShaderProgram;
			}
			ResolveDefaultUniformLocation(_shaderProgram);
			Log = string.Empty;
		}
		catch (ShaderException e)
		{
			//TODO: write shader log
			Log = e.Message;
		}
		return true;
	}

	[Description("Log of shader compilation/link")]
	public string Log { get => log; set => Set(ref log, value); }

	[Description("Left-handed coordinate system with the z-axis pointing in the view direction")]
	public Vector3 CamPos { get => camPos; set => Set(ref camPos, value); }

	[Description("Left-handed coordinate system with the z-axis pointing in the view direction")]
	public Vector3 CamRot { get => camRot; set => Set(ref camRot, value); }

	[Description("Mouse.X\nMouse.Y\nMouse.Z: 1 == left button, 2 == middle button, 3 == right button")]
	public Vector3 Mouse { get => _mouse; set => Set(ref _mouse, value); }

	[Description("Time in seconds")]
	public float Time { get => _time; set => Set(ref _time, MathF.Max(0f, value)); }

	[Description("Viewport resolution in pixels")]
	public Vector2 Resolution { get => _resolution; set => Set(ref _resolution, Vector2.ComponentMax(value, Vector2.One)); }

	internal void Render()
	{
		if (-1 != _locResolution) _shaderProgram.Uniform(_locResolution, _resolution);
		if (-1 != _locMouse) _shaderProgram.Uniform(_locMouse, Mouse.Xy);
		if (-1 != _locMouseButton) _shaderProgram.Uniform(_locMouse, Mouse);
		UniformF(_locTime, Time);

		UniformV3(_locCamPos, CamPos);
		UniformV3(_locCamRot, CamRot);

		_shaderProgram.Bind();
		GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
	}

	private void UniformF(int location, float value)
	{
		if (-1 != location) _shaderProgram.Uniform(location, value);
	}

	private void UniformV3(int location, Vector3 value)
	{
		if (-1 != location) _shaderProgram.Uniform(location, value);
	}

	private static readonly EmbeddedResourceDirectory _dir = new(nameof(ShaderForm2) + ".content");
	private static readonly string _defaultVertexSource = _dir.Resource("screenQuad.vert").AsString();
	private static readonly string _defaultFragmentSource = _dir.Resource("checker.frag").AsString();

	private Vector3 camPos;
	private Vector3 camRot;
	private int _locResolution = -1;
	private int _locTime = -1;
	private int _locMouse = -1;
	private int _locMouseButton = -1;
	//TODO: Use vector uniform for camera next semester
	private int _locCamPos = -1;
	private int _locCamRot = -1;
	private Vector3 _mouse;
	private ShaderProgram _shaderProgram;
	private Vector2 _resolution = Vector2.One;
	private float _time;
	private string log = "";

	private static ShaderProgram LoadFragmentShader(string fragmentSource)
	{
		(ShaderType, string)[] shaderInput = new[]
		{
			(ShaderType.VertexShader, _defaultVertexSource),
			(ShaderType.FragmentShader, fragmentSource)
		};
		return new ShaderProgram().CompileLink(shaderInput);
	}

	private void ResolveDefaultUniformLocation(ShaderProgram shaderProgram)
	{
		int GetLocation(string name) => GL.GetUniformLocation(shaderProgram.Handle, name);

		_locResolution = GetLocation("u_resolution");
		if (-1 == _locResolution) _locResolution = GetLocation("iResolution");

		_locTime = GetLocation("u_time");
		if (-1 == _locTime) _locTime = GetLocation("iGlobalTime");
		if (-1 == _locTime) _locTime = GetLocation("iTime");

		_locMouse = GetLocation("u_mouse");
		_locMouseButton = GetLocation("iMouse");

		_locCamPos = GetLocation("iCamPos");
		_locCamRot = GetLocation("iCamRot");
	}
}
