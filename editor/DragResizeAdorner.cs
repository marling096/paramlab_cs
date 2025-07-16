using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Editor
{
    public class DragResizeAdorner
    {
        public void Attach(Canvas hostCanvas, Grid target)
        {
            DragResizeThumbs thumbs = new DragResizeThumbs();
            thumbs.AttachTo(hostCanvas , target);

            hostCanvas.Children.Add(target);
            foreach (var thumb in thumbs.AllThumbs)
            {
                hostCanvas.Children.Add(thumb);
            }
            Console.WriteLine("success");


        }
    }
}
