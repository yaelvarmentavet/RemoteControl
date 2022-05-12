using RemoteControl.UWP;
using RemoteControl.Views;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(CameraPreview), typeof(CameraPreviewRenderer))]
namespace RemoteControl.UWP
{
    public class CameraPreviewRenderer : ViewRenderer<CameraPreview, Windows.UI.Xaml.Controls.CaptureElement>
    {
        private CaptureElement CaptureElement;
        //private static bool TaskRunning = false;
        //private CaptureElement CaptureElementPrev;
        //private bool Connected = false;
        //bool _isPreviewing;
        protected override void OnElementChanged(ElementChangedEventArgs<CameraPreview> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // Unsubscribe
                //Tapped -= OnCameraPreviewTapped;
                Task.Run(async () =>
                {
                    await Disconnect();
                });
            }
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    CaptureElement = new CaptureElement();
                    CaptureElement.Stretch = Stretch.UniformToFill;

                    //SetupCamera();
                    //App.UsbCamera.EUsbCameraGUI += Connect;
                    Task.Run(async () =>
                    {
                        await Connect();
                    });
                    SetNativeControl(CaptureElement);
                }
                // Subscribe
                //Tapped += OnCameraPreviewTapped;
            }
        }

        private async Task Disconnect()
        {
            await CaptureElement?.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                //var wait = new SpinWait();
                //while (TaskRunning)
                //    wait.SpinOnce();
                //if (!TaskRunning)
                //{
                //TaskRunning = true;
                MediaCapture mediaCapture = new MediaCapture();
                if (App.CaptureElements.Where(c =>
                {
                    if (c.Value == CaptureElement)
                    {
                        mediaCapture = c.Key;
                        return true;
                    }
                    else
                        return false;
                }).Any())
                {
                    //await mediaCapture.StopPreviewAsync();
                    App.CaptureElements.Remove(mediaCapture, out CaptureElement captureElement);
                }
                //    TaskRunning = false;
                //}
            });
        }

        private async Task Connect()
        {
            await CaptureElement?.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                //var wait = new SpinWait();
                //while (TaskRunning)
                //    wait.SpinOnce();
                //if (!TaskRunning)
                //{
                //TaskRunning = true;
                MediaCapture mediaCapture = new MediaCapture();
                if ((App.UsbCamera.MediaCaptures.Where(m =>
                {
                    if (!App.CaptureElements.ContainsKey(m.Value))
                    {
                        mediaCapture = m.Value; return true;
                    }
                    else return false;
                })).Any())
                {
                    App.CaptureElements.GetOrAdd(mediaCapture, CaptureElement);
                    CaptureElement.Source = mediaCapture;
                    try
                    {
                        await mediaCapture.StartPreviewAsync();
                    }
                    catch
                    {
                        await mediaCapture.StopPreviewAsync();
                        await mediaCapture.StartPreviewAsync();
                    }
                }
                //    TaskRunning = false;
                //}
            });
        }

        //async void OnCameraPreviewTapped(object sender, TappedRoutedEventArgs e)
        //{
        //    if (_isPreviewing)
        //    {
        //        await StopPreviewAsync();
        //    }
        //    else
        //    {
        //        await StartPreviewAsync();
        //    }
        //}

        //private Task StartPreviewAsync()
        //{
        //    return Task.CompletedTask;
        //}

        //private Task StopPreviewAsync()
        //{
        //    return Task.CompletedTask;
        //}
    }
}
