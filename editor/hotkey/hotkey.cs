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

namespace Editor
{
    public class HotKey : EditorPlugin
    {
        public string Name { get; } = "Hot Key";

        public void Initialize(CanvasEditor editor)
        {
            Console.WriteLine("plugin init");
            editor.Hotkey += (e) => OnKeyDownHandler(editor, e);
        }

        private void OnKeyDownHandler(CanvasEditor editor, KeyEventArgs e)
        {
            var key = $"{e.KeyModifiers}+{e.Key}";

            if (e.KeyModifiers == KeyModifiers.Control && e.Key == Key.S)
            {
                e.Handled = true;
                editor.SaveToFile?.Invoke();
                Console.WriteLine("[热键] 保存");
            }
            if (e.KeyModifiers == KeyModifiers.Control && e.Key == Key.O)
            {
                e.Handled = true;
                editor.LoadFromFile?.Invoke();
            }


        }

    }



}