using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.ComponentModel;
using System.IO;
using Zenseless.OpenTK;
using Zenseless.Patterns;
using Zenseless.Resources;

namespace ShaderForm2
{
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
			return true;
		}

		[Description("Left-handed coordinate system with the z-axis pointing in the view direction")]
		public float CamPosX { get => camPosX; set => Set(ref camPosX, value); }
		
		[Description("Left-handed coordinate system with the z-axis pointing in the view direction")]
		public float CamPosY { get => camPosY; set => Set(ref camPosY, value); }
		
		[Description("Left-handed coordinate system with the z-axis pointing in the view direction")]
		public float CamPosZ { get => camPosZ; set => Set(ref camPosZ, value); }

		[Description("Left-handed coordinate system with the z-axis pointing in the view direction")]
		public float CamRotX { get => camRotX; set => Set(ref camRotX, value); }
		
		[Description("Left-handed coordinate system with the z-axis pointing in the view direction")]
		public float CamRotY { get => camRotY; set => Set(ref camRotY, value); }
		
		[Description("Left-handed coordinate system with the z-axis pointing in the view direction")]
		public float CamRotZ { get => camRotZ; set => Set(ref camRotZ, value); }

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

			UniformF(_locCamPosX, CamPosX);
			UniformF(_locCamPosY, CamPosY);
			UniformF(_locCamPosZ, CamPosZ);

			UniformF(_locCamRotX, CamRotX);
			UniformF(_locCamRotY, CamRotY);
			UniformF(_locCamRotZ, CamRotZ);
			_shaderProgram.Bind();
			GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
		}

		private void UniformF(int location, float value)
		{
			if (-1 != location) _shaderProgram.Uniform(location, value);
		}

		private static readonly EmbeddedResourceDirectory _dir = new(nameof(ShaderForm2) + ".content");
		private static readonly string _defaultVertexSource = _dir.Resource("screenQuad.vert").OpenText();
		private static readonly string _defaultFragmentSource = _dir.Resource("checker.frag").OpenText();

		private float camPosX;
		private float camPosY;
		private float camPosZ;
		private float camRotX;
		private float camRotY;
		private float camRotZ;
		private int _locResolution = -1;
		private int _locTime = -1;
		private int _locMouse = -1;
		private int _locMouseButton = -1;
		//TODO: Use vector uniform for camera next semester
		private int _locCamPosX = -1;
		private int _locCamPosY = -1;
		private int _locCamPosZ = -1;
		private int _locCamRotX = -1;
		private int _locCamRotY = -1;
		private int _locCamRotZ = -1;
		private Vector3 _mouse;
		private ShaderProgram _shaderProgram;
		private Vector2 _resolution = Vector2.One;
		private float _time;

		private static ShaderProgram? LoadFragmentShader(string fragmentSource)
		{
			try
			{
				(ShaderType, string)[] shaderInput = new[]
				{
					(ShaderType.VertexShader, _defaultVertexSource),
					(ShaderType.FragmentShader, fragmentSource)
				};
				return new ShaderProgram().CompileLink(shaderInput);
			}
			catch
			{
				return null;
			}
		}

		private void ResolveDefaultUniformLocation(ShaderProgram shaderProgram)
		{
			int GetLocation(string name) => GL.GetUniformLocation(shaderProgram.Handle, name);

			_locResolution = GetLocation("u_resolution");
			if(-1 == _locResolution) _locResolution = GetLocation("iResolution");

			_locTime = GetLocation("u_time");
			if (-1 == _locTime) _locTime = GetLocation("iGlobalTime");
			if (-1 == _locTime) _locTime = GetLocation("iTime");

			_locMouse = GetLocation("u_mouse");
			_locMouseButton = GetLocation("iMouse");

			_locCamPosX = GetLocation("iCamPosX");
			_locCamPosY = GetLocation("iCamPosY");
			_locCamPosZ = GetLocation("iCamPosZ");

			_locCamRotX = GetLocation("iCamRotX");
			_locCamRotY = GetLocation("iCamRotY");
			_locCamRotZ = GetLocation("iCamRotZ");
		}
	}
}
