using OpenTK.Mathematics;
using System.ComponentModel;
using Zenseless.Patterns.Property;

namespace ShaderForm2.Tools;

public class FirstPersonCamera : NotifyPropertyChanged
{
    private float heading;
    private Vector3 position;
    private float tilt;

    [Description("Heading in degrees")]
    public float Heading { get => heading; set => Set(ref heading, value); }

    [Description("left-right: keys:A/D\nbackward-forward: keys:S/W\ndown-up: keys:Q/E\n")]
    public Vector3 Position { get => position; set => Set(ref position, value); }

    [Description("Tilt in degrees")]
    public float Tilt { get => tilt; set => Set(ref tilt, value); }

    public void MoveLocal(Vector3 movement)
    {
        Matrix3 rotation = CalcRotationMatrix();
        Position += Vector3.TransformRow(movement, rotation);
    }

    private Matrix3 CalcRotationMatrix()
    {
        float heading = MathHelper.DegreesToRadians(Heading);
        float tilt = MathHelper.DegreesToRadians(Tilt);
        Matrix3 rotX = Matrix3.CreateRotationX(-tilt);
        Matrix3 rotY = Matrix3.CreateRotationY(heading);
        return rotX * rotY;
    }
}