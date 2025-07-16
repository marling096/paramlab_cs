using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;

namespace Editor
{
    public partial class DragResizeThumbs : UserControl
    {
        private Grid? _target;

        private double _target_left;

        private double _target_top;

        private Canvas? _canvas;
        public Thumb TopLeft { get; } = new Thumb();
        public Thumb TopRight { get; } = new Thumb();
        public Thumb BottomLeft { get; } = new Thumb();
        public Thumb BottomRight { get; } = new Thumb();
        public IEnumerable<Thumb> AllThumbs => new[] { TopLeft, TopRight, BottomLeft, BottomRight };
        public DragResizeThumbs()
        {
            // InitializeComponent();
            CreateThumbs();
            // _canvas = this.FindControl<Canvas>("EditorCanvas");
        }

        private void CreateThumbs()
        {
            TopLeft.Classes.Set("resize", true);
            TopLeft.Classes.Add("topLeftCorner");
            TopRight.Classes.Set("resize", true);
            TopRight.Classes.Add("topRightCorner");
            BottomLeft.Classes.Set("resize", true);
            BottomLeft.Classes.Add("bottomLeftCorner");
            BottomRight.Classes.Set("resize", true);
            BottomRight.Classes.Add("bottomRightCorner");


            TopLeft.DragDelta += (s, e) => ResizeTopLeft(e.Vector);
            TopRight.DragDelta += (s, e) => ResizeTopRight(e.Vector);
            BottomLeft.DragDelta += (s, e) => ResizeBottomLeft(e.Vector);
            BottomRight.DragDelta += (s, e) => ResizeBottomRight(e.Vector);
        }
        public void AttachTo(Canvas hostCanvas, Grid target)
        {
            _target = target;

            Width = target.Bounds.Width;
            Height = target.Bounds.Height;

            _target_left = Canvas.GetLeft(target);
            _target_top = Canvas.GetTop(target);
            var rect = new Rectangle
            {
                Width = 10,
                Height = 10,
                Stroke = Brushes.Black,
                Fill = Brushes.Black,
            };

            Canvas.SetLeft(rect, 0);
            Canvas.SetTop(rect, 0);
            // _canvas.Children.Add(rect); // 👈 必须添加

            Console.WriteLine($"AttachTo: target left {_target_left}, top {_target_top}, width {Width}, height {Height}");

            UpdateThumbPositions();
        }

        private void UpdateThumbPositions()
        {
            if (_canvas.Children.Contains(TopLeft))
            { 
                Console.WriteLine("Contains");
            }
            Canvas.SetLeft(TopLeft, _target_left);
            Canvas.SetTop(TopLeft, _target_top);

            Canvas.SetLeft(TopRight, Width + _target_left);
            Canvas.SetTop(TopRight, _target_top);

            Canvas.SetLeft(BottomLeft, _target_left);
            Canvas.SetTop(BottomLeft, Height + _target_top);

            Canvas.SetLeft(BottomRight, Width + _target_left);
            Canvas.SetTop(BottomRight, Height + _target_top);
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
            if (_target == null) return;

            _target_left = Canvas.GetLeft(_target);
            _target_top = Canvas.GetTop(_target);

            Canvas.SetLeft(this, _target_left);
            Canvas.SetTop(this, _target_top);

            Width = _target.Width;
            Height = _target.Height;

            UpdateThumbPositions();
        }
    }
}
