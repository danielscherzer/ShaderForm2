using OpenTK.Mathematics;
using Zenseless.Patterns;

namespace ShaderForm2
{
	public class FirstPersonCamera : NotifyPropertyChanged
	{
		private float heading;
		private Vector3 position;
		private float tilt;

		public float Heading { get => heading; set => Set(ref heading, value); }
		public Vector3 Position { get => position; set => Set(ref position, value); }
		public float Tilt { get => tilt; set => Set(ref tilt, value); }

		public void MoveLocal(Vector3 movement)
		{
			var invRotation = Matrix3.Transpose(CalcRotationMatrix());
			Position += Vector3.TransformColumn(invRotation, movement);
		}

		private Matrix3 CalcRotationMatrix()
		{
			var heading = OpenTK.Mathematics.MathHelper.DegreesToRadians(Heading);
			var tilt = OpenTK.Mathematics.MathHelper.DegreesToRadians(Tilt);
			var rotX = Matrix3.CreateRotationX(tilt);
			var rotY = Matrix3.CreateRotationY(heading);
			var mtxRotate = rotY * rotX;
			return mtxRotate;
		}

		//private Matrix4 CalcViewMatrix()
		//{
		//	var mtxTranslate = Matrix4.CreateTranslation(-Position);
		//	var mtxRotate = CalcRotationMatrix();
		//	return mtxTranslate * mtxRotate;
		//}
	}
}