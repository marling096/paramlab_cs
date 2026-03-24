using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace components;

public partial class OpenCvModelView : UserControl
{
    public OpenCvModelView()
    {
        InitializeComponent();
        if (this.DataContext == null)
            this.DataContext = new OpencvModelViewModel();

        SourceComboBox.DropDownOpened += Source_ComboBox_DropDownOpened;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

        if (this.DataContext is IDisposable disposable)
        {
            disposable.Dispose();
            this.DataContext = null;
        }
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
}
