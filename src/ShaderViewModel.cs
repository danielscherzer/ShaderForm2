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
			_shaderProgram = LoadFragmentShader(_defaultFragmentSource) ?? throw new Exception("Could not load default shader!");
			ResolveDefaultUniformLocation(_shaderProgram);
		}

		public void Load(string filePath)
		{
			var fragmentSource = File.ReadAllText(filePath);
			var newShaderProgram = LoadFragmentShader(fragmentSource);
			if(newShaderProgram is not null)
			{
				_shaderProgram.Dispose();
				_shaderProgram = newShaderProgram;
				ResolveDefaultUniformLocation(_shaderProgram);
				FilePath = filePath;
			}
		}

		public string FilePath { get => _filePath; private set => Set(ref _filePath, value); }
		public float Time { get => _time; set => Set(ref _time, value); }

		internal void Resize(int frameBufferWidth, int frameBufferHeight)
		{
			_resolution = new Vector2(frameBufferWidth, frameBufferHeight);
		}

		internal void Render(float frameTime)
		{
			//Time += frameTime;
			if (-1 != locResolution) _shaderProgram.Uniform(locResolution, _resolution);
			if (-1 != locTime) _shaderProgram.Uniform(locTime, Time);
			_shaderProgram.Bind();
			GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
		}

		private static readonly EmbeddedResourceDirectory _dir = new(nameof(ShaderForm2) + ".content");
		private static readonly string _defaultVertexSource = _dir.Resource("screenQuad.vert").OpenText();
		private static readonly string _defaultFragmentSource = _dir.Resource("checker.frag").OpenText();

		private string _filePath = "";
		private ShaderProgram _shaderProgram;
		private Vector2 _resolution = Vector2.One;
		private float _time;
		private int locResolution = -1;
		private int locTime = -1;

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
			locResolution = GL.GetUniformLocation(shaderProgram.Handle, "u_resolution");
			if(-1 == locResolution) locResolution = GL.GetUniformLocation(shaderProgram.Handle, "iResolution");

			locTime = GL.GetUniformLocation(shaderProgram.Handle, "u_time");
			if (-1 == locTime) locTime = GL.GetUniformLocation(shaderProgram.Handle, "iGlobalTime");
			if (-1 == locTime) locTime = GL.GetUniformLocation(shaderProgram.Handle, "iTime");

			//visualContext.SetUniform("iMouse", mouseX, mouseY, mouseButton);
			//visualContext.SetUniform("u_mouse", mouseX, mouseY);
		}
	}
}
