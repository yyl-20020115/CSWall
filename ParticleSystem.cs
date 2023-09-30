﻿using System.Collections.Generic;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Drawing;
using GraphAlgorithmTester.Colors;
using System.Windows.Media.Imaging;

namespace CSWall;

public class ParticleSystem
{
    private readonly List<Particle> particles = new();
    private Particle[,] particle_matrix = new Particle[0, 0];
    public ModelVisual3D WorldVisual = new() { Content = new Model3DGroup() };
    public Model3DGroup? WorldModels => WorldVisual.Content as Model3DGroup;
    public bool WithWalls { get; set; } = false;
    public int BoxEdgeWidth { get; set; } = 8;
    public int BoxHeight { get; set; } = 512;
    protected Bitmap? bitmap = null;
    protected BitmapImage? image = null;
    protected SolidColorBrush WallBrush = new(Colors.White);

    public ParticleSystem()
    {
    }

    public void SpawnParticleWithBoxes(Bitmap bitmap)
    {
        this.particles.Clear();
        this.WorldModels?.Children.Clear();
        var width = bitmap.Width;
        var height = bitmap.Height;
        var halfWidth = width >> 1;
        var halfHeight = height >> 1;

        particle_matrix = new Particle[height, width];
        // 初始化粒子位置和大小
        for (int ix = 0; ix < width; ix++)
        {
            for (int iy = 0; iy < height; iy++)
            {
                var c = bitmap.GetPixel(ix, iy);
                var v = ColorConvert.GetColorValue(c);
                var p = new Particle
                {
                    Position = new(
                        (halfWidth - ix) * BoxEdgeWidth,
                        (iy - halfHeight) * BoxEdgeWidth,
                         0),
                    Thickness = BoxEdgeWidth,
                    Height = /*v == 0 ? 1 : */v * BoxHeight,//c.R=c.G=c.B
                    Color = System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B)
                };
                particles.Add(p);
                particle_matrix[iy, ix] = p;
            }
        }
        this.image = BitmapUtils.GetBitmapImageBybitmap(this.bitmap = bitmap);
        this.UpdateGeometry();
    }

    protected void UpdateGeometry()
    {
        var worldModels = WorldModels;
        if (worldModels != null)
        {
            var collection = new Model3DCollection
            {
                new AmbientLight
                {
                    Color = Colors.White,
                }
            };

            var positions = new Point3DCollection();
            var indices = new Int32Collection();
            var textures = new PointCollection();

            int h = this.particle_matrix.GetLength(0);
            int w = this.particle_matrix.GetLength(1);
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    var particle = this.particle_matrix[y, x];
                    //0,0,0
                    double height = particle.Height;
                    var p0 = new Point3D(particle.Position.X, particle.Position.Y, particle.Position.Z + height);
                    var p1 = new Point3D(particle.Position.X, particle.Position.Y, particle.Position.Z);

                    positions.Add(p0);//0,0,0 -> 0
                    positions.Add(p1);//0,0,0 -> 0

                    var pt = new System.Windows.Point(x / (double)w, y / (double)h);
                    textures.Add(pt);
                    textures.Add(pt);
                }
            }
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int nowIndex = x + y * w;
                    //三角形1
                    indices.Add((nowIndex) << 1);
                    indices.Add((nowIndex + w) << 1);
                    indices.Add((nowIndex + w + 1) << 1);

                    //三角形2
                    indices.Add((nowIndex) << 1);
                    indices.Add((nowIndex + w + 1) << 1);
                    indices.Add((nowIndex + 1) << 1);

                    //三角形1
                    indices.Add(((nowIndex + w + 1) << 1) + 1);
                    indices.Add(((nowIndex + w) << 1) + 1);
                    indices.Add(((nowIndex) << 1) + 1);

                    //三角形2
                    indices.Add(((nowIndex + 1) << 1) + 1);
                    indices.Add(((nowIndex + w + 1) << 1) + 1);
                    indices.Add(((nowIndex) << 1) + 1);

                }
            }

            var geometry = new MeshGeometry3D();
            var visualBrush = new ImageBrush(this.image);
            var material = new DiffuseMaterial(visualBrush);
            geometry.Positions = positions;
            geometry.TriangleIndices = indices;
            geometry.TextureCoordinates = textures;
            collection.Add(new GeometryModel3D(geometry, material));
            if (this.WithWalls)
            {
                //wall_0
                positions = new();
                indices = new();
                textures = new();
                int pc = 0;
                for (int x = 0; x < w; x++)
                {
                    var particle = this.particle_matrix[0, x];
                    //0,0,0
                    double height = particle.Height;
                    var p0 = new Point3D(particle.Position.X, particle.Position.Y, particle.Position.Z + height);
                    var p1 = new Point3D(particle.Position.X, particle.Position.Y, particle.Position.Z);

                    positions.Add(p0);//0,0,0 -> 0
                    positions.Add(p1);//0,0,0 -> 0

                    if (x < w - 1)
                    {
                        indices.Add(pc + 2 * x);
                        indices.Add(pc + 2 * (x + 1));
                        indices.Add(pc + 2 * x + 1);

                        indices.Add(pc + 2 * (x + 1));
                        indices.Add(pc + 2 * (x + 1) + 1);
                        indices.Add(pc + 2 * x + 1);
                    }
                }

                var geometry_wall_0 = new MeshGeometry3D();
                var material_wall_0 = new DiffuseMaterial(this.WallBrush);
                geometry_wall_0.Positions = positions;
                geometry_wall_0.TriangleIndices = indices;
                geometry_wall_0.TextureCoordinates = textures;
                collection.Add(new GeometryModel3D(geometry_wall_0, material_wall_0));
                //wall_1
                positions = new();
                indices = new();
                textures = new();
                pc = 0;
                for (int x = 0; x < w; x++)
                {
                    var particle = this.particle_matrix[h - 1, x];
                    //0,0,0
                    double height = particle.Height;
                    var p0 = new Point3D(particle.Position.X, particle.Position.Y, particle.Position.Z + height);
                    var p1 = new Point3D(particle.Position.X, particle.Position.Y, particle.Position.Z);

                    positions.Add(p0);//0,0,0 -> 0
                    positions.Add(p1);//0,0,0 -> 0

                    if (x < w - 1)
                    {
                        indices.Add(pc + 2 * x);
                        indices.Add(pc + 2 * x + 1);
                        indices.Add(pc + 2 * (x + 1));

                        indices.Add(pc + 2 * (x + 1));
                        indices.Add(pc + 2 * x + 1);
                        indices.Add(pc + 2 * (x + 1) + 1);
                    }
                }

                var geometry_wall_1 = new MeshGeometry3D();
                var material_wall_1 = new DiffuseMaterial(this.WallBrush);
                geometry_wall_1.Positions = positions;
                geometry_wall_1.TriangleIndices = indices;
                geometry_wall_1.TextureCoordinates = textures;
                collection.Add(new GeometryModel3D(geometry_wall_1, material_wall_1));

                //wall_2
                positions = new();
                indices = new();
                textures = new();
                pc = 0;
                for (int y = 0; y < h; y++)
                {
                    var particle = this.particle_matrix[y, 0];
                    //0,0,0
                    double height = particle.Height;
                    var p0 = new Point3D(particle.Position.X, particle.Position.Y, particle.Position.Z + height);
                    var p1 = new Point3D(particle.Position.X, particle.Position.Y, particle.Position.Z);

                    positions.Add(p0);//0,0,0 -> 0
                    positions.Add(p1);//0,0,0 -> 0

                    if (y < h - 1)
                    {
                        indices.Add(pc + 2 * y);
                        indices.Add(pc + 2 * y + 1);
                        indices.Add(pc + 2 * (y + 1));

                        indices.Add(pc + 2 * (y + 1));
                        indices.Add(pc + 2 * y + 1);
                        indices.Add(pc + 2 * (y + 1) + 1);
                    }
                }

                var geometry_wall_2 = new MeshGeometry3D();
                var material_wall_2 = new DiffuseMaterial(this.WallBrush);
                geometry_wall_2.Positions = positions;
                geometry_wall_2.TriangleIndices = indices;
                geometry_wall_2.TextureCoordinates = textures;
                collection.Add(new GeometryModel3D(geometry_wall_2, material_wall_2));

                positions = new();
                indices = new();
                textures = new();
                pc = 0;
                for (int y = 0; y < h; y++)
                {
                    var particle = this.particle_matrix[y, w - 1];
                    //0,0,0
                    double height = particle.Height;
                    var p0 = new Point3D(particle.Position.X, particle.Position.Y, particle.Position.Z + height);
                    var p1 = new Point3D(particle.Position.X, particle.Position.Y, particle.Position.Z);

                    positions.Add(p0);//0,0,0 -> 0
                    positions.Add(p1);//0,0,0 -> 0

                    if (y < h - 1)
                    {
                        indices.Add(pc + 2 * y);
                        indices.Add(pc + 2 * (y + 1));
                        indices.Add(pc + 2 * y + 1);

                        indices.Add(pc + 2 * (y + 1));
                        indices.Add(pc + 2 * (y + 1) + 1);
                        indices.Add(pc + 2 * y + 1);
                    }
                }

                var geometry_wall_3 = new MeshGeometry3D();
                var material_wall_3 = new DiffuseMaterial(this.WallBrush);
                geometry_wall_3.Positions = positions;
                geometry_wall_3.TriangleIndices = indices;
                geometry_wall_3.TextureCoordinates = textures;
                collection.Add(new GeometryModel3D(geometry_wall_3, material_wall_3));
            }
            worldModels.Children = collection;
        }
    }
}
