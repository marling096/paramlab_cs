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
using core;
using components;
using Avalonia.Controls.Shapes;
using System.Security.Cryptography;
using paramlab_cs;
using Avalonia.VisualTree;
using System.Text.Json.Serialization;


namespace Editor
{

    public interface EditorPlugin
    {
        string Name { get; }
        void Initialize(CanvasEditor editor);
    }


    public partial class CanvasEditor : UserControl
    {
        public List<ComponentModel> componentsData = new List<ComponentModel>();
        public List<EditorPlugin> plugins = new List<EditorPlugin>();

        public Dictionary<Control, string> components = new();//ctrl id
        public Dictionary<string, Control> _components = new();//id ctrl 
        public ContextMenu contextMenu = new ContextMenu();
        public double currentX = 0;
        public double currentY = 0;

        public Action SaveToFile;

        public Action LoadFromFile;

        public Action<KeyEventArgs> Hotkey;

        public Action UnDo;



        public CanvasEditor(MainWindow win)
        {

            var t = ComponentManager.Instance.GetAllTypes();
            foreach (var c in t)
            {
                contextMenu.Items.Add(new MenuItem
                {
                    Header = c.Key,
                    Command = ContextMenuHandlerCommand,
                    CommandParameter = c.Key,
                });
            }
            InitializeComponent();
            EditorCanvas.PointerPressed += PointerPressedHandler;


            win.KeyDown += OnKeyDownHandler;


        }

        public void AddPlugin(EditorPlugin plugin)
        {
            plugin.Initialize(this);
            plugins.Add(plugin);
        }

        [RelayCommand]
        private void ContextMenuHandler(string description)
        {
            AddComponent(description, currentX, currentY);
        }

        private void OnKeyDownHandler(object? sender, KeyEventArgs e)
        {
            Hotkey?.Invoke(e);
            Console.WriteLine("调用");
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

        public void AddComponent(string description, double x, double y, string? id = null)
        {
            if (id == null) id = Guid.NewGuid().ToString();
            Control ctrl = ComponentManager.Instance.CreateFromBase(description, id);
            components[ctrl] = id;
            _components[id] = ctrl;
            var comp = ComponentManager.Instance.GetComponent(id);
            componentsData.Add(new ComponentModel
            {
                id = id,
                description = description,
                Subs = comp.Subs,
                Pubs = comp.Pubs,
                X = x,
                Y = y
            });

            Canvas.SetLeft(ctrl, x);
            Canvas.SetTop(ctrl, y);
            ctrl.PointerPressed += RightPressed;

            DragResizeAdorner addone = new DragResizeAdorner();
            addone.AttachThumbs(EditorCanvas, ctrl as Grid);


        }

        private void RightPressed(object? sender, PointerPressedEventArgs e)
        {
            var point = e.GetCurrentPoint(sender as Control);
            if (point.Properties.IsRightButtonPressed)
            {
                e.Handled = true; // 阻止传给 Canvas
                if (sender is Control ctrl)
                {
                    var id = components.TryGetValue(ctrl, out var componentId) ? componentId : throw new ArgumentException("Component not found.");
                    var pal = ComponentManager.Instance.CreateParamPanel(id);
                    if (pal is Window win)
                    {
                        var mainWindow = Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                                ? desktop.MainWindow : null;

                        win.Show(mainWindow);
                    }
                }

            }

        }


        public void LoadComponents(List<ComponentModel> data)
        {
            EditorCanvas.Children.Clear();
            componentsData.Clear();
            foreach (var model in data)
            {
                AddComponent(model.description, model.X, model.Y, model.id);
                LoadComponentparam(model.id, model.Subs, model.Pubs);
            }

        }

        public void LoadComponentparam(string id, List<string> Subs, List<string> Pubs)
        {
            var comp = ComponentManager.Instance.GetComponent(id);
            if (comp != null)
            {
                foreach (var sub in Subs)
                {
                    comp.RegisterSubscriptions(sub);
                }
                foreach (var pub in Pubs)
                {
                    comp.RegisterPublisher(pub);
                }
            }

        }

    }

    public class ComponentModel
    {
        public string id { get; set; } = "";
        public string description { get; set; } = "";

        public List<string> Subs { get; set; } = new List<string>();

        public List<string> Pubs { get; set; } = new List<string>();


        public double X { get; set; }
        public double Y { get; set; }
    }


}