using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace components
{

    public class Serial
    {
        public SerialPort port = new SerialPort();
        public Action<byte[]>? dataReceivedCallback;
        public Action<string>? dataSendStringCallback;
        public Action<byte[]>? dataSendCallback;

        public void SerialManager(string portName, int baudRate, string parity, int dataBits, string stopBits)
        {
            port = new SerialPort(
                portName,
                baudRate,
                Enum.Parse<Parity>(parity),
                dataBits,
                Enum.Parse<StopBits>(stopBits)
            );
            port.DataReceived += Port_DataReceived;
            dataSendCallback += Send;
            dataSendStringCallback += Send;
        }

        public void Send(string data)
        {
            if (!port.IsOpen)
            {
                System.Console.WriteLine("串口未打开");
                return;
            }
            try
            {
                port.WriteLine(data);

            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"发送失败: {ex.Message}");
            }
        }

        public void Send(byte[] bytes)
        {
            if (!port.IsOpen)
            {
                System.Console.WriteLine("串口未打开");
                return;
            }
            try
            {
                port.Write(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"发送失败: {ex.Message}");
            }
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int bytesToRead = port.BytesToRead;
                byte[] buffer = new byte[bytesToRead];
                port.Read(buffer, 0, bytesToRead);
                dataReceivedCallback?.Invoke(buffer);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"接收失败: {ex.Message}");
            }
        }

        public static string[] GetAvailablePorts()
        {
            return SerialPort.GetPortNames();
        }

        public static void OpenPort(SerialPort port)
        {
            try
            {
                port.Open();
                port.DiscardInBuffer();
                port.DiscardOutBuffer();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"打开失败: {ex.Message}");
            }
        }

        public static void ClosePort(SerialPort port)
        {
            try
            {
                port.DiscardInBuffer();
                port.DiscardOutBuffer();
                port.Close();
                port.Dispose();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"关闭失败: {ex.Message}");
            }
        }


    }

    public partial class SerialPortSettingsViewModel : ObservableObject
    {
        public ObservableCollection<string> PortNames { get; } = new(SerialPort.GetPortNames());
        public ObservableCollection<int> BaudRates { get; } = new() { 9600, 19200, 38400, 57600, 115200 };
        public ObservableCollection<string> ParityOptions { get; } = new(Enum.GetNames<Parity>());
        public ObservableCollection<int> DataBitsOptions { get; } = new() { 5, 6, 7, 8 };
        public ObservableCollection<string> StopBitsOptions { get; } = new(Enum.GetNames<StopBits>());
        public ObservableCollection<string> HandshakeOptions { get; } = new(Enum.GetNames<Handshake>());

        private readonly Serial serial_port = new();

        [ObservableProperty]
        private string selectedPort = string.Empty;
        [ObservableProperty]
        private int selectedBaudRate = 115200;
        [ObservableProperty]
        private string selectedParity = Parity.None.ToString();
        [ObservableProperty]
        private int selectedDataBits = 8;
        [ObservableProperty]
        private string selectedStopBits = StopBits.One.ToString();
        [ObservableProperty]
        private string selectedHandshake = Handshake.None.ToString();

        [ObservableProperty]
        private string sendText = string.Empty;

        [ObservableProperty]
        private string receiveText = string.Empty;

        public ICommand ConfirmCommand { get; }

        public ICommand SerialSendCommand { get; }

        public SerialPortSettingsViewModel()
        {
            if (PortNames.Count > 0)
                SelectedPort = PortNames[0];
            else
                SelectedPort = string.Empty;

            ConfirmCommand = new RelayCommand(OnConfirm); // Initialize the ConfirmCommand
            SerialSendCommand = new RelayCommand(Port_send); // Initialize the SerialSendCommand

        }

        /// <summary>
        /// 尝试打开串口，返回状态字符串用于UI样式
        /// </summary>
        /// <returns>"Opened" | "Closed" | "Error"</returns>

        private void OnConfirm()
        {
            try
            {
                if (serial_port.port.IsOpen)
                {
                    Serial.ClosePort(serial_port.port);
                    serial_port.dataReceivedCallback -= Port_received;
                    System.Console.WriteLine("Serial close ");
                    return;
                }
                if (!serial_port.port.IsOpen)
                {
                    serial_port.SerialManager(SelectedPort, SelectedBaudRate, SelectedParity, SelectedDataBits, SelectedStopBits);
                    serial_port.dataReceivedCallback += Port_received;
                    Serial.OpenPort(serial_port.port);
                    System.Console.WriteLine("Serial open ");
                    return;
                }


            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"串口打开失败: {ex.Message}");

            }
        }

        public void RefreshPortNames()
        {
            PortNames.Clear();
            foreach (var port in Serial.GetAvailablePorts())
            {
                PortNames.Add(port);
            }
            if (!PortNames.Contains(SelectedPort))
            {
                SelectedPort = PortNames.Count > 0 ? PortNames[0] : string.Empty;
            }
        }

        public void Port_send()
        {
            serial_port.dataSendStringCallback?.Invoke(SendText);
            System.Console.WriteLine("Serial Test");
        }


        public void Port_received(byte[] message)
        {
            ReceiveText = System.Text.Encoding.UTF8.GetString(message);
        }


    }
}
