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
using System.Text.Json.Serialization;
using Avalonia.VisualTree;

namespace Editor
{

    public class Filemanager : EditorPlugin
    {
        public string Name { get; } = "File Manager";

        public void Initialize(CanvasEditor editor)
        {
            editor.SaveToFile += () => SaveToFile(editor);
            editor.LoadFromFile += () => LoadFromFile(editor);
        }

        private async void SaveToFile(CanvasEditor editor)
        {
            try
            {
                var storage = TopLevel.GetTopLevel(editor)?.StorageProvider;
                if (storage == null) return;

                var file = await storage.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "保存工程",
                    SuggestedFileName = "project.json",
                    FileTypeChoices = new List<FilePickerFileType>
            {
                new FilePickerFileType("JSON 文件")
                {
                    Patterns = new[] { "*.json" }
                }
            }
                });

                if (file == null) return;

                var data = editor.componentsData.Select(c => new ComponentModel
                {
                    id = c.id,
                    description = c.description,
                    Subs = c.Subs,
                    Pubs = c.Pubs,
                    X = double.IsFinite(Canvas.GetLeft(editor._components[c.id])) ? Canvas.GetLeft(editor._components[c.id]) : 0,
                    Y = double.IsFinite(Canvas.GetTop(editor._components[c.id])) ? Canvas.GetTop(editor._components[c.id]) : 0

                }).ToList();

                await using var stream = await file.OpenWriteAsync();
                await JsonSerializer.SerializeAsync(stream, data, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[保存出错] {ex.Message}");

            }
        }

        private async void LoadFromFile(CanvasEditor editor)
        {
            try
            {
                var storage = TopLevel.GetTopLevel(editor)?.StorageProvider;
                if (storage == null) return;

                var files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "导入工程",
                    AllowMultiple = false,
                    FileTypeFilter = new List<FilePickerFileType>
            {
                new FilePickerFileType("JSON 文件")
                {
                    Patterns = new[] { "*.json" }
                }
            }
                });

                if (files.Count == 0) return;

                await using var stream = await files[0].OpenReadAsync();
                var data = await JsonSerializer.DeserializeAsync<List<ComponentModel>>(stream);

                if (data == null || data.Count == 0)
                {
                    Console.WriteLine($"[加载出错] 文件内容无效。");
                    return;
                }

                editor.LoadComponents(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[加载出错] 加载失败，请检查文件格式。 {ex.Message}");
            }
        }

    }

}