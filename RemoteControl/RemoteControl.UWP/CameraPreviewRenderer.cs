using RemoteControl.UWP;
using RemoteControl.Views;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.UI.Core;
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
        //bool _isPreviewing;
        private readonly object LockConnect = new object();
        private readonly object LockDisconnect = new object();

        protected override void OnElementChanged(ElementChangedEventArgs<CameraPreview> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // Unsubscribe
                //Tapped -= OnCameraPreviewTapped;
                Disconnect();
            }
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    CaptureElement = new CaptureElement();
                    CaptureElement.Stretch = Stretch.UniformToFill;

                    //SetupCamera();
                    //App.UsbCamera.EUsbCameraGUI += Connect;
                    Connect();
                    SetNativeControl(CaptureElement);
                }
                // Subscribe
                //Tapped += OnCameraPreviewTapped;
            }
        }

        private void Disconnect()
        {
            CaptureElement?.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                //lock (LockDisconnect)
                //{
                MediaCapture mediaCapture = new MediaCapture();
                if (App.CaptureElements.Where(p =>
                {
                    if (p.Value == CaptureElement)
                    {
                        mediaCapture = p.Key;
                        return true;
                    }
                    else
                        return false;
                }).Any())
                {
                    App.CaptureElements.Remove(mediaCapture, out CaptureElement captureElement);
                    await mediaCapture.StopPreviewAsync();
                }
                //}
            });
        }

        //private void Connect(object sender, EventArgs args)
        private void Connect()
        {
            //Task.Run( async () =>
            CaptureElement?.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                MediaCapture mediaCapture = new MediaCapture();
                //lock (LockConnect)
                //{
                if ((App.UsbCamera.MediaCaptures.Where(m => 
                { 
                    if (!App.CaptureElements.ContainsKey(m.Value)) 
                    { 
                        mediaCapture = m.Value; return true; 
                    } 
                    else return false; })).Any())
                {
                    App.CaptureElements.GetOrAdd(mediaCapture, CaptureElement);
                    CaptureElement.Source = mediaCapture;
                    await mediaCapture.StartPreviewAsync();
                }
                //}
                //await mediaCapture.StartPreviewAsync();
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
