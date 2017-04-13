using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.Media.Streaming.Adaptive;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Media.Playback;
using Windows.Media.Core;

using Windows.Media.Capture;
using Windows.ApplicationModel;
using Windows.System.Display;
using Windows.Graphics.Display;
using System.Diagnostics;
using Windows.Devices.Enumeration;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.Media;
using Windows.UI.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IntelligentKiosk
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MediaCapture _mediaCapture;
        private DisplayRequest _displayRequest=new DisplayRequest();
        private readonly DisplayInformation _displayInformation = DisplayInformation.GetForCurrentView();
        private readonly SystemMediaTransportControls _systemMediaControls = SystemMediaTransportControls.GetForCurrentView();

        private bool _isPreviewing=false;
        private bool _isInitialized=false;
        private bool _mirroringPreview=false;
        private bool _externalCamera=false;


        public MainPage()
        {
            this.InitializeComponent();

            Uri manifestUri = new Uri("");
            _mediaPlayerElement.Source = MediaSource.CreateFromUri(manifestUri);
            _mediaPlayerElement.MediaPlayer.Play();

            
        }

        
        // start preview async  @jack
        private async Task StartPreviewAsync()
        {
            Debug.WriteLine("StartPreviewAsync");

            // prevent sleep mode  @jack
            _displayRequest.RequestActive();

            _systemMediaControls.PropertyChanged += SystemMediaControls_PropertyChanged;

            PreviewControl.Source = _mediaCapture;
            PreviewControl.FlowDirection = _mirroringPreview ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            // start the preview  @jack
            await _mediaCapture.StartPreviewAsync();
            _isPreviewing = true;
        }

        // stop preview async  @jack
        private async Task StopPreviewAsync()
        {
            _isPreviewing = false;
            await _mediaCapture.StopPreviewAsync();

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                PreviewControl.Source = null;

                _displayRequest.RequestRelease();
            });
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await InitializeCameraAsync();
        }

        private async Task InitializeCameraAsync()
        {
            Debug.WriteLine("InitializeCameraAync");

            if (_mediaCapture == null)
            {
                // attempt to get the back camera if one is availale, but use any camera device if not  @jack
                var cameraDevice = await FindCameraDeviceByPanelAync(Windows.Devices.Enumeration.Panel.Back);

                if (cameraDevice == null)
                {
                    Debug.WriteLine("No camera device found!");
                    return;
                }

                _mediaCapture = new MediaCapture();

                // notification when something goes wrong  @jack
                _mediaCapture.Failed += MediaCapture_Failed;

                var settings = new MediaCaptureInitializationSettings { VideoDeviceId = cameraDevice.Id };

                // initialize mediacapture  @jack
                try
                {
                    await _mediaCapture.InitializeAsync(settings);
                    _isInitialized = true;
                }
                catch (UnauthorizedAccessException)
                {
                    Debug.WriteLine("The app was denied access to the camera");
                }

                // if initialization succeeded  @jack
                if (_isInitialized)
                {
                    if (cameraDevice.EnclosureLocation == null || cameraDevice.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Unknown)
                    {
                        _externalCamera = true;
                    }
                    else
                    {
                        _externalCamera = false;

                        _mirroringPreview = (cameraDevice.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Front);
                    }

                    await StartPreviewAsync();
                }
            }
        }

        // free the camera resource  @jack
        private async Task CleanupCameraAsync()
        {
            if (_isInitialized)
            {
                if (_isPreviewing)
                {
                    await StopPreviewAsync();
                }

                _isInitialized = false;
            }

            if (_mediaCapture != null)
            {
                _mediaCapture.Failed -= MediaCapture_Failed;
                _mediaCapture.Dispose();
                _mediaCapture = null;
            }
        }

        private async void Application_Resuming(object sender, object o)
        {
            if (Frame.CurrentSourcePageType == typeof(MainPage))
            {
                await InitializeCameraAsync();
            }
        }

        private async void Application_Suspending(object sender, SuspendingEventArgs e)
        {
            if (Frame.CurrentSourcePageType == typeof(MainPage))
            {
                var deferral = e.SuspendingOperation.GetDeferral();
                await CleanupCameraAsync();
                deferral.Complete();
            }
        }

        private static async Task<DeviceInformation> FindCameraDeviceByPanelAync(Windows.Devices.Enumeration.Panel desiredPanel)
        {
            var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            DeviceInformation desiredDevice = allVideoDevices.FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == desiredPanel);
            return desiredDevice ?? allVideoDevices.FirstOrDefault();
        }

        private async void MediaCapture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
        {
            Debug.WriteLine("MediaCapture_Failed: (0x{0:X}) {1}", errorEventArgs.Code, errorEventArgs.Message);

            await CleanupCameraAsync();
        }  

        private async void SystemMediaControls_PropertyChanged(SystemMediaTransportControls sender, SystemMediaTransportControlsPropertyChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                // Only handle this event if this page is currently being displayed
                if (args.Property == SystemMediaTransportControlsProperty.SoundLevel && Frame.CurrentSourcePageType == typeof(MainPage))
                {
                    // Check to see if the app is being muted. If so, it is being minimized.
                    // Otherwise if it is not initialized, it is being brought into focus.
                    if (sender.SoundLevel == SoundLevel.Muted)
                    {
                        await CleanupCameraAsync();
                    }
                    else if (!_isInitialized)
                    {
                        await InitializeCameraAsync();
                    }
                }
            });
        }
    }
}
