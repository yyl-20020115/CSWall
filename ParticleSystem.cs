using System.Collections.Generic;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Drawing;
using GraphAlgorithmTester.Colors;

namespace CSWall;

public class ParticleSystem
{
    private readonly List<Particle> particles = new();
    public ModelVisual3D WorldVisual = new() { Content = new Model3DGroup() };
    public Model3DGroup? WorldModels => WorldVisual.Content as Model3DGroup;

    public int BoxEdgeWidth { get; set; } = 8;
    public int BoxHeight { get; set; } = 512;

    public ParticleSystem()
    {
    }
    public void SpawnParticle(Bitmap bitmap)
    {
        this.particles.Clear();
        this.WorldModels?.Children.Clear();
        var width = bitmap.Width;
        var height = bitmap.Height;
        var halfWidth = width >> 1;
        var halfHeight = height >> 1;
        // 初始化粒子位置和大小
        for (int ix = 0; ix < width; ix++)
        {
            for (int iy = 0; iy < height; iy++)
            {
                var c = bitmap.GetPixel(ix, iy);
                var v = Convert.GetColorValue(c);
                var p = new Particle
                {
                    Position = new(
                        (halfWidth - ix) * BoxEdgeWidth,
                        (iy-halfHeight) * BoxEdgeWidth,
                         0),
                    Thickness = BoxEdgeWidth,
                    Height = /*v == 0 ? 1 : */v * BoxHeight,//c.R=c.G=c.B
                    Color = System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B)
                };
                particles.Add(p);
            }
        }
        this.UpdateGeometry();
    }

    protected void UpdateGeometry()
    {
        var myDirectionalLight = new AmbientLight
        {
            Color = Colors.White,
        };
        var worldModels = WorldModels;
        if (worldModels != null)
        {
            var collection = new Model3DCollection(); ;
            collection.Add(myDirectionalLight);

            for (var i = 0; i < particles.Count; ++i)
            {
                var particle = particles[i];
                var geometry = new MeshGeometry3D();
                var color = particle.Color;
                var brush = new SolidColorBrush(color);
                var material = new DiffuseMaterial(
                    brush);

                var model3D = new GeometryModel3D(geometry, material);
                var positions = new Point3DCollection();
                var indices = new Int32Collection();

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

                var positionIndex = 0;// i << 3;
                //BOTTOM (Z=0)
                indices.Add(positionIndex + 3);
                indices.Add(positionIndex + 1);
                indices.Add(positionIndex + 0);

                indices.Add(positionIndex + 3);
                indices.Add(positionIndex + 2);
                indices.Add(positionIndex + 1);

                //Right
                indices.Add(positionIndex + 4);
                indices.Add(positionIndex + 3);
                indices.Add(positionIndex + 0);

                indices.Add(positionIndex + 7);
                indices.Add(positionIndex + 3);
                indices.Add(positionIndex + 4);

                //TOP
                indices.Add(positionIndex + 4);
                indices.Add(positionIndex + 6);
                indices.Add(positionIndex + 7);

                indices.Add(positionIndex + 4);
                indices.Add(positionIndex + 5);
                indices.Add(positionIndex + 6);
                //
                indices.Add(positionIndex + 1);
                indices.Add(positionIndex + 4);
                indices.Add(positionIndex + 0);

                indices.Add(positionIndex + 5);
                indices.Add(positionIndex + 4);
                indices.Add(positionIndex + 1);

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

                geometry.Positions = positions;
                geometry.TriangleIndices = indices;

                collection.Add(model3D);
            }
            worldModels.Children = collection;
        }
    }
}
