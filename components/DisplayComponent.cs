using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using core;

namespace components
{

    public class DisplayComponent : VisualComponentBase
    {

        public DisplayComponent(string id)
        {
            Id = id;
        }
        private TextBlock? _text;
        // private string _eventName;
        private string[] _events = new string[] { };

        public static string Description { get; } = "显示器";
        public override string Id { get; set; }

        public override void Initialize()
        {
            //初始化ui
            RegisterSubscriptions("Test");
            RegisterPublisher("Test");

        }

        public override void Activate()
        {
            //激活时可以做一些额外的事情
        }

        public override void Deactivate()
        {
            //停用时可以做一些清理工作
            foreach (var sub in _events)
            {
                UnSubscribe<string>(sub, OnEvent);
            }

        }

        public override void RegisterSubscriptions(string? _eventName)
        {
            if (_eventName != null)
            {
                _events.Append(_eventName);
                Subscribe<string>(_eventName, OnEvent);
            }

        }

        private void OnEvent(string data)
        {
            //show data
            Console.WriteLine($"Event received: {data}");
        }

        public override void RegisterPublisher(string? _eventName)
        {
            //do nothing
            if (_eventName != null)
            {
                _events.Append(_eventName);
                Publish<string>(_eventName, "test success");
            }
        }
        protected override Control CreateView()
        {

            var canvas = new Canvas
            {

                Background = new SolidColorBrush(Color.FromArgb(64, 0, 0, 255))
            };

            var grid = new Grid
            {
                Width = 200,
                Height = 100,
                Background = Brushes.LightGreen
            };
            grid.Children.Add(canvas);
            // grid.PointerPressed += (s, e) =>
            //     {
            //         var point = e.GetCurrentPoint(s as Control);
            //         if (point.Properties.IsRightButtonPressed)
            //         {
            //             e.Handled = true; // 阻止传给 Canvas
            //             Console.WriteLine("阻止传给 Canvas");
            //         }
            //         else
            //         {
            //             Console.WriteLine("未阻止");
            //         }


            //     };
            return grid;
        }
    }

}



