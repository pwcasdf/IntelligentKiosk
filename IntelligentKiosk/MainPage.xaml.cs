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

using ServiceHelpers;
using IntelligentKiosk.Controls;
using Windows.UI.Popups;

using Windows.UI.Xaml.Media.Imaging;

using IntelligentKiosk.Views.TrigerInfo;
using Microsoft.ProjectOxford.Face.Contract;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IntelligentKiosk
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly DisplayInformation _displayInformation = DisplayInformation.GetForCurrentView();
        private readonly SystemMediaTransportControls _systemMediaControls = SystemMediaTransportControls.GetForCurrentView();

        private string mediaURI;
        private MediaPlaybackList playbackList = new MediaPlaybackList();
        private Dictionary<string, BitmapImage> artCache = new Dictionary<string, BitmapImage>();
        private DisplayRequest appDisplayRequest = null;

        private Recommendation currentRecommendation;

        public MainPage()
        {
            this.InitializeComponent();
            
            InitializePlaybackList();
            mediaElement.CurrentStateChanged += MediaElement_CurrentStateChanged;
            playlistView.ItemClick += PlaylistView_ItemClick;

            Window.Current.Activated += CurrentWindowActivationStateChanged;
            this.cameraControl.EnableAutoCaptureMode = true;
            this.cameraControl.FilterOutSmallFaces = true;
            this.cameraControl.AutoCaptureStateChanged += CameraControl_AutoCaptureStateChanged;
            this.cameraControl.CameraAspectRatioChanged += CameraControl_CameraAspectRatioChanged;
        }


        /***************************************************************************
         ***************************************************************************
         ****************************** media player *******************************  @jack
         ***************************************************************************
         ***************************************************************************/


        void InitializePlaybackList()
        {
            // Initialize the playlist data/view model.
            // In a production app your data would be sourced from a data store or service.

            // Add content
            var media1 = new Models.MediaModel();
            media1.Title = "A Sky Full of Stars - Coldplay";
            media1.MediaUri = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)");
            media1.ArtUri = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
            playlistView.Media.Add(media1);

            var media2 = new Models.MediaModel();
            media2.Title = "Hymm for the Weekend - Coldplay";
            media2.MediaUri = new Uri("http://seteam.streaming.mediaservices.windows.net/c3373fd7-4e2a-498d-a96f-2298303ef0ed/Hymm_for_the_weekend.ism/manifest(format=m3u8-aapl)");
            media2.ArtUri = new Uri("http://seteam.streaming.mediaservices.windows.net/c3373fd7-4e2a-498d-a96f-2298303ef0ed/Hymm_for_the_weekend_000001.png");
            playlistView.Media.Add(media2);

            var media3 = new Models.MediaModel();
            media3.Title = "Something just like this - Coldplay";
            media3.MediaUri = new Uri("http://seteam.streaming.mediaservices.windows.net/8b4077ab-b80a-4cd2-8aa5-468f63fe2795/Something_just_like_this.ism/manifest(format=m3u8-aapl)");
            media3.ArtUri = new Uri("http://seteam.streaming.mediaservices.windows.net/8b4077ab-b80a-4cd2-8aa5-468f63fe2795/Something_just_like_this_000001.png");
            playlistView.Media.Add(media3);

            var media4 = new Models.MediaModel();
            media4.Title = "Up&Up - Coldplay";
            media4.MediaUri = new Uri("http://seteam.streaming.mediaservices.windows.net/cd3e82a2-1f5d-4c0c-9412-23ff0a03e0cf/Up&Up.ism/manifest(format=m3u8-aapl)");
            media4.ArtUri = new Uri("http://seteam.streaming.mediaservices.windows.net/cd3e82a2-1f5d-4c0c-9412-23ff0a03e0cf/Up&Up_000001.png");
            playlistView.Media.Add(media4);

            var media5 = new Models.MediaModel();
            media5.Title = "Shape of You";
            media5.MediaUri = new Uri("http://seteam.streaming.mediaservices.windows.net/ae962334-b864-404a-9142-7f51b2beda59/Shape_of_You.ism/manifest(format=m3u8-aapl)");
            media5.ArtUri = new Uri("http://seteam.streaming.mediaservices.windows.net/ae962334-b864-404a-9142-7f51b2beda59/Shape_of_You_000001.png");
            playlistView.Media.Add(media5);

            // Pre-cache all album art to facilitate smooth gapless transitions.
            // A production app would have a more sophisticated object cache.
            foreach (var media in playlistView.Media)
            {
                var bitmap = new BitmapImage();
                bitmap.UriSource = media.ArtUri;
                artCache[media.ArtUri.ToString()] = bitmap;
            }

            // Initialize the playback list for this content
            foreach (var media in playlistView.Media)
            {
                var mediaSource = MediaSource.CreateFromUri(media.MediaUri);
                mediaSource.CustomProperties["uri"] = media.MediaUri;

                var playbackItem = new MediaPlaybackItem(mediaSource);

                playbackList.Items.Add(playbackItem);
            }

            // Subscribe for changes
            playbackList.CurrentItemChanged += PlaybackList_CurrentItemChanged;

            // Loop
            playbackList.AutoRepeatEnabled = true;
        }


        private void PlaybackList_CurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            var ignoreAwaitWarning = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var currentItem = sender.CurrentItem;
                playlistView.SelectedIndex = playbackList.Items.ToList().FindIndex(i => i == currentItem);
                playlistView.scrolling();
            });
        }

        /// <summary>
        /// MediaPlayer state changed event handlers. 
        /// Note that we can subscribe to events even if Media Player is playing media in background
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void MediaElement_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            var currentState = mediaElement.CurrentState; // cache outside of completion or you might get a different value
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Update state label
                txtCurrentState.Text = currentState.ToString();
            });
        }

        private void PlaylistView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as Models.MediaModel;
            Debug.WriteLine("Clicked item: " + item.MediaUri.ToString());

            // Start the background task if it wasn't running
            playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == item.MediaUri));
        }
        
        /***************************************************************************
         ***************************************************************************
         ******************************* camera work *******************************  @jack
         ***************************************************************************
         ***************************************************************************/


        private void CameraControl_CameraAspectRatioChanged(object sender, EventArgs e)
        {
            this.UpdateCameraHostSize();
        }

        private async void CurrentWindowActivationStateChanged(object sender, WindowActivatedEventArgs e)
        {
            if ((e.WindowActivationState == CoreWindowActivationState.CodeActivated ||
                e.WindowActivationState == CoreWindowActivationState.PointerActivated) &&
                this.cameraControl.CameraStreamState == Windows.Media.Devices.CameraStreamState.Shutdown)
            {
                // When our Window loses focus due to user interaction Windows shuts it down, so we 
                // detect here when the window regains focus and trigger a restart of the camera.
                await this.cameraControl.StartStreamAsync();
            }
        }

        private async void CameraControl_AutoCaptureStateChanged(object sender, AutoCaptureState e)
        {
            // remove the image from grid(it is right next to camera grid)  @jack
            imageInfoImage.Source = null;

            switch (e)
            {
                case AutoCaptureState.WaitingForFaces:
                    this.cameraGuideBallon.Opacity = 1;
                    this.cameraGuideText.Text = "Step in front of the camera to start!";
                    this.cameraGuideHost.Opacity = 1;
                    break;
                case AutoCaptureState.WaitingForStillFaces:
                    this.cameraGuideText.Text = "Hold still...";
                    break;
                case AutoCaptureState.ShowingCountdownForCapture:
                    this.cameraGuideText.Text = "";
                    this.cameraGuideBallon.Opacity = 0;

                    this.cameraGuideCountdownHost.Opacity = 1;
                    
                    this.countDownTextBlock.Text = "3";
                    await Task.Delay(1000);
                    this.countDownTextBlock.Text = "2";
                    await Task.Delay(1000);
                    this.countDownTextBlock.Text = "1";
                    await Task.Delay(1000);
                    this.cameraGuideCountdownHost.Opacity = 0;

                    this.ProcessCameraCapture(await this.cameraControl.TakeAutoCapturePhoto());
                    break;
                case AutoCaptureState.ShowingCapturedPhoto:
                    this.cameraGuideHost.Opacity = 0;
                    break;
                default:
                    break;
            }
        }

        private void ProcessCameraCapture(ImageAnalyzer e)
        {
            // if the image is null  @jack
            if (e == null)
            {
                this.cameraControl.RestartAutoCaptureCycle();
                return;
            }

            this.imageFromCameraWithFaces.DataContext = e;

            e.FaceRecognitionCompleted += async (s, args) =>
            {
                this.photoCaptureBalloonHost.Opacity = 0.6;

                int photoDisplayDuration = 10;
                double decrementPerSecond = 100.0 / photoDisplayDuration;
                for (double i = 100; i >= 0; i -= decrementPerSecond)
                {
                    this.resultDisplayTimerUI.Value = i;
                    await Task.Delay(1000);
                }


                this.photoCaptureBalloonHost.Opacity = 0;
                this.imageFromCameraWithFaces.DataContext = null;
                this.cameraControl.RestartAutoCaptureCycle();
            };
            CameraControl_ImageCaptured(e);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            EnterKioskMode();
            
            await this.cameraControl.StartStreamAsync();

            mediaElement.SetPlaybackSource(playbackList);

            base.OnNavigatedTo(e);
        }

        private void EnterKioskMode()
        {
            ApplicationView view = ApplicationView.GetForCurrentView();
            if (!view.IsFullScreenMode)
            {
                view.TryEnterFullScreenMode();
            }
        }

        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Window.Current.Activated -= CurrentWindowActivationStateChanged;
            this.cameraControl.AutoCaptureStateChanged -= CameraControl_AutoCaptureStateChanged;
            this.cameraControl.CameraAspectRatioChanged -= CameraControl_CameraAspectRatioChanged;

            await this.cameraControl.StopStreamAsync();
            base.OnNavigatingFrom(e);
        }

        private void OnPageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdateCameraHostSize();
        }

        private void UpdateCameraHostSize()
        {
            this.cameraHostGrid.Width = this.cameraHostGrid.ActualHeight * (this.cameraControl.CameraAspectRatio != 0 ? this.cameraControl.CameraAspectRatio : 1.777777777777);
        }


        /***************************************************************************
         ***************************************************************************
         ***************************** video trigger *******************************  @jack
         ***************************************************************************
         ***************************************************************************/


        private async void CameraControl_ImageCaptured(ImageAnalyzer e)
        {
            // We induce a delay here to give the captured image some time to render before we hide the camera.
            // This avoids a black flash.
            await Task.Delay(50);

            e.FaceRecognitionCompleted += (s, args) =>
            {
                ShowRecommendations(e);
            };
        }

        //camera trigger  @jack
        private async void ShowRecommendations(ImageAnalyzer imageWithFaces)
        {
            // if there is no face on the image  @jack
            if(imageWithFaces.DetectedFaces.Count()==0)
            {
                this.photoCaptureBalloonHost.Opacity = 0.6;

                int photoDisplayDuration = 10;
                double decrementPerSecond = 100.0 / photoDisplayDuration;
                for (double i = 100; i >= 0; i -= decrementPerSecond)
                {
                    this.resultDisplayTimerUI.Value = i;
                    await Task.Delay(1000);
                }


                this.photoCaptureBalloonHost.Opacity = 0;
                this.imageFromCameraWithFaces.DataContext = null;
                this.cameraControl.RestartAutoCaptureCycle();

                return;
            }

            // this part is for recommandation for the people who are recognized  @jack
            Recommendation recommendation = null;
            this.currentRecommendation = null;

            Face face = imageWithFaces.DetectedFaces.First();

            int numberOfPeople = imageWithFaces.DetectedFaces.Count();
            int analyzedAge = ((int)face.FaceAttributes.Age / 10) * 10;

            var adImage = new BitmapImage();
            string averageAge = null;


            // for single person  @jack
            if (numberOfPeople == 1)
            {
                IdentifiedPerson identifiedPerson = imageWithFaces.IdentifiedPersons.FirstOrDefault();

                // those who identified  @jack
                if (identifiedPerson != null)
                {
                    // for identified person, need to chanage the TrigerInfo.cs in the views folder to use below line  @jack
                    //recommendation = this.kioskSettings.PersonalizedRecommendations.FirstOrDefault(r => r.Id.Equals(identifiedPerson.Person.Name, StringComparison.OrdinalIgnoreCase)); 
                }
                
                // those who are new  @jack
                if (recommendation == null)
                {
                    // when the person is male  @jack
                    if (face.FaceAttributes.Gender.Equals("Male",StringComparison.OrdinalIgnoreCase))
                    {
                        if (face.FaceAttributes.Age <= 12)
                        {
                            playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));

                            averageAge = (imageWithFaces.DetectedFaces.Average(f => (double)f.FaceAttributes.Age)).ToString();

                            adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                            imageInfoImage.Source = adImage;

                            imageInfoTB.Text = "Gender: " + face.FaceAttributes.Gender + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                        }
                        else if (face.FaceAttributes.Age >= 60)
                        {
                            playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));

                            averageAge = (imageWithFaces.DetectedFaces.Average(f => (double)f.FaceAttributes.Age)).ToString();

                            adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                            imageInfoImage.Source = adImage;

                            imageInfoTB.Text = "Gender: " + face.FaceAttributes.Gender + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                        }
                        else
                        {
                            switch (analyzedAge)
                            {
                                case 20:
                                    playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));

                                    averageAge= (imageWithFaces.DetectedFaces.Average(f => (double)f.FaceAttributes.Age)).ToString();

                                    adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                                    imageInfoImage.Source = adImage;

                                    imageInfoTB.Text = "Gender: " + face.FaceAttributes.Gender + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                                    break;
                                case 30:
                                    playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/8b4077ab-b80a-4cd2-8aa5-468f63fe2795/Something_just_like_this.ism/manifest(format=m3u8-aapl)")));

                                    averageAge = (imageWithFaces.DetectedFaces.Average(f => (double)f.FaceAttributes.Age)).ToString();

                                    adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/8b4077ab-b80a-4cd2-8aa5-468f63fe2795/Something_just_like_this_000001.png");
                                    imageInfoImage.Source = adImage;

                                    imageInfoTB.Text = "Gender: " + face.FaceAttributes.Gender + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                                    break;
                                case 40:
                                    playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));

                                    averageAge = (imageWithFaces.DetectedFaces.Average(f => (double)f.FaceAttributes.Age)).ToString();

                                    adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                                    imageInfoImage.Source = adImage;

                                    imageInfoTB.Text = "Gender: " + face.FaceAttributes.Gender + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                                    break;
                                case 50:
                                    playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));

                                    averageAge = (imageWithFaces.DetectedFaces.Average(f => (double)f.FaceAttributes.Age)).ToString();

                                    adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                                    imageInfoImage.Source = adImage;

                                    imageInfoTB.Text = "Gender: " + face.FaceAttributes.Gender + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    // when the person is female  @jack
                    else
                    {
                        if (face.FaceAttributes.Age <= 12)
                        {
                            playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));

                            averageAge = (imageWithFaces.DetectedFaces.Average(f => (double)f.FaceAttributes.Age)).ToString();

                            adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                            imageInfoImage.Source = adImage;

                            imageInfoTB.Text = "Gender: " + face.FaceAttributes.Gender + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                        }
                        else if (face.FaceAttributes.Age >= 60)
                        {
                            playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));

                            averageAge = (imageWithFaces.DetectedFaces.Average(f => (double)f.FaceAttributes.Age)).ToString();

                            adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                            imageInfoImage.Source = adImage;

                            imageInfoTB.Text = "Gender: " + face.FaceAttributes.Gender + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                        }
                        else
                        {
                            switch (analyzedAge)
                            {
                                case 20:
                                    playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));

                                    averageAge = (imageWithFaces.DetectedFaces.Average(f => (double)f.FaceAttributes.Age)).ToString();

                                    adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                                    imageInfoImage.Source = adImage;

                                    imageInfoTB.Text = "Gender: " + face.FaceAttributes.Gender + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                                    break;
                                case 30:
                                    playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));

                                    averageAge = (imageWithFaces.DetectedFaces.Average(f => (double)f.FaceAttributes.Age)).ToString();

                                    adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                                    imageInfoImage.Source = adImage;

                                    imageInfoTB.Text = "Gender: " + face.FaceAttributes.Gender + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                                    break;
                                case 40:
                                    playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));

                                    averageAge = (imageWithFaces.DetectedFaces.Average(f => (double)f.FaceAttributes.Age)).ToString();

                                    adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                                    imageInfoImage.Source = adImage;

                                    imageInfoTB.Text = "Gender: " + face.FaceAttributes.Gender + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                                    break;
                                case 50:
                                    playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));

                                    averageAge = (imageWithFaces.DetectedFaces.Average(f => (double)f.FaceAttributes.Age)).ToString();

                                    adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                                    imageInfoImage.Source = adImage;

                                    imageInfoTB.Text = "Gender: " + face.FaceAttributes.Gender + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                                    break;
                            }
                        }
                    }


                    //recommendation = this.kioskSettings.GetGenericRecommendationForPerson((int)face.FaceAttributes.Age, face.FaceAttributes.Gender);
                }
            }
            // for the couple  @jack
            else if(numberOfPeople==2 && imageWithFaces.DetectedFaces.Any(f => f.FaceAttributes.Gender.Equals("Male", StringComparison.OrdinalIgnoreCase)) && 
                imageWithFaces.DetectedFaces.Any(f => f.FaceAttributes.Gender.Equals("Female", StringComparison.OrdinalIgnoreCase)))
            {
                playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/ae962334-b864-404a-9142-7f51b2beda59/Shape_of_You.ism/manifest(format=m3u8-aapl)")));

                averageAge = (imageWithFaces.DetectedFaces.Average(f => (double)f.FaceAttributes.Age)).ToString();

                adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/ae962334-b864-404a-9142-7f51b2beda59/Shape_of_You_000001.png");
                imageInfoImage.Source = adImage;

                imageInfoTB.Text = "Type: " + "Couple of people" + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
            }
            // for the people  @jack
            else if (numberOfPeople > 1 && imageWithFaces.DetectedFaces.Any(f => f.FaceAttributes.Age <= 12) &&
                     imageWithFaces.DetectedFaces.Any(f => f.FaceAttributes.Age > 12))
            {
                // Group with at least one child
                //recommendation = this.kioskSettings.GenericRecommendations.FirstOrDefault(r => r.Id == "ChildWithOneOrMoreAdults");

                playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));

                averageAge = (imageWithFaces.DetectedFaces.Average(f => (double)f.FaceAttributes.Age)).ToString();

                adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                imageInfoImage.Source = adImage;

                imageInfoTB.Text = "Type: " + "People with children" + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
            }
            else if (numberOfPeople > 1 && !imageWithFaces.DetectedFaces.Any(f => f.FaceAttributes.Age <= 12))
            {
                // Group of adults without a child
                // recommendation = this.kioskSettings.GenericRecommendations.FirstOrDefault(r => r.Id == "TwoOrMoreAdults");

                playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));

                averageAge = (imageWithFaces.DetectedFaces.Average(f => (double)f.FaceAttributes.Age)).ToString();

                adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                imageInfoImage.Source = adImage;

                imageInfoTB.Text = "Type: " + "Group of people" + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
            }
            else
            {
                
            }
        }
    }
}
