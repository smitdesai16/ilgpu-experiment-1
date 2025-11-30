using AForge.Video;
using AForge.Video.DirectShow;
using CPUvsGPU.Models;
using System.Diagnostics;

namespace CPUvsGPU
{
    public partial class Form1 : Form
    {
        private List<Camera> cameras = new List<Camera>();
        private Camera selectedCamera = null;
        private VideoCaptureDevice videoSource;

        private Stopwatch stopwatch = new Stopwatch();
        private int frameCount = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GetCameras();
            GetAccelerators();

            InitializeForm();
        }

        private void GetCameras()
        {
            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in videoDevices)
            {
                Camera camera = new Camera
                {
                    Name = device.Name,
                    MonikerString = device.MonikerString
                };
                cameras.Add(camera);
            }
        }

        private void GetAccelerators()
        {

        }

        private void InitializeForm()
        {
            comboBoxCamera.Items.Clear();

            foreach (var camera in cameras)
            {
                comboBoxCamera.Items.Add(camera.Name);
            }

            if(comboBoxCamera.Items.Count > 0)
            {
                comboBoxCamera.SelectedIndex = 0;
            }
        }

        private void comboBoxCamera_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxCamera.SelectedItem != null && selectedCamera != cameras[comboBoxCamera.SelectedIndex])
            {
                selectedCamera = cameras[comboBoxCamera.SelectedIndex];

                StartVideoSource();
            }
        }

        private void StartVideoSource()
        {
            CloseExistingVideoSource();
            if (selectedCamera != null)
            {
                videoSource = new VideoCaptureDevice(selectedCamera.MonikerString);
                videoSource.NewFrame += VideoSource_NewFrame;
                videoSource.Start();
                stopwatch.Start();
            }
        }

        private void CloseExistingVideoSource()
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.NewFrame -= VideoSource_NewFrame;
                videoSource.WaitForStop();
                stopwatch.Stop();
            }
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap frame = (Bitmap)eventArgs.Frame.Clone();
            
            //ProcessFrame(frame);

            pictureBoxVideoOutput.Image = frame;

            frameCount++;
            if (stopwatch.ElapsedMilliseconds >= 1000)
            {
                int fps = frameCount;
                frameCount = 0;
                stopwatch.Restart();
                labelFPS.Text = fps.ToString();
            }
        }

        private void ProcessFrame(Bitmap frame)
        {
            Parallel.For(0, 2, new ParallelOptions() { MaxDegreeOfParallelism = 16 }, (index) =>
            {
                int y = index / frame.Width;
                int x = index % frame.Width;
                Color pixelColor = frame.GetPixel(x, y);
                int grayValue = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11);
                Color grayColor = Color.FromArgb(grayValue, grayValue, grayValue);
                frame.SetPixel(x, y, grayColor);
            });

            //for (int y = 0; y < frame.Height; y++)
            //{
            //    for (int x = 0; x < frame.Width; x++)
            //    {
            //        Color pixelColor = frame.GetPixel(x, y);
            //        int grayValue = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11);
            //        Color grayColor = Color.FromArgb(grayValue, grayValue, grayValue);
            //        frame.SetPixel(x, y, grayColor);
            //    }
            //}
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseExistingVideoSource();
        }
    }
}
