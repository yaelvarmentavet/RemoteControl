using RemoteControl.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.Media.MediaProperties;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Controls;
using Windows.Storage.Streams;
using Windows.Storage;
using Xamarin.Forms;
//using Xamarin.Forms;

namespace RemoteControl.UWP
{
    class UsbCamera : IUsbCamera
    {
        private DeviceWatcher Watcher;
        private Dictionary<string, MediaFrameSourceGroup> Cameras = new Dictionary<string, MediaFrameSourceGroup>();
        private Semaphore SemaphoreConnect = new Semaphore(1, 1);
        private MediaCapture mediaCapture;
        private MediaFrameReader mediaFrameReader;
        private SoftwareBitmap backBuffer;
        private bool taskRunning = false;
        private SoftwareBitmapSource imageSource;
        private EventHandler EventSource;
        private Windows.UI.Xaml.Controls.Image imageElement;
        private Xamarin.Forms.Image Image;
        private string OutputFile;

        public UsbCamera()
        {
            Watcher = DeviceInformation.CreateWatcher(DeviceClass.All);
            Watcher.Added += DeviceAdded;
            Watcher.Removed += DeviceRemoved;
            Watcher.EnumerationCompleted += DeviceEnumerationCompleted;
            Watcher.Updated += DeviceUpdated;
            Watcher.Stopped += DeviceStopped;
            //DeviceWatcherTrigger deviceWatcherTrigger = deviceWatcher.GetBackgroundTrigger(new List<DeviceWatcherEventKind>() { DeviceWatcherEventKind.Add, DeviceWatcherEventKind.Remove });
            Watcher.Start();
        }

        private void DeviceStopped(DeviceWatcher sender, object args)
        {
        }

        private void DeviceUpdated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
        }

        private void DeviceEnumerationCompleted(DeviceWatcher sender, object args)
        {
            new Thread(async () =>
            {
                SemaphoreConnect.WaitOne();
                await Connect();
                SemaphoreConnect.Release();
            }).Start();
        }

        private void DeviceRemoved(DeviceWatcher sender, DeviceInformationUpdate args)
        {
        }

        private void DeviceAdded(DeviceWatcher sender, DeviceInformation args)
        {
        }

        private async Task<bool> Connect()
        {
            DeviceInformationCollection mediaDeviceInfos = await DeviceInformation.FindAllAsync(MediaFrameSourceGroup.GetDeviceSelector());

            foreach (DeviceInformation mediaDeviceInfo in mediaDeviceInfos)
            {
                await Connect(mediaDeviceInfo.Id);
            }

            var frameSourceGroups = await MediaFrameSourceGroup.FindAllAsync();

            var selectedGroupObjects = frameSourceGroups.Select(group =>
               new
               {
                   sourceGroup = group,
                   colorSourceInfo = group.SourceInfos.FirstOrDefault((sourceInfo) =>
                   {
                       // On Xbox/Kinect, omit the MediaStreamType and EnclosureLocation tests
                       return //sourceInfo.MediaStreamType == MediaStreamType.VideoPreview
                        group.DisplayName.Contains("2K HD")
                        && sourceInfo.SourceKind == MediaFrameSourceKind.Color;
                       //&& sourceInfo.DeviceInformation?.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Front;
                   })

               }).Where(t => t.colorSourceInfo != null)
               .FirstOrDefault();

            MediaFrameSourceGroup selectedGroup = selectedGroupObjects?.sourceGroup;
            MediaFrameSourceInfo colorSourceInfo = selectedGroupObjects?.colorSourceInfo;

            if (selectedGroup == null)
            {
                return false;
            }

            mediaCapture = new MediaCapture();
            var settings = new MediaCaptureInitializationSettings()
            {
                SourceGroup = selectedGroup,
                SharingMode = MediaCaptureSharingMode.ExclusiveControl,
                MemoryPreference = MediaCaptureMemoryPreference.Cpu,
                StreamingCaptureMode = StreamingCaptureMode.Video
            };
            try
            {
                await mediaCapture.InitializeAsync(settings);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("MediaCapture initialization failed: " + ex.Message);
            }

            MediaFrameSource colorFrameSource = mediaCapture.FrameSources[colorSourceInfo.Id];

            mediaFrameReader = await mediaCapture.CreateFrameReaderAsync(colorFrameSource, MediaEncodingSubtypes.Argb32);
            mediaFrameReader.FrameArrived += ColorFrameReader_FrameArrived;
            //await mediaFrameReader.StartAsync();
            return true;
        }

        private void ColorFrameReader_FrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            var mediaFrameReference = sender.TryAcquireLatestFrame();
            var videoMediaFrame = mediaFrameReference?.VideoMediaFrame;
            var softwareBitmap = videoMediaFrame?.SoftwareBitmap;

            if (softwareBitmap != null)
            {
                if (softwareBitmap.BitmapPixelFormat != Windows.Graphics.Imaging.BitmapPixelFormat.Bgra8 ||
                    softwareBitmap.BitmapAlphaMode != Windows.Graphics.Imaging.BitmapAlphaMode.Premultiplied)
                {
                    softwareBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                }
                //WriteableBitmap bitmap = new WriteableBitmap(softwareBitmap.PixelWidth, softwareBitmap.PixelHeight);

                //softwareBitmap.CopyToBuffer(bitmap.PixelBuffer);

                //using (Stream stream = bitmap.PixelBuffer.AsStream())
                //{
                //    await stream.WriteAsync(sourcePixels, 0, sourcePixels.Length);
                //}


                Task.Run(async () =>
                {
                    if (taskRunning)
                    {
                        return;
                    }
                    taskRunning = true;
                    {
                            //StorageFile.CreateStreamedFileAsync
                            await SaveSoftwareBitmapToFile(softwareBitmap);
                        }
                    taskRunning = false;
                });
                return;

                // Swap the processed frame to _backBuffer and dispose of the unused image.
                softwareBitmap = Interlocked.Exchange(ref backBuffer, softwareBitmap);
                softwareBitmap?.Dispose();

                // Changes to XAML ImageElement must happen on UI thread through Dispatcher
                var task = imageElement?.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    async () =>
                    {
                        // Don't let two copies of this task run at the same time.
                        if (taskRunning)
                        {
                            return;
                        }
                        taskRunning = true;

                        // Keep draining frames from the backbuffer until the backbuffer is empty.
                        SoftwareBitmap latestBitmap;
                        while ((latestBitmap = Interlocked.Exchange(ref backBuffer, null)) != null)
                        {
                            if (imageElement.Source == null)
                                imageElement.Source = new SoftwareBitmapSource();
                            SoftwareBitmapSource imageSource = (SoftwareBitmapSource)imageElement.Source;
                            await imageSource.SetBitmapAsync(latestBitmap);
                            latestBitmap.Dispose();
                        }

                        taskRunning = false;
                    });
            }

            mediaFrameReference?.Dispose();
        }

        //private async Task SaveSoftwareBitmapToFile(SoftwareBitmap softwareBitmap, StorageFile outputFile)
        private async Task SaveSoftwareBitmapToFile(SoftwareBitmap softwareBitmap)
        {
            //using (IRandomAccessStream stream = await outputFile.OpenAsync(FileAccessMode.ReadWrite))
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                // Create an encoder with the desired format
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);

                // Set the software bitmap
                encoder.SetSoftwareBitmap(softwareBitmap);

                // Set additional encoding parameters, if needed
                encoder.BitmapTransform.ScaledWidth = 320;
                encoder.BitmapTransform.ScaledHeight = 240;
                encoder.BitmapTransform.Rotation = Windows.Graphics.Imaging.BitmapRotation.Clockwise90Degrees;
                encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Fant;
                encoder.IsThumbnailGenerated = true;

                try
                {
                    await encoder.FlushAsync();
                }
                catch (Exception err)
                {
                    const int WINCODEC_ERR_UNSUPPORTEDOPERATION = unchecked((int)0x88982F81);
                    switch (err.HResult)
                    {
                        case WINCODEC_ERR_UNSUPPORTEDOPERATION:
                            // If the encoder does not support writing a thumbnail, then try again
                            // but disable thumbnail generation.
                            encoder.IsThumbnailGenerated = false;
                            break;
                        default:
                            throw;
                    }
                }

                if (encoder.IsThumbnailGenerated == false)
                {
                    await encoder.FlushAsync();
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    //EventSource.Invoke(this, new StreamEventArgs() { Stream = stream.AsStream() });
                    if(Image != null)
                        Image.Source = Xamarin.Forms.ImageSource.FromStream(() => stream.AsStream());
                });
            }
        }
        
        private async Task Connect(string id)
        {
            try
            {
                //SemaphoreConnect.WaitOne();
                MediaFrameSourceGroup camera = await MediaFrameSourceGroup.FromIdAsync(id);
                
                if (camera != null)
                {
                    Cameras.Add(id, camera);
                }
            }
            catch
            {
                if (Cameras.ContainsKey(id))
                {
                    Cameras.Remove(id);
                }
            }
            finally
            {
                //SemaphoreConnect.Release();
            }
        }

        public void ImageSet(Xamarin.Forms.Image image)
        {
            Image = image;
        }

        public void File(string outputFile)
        {
            OutputFile = outputFile;
        }

        public void Event(EventHandler eventSource)
        {
            EventSource = eventSource;
        }
    }
}
