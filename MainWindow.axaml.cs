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
    }
}