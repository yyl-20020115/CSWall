using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using System.Drawing;

namespace CSWall;


public partial class MainWindow : Window
{
    private ParticleSystem system;
    private DispatcherTimer _frameTimer;
    private System.Windows.Point pMouse = new(-1, -1);

    //声明摄像头
    PerspectiveCamera myPCamera;
    //鼠标灵敏度调节
    double mouseDeltaFactor = 0.4;
    Model3DGroup WorldModels = new Model3DGroup();

    public MainWindow()
    {
        InitializeComponent();

        //摄像头
        myPCamera = new PerspectiveCamera();
        myPCamera.Position = new Point3D(0, 0, 2000);
        myPCamera.LookDirection = new Vector3D(0, 0, -1);
        myPCamera.FieldOfView = 1000;
        viewPort.Camera = myPCamera;
        _frameTimer = new DispatcherTimer();
        _frameTimer.Tick += OnFrame;
        _frameTimer.Interval = TimeSpan.FromMilliseconds(100);
        _frameTimer.Start();

        system = new ParticleSystem(Colors.Blue);

        WorldModels.Children.Add(system.ParticleModel);
        BuildModel();

    }
    private void OnFrame(object sender, EventArgs e)
    {
        system.UpdateGeometry();
    }
    private void Grid_Drop(object sender, DragEventArgs e)
    {
        var f = e.Data.GetData(DataFormats.FileDrop);
        if (f is string[] files && files.Length == 1)
        {
            var bitmap =
                Bitmap.FromFile(files[0]) as Bitmap;
            if (bitmap != null)
            {
                system.SpawnParticle(bitmap);
            }
        }
    }
    public void BuildModel()
    {
        //光源
        //AmbientLight （自然光）
        //DirectionalLight （方向光）
        //PointLight （点光源）
        //SpotLight （聚光源）
        DirectionalLight myDirectionalLight = new DirectionalLight();
        myDirectionalLight.Color = Colors.Red;
        myDirectionalLight.Direction = new Vector3D(0.61, 0.5, 0.61);
        WorldModels.Children.Add(myDirectionalLight);

        //DirectionalLight myDirectionalLight2 = new DirectionalLight();
        //myDirectionalLight2.Color = Colors.White;
        //myDirectionalLight2.Direction = new Vector3D(0.61, 0.5, 0.61);
        //myModel3DGroup.Children.Add(myDirectionalLight2);

        var modelVisual3D = new ModelVisual3D();

        modelVisual3D.Content = WorldModels;

        //下面是调整n的位置，初学者可以先注释掉。
        //var tt = new TranslateTransform3D();
        //tt.OffsetX = 110;
        //tt.OffsetZ = -50;
        //tt.OffsetY = -100;
        //var tr = new RotateTransform3D();
        //tr.Rotation = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 90);

        //var tr2 = new RotateTransform3D();
        //tr2.Rotation = new AxisAngleRotation3D(new Vector3D(0, 0, 1), -45);

        //var ts = new ScaleTransform3D();
        //ts.ScaleX = 1.5;
        //ts.ScaleY = 1.5;
        //ts.ScaleZ = 1.6;
        //var tg = new Transform3DGroup();
        //tg.Children.Add(tr); tg.Children.Add(tr2); tg.Children.Add(tt); tg.Children.Add(ts);
        //modelVisual3D.Transform = tg;
        //将两个模型添加到场景中
        viewPort.Children.Add(modelVisual3D);
        //添加鼠标事件，用于显示隐藏光晕特效
        viewPort.MouseEnter += Vp_MouseEnter;
        viewPort.MouseLeave += Vp_MouseLeave;

    }

    private void Vp_MouseLeave(object sender, MouseEventArgs e)
    {
        viewPort.Effect = null;
    }

    private void Vp_MouseEnter(object sender, MouseEventArgs e)
    {
        DropShadowEffect BlurRadius = new DropShadowEffect();

        BlurRadius.BlurRadius = 20;
        BlurRadius.Color = Colors.Yellow;
        BlurRadius.Direction = 0;
        BlurRadius.Opacity = 1;
        BlurRadius.ShadowDepth = 0;
        viewPort.Effect = BlurRadius;
    }


    public HitTestResultBehavior HTResult(HitTestResult rawresult)
    {
        //MessageBox.Show(rawresult.ToString());
        // RayHitTestResult rayResult = rawresult as RayHitTestResult;
        RayHitTestResult rayResult = rawresult as RayHitTestResult;
        if (rayResult != null)
        {
            //RayMeshGeometry3DHitTestResult rayMeshResult = rayResult as RayMeshGeometry3DHitTestResult;
            RayHitTestResult rayMeshResultrayResult = rayResult as RayHitTestResult;
            if (rayMeshResultrayResult != null)
            {
                //GeometryModel3D hitgeo = rayMeshResult.ModelHit as GeometryModel3D;
                var visual3D = rawresult.VisualHit as ModelVisual3D;

                //do something

            }
        }

        return HitTestResultBehavior.Continue;
    }

    //鼠标位置
    System.Windows.Point mouseLastPosition;

    private void vp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        mouseLastPosition = e.GetPosition(this);
        //RayHitTestParameters hitParams = new RayHitTestParameters(myPCamera.Position, myPCamera.LookDirection);
        //VisualTreeHelper.HitTest(vp.Children[0], null, ResultCallback, hitParams);

        //下面是进行点击触发检测，可忽略，注释
        Point3D testpoint3D = new Point3D(mouseLastPosition.X, mouseLastPosition.Y, 0);
        Vector3D testdirection = new Vector3D(mouseLastPosition.X, mouseLastPosition.Y, 100);
        PointHitTestParameters pointparams = new PointHitTestParameters(mouseLastPosition);
        RayHitTestParameters rayparams = new RayHitTestParameters(testpoint3D, testdirection);

        //test for a result in the Viewport3D
        VisualTreeHelper.HitTest(viewPort, null, HTResult, pointparams);
    }

    //鼠标旋转
    private void Window_MouseMove(object sender, MouseEventArgs e)
    {
        if (Mouse.LeftButton == MouseButtonState.Pressed)//如果按下鼠标左键
        {
            var newMousePosition = e.GetPosition(this);

            if (mouseLastPosition.X != newMousePosition.X)
            {
                //进行水平旋转
                HorizontalTransform(mouseLastPosition.X < newMousePosition.X, mouseDeltaFactor);//水平变换
            }

            if (mouseLastPosition.Y != newMousePosition.Y)// change position in the horizontal direction

            {
                //进行垂直旋转
                VerticalTransform(mouseLastPosition.Y > newMousePosition.Y, mouseDeltaFactor);//垂直变换 

            }

            mouseLastPosition = newMousePosition;

        }

    }

    //鼠标滚轮缩放
    private void VP_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        double scaleFactor = 240;
        //120 near ,   -120 far
        System.Diagnostics.Debug.WriteLine(e.Delta.ToString());
        Point3D currentPosition = myPCamera.Position;
        Vector3D lookDirection = myPCamera.LookDirection;//new Vector3D(camera.LookDirection.X, camera.LookDirection.Y, camera.LookDirection.Z);
        lookDirection.Normalize();

        lookDirection *= scaleFactor;

        if (e.Delta == 120)//getting near
        {
            //myPCamera.FieldOfView /= 1.2;

            if ((currentPosition.X + lookDirection.X) * currentPosition.X > 0)
            {
                currentPosition += lookDirection;
            }
        }
        if (e.Delta == -120)//getting far
        {
            //myPCamera.FieldOfView *= 1.2;
            currentPosition -= lookDirection;
        }

        var positionAnimation = new Point3DAnimation();
        positionAnimation.BeginTime = new TimeSpan(0, 0, 0);
        positionAnimation.Duration = TimeSpan.FromMilliseconds(100);
        positionAnimation.To = currentPosition;
        positionAnimation.From = myPCamera.Position;
        positionAnimation.Completed += new EventHandler(positionAnimation_Completed);
        myPCamera.BeginAnimation(PerspectiveCamera.PositionProperty, positionAnimation, HandoffBehavior.Compose);
    }

    void positionAnimation_Completed(object sender, EventArgs e)
    {
        Point3D position = myPCamera.Position;
        myPCamera.BeginAnimation(PerspectiveCamera.PositionProperty, null);
        myPCamera.Position = position;
    }

    // 垂直变换
    private void VerticalTransform(bool upDown, double angleDeltaFactor)
    {
        Vector3D postion = new Vector3D(myPCamera.Position.X, myPCamera.Position.Y, myPCamera.Position.Z);
        Vector3D rotateAxis = Vector3D.CrossProduct(postion, myPCamera.UpDirection);
        RotateTransform3D rt3d = new RotateTransform3D();
        AxisAngleRotation3D rotate = new AxisAngleRotation3D(rotateAxis, angleDeltaFactor * (upDown ? 1 : -1));
        rt3d.Rotation = rotate;
        Matrix3D matrix = rt3d.Value;
        Point3D newPostition = matrix.Transform(myPCamera.Position);
        myPCamera.Position = newPostition;
        myPCamera.LookDirection = new Vector3D(-newPostition.X, -newPostition.Y, -newPostition.Z);

        //update the up direction
        Vector3D newUpDirection = Vector3D.CrossProduct(myPCamera.LookDirection, rotateAxis);
        newUpDirection.Normalize();
        myPCamera.UpDirection = newUpDirection;
    }
    // 水平变换：
    private void HorizontalTransform(bool leftRight, double angleDeltaFactor)
    {
        Vector3D postion = new Vector3D(myPCamera.Position.X, myPCamera.Position.Y, myPCamera.Position.Z);
        Vector3D rotateAxis = myPCamera.UpDirection;
        RotateTransform3D rt3d = new RotateTransform3D();
        AxisAngleRotation3D rotate = new AxisAngleRotation3D(rotateAxis, angleDeltaFactor * (leftRight ? 1 : -1));
        rt3d.Rotation = rotate;
        Matrix3D matrix = rt3d.Value;
        Point3D newPostition = matrix.Transform(myPCamera.Position);
        myPCamera.Position = newPostition;
        myPCamera.LookDirection = new Vector3D(-newPostition.X, -newPostition.Y, -newPostition.Z);
    }



}