using System.IO;
using Zenseless.Patterns;

namespace ShaderForm2
{
	internal class ShaderViewModel : NotifyPropertyChanged
	{
		private string _filePath = "";

		public ShaderViewModel(string filePath = "")
		{
			if (string.IsNullOrEmpty(filePath))
			{

			}
			else
			{
				FilePath = filePath;
				var sourceCode = File.ReadAllText(filePath);
			}
		}

		public string FilePath { get => _filePath; private set => Set(ref _filePath, value); }

		internal void Resize(int frameBufferWidth, int frameBufferHeight)
		{
			//throw new NotImplementedException();
		}

		internal void Render(float frameTime)
		{
			//throw new NotImplementedException();
		}
	}
}
