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
using core;
using Avalonia.Controls.ApplicationLifetimes;


namespace paramlab_cs
{
    public partial class CanvasEditor : UserControl
    {

        public CanvasEditor(ComponentManager manger)
        {
            this.manager = manger;
            var t = this.manager.GetAll();
            foreach (var c in t)
            {
                BaseList.Add(c.Key);
                contextMenu.Items.Add(new MenuItem
                {
                    Header = c.Key,
                    Command = ContextMenuHandlerCommand,
                    CommandParameter = c.Key,
                });
            }
            InitializeComponent();
            EditorCanvas.PointerPressed += PointerPressedHandler;
        }
        public ComponentManager manager = new ComponentManager();
        private Point _dragStart;
        private Control? _dragging;
        private Dictionary<string, string> components = new();
        private Dictionary<Control, string> _components = new();//ctrl id
        private List<string> BaseList = new();
        private ContextMenu contextMenu = new ContextMenu();
        private double currentX = 0;
        private double currentY = 0;

        [RelayCommand]
        private void ContextMenuHandler(string description)
        {
            AddComponent(description, currentX, currentY);
        }

        private void PointerPressedHandler(object? sender, PointerPressedEventArgs args)
        {
            var point = args.GetCurrentPoint(sender as Control);
            this.currentX = point.Position.X;
            this.currentY = point.Position.Y;
            if (point.Properties.IsLeftButtonPressed)
            {
            }
            if (point.Properties.IsRightButtonPressed)
            {
                contextMenu.Open(EditorCanvas);
            }
        }

        public void AddComponent(string description, double x, double y)
        {
            var id = Guid.NewGuid().ToString();
            Control ctrl = manager.CreateFromBase(description, id);
            components[id] = description;
            _components[ctrl] = id;
            Canvas.SetLeft(ctrl, x);
            Canvas.SetTop(ctrl, y);

            ctrl.PointerPressed += ComponentPressed;
            ctrl.PointerMoved += OnDrag;
            ctrl.PointerReleased += OnDragEnd;
            EditorCanvas.Children.Add(ctrl);

        }
        private void AddInput(string id, string subscription)
        {
            manager.AddComponentIn(id, subscription);
        }

        private void AddOutput(string id, string publisher)
        {
            manager.AddComponentOut(id, publisher);
        }
        private void ComponentPressed(object? sender, PointerPressedEventArgs e)
        {
            var point = e.GetCurrentPoint(sender as Control);
            if (point.Properties.IsLeftButtonPressed)
            {
                if (sender is Control ctrl)
                {
                    _dragStart = e.GetPosition(EditorCanvas);
                    _dragging = ctrl;
                    e.Pointer.Capture(ctrl);
                }
            }
            if (point.Properties.IsRightButtonPressed)
            {
                e.Handled = true; // 阻止传给 Canvas
                if (sender is Control ctrl)
                {
                    var id = _components.TryGetValue(ctrl, out var componentId) ? componentId : throw new ArgumentException("Component not found.");
                    var des = components.TryGetValue(id, out var description) ? description : throw new ArgumentException("Description not found.");
                    var pal = manager.CreateParamPanel(des, id);
                    if (pal is Window win)
                    {
                        var mainWindow = Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                                ? desktop.MainWindow: null;

                        win.Show(mainWindow);
                    }
                }

            }

        }

        private void OnDrag(object? sender, PointerEventArgs e)
        {
            if (_dragging != null && e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                var pos = e.GetPosition(EditorCanvas);
                double dx = pos.X - _dragStart.X;
                double dy = pos.Y - _dragStart.Y;

                var left = Canvas.GetLeft(_dragging);
                var top = Canvas.GetTop(_dragging);

                Canvas.SetLeft(_dragging, left + dx);
                Canvas.SetTop(_dragging, top + dy);

                _dragStart = pos;
            }
        }

        private void OnDragEnd(object? sender, PointerReleasedEventArgs e)
        {
            e.Pointer.Capture(null);
            _dragging = null;
        }

        // private async void OnSaveClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        // {
        //     var storage = TopLevel.GetTopLevel(this)?.StorageProvider;
        //     if (storage == null) return;

        //     var file = await storage.SaveFilePickerAsync(new FilePickerSaveOptions
        //     {
        //         SuggestedFileName = "components.json"
        //     });

        //     if (file == null) return;

        //     var data = new List<ComponentModel>();
        //     foreach (var ctrl in _components)
        //     {
        //         data.Add(new ComponentModel
        //         {
        //             Type = "Rectangle", // simple type name
        //             X = Canvas.GetLeft(ctrl),
        //             Y = Canvas.GetTop(ctrl)
        //         });
        //     }

        //     await using var stream = await file.OpenWriteAsync();
        //     await JsonSerializer.SerializeAsync(stream, data);
        // }

        // private async void OnLoadClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        // {
        //     var storage = TopLevel.GetTopLevel(this)?.StorageProvider;
        //     if (storage == null) return;

        //     var files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions());
        //     if (files.Count == 0) return;

        //     await using var stream = await files[0].OpenReadAsync();
        //     var data = await JsonSerializer.DeserializeAsync<List<ComponentModel>>(stream);

        //     if (data == null) return;

        //     EditorCanvas.Children.Clear();
        //     _components.Clear();

        //     foreach (var model in data)
        //     {
        //         if (model.Type == "Rectangle")
        //         {
        //             var rect = new Control();////
        //             // AddComponent(rect, model.X, model.Y);
        //         }
        //     }
        // }

        private class ComponentModel
        {
            public string Type { get; set; } = "";
            public double X { get; set; }
            public double Y { get; set; }
        }
    }

    public class ComponentEditor
    {




    }




}