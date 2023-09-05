using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Drawing;

namespace CSWall;

public class ParticleSystem
{
    private readonly List<Particle> particles;
    private readonly GeometryModel3D model;
    private readonly int CUBOIDHEIGHT = 10;
    private readonly int MOUSERADIUS = 1000;
    private int XParticleCount = 30;
    private int YParticleCount = 30;
    private int Size = 50;
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

        // 初始化粒子位置和大小
        for (int ix = 0; ix < XParticleCount; ix++)
        {
            for (int iy = 0; iy < YParticleCount; iy++)
            {
                var c = bitmap.GetPixel(ix, iy);
                var p = new Particle
                {
                    Position = new Point3D(ix * Size, iy * Size, 0),
                    Width = Size,
                    Height = c.R,//c.R=c.G=c.B
                };
                particles.Add(p);
            }
        }
    }

    public void SpawnParticle(double size)
    {
        // 初始化粒子位置和大小
        for (int ix = 0; ix < XParticleCount; ix++)
        {
            for (int iy = 0; iy < YParticleCount; iy++)
            {
                var p = new Particle
                {
                    Position = new Point3D(ix * size, iy * size, 0),
                    Width = size,
                    Height = CUBOIDHEIGHT,
                };
                particles.Add(p);
            }
        }
    }

    public void Update(System.Windows.Point mp)
    {
        foreach (var p in particles)
        {
            //求点到圆心的距离
            double c = Math.Pow(Math.Pow(mp.X - p.Position.X, 2) + Math.Pow(mp.Y - p.Position.Y, 2), 0.5);
            p.Height = (MOUSERADIUS / (c + CUBOIDHEIGHT)) * CUBOIDHEIGHT;
        }
        UpdateGeometry();
    }

    private void UpdateGeometry()
    {
        var positions = new Point3DCollection();
        var indices = new Int32Collection();

        for (var i = 0; i < particles.Count; ++i)
        {
            var positionIndex = i * 8;
            var particle = particles[i];

            var p1 = new Point3D(particle.Position.X, particle.Position.Y, particle.Position.Z);
            var p2 = new Point3D(particle.Position.X + particle.Width, particle.Position.Y, particle.Position.Z);
            var p3 = new Point3D(particle.Position.X + particle.Width, particle.Position.Y + particle.Width, particle.Position.Z);
            var p4 = new Point3D(particle.Position.X, particle.Position.Y + particle.Width, particle.Position.Z);
            var p5 = new Point3D(particle.Position.X, particle.Position.Y, particle.Position.Z + particle.Height);
            var p6 = new Point3D(particle.Position.X + particle.Width, particle.Position.Y, particle.Position.Z + particle.Height);
            var p7 = new Point3D(particle.Position.X + particle.Width, particle.Position.Y + particle.Width, particle.Position.Z + particle.Height);
            var p8 = new Point3D(particle.Position.X, particle.Position.Y + particle.Width, particle.Position.Z + particle.Height);

            positions.Add(p1);
            positions.Add(p2);
            positions.Add(p3);
            positions.Add(p4);
            positions.Add(p5);
            positions.Add(p6);
            positions.Add(p7);
            positions.Add(p8);

            indices.Add(positionIndex);
            indices.Add(positionIndex + 1);
            indices.Add(positionIndex + 3);
            indices.Add(positionIndex + 1);
            indices.Add(positionIndex + 2);
            indices.Add(positionIndex + 3);
            indices.Add(positionIndex);
            indices.Add(positionIndex + 4);
            indices.Add(positionIndex + 3);
            indices.Add(positionIndex + 4);
            indices.Add(positionIndex + 7);
            indices.Add(positionIndex + 3);
            indices.Add(positionIndex + 4);
            indices.Add(positionIndex + 6);
            indices.Add(positionIndex + 7);
            indices.Add(positionIndex + 4);
            indices.Add(positionIndex + 5);
            indices.Add(positionIndex + 6);
            indices.Add(positionIndex);
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

        ((MeshGeometry3D)model.Geometry).Positions = positions;
        ((MeshGeometry3D)model.Geometry).TriangleIndices = indices;
    }
}
