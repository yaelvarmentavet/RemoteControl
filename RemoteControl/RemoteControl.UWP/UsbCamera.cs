using RemoteControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.Media.MediaProperties;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace RemoteControl.UWP
{
    class UsbCamera : IUsbCamera
    {
        private DeviceWatcher Watcher;
        private Dictionary<string, MediaFrameSourceGroup> Cameras = new Dictionary<string, MediaFrameSourceGroup>();
        private Semaphore SemaphoreConnect = new Semaphore(1, 1);
        private MediaCapture MediaCapture;
        private MediaFrameReader MediaFrameReader;
        private SoftwareBitmap BackBuffer;
        private bool TaskRunning = false;
        private SoftwareBitmapSource ImageSource;
        private EventHandler EventSource;
        private Image ImageElement;

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

            MediaCapture = new MediaCapture();
            var settings = new MediaCaptureInitializationSettings()
            {
                SourceGroup = selectedGroup,
                SharingMode = MediaCaptureSharingMode.ExclusiveControl,
                MemoryPreference = MediaCaptureMemoryPreference.Cpu,
                StreamingCaptureMode = StreamingCaptureMode.Video
            };
            try
            {
                await MediaCapture.InitializeAsync(settings);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("MediaCapture initialization failed: " + ex.Message);
            }

            MediaFrameSource colorFrameSource = MediaCapture.FrameSources[colorSourceInfo.Id];

            MediaFrameReader = await MediaCapture.CreateFrameReaderAsync(colorFrameSource, MediaEncodingSubtypes.Argb32);
            MediaFrameReader.FrameArrived += ColorFrameReader_FrameArrived;
            //await MediaFrameReader.StartAsync();
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

                // Swap the processed frame to _backBuffer and dispose of the unused image.
                softwareBitmap = Interlocked.Exchange(ref BackBuffer, softwareBitmap);
                softwareBitmap?.Dispose();

                // Changes to XAML ImageElement must happen on UI thread through Dispatcher
                var task = ImageElement?.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    // Don't let two copies of this task run at the same time.
                    if (TaskRunning)
                    {
                        return;
                    }
                    TaskRunning = true;

                    // Keep draining frames from the backbuffer until the backbuffer is empty.
                    SoftwareBitmap latestBitmap;
                    while ((latestBitmap = Interlocked.Exchange(ref BackBuffer, null)) != null)
                    {
                        if (ImageElement.Source == null)
                            ImageElement.Source = new SoftwareBitmapSource();
                        SoftwareBitmapSource imageSource = (SoftwareBitmapSource)ImageElement.Source;
                        await imageSource.SetBitmapAsync(latestBitmap);
                        latestBitmap.Dispose();
                    }

                    TaskRunning = false;
                });
            }

            mediaFrameReference?.Dispose();
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

        public void Image(object image)
        {
            ImageElement = (Image)image;
        }

        public void Event(EventHandler eventSource)
        {
            EventSource = eventSource;
        }
    }
}
