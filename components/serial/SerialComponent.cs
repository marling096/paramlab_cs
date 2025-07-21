// using System;
// using System.Collections.Generic;
// using System.Collections.ObjectModel;
// using System.ComponentModel;
// using System.Linq;
// using System.Runtime.InteropServices;
// using Avalonia;
// using Avalonia.Controls;
// using Avalonia.Controls.Primitives;
// using Avalonia.Media;
// using Avalonia.PropertyGrid.Controls;
// using CommunityToolkit.Mvvm.ComponentModel;
// using core;

// namespace components
// {




//     public class SerialView : Control
//     {

//         public SerialView()
//         {
//         }
//         public Control CreateComponentView()
//         {
//             ComponentBody body = new ComponentBody();
//             SerialPortSettingsView SerialView = new SerialPortSettingsView();
//             var grid = new Grid
//             {
//                 Width = 237,
//                 Height = 539
//             };

//             grid.Children.Add(SerialView);
//             return grid;
//         }

//         public Control CreateEditView(object param)
//         {
//             var win = new Window
//             {
//                 Width = 500,
//                 Height = 300,
//                 Title = "组件编辑",
//                 CornerRadius = new CornerRadius(1),
//                 WindowStartupLocation = WindowStartupLocation.CenterOwner,
//                 SystemDecorations = SystemDecorations.Full, // 有边框与标题栏
//                 CanResize = true
//             };
//             var propertyGrid = new PropertyGrid
//             {
//                 BorderThickness = new Thickness(1),
//                 Background = Brushes.Black,
//                 CornerRadius = new CornerRadius(0.5),
//                 Margin = new Thickness(1),
//                 DataContext = param,
//             };
//             win.Content = propertyGrid;
//             return win;
//         }

//     }

//     public class SerialComponent : VisualComponentBase
//     {

//         public override List<string> Pubs { set; get; } = new List<string>();

//         public override List<string> Subs { set; get; } = new List<string>();

//         private Dictionary<string, List<Action<Object>>> Sub_handler = new();

//         private Lazy<SerialView> ComponentView = new Lazy<SerialView>();

//         public SerialComponent(string id)
//         {
//             Id = id;
//         }

//         public static string Description { get; } = "串口";
//         public override string Id { get; set; }

//         public override void Mount()
//         {

//         }

//         public override void Unmount()
//         {
//             foreach (var sub in Sub_handler)
//             {
//                 foreach (var handler in sub.Value)
//                 {
//                     UnRegisterSubscriptions(sub.Key, handler);
//                 }

//             }

//         }


//         public override void RegisterSubscriptions(string? _eventName, Action<Object> handler)
//         {
//             if (_eventName != null)
//             {
//                 Subs.Add(_eventName);
//                 Sub_handler[_eventName].Add(handler);
//                 Subscribe<string>(_eventName, OnEvent);
//             }

//         }
//         public override void UnRegisterSubscriptions(string? _eventName, Action<Object> handler)
//         {
//             Subs.Remove(_eventName);
//             Sub_handler[_eventName].Remove(handler);
//             UnSubscribe<Object>(_eventName, handler);
//         }

//         private void OnEvent(string data)
//         {
//             //show data
//             Console.WriteLine($"Event received: {data}");
//             foreach (var sub in Subs)
//             {
//                 Console.WriteLine($"Subscribed to {sub}");
//             }
//         }

//         public override void RegisterPublisher(string? _eventName, object? message = null)
//         {

//             if (_eventName != null)
//             {
//                 Pubs.Add(_eventName);
//                 Publish<Object>(_eventName, message);
//             }
//         }


//         public override void UnRegisterPublisher(string? _eventName) { }
//         protected override Control CreateParamView()
//         {
//             var dataContext = new
//             {
//                 subs = Subs,
//                 pubs = Pubs
//             };

//             return ComponentView.Value.CreateEditView(dataContext);

//         }
//         protected override Control CreateView()
//         {
//             return ComponentView.Value.CreateComponentView();
//         }
//     }

// }



