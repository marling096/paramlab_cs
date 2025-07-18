using System;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace Editor
{
    public class DragResizeAdorner
    {
        public DragResizeThumbs topleft;
        public DragResizeThumbs topright;
        public DragResizeThumbs bottomleft;
        public DragResizeThumbs bottomright;

        public Action thumbsevent;

        public Canvas _canvas;

        public Grid _target;

        private Control? _dragging;

        private Point _dragStart;


        public void AttachThumbs(Canvas hostCanvas, Control target)
        {
            topleft = new DragResizeThumbs();
            topright = new DragResizeThumbs();
            bottomleft = new DragResizeThumbs();
            bottomright = new DragResizeThumbs();

            thumbsevent += () => updateThumbs(target);

            topleft.AttachTo(Guid.NewGuid().ToString(), "topleft", target, thumbsevent);
            topright.AttachTo(Guid.NewGuid().ToString(), "topright", target, thumbsevent);
            bottomleft.AttachTo(Guid.NewGuid().ToString(), "bottomleft", target, thumbsevent);
            bottomright.AttachTo(Guid.NewGuid().ToString(), "bottomright", target, thumbsevent);

            target.PointerMoved += OnDrag;
            target.PointerReleased += OnDragEnd;
            target.PointerPressed += LeftPressed;

            hostCanvas.Children.Add(target);
            hostCanvas.Children.Add(topleft);
            hostCanvas.Children.Add(topright);
            hostCanvas.Children.Add(bottomleft);
            hostCanvas.Children.Add(bottomright);

            // Console.WriteLine("success");

        }

        public void updateThumbs(Control target)
        {
            // Console.WriteLine("invoke");
            var _target_left = Canvas.GetLeft(target);
            var _target_top = Canvas.GetTop(target);

            var _target_width = target.Width;
            var _target_height = target.Height;

            Canvas.SetLeft(topleft, _target_left);
            Canvas.SetTop(topleft, _target_top);

            Canvas.SetLeft(topright, _target_width + _target_left);
            Canvas.SetTop(topright, _target_top);

            Canvas.SetLeft(bottomleft, _target_left);
            Canvas.SetTop(bottomleft, _target_height + _target_top);

            Canvas.SetLeft(bottomright, _target_width + _target_left);
            Canvas.SetTop(bottomright, _target_height + _target_top);

        }

        private void OnDrag(object? sender, PointerEventArgs e)
        {
            // Console.WriteLine("Drag invoke");
            if (_dragging != null && e.GetCurrentPoint(_canvas).Properties.IsLeftButtonPressed)
            {
                var pos = e.GetPosition(_canvas);
                double dx = pos.X - _dragStart.X;
                double dy = pos.Y - _dragStart.Y;

                var left = Canvas.GetLeft(_dragging);
                var top = Canvas.GetTop(_dragging);

                Canvas.SetLeft(_dragging, left + dx);
                Canvas.SetTop(_dragging, top + dy);

                thumbsevent.Invoke();

                _dragStart = pos;
            }
        }

        private void OnDragEnd(object? sender, PointerReleasedEventArgs e)
        {
            e.Pointer.Capture(null);
            _dragging = null;
        }

        private void LeftPressed(object? sender, PointerPressedEventArgs e)
        {
            var point = e.GetCurrentPoint(sender as Control);
            if (point.Properties.IsLeftButtonPressed)
            {
                if (sender is Control ctrl)
                {
                    _dragStart = e.GetPosition(_canvas);
                    _dragging = ctrl;
                    e.Pointer.Capture(ctrl);
                }
            }

        }

    }
}
