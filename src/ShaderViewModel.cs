using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
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
			EmbeddedResourceDirectory _dir = new(nameof(ShaderForm2) + ".content");
			_defaultVertexSource = _dir.Resource("screenQuad.vert").OpenText();
			_defaultFragmentSource = _dir.Resource("checker.frag").OpenText();
			_shaderProgram = LoadFragmentShader(_defaultFragmentSource) ?? throw new Exception("Could not load default shader!");
		}

		public void Load(string filePath)
		{
			var fragmentSource = File.ReadAllText(filePath);
			var newShaderProgram = LoadFragmentShader(fragmentSource);
			if(newShaderProgram is not null)
			{
				_shaderProgram.Dispose();
				_shaderProgram = newShaderProgram;
				FilePath = filePath;
			}
		}

		public string FilePath { get => _filePath; private set => Set(ref _filePath, value); }
		public float Time { get => _time; private set => Set(ref _time, value); }

		internal void Resize(int frameBufferWidth, int frameBufferHeight)
		{
			_resolution = new Vector2(frameBufferWidth, frameBufferHeight);
		}

		internal void Render(float frameTime)
		{
			Time += frameTime;
			_shaderProgram.Bind();
			_shaderProgram.Uniform("u_resolution", _resolution);
			_shaderProgram.Uniform("u_time", Time);
			GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
		}

		private string _filePath = "";
		private ShaderProgram _shaderProgram;
		private Vector2 _resolution = Vector2.One;
		private float _time;
		private readonly string _defaultFragmentSource;
		private readonly string _defaultVertexSource;

		private ShaderProgram? LoadFragmentShader(string fragmentSource)
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
	}
}
