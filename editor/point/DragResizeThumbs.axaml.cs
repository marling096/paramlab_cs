using System;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace Editor
{
    public partial class DragResizeThumbs : UserControl
    {
        private Control? _target;

        private double _target_width;
        private double _target_height;
        private double _target_left;
        private double _target_top;

        public Action? TopLeft;
        public Action? TopRight;
        public Action? BottomLeft;
        public Action? BottomRight;

        public DragResizeThumbs()
        {
            InitializeComponent();
        }
        public void AttachTo(string name, string type, Control ctl, Action act)
        {
            Default.Name = name;
            _target = ctl;

            // Canvas.SetLeft(this, 50);
            // Canvas.SetTop(this, 50);
            // Canvas.SetLeft(ctl, 100);
            // Canvas.SetTop(ctl, 100);

            _target_left = Canvas.GetLeft(_target);
            _target_top = Canvas.GetTop(_target);
            Console.WriteLine($"left : {_target_left} Top : {_target_top}");

            _target_width = _target.Width;
            _target_height = _target.Height;

            // Console.WriteLine("add thumb");

            if (type == "topleft")
            {
                Default.Cursor = new Cursor(StandardCursorType.TopLeftCorner);
                Default.DragDelta += (s, e) => { ResizeTopLeft(e.Vector); act.Invoke(); };
                // TopLeft += () =>
                // {
                //     Canvas.SetLeft(this, _target_left);
                //     Canvas.SetTop(this, _target_top);
                // };
            }
            if (type == "topright")
            {   
                Default.Cursor = new Cursor(StandardCursorType.TopRightCorner);
                Default.DragDelta += (s, e) => { ResizeTopRight(e.Vector); act.Invoke(); };
                // TopRight += () =>
                // {
                //     Console.WriteLine("ThumbTopRight");
                //     Canvas.SetLeft(this, _target_width + _target_left);
                //     Canvas.SetTop(this, _target_top);
                // };
            }
            if (type == "bottomleft")
            {   
                Default.Cursor = new Cursor(StandardCursorType.BottomLeftCorner);
                Default.DragDelta += (s, e) => { ResizeBottomLeft(e.Vector); act.Invoke(); };
                // BottomLeft += () =>
                // {
                //     Canvas.SetLeft(this, _target_left);
                //     Canvas.SetTop(this, _target_height + _target_top);
                // };
            }
            if (type == "bottomright")
            {   
                Default.Cursor = new Cursor(StandardCursorType.BottomRightCorner);
                Default.DragDelta += (s, e) => { ResizeBottomRight(e.Vector); act.Invoke(); };
                // BottomRight += () =>
                // {
                //     Canvas.SetLeft(this, _target_width + _target_left);
                //     Canvas.SetTop(this, _target_height + _target_top);
                // };
            }
            UpdatePositionAndSize();

        }


        private void OnDragThumb_DragDelta(object? sender, VectorEventArgs e)
        {
            if (_target == null) return;

            var newLeft = Canvas.GetLeft(_target) + e.Vector.X;
            var newTop = Canvas.GetTop(_target) + e.Vector.Y;

            Canvas.SetLeft(_target, newLeft);
            Canvas.SetTop(_target, newTop);

            Canvas.SetLeft(this, newLeft);
            Canvas.SetTop(this, newTop);
        }

        private void ResizeTopLeft(Vector delta)
        {
            if (_target == null) return;

            double newWidth = _target.Width - delta.X;
            double newHeight = _target.Height - delta.Y;

            if (newWidth > 20 && newHeight > 20)
            {
                _target.Width = newWidth;
                _target.Height = newHeight;

                var newLeft = Canvas.GetLeft(_target) + delta.X;
                var newTop = Canvas.GetTop(_target) + delta.Y;

                Canvas.SetLeft(_target, newLeft);
                Canvas.SetTop(_target, newTop);
            }

            UpdatePositionAndSize();
        }

        private void ResizeTopRight(Vector delta)
        {

            // Console.WriteLine("ResizeTopRight");
            if (_target == null) return;

            double newWidth = _target.Width + delta.X;
            double newHeight = _target.Height - delta.Y;

            if (newWidth > 20 && newHeight > 20)
            {
                _target.Width = newWidth;
                _target.Height = newHeight;

                var newTop = Canvas.GetTop(_target) + delta.Y;

                Canvas.SetTop(_target, newTop);
            }

            UpdatePositionAndSize();
        }

        private void ResizeBottomLeft(Vector delta)
        {
            if (_target == null) return;

            double newWidth = _target.Width - delta.X;
            double newHeight = _target.Height + delta.Y;

            if (newWidth > 20 && newHeight > 20)
            {
                _target.Width = newWidth;
                _target.Height = newHeight;

                var newLeft = Canvas.GetLeft(_target) + delta.X;

                Canvas.SetLeft(_target, newLeft);

            }

            UpdatePositionAndSize();
        }

        private void ResizeBottomRight(Vector delta)
        {
            if (_target == null) return;

            double newWidth = _target.Width + delta.X;
            double newHeight = _target.Height + delta.Y;

            if (newWidth > 20 && newHeight > 20)
            {
                _target.Width = newWidth;
                _target.Height = newHeight;

            }

            UpdatePositionAndSize();
        }


        private void UpdatePositionAndSize()
        {
            // if (_target == null) return;

            // _target_left = Canvas.GetLeft(_target);
            // _target_top = Canvas.GetTop(_target);

            // TopLeft?.Invoke();
            // TopRight?.Invoke();
            // BottomLeft?.Invoke();
            // BottomRight?.Invoke();

        }
    }
}
