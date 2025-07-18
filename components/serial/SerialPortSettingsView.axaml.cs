using System;
using Avalonia.Controls;

namespace components
{
    public partial class SerialPortSettingsView : UserControl
    {
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

    }
}
