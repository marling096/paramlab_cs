using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using core;

namespace components
{

    public class TimerComponent : VisualComponentBase
    {

        public TimerComponent(string id)
        {
            Id = id;
        }
        private TextBlock? _text;
        private List<string> subs = new List<string>();

        private List<string> pubs = new List<string>();
        public static string Description { get; } = "定时";
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

        }

        public override void RegisterSubscriptions(string? _eventName)
        {
            if (_eventName != null)
            {
                subs.Add(_eventName);
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
                pubs.Add(_eventName);
                Publish<string>(_eventName, "test success");
            }
        }

        protected override Control CreateParamPanel()
        {


            return new Canvas
            {
                Background = Brushes.LightGreen,
             };
            
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
            return grid;
        }
    }

}



