using System;
using System.Timers;
using Avalonia.Controls;
using core;
using Editor;
namespace paramlab_cs;

public partial class MainWindow : Window
{
    private static readonly Lazy<MainWindow> _instance = new(() => new MainWindow());
    public static MainWindow Instance => _instance.Value;
    public MainWindow()
    {
        InitializeComponent();

        var manager = new ComponentManager();
        manager.RegisterBase("components");
        var editor = new CanvasEditor(manager);
        MainGrid.Children.Add(editor);
                var task = new ScheduledTask(500, () =>
        {
            EventHub.Instance.Publish("paramTest", "fetch subs");
        });


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