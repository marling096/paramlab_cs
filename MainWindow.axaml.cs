using System;
using System.IO;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using core;
using Editor;
namespace paramlab_cs;

public partial class MainWindow : Window
{
    public bool _forceClose = false;
    CanvasEditor editor { set; get; }
    public MainWindow()
    {
        InitializeComponent();
        this.Closed += OnMainWindowClosed;

        ComponentManager.Instance.RegisterBase("components");
        editor = new CanvasEditor(this);
        editor.AddPlugin(new HotKey());
        editor.AddPlugin(new Filemanager());
        MainGrid.Children.Add(editor);
        var task = new ScheduledTask(500, () =>
{
    string imagePath = "1.png";
    if (File.Exists(imagePath))
    {
        var bitmap = new Bitmap(imagePath);
        EventHub.Instance.Publish("paramTest", bitmap);
    }

});


    }


    private void OnMainWindowClosed(object? sender, EventArgs e)
    {
        _forceClose = true;

        // 彻底关闭应用程序
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }


}

public class ScheduledTask
{
    private readonly Timer _timer;

    public ScheduledTask(double intervalInMilliseconds, Action onTick)
    {
        _timer = new Timer(intervalInMilliseconds);
        _timer.Elapsed += (s, e) => onTick();
        _timer.AutoReset = true;   // 是否周期性执行
        _timer.Enabled = true;
    }

    public void Start() => _timer.Start();
    public void Stop() => _timer.Stop();
    public void Dispose() => _timer.Dispose();
}