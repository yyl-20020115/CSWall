using System.Collections.Generic;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Drawing;
using GraphAlgorithmTester.Colors;

namespace CSWall;

public class ParticleSystem
{
    private readonly List<Particle> particles;
    private readonly GeometryModel3D model;
    private int XParticleCount = 30;
    private int YParticleCount = 30;
    private int Size = 10;
    public Model3D ParticleModel => model;
    public ParticleSystem(System.Windows.Media.Color color,int amountX = 30, int amountY=30)
    {
        XParticleCount = amountX;
        YParticleCount = amountY;

        particles = new List<Particle>();
        model = new GeometryModel3D { Geometry = new MeshGeometry3D() };
        var material = new DiffuseMaterial(new SolidColorBrush(color));
        model.Material = material;
    }
    public void SpawnParticle(Bitmap bitmap)
    {
        XParticleCount = bitmap.Width;
        YParticleCount = bitmap.Height;
        var halfWidth = YParticleCount * Size>>1;
        var halfHeight = YParticleCount * Size>>1;
        var halfSize = Size >> 1;
        // 初始化粒子位置和大小
        for (int ix = 0; ix < XParticleCount; ix++)
        {
            for (int iy = 0; iy < YParticleCount; iy++)
            {
                var c = bitmap.GetPixel(ix, iy);
                var p = new Particle
                {
                    Position = new (-halfWidth - halfSize + ix * Size,-halfHeight - halfSize + iy * Size, 0),
                    Thickness = Size,
                    Height = Convert.GetColorValue(c)*256.0,//c.R=c.G=c.B
                };
                particles.Add(p);
            }
        }
    }

    public void UpdateGeometry()
    {
        var positions = new Point3DCollection();
        var indices = new Int32Collection();

        for (var i = 0; i < particles.Count; ++i)
        {
            var positionIndex = i << 3;
            var particle = particles[i];

            //0,0,0
            var p0 = new Point3D(particle.Position.X, particle.Position.Y, particle.Position.Z);
            //1,0,0
            var p1 = new Point3D(particle.Position.X + particle.Thickness, particle.Position.Y, particle.Position.Z);
            //1,1,0
            var p2 = new Point3D(particle.Position.X + particle.Thickness, particle.Position.Y + particle.Thickness, particle.Position.Z);
            //0,1,0
            var p3 = new Point3D(particle.Position.X, particle.Position.Y + particle.Thickness, particle.Position.Z);
            //0,0,1
            var p4 = new Point3D(particle.Position.X, particle.Position.Y, particle.Position.Z + particle.Height);
            //1,0,1
            var p5 = new Point3D(particle.Position.X + particle.Thickness, particle.Position.Y, particle.Position.Z + particle.Height);
            //1,1,1
            var p6 = new Point3D(particle.Position.X + particle.Thickness, particle.Position.Y + particle.Thickness, particle.Position.Z + particle.Height);
            //0,1,1
            var p7 = new Point3D(particle.Position.X, particle.Position.Y + particle.Thickness, particle.Position.Z + particle.Height);

            positions.Add(p0);//0,0,0 -> 0
            positions.Add(p1);//1,0,0 -> 1
            positions.Add(p2);//1,1,0 -> 2
            positions.Add(p3);//0,1,0 -> 3
            positions.Add(p4);//0,0,1 -> 4
            positions.Add(p5);//1,0,1 -> 5
            positions.Add(p6);//1,1,1 -> 6
            positions.Add(p7);//0,1,1 -> 7

            indices.Add(positionIndex + 0);
            indices.Add(positionIndex + 1);
            indices.Add(positionIndex + 3);

            indices.Add(positionIndex + 1);
            indices.Add(positionIndex + 2);
            indices.Add(positionIndex + 3);

            indices.Add(positionIndex + 0);
            indices.Add(positionIndex + 3);
            indices.Add(positionIndex + 4);

            indices.Add(positionIndex + 4);
            indices.Add(positionIndex + 3);
            indices.Add(positionIndex + 7);

            indices.Add(positionIndex + 4);
            indices.Add(positionIndex + 6);
            indices.Add(positionIndex + 7);

            indices.Add(positionIndex + 4);
            indices.Add(positionIndex + 5);
            indices.Add(positionIndex + 6);

            indices.Add(positionIndex + 0);
            indices.Add(positionIndex + 4);
            indices.Add(positionIndex + 1);

            indices.Add(positionIndex + 1);
            indices.Add(positionIndex + 4);
            indices.Add(positionIndex + 5);

            indices.Add(positionIndex + 1);
            indices.Add(positionIndex + 2);
            indices.Add(positionIndex + 6);
            indices.Add(positionIndex + 6);
            indices.Add(positionIndex + 5);
            indices.Add(positionIndex + 1);
            indices.Add(positionIndex + 2);
            indices.Add(positionIndex + 3);
            indices.Add(positionIndex + 7);
            indices.Add(positionIndex + 7);
            indices.Add(positionIndex + 6);
            indices.Add(positionIndex + 2);
        }

        (model.Geometry as MeshGeometry3D)!.Positions = positions;
        (model.Geometry as MeshGeometry3D)!.TriangleIndices = indices;
    }
}
