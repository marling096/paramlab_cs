using System;
using Avalonia.Controls;

namespace components
{
    public partial class SerialPortSettingsView : UserControl
    {
        private static readonly Lazy<SerialPortSettingsView> _instance = new(() => new SerialPortSettingsView());
        public static SerialPortSettingsView Instance => _instance.Value;
        public SerialPortSettingsView()
        {
            Console.WriteLine("SerialPortSettingsView initialized");
            InitializeComponent();
            PortComboBox.DropDownOpened += PortComboBox_DropDownOpened;
        }

        private void PortComboBox_DropDownOpened(object? sender, System.EventArgs e)
        {
            if (DataContext is components.SerialPortSettingsViewModel vm)
            {
                vm.RefreshPortNames();
            }
        }

        public Control GetView()
        {
            return this;
        }
    }
}
