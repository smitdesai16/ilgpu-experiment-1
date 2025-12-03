using AForge.Video;
using AForge.Video.DirectShow;
using CPUvsGPU.Models;
using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using ILGPU.Runtime.Cuda;
using ILGPU.Runtime.OpenCL;
using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CPUvsGPU
{
    public partial class Form1 : Form
    {
        private List<Camera> cameras = new List<Camera>();
        private Camera selectedCamera = null;
        private VideoCaptureDevice videoSource = null;

        Context context = Context.CreateDefault();
        private List<ProcessingUnit> processingUnits = new List<ProcessingUnit>();
        private ProcessingUnit selectedProcessingUnit = null;
        private Accelerator accelerator = null;

        private Stopwatch stopwatch = new Stopwatch();
        private int frameCount = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                GetCameras();
                GetAccelerators();

                InitializeForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
            foreach (var device in context)
            {
                processingUnits.Add(new ProcessingUnit()
                {
                    Name = device.Name,
                    Type = device.AcceleratorType,
                    Index = 0 // todo fetch correct index
                });
            }
        }

        private void InitializeForm()
        {
            comboBoxCamera.Items.Clear();
            foreach (var camera in cameras)
            {
                comboBoxCamera.Items.Add(camera.Name);
            }
            if (comboBoxCamera.Items.Count > 0)
            {
                comboBoxCamera.SelectedIndex = 0;
            }

            comboBoxAccelerator.Items.Clear();
            foreach (var processingUnit in processingUnits)
            {
                comboBoxAccelerator.Items.Add(processingUnit.Name);
            }
            if (comboBoxAccelerator.Items.Count > 0)
            {
                comboBoxAccelerator.SelectedIndex = 0;
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

        private void comboBoxAccelerator_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxAccelerator.SelectedItem != null && selectedProcessingUnit != processingUnits[comboBoxAccelerator.SelectedIndex])
            {
                selectedProcessingUnit = processingUnits[comboBoxAccelerator.SelectedIndex];

                if (accelerator != null)
                {
                    accelerator.Dispose();
                }

                switch (selectedProcessingUnit.Type)
                {
                    case AcceleratorType.CPU:
                        accelerator = context.CreateCPUAccelerator(selectedProcessingUnit.Index);
                        break;
                    case AcceleratorType.Cuda:
                        accelerator = context.CreateCudaAccelerator(selectedProcessingUnit.Index);
                        break;
                    case AcceleratorType.OpenCL:
                        accelerator = context.CreateCLAccelerator(selectedProcessingUnit.Index);
                        break;
                }

                textBoxAcceleratorDetails.Text = @$"Accelerator Details

Name: {accelerator.Name}

Type: {accelerator.AcceleratorType}

Memory Size: {accelerator.MemorySize / Math.Pow(1024, 3)}

Max Threads: {accelerator.NumMultiprocessors * accelerator.MaxNumThreadsPerMultiprocessor}
";
            }
        }

        private void StartVideoSource()
        {
            CloseExistingVideoSource(true);
            if (selectedCamera != null)
            {
                videoSource = new VideoCaptureDevice(selectedCamera.MonikerString);
                videoSource.NewFrame += VideoSource_NewFrame;
                videoSource.Start();
                stopwatch.Start();
            }
        }

        private void CloseExistingVideoSource(bool waitForStop)
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.NewFrame -= VideoSource_NewFrame;
                if (waitForStop)
                {
                    videoSource.WaitForStop();
                }
                stopwatch.Stop();
            }
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();

                var oldImage = pictureBoxVideoOutput.Image as IDisposable;

                if (checkBoxFilter.Checked)
                {
                    int width = bitmap.Width;
                    int height = bitmap.Height;
                    int pixelCount = width * height;
                    int byteCount = pixelCount * 4;

                    byte[] pixels = new byte[byteCount];
                    BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                    Marshal.Copy(bmpData.Scan0, pixels, 0, byteCount);

                    using var devPixels = accelerator.Allocate1D(pixels);
                    var kernel = accelerator.LoadAutoGroupedStreamKernel<Index1D, ArrayView<byte>>(KernelInverPixel);
                    kernel(pixelCount, devPixels.View);
                    accelerator.Synchronize();
                    pixels = devPixels.GetAsArray1D();

                    Marshal.Copy(pixels, 0, bmpData.Scan0, byteCount);
                    bitmap.UnlockBits(bmpData);
                }

                this.Invoke((MethodInvoker)(() =>
                {
                    pictureBoxVideoOutput.Image = bitmap;
                }));

                oldImage?.Dispose();

                frameCount++;
                if (stopwatch.ElapsedMilliseconds >= 1000)
                {
                    int fps = frameCount;
                    frameCount = 0;
                    stopwatch.Restart();
                    this.Invoke((MethodInvoker)(() =>
                    {
                        labelFPS.Text = fps.ToString();
                    }));
                }
                bitmap = null;
            }
            catch (Exception ex)
            {
            }
        }

        static void KernelInverPixel(Index1D index, ArrayView<byte> pixel)
        {
            int baseIdx = index * 4; // ARGB

            byte a = pixel[baseIdx + 0];
            byte r = pixel[baseIdx + 1];
            byte g = pixel[baseIdx + 2];
            byte b = pixel[baseIdx + 3];

            pixel[baseIdx + 0] = a;
            pixel[baseIdx + 1] = (byte)(255 - r);
            pixel[baseIdx + 2] = (byte)(255 - g);
            pixel[baseIdx + 3] = (byte)(255 - b);
        }

        static void KernelGrayPixel(Index1D index, ArrayView<byte> pixel)
        {
            int baseIdx = index * 4; // Format32bppArgb in memory is BGRA (B,G,R,A)

            // Read in BGRA order (lowest address = Blue)
            byte b = pixel[baseIdx + 0];
            byte g = pixel[baseIdx + 1];
            byte r = pixel[baseIdx + 2];
            byte a = pixel[baseIdx + 3];

            // Use standard luminance coefficients for human perception
            float rf = (float)r;
            float gf = (float)g;
            float bf = (float)b;
            byte gray = (byte)(rf * 0.299f + gf * 0.587f + bf * 0.114f + 0.5f); // +0.5f for rounding

            // Write back in BGRA order, preserving alpha
            pixel[baseIdx + 0] = gray;
            pixel[baseIdx + 1] = gray;
            pixel[baseIdx + 2] = gray;
            pixel[baseIdx + 3] = a;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseExistingVideoSource(false);
            accelerator?.Dispose();
            context?.Dispose();
        }

        private void checkBoxFilter_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxFilter.Checked)
            {
                comboBoxAccelerator.Enabled = false;
            }
            else
            {
                comboBoxAccelerator.Enabled = true;
            }
        }
    }
}
