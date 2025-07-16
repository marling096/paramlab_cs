using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;

namespace components
{
    public partial class ComponentBody : UserControl
    {
        private SelectedAdorner? _selected;
        public ComponentBody()
        {

            InitializeComponent();

        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            var Component_Canvas = this.FindControl<Canvas>("ComponentCanvas");
            var Component_Border = this.FindControl<Grid>("ComponentContainer");


            PointerPressed += (_, _) =>
            {
                if (_selected is null)
                {
                    AddSelected(Component_Border, Component_Canvas);
                }
            };
        }

        public Control getView()
        {
            return this;
        }
        private void UpdateTransform()
        {
            var canvas = this.FindControl<Canvas>("Canvas");
            var rectangle = this.FindControl<Rectangle>("Rectangle");

            var width = rectangle.Width;
            var height = rectangle.Height;

            var ul = this.FindControl<Thumb>("UL");
            var ur = this.FindControl<Thumb>("UR");
            var ll = this.FindControl<Thumb>("LL");
            var lr = this.FindControl<Thumb>("LR");

            var ptUL = new Point(Canvas.GetLeft(ul), Canvas.GetTop(ul));
            var ptUR = new Point(Canvas.GetLeft(ur), Canvas.GetTop(ur));
            var ptLL = new Point(Canvas.GetLeft(ll), Canvas.GetTop(ll));
            var ptLR = new Point(Canvas.GetLeft(lr), Canvas.GetTop(lr));

            var result = ComputeMatrix(new Size(width, height), ptUL, ptUR, ptLL, ptLR);

            rectangle.RenderTransformOrigin = RelativePoint.Center;
            rectangle.RenderTransform = new MatrixTransform(result);
        }

        static Point MapPoint(Matrix matrix, Point point)
        {
            return new Point(
                (point.X * matrix.M11) + (point.Y * matrix.M21) + matrix.M31,
                (point.X * matrix.M12) + (point.Y * matrix.M22) + matrix.M32);
        }

        static Matrix ComputeMatrix(Size size, Point ptUL, Point ptUR, Point ptLL, Point ptLR)
        {
            // Scale transform
            var S = Matrix.CreateScale(1 / size.Width, 1 / size.Height);

            // Affine transform
            var A = new Matrix(
                ptUR.X - ptUL.X,
                ptUR.Y - ptUL.Y,
                0,
                ptLL.X - ptUL.X,
                ptLL.Y - ptUL.Y,
                0,
                ptUL.X,
                ptUL.Y,
                1);

            // Non-Affine transform
            //A.TryInvert(out var inverseA);
            //var abPoint = MapPoint(inverseA, ptLR);
            //var a = abPoint.X;
            //var b = abPoint.Y;
            double den = A.M11 * A.M22 - A.M12 * A.M21;
            double a = (A.M22 * ptLR.X - A.M21 * ptLR.Y + A.M21 * A.M32 - A.M22 * A.M31) / den;
            double b = (A.M11 * ptLR.Y - A.M12 * ptLR.X + A.M12 * A.M31 - A.M11 * A.M32) / den;

            var scaleX = a / (a + b - 1);
            var scaleY = b / (a + b - 1);

            var N = new Matrix(
                scaleX,
                0,
                scaleX - 1,
                0,
                scaleY,
                scaleY - 1,
                0,
                0,
                1);

            // Multiply S * N * A
            var result = S * N * A;

            // TODO: Multiply does not include perspective

            return result;
        }

        private void OnDragDelta(object? sender, VectorEventArgs e)
        {
            if (sender is Thumb thumb)
            {
                Canvas.SetLeft(thumb, Canvas.GetLeft(thumb) + e.Vector.X);
                Canvas.SetTop(thumb, Canvas.GetTop(thumb) + e.Vector.Y);
                UpdateTransform();
            }
        }

        private void ResizeThumb_DragDelta(object? sender, VectorEventArgs e)
        {
            if (sender is Thumb thumb)
            {
                var canvas = this.FindControl<Canvas>("Canvas");
                if (canvas != null)
                {
                    // 获取原始宽高
                    double newWidth = canvas.Width + e.Vector.X;
                    double newHeight = canvas.Height + e.Vector.Y;

                    // 设置最小尺寸限制
                    if (newWidth > 20)
                        canvas.Width = newWidth;

                    if (newHeight > 20)
                        canvas.Height = newHeight;
                }
            }
        }

        private void AddSelected(Control control, Canvas canvas)
        {
            var layer = AdornerLayer.GetAdornerLayer(canvas);
            if (layer is null)
            {
                return;
            }

            _selected = new SelectedAdorner
            {
                [AdornerLayer.AdornedElementProperty] = canvas,
                IsHitTestVisible = true,
                ClipToBounds = false,
                Control = control
            };

            ((ISetLogicalParent)_selected).SetParent(canvas);
            layer.Children.Add(_selected);
        }

        private void RemoveSelected(Canvas canvas)
        {
            var layer = AdornerLayer.GetAdornerLayer(canvas);
            if (layer is null || _selected is null)
            {
                return;
            }

            layer.Children.Remove(_selected);
            ((ISetLogicalParent)_selected).SetParent(null);
            _selected = null;
        }
    }

}















