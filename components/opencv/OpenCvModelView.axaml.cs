using System;
using Avalonia.Controls;
using Avalonia.Input;

namespace components;

public partial class OpenCvModelView : UserControl
{
    private static readonly Lazy<OpenCvModelView> _instance = new(() => new OpenCvModelView());
    public static OpenCvModelView Instance => _instance.Value;
    public OpenCvModelView()
    {
        InitializeComponent();
        this.DataContext = new OpencvModelViewModel();
        SourceComboBox.DropDownOpened += Source_ComboBox_DropDownOpened;
    }

    private void Source_ComboBox_DropDownOpened(object? sender, System.EventArgs e)
    {
        if (DataContext is OpencvModelViewModel vm)
            vm.Source_ComboBox_DropDownOpened(sender, e);
    }

    private void CameraImage_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is OpencvModelViewModel vm && sender is Image img)
        {
            var point = e.GetPosition(img);
            // 优化坐标显示位置，防止超出图片边界
            double offsetX = point.X + 8;
            double offsetY = point.Y + 8;
            if (offsetX > img.Bounds.Width - 60) offsetX = img.Bounds.Width - 60;
            if (offsetY > img.Bounds.Height - 24) offsetY = img.Bounds.Height - 24;
            vm.OnImageClicked(new Avalonia.Point(offsetX, offsetY), img.Bounds.Width, img.Bounds.Height);
        }
    }

    public Control GetView()
    {
        return this;
    }
}
