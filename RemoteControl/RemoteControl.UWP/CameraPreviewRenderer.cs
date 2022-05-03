using RemoteControl.UWP;
using RemoteControl.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(CameraPreview), typeof(CameraPreviewRenderer))]
namespace RemoteControl.UWP
{
    public class CameraPreviewRenderer : ViewRenderer<CameraPreview, Windows.UI.Xaml.Controls.CaptureElement>
    {
        CaptureElement CaptureElement;
        //bool _isPreviewing;

        protected override void OnElementChanged(ElementChangedEventArgs<CameraPreview> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // Unsubscribe
                //Tapped -= OnCameraPreviewTapped;
            }
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    CaptureElement = new CaptureElement();
                    CaptureElement.Stretch = Stretch.UniformToFill;

                    SetupCamera();
                    SetNativeControl(CaptureElement);
                }
                // Subscribe
                //Tapped += OnCameraPreviewTapped;
            }
        }

        private async Task SetupCamera()
        {
            MediaCapture mediaCapture = new MediaCapture();
            if ((App.UsbCamera.MediaCaptures.Where(m => { if (!App.UsbCameras.ContainsValue(m)) { mediaCapture = m; return true; } else return false; })).Any())
            {
                App.UsbCameras.Add(CaptureElement.Name, mediaCapture);
                CaptureElement.Source = mediaCapture;
                await mediaCapture.StartPreviewAsync();
            }
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
