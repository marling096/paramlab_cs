using System;
using System.Collections.ObjectModel;
using System.IO;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading;
using OpenCvSharp;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using Avalonia;

namespace components
{
    public class PictureInterface : IDisposable
    {
        private Mat _mat;
        public Mat Mat => _mat;
        public int Width => _mat.Width;
        public int Height => _mat.Height;
        public bool IsEmpty => _mat.Empty();

        public PictureInterface(Mat source)
        {
            _mat = source.Clone();
        }

        public static PictureInterface FromFile(string path)
        {
            return new PictureInterface(Cv2.ImRead(path));
        }

        public static PictureInterface FromBitmap(Bitmap bmp)
        {
            using var ms = new MemoryStream();
            bmp.Save(ms);
            ms.Seek(0, SeekOrigin.Begin);
            var data = ms.ToArray();
            Mat mat = Mat.FromImageData(data, ImreadModes.Color);
            return new PictureInterface(mat);
        }

        public void Resize(OpenCvSharp.Size size)
        {
            Cv2.Resize(_mat, _mat, size);
        }

        public PictureInterface Clone()
        {
            return new PictureInterface(_mat.Clone());
        }

        public void ISP()
        {
            // Split the channels
            Mat[] imageRGB = Cv2.Split(_mat);

            // Calculate mean for each channel
            Scalar meanB = Cv2.Mean(imageRGB[0]);
            Scalar meanG = Cv2.Mean(imageRGB[1]);
            Scalar meanR = Cv2.Mean(imageRGB[2]);
            double B = meanB.Val0;
            double G = meanG.Val0;
            double R = meanR.Val0;

            // Calculate gain for each channel
            double KB = (R + G + B) / (3 * B);
            double KG = (R + G + B) / (3 * G);
            double KR = (R + G + B) / (3 * R);

            // Adjust each channel
            imageRGB[0].ConvertTo(imageRGB[0], imageRGB[0].Type(), KB);
            imageRGB[1].ConvertTo(imageRGB[1], imageRGB[1].Type(), KG);
            imageRGB[2].ConvertTo(imageRGB[2], imageRGB[2].Type(), KR);

            // Merge channels back
            Cv2.Merge(imageRGB, _mat);

            // Dispose temporary Mats
            foreach (var m in imageRGB)
            {
                m.Dispose();
            }
        }

        public void ToGray()
        {
            Cv2.CvtColor(_mat, _mat, ColorConversionCodes.BGR2GRAY);
        }

        public void Canny(double threshold1, double threshold2)
        {
            Cv2.Canny(_mat, _mat, threshold1, threshold2);
        }

        public void Save(string path)
        {
            _mat.SaveImage(path);
        }

        public Bitmap ToBitmap()
        {
            using var ms = ToMemoryStream();
            ms.Seek(0, SeekOrigin.Begin);
            return new Bitmap(ms);
        }

        public MemoryStream ToMemoryStream()
        {
            var ms = new MemoryStream();
            var data = _mat.ToBytes();
            ms.Write(data, 0, data.Length);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        public void ToBinary(double thresh = 0, double maxval = 255, ThresholdTypes type = ThresholdTypes.Binary | ThresholdTypes.Otsu)
        {
            using var gray = new Mat();
            Cv2.Threshold(_mat, gray, thresh, maxval, type);
            gray.CopyTo(_mat);
        }

        public void color_track(string upcolor, string downcolor)
        {
            // 解析16进制色彩字符串为RGB
            (int R, int G, int B) ParseHexColor(string hex)
            {
                hex = hex?.TrimStart('#') ?? "";
                if (hex.Length == 6)
                {
                    try
                    {
                        int r = Convert.ToInt32(hex.Substring(0, 2), 16);
                        int g = Convert.ToInt32(hex.Substring(2, 2), 16);
                        int b = Convert.ToInt32(hex.Substring(4, 2), 16);
                        return (r, g, b);
                    }
                    catch { }
                }
                return (0, 0, 0);
            }
            var upRgb = ParseHexColor(upcolor);
            var downRgb = ParseHexColor(downcolor);

            // 转换upcolor到HSV
            using var upMat = new Mat(1, 1, MatType.CV_8UC3, new Scalar(upRgb.B, upRgb.G, upRgb.R));
            using var upHsv = new Mat();
            Cv2.CvtColor(upMat, upHsv, ColorConversionCodes.BGR2HSV);
            var upHsvVec = upHsv.Get<Vec3b>(0, 0);
            int upH = upHsvVec.Item0;
            int upS = upHsvVec.Item1;
            int upV = upHsvVec.Item2;

            // 转换downcolor到HSV
            using var downMat = new Mat(1, 1, MatType.CV_8UC3, new Scalar(downRgb.B, downRgb.G, downRgb.R));
            using var downHsv = new Mat();
            Cv2.CvtColor(downMat, downHsv, ColorConversionCodes.BGR2HSV);
            var downHsvVec = downHsv.Get<Vec3b>(0, 0);
            int downH = downHsvVec.Item0;
            int downS = downHsvVec.Item1;
            int downV = downHsvVec.Item2;

            // 自动调整上下限，保证down<=up
            int minH = Math.Min(downH, upH);
            int maxH = Math.Max(downH, upH);
            int minS = Math.Min(downS, upS);
            int maxS = Math.Max(downS, upS);
            int minV = Math.Min(downV, upV);
            int maxV = Math.Max(downV, upV);

            Debug.WriteLine($"{upcolor} RGB({upRgb.R},{upRgb.G},{upRgb.B}) -> HSV({upH},{upS},{upV})");
            Debug.WriteLine($"{downcolor} RGB({downRgb.R},{downRgb.G},{downRgb.B}) -> HSV({downH},{downS},{downV})");
            // 你可以根据需要返回或保存HSV值

            Mat pic = new Mat();
            Cv2.CvtColor(_mat, pic, ColorConversionCodes.BGR2HSV);
            Cv2.InRange(pic, new Scalar(minH, minS, minV), new Scalar(maxH, maxS, maxV), _mat);
            pic = _mat.Clone();
            Cv2.Erode(_mat, pic, null, null, 2);
            Cv2.Dilate(pic, _mat, null, null, 2);
            // pic = _mat.Clone();
            // Cv2.BitwiseAnd(pic, pic, _mat, null);
            // _mat = pic.Clone();

        }


        public void Dispose()
        {
            _mat?.Dispose();
        }
    }

    public partial class OpencvModelViewModel : ObservableObject
    {
        public ObservableCollection<string> CameraDevices { get; } = new ObservableCollection<string>();
        [ObservableProperty]
        private int selectedCameraIndex;
        private Bitmap? cameraImage;
        public Bitmap? CameraImage
        {
            get => cameraImage;
            set
            {
                SetProperty(ref cameraImage, value);
                if (value != null)
                {
                    ImageAspectRatio = (double)value.PixelSize.Width / value.PixelSize.Height;
                }
            }
        }
        private Thread? _cameraThread;
        private bool _running = false;

        private readonly object _lock = new();
        private PictureInterface _picture = new PictureInterface(new Mat());
        // private bool _isCannyMode = true;
        [ObservableProperty]
        private string cannyButtonText = "canny";
        // private bool _isIspMode = false;
        [ObservableProperty]
        private string ispButtonText = "白平衡";

        [ObservableProperty]
        private double clickCoordX;
        [ObservableProperty]
        private double clickCoordY;
        [ObservableProperty]
        private string clickCoordText = string.Empty;
        [ObservableProperty]
        private bool isCoordVisible = false;
        [ObservableProperty]
        private Thickness clickCoordMargin;
        [ObservableProperty]
        private bool isCannyEnabled = false;
        [ObservableProperty]
        private bool isIspEnabled = false;
        [ObservableProperty]
        private bool isOriginalEnabled = true;
        [ObservableProperty]
        private bool isBinaryEnabled = false;
        [ObservableProperty]
        private double imageAspectRatio = 4.0 / 3.0; // 默认宽高比
        [ObservableProperty]
        private int cannyThreshold1 = 100;
        [ObservableProperty]
        private int cannyThreshold2 = 200;
        [ObservableProperty]
        private bool isColorTrackEnabled = false;
        [ObservableProperty]
        private string colorTrackUp = "#444444";
        [ObservableProperty]
        private string colorTrackDown = "#000000";

        public OpencvModelViewModel()
        {
            RefreshCameraDevices();
            StartCamera();
        }

        public void RefreshCameraDevices()
        {
            CameraDevices.Clear();
            for (int i = 0; i < 5; i++)
            {
                using var cap = new VideoCapture(i);
                if (cap.IsOpened())
                {
                    CameraDevices.Add($"摄像头 {i}");
                    cap.Release();
                }
            }
            if (CameraDevices.Count > 0)
                SelectedCameraIndex = 0;
        }

        partial void OnSelectedCameraIndexChanged(int value)
        {
            StartCamera();
        }

        public void Source_ComboBox_DropDownOpened(object? sender, System.EventArgs e)
        {
            RefreshCameraDevices();
        }

        private void StartCamera()
        {
            _running = false;
            _cameraThread?.Join();
            _running = true;
            _cameraThread = new Thread(CameraLoop) { IsBackground = true };
            _cameraThread.Start();
        }


        private void CameraLoop()
        {
            using var cap = new VideoCapture(SelectedCameraIndex);
            while (_running)
            {
                lock (_lock)
                {
                    cap.Read(_picture.Mat);
                    if (!_picture.Mat.Empty())
                    {
                        Bitmap bmp;
                        var tempPic = _picture.Clone();
                        // 处理顺序：ISP -> Gray -> Binary -> Canny -> ColorTrack
                        if (IsIspEnabled)
                            tempPic.ISP();
                        if (IsBinaryEnabled)
                        {
                            tempPic.ToGray();
                            tempPic.ToBinary();
                        }
                        if (IsCannyEnabled)
                        {
                            if (!IsBinaryEnabled)
                                tempPic.ToGray();
                            tempPic.Canny(CannyThreshold1, CannyThreshold2);
                        }
                        if (IsColorTrackEnabled)
                        {
                            tempPic.color_track(ColorTrackUp, ColorTrackDown);
                        }
                        using var ms = tempPic.ToMemoryStream();
                        ms.Seek(0, System.IO.SeekOrigin.Begin);
                        bmp = new Bitmap(ms);
                        tempPic.Dispose();
                        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                        {
                            CameraImage = bmp;
                        });
                    }
                }
                Thread.Sleep(30);
            }
        }

        [RelayCommand]
        private void Canny()
        {
            IsCannyEnabled = !IsCannyEnabled;
            IsOriginalEnabled = false;
        }

        [RelayCommand]
        private void Isp()
        {
            IsIspEnabled = !IsIspEnabled;
            IsOriginalEnabled = false;
        }

        [RelayCommand]
        private void ShowOriginal()
        {
            IsCannyEnabled = false;
            IsIspEnabled = false;
            IsBinaryEnabled = false;
        }

        public void OnImageClicked(Avalonia.Point point, double imgWidth, double imgHeight)
        {
            // 坐标显示固定在图像左上角，仅在图像范围内显示
            ClickCoordMargin = new Thickness(5, 5, 0, 0);
            ClickCoordText = $"({(int)point.X},{(int)point.Y})";
            IsCoordVisible = point.X >= 0 && point.X <= imgWidth && point.Y >= 0 && point.Y <= imgHeight;
        }
    }

}


