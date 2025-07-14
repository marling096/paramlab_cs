using System;
using System.Timers;
using Avalonia.Controls;
using core;
namespace paramlab_cs;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var manger = new ComponentManager();
        manger.RegisterBase("components");
        var ediotr = new CanvasEditor(manger);
        MainGrid.Children.Add(ediotr);
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