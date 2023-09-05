using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace CSWall;

/// <summary>
/// Index.xaml 的交互逻辑
/// </summary>
public partial class Index : Page
{
    private ParticleSystem system;
    private DispatcherTimer _frameTimer;
    private Point pMouse = new (-1, -1);
    private PerspectiveCamera Camera;

    private CameraController CamerController;
    public Index()
    {
        InitializeComponent();

        _frameTimer = new DispatcherTimer();
        _frameTimer.Tick += OnFrame;
        _frameTimer.Interval = TimeSpan.FromMilliseconds(100);
        _frameTimer.Start();

        system = new ParticleSystem(Colors.White);
        WorldModels.Children.Add(system.ParticleModel);

        this.Camera = new PerspectiveCamera
        {
            FieldOfView = 60
        };

        this.CamerController = new CameraController(
            this.Camera, this.TheViewport, Application.Current.MainWindow);
    }

    private void OnFrame(object sender, EventArgs e)
    {
        system.Update(pMouse);
    }

    private void Page_Unloaded(object sender, RoutedEventArgs e)
    {
        //避免页面关闭后资源占用
        _frameTimer.Stop();
        _frameTimer.Tick -= OnFrame;
        _frameTimer = null;
        system = null;
    }
}
