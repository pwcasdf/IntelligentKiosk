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

            // Add contents

            //single_male
            var media1 = new Models.MediaModel();
            media1.Title = "Pororo Alphabet";
            media1.MediaUri = new Uri("");
            media1.ArtUri = new Uri("");
            playlistView.Media.Add(media1);

            var media2 = new Models.MediaModel();
            media2.Title = "Returning to farm";
            media2.MediaUri = new Uri("");
            media2.ArtUri = new Uri("");
            playlistView.Media.Add(media2);

            var media3 = new Models.MediaModel();
            media3.Title = "Onepiece";
            media3.MediaUri = new Uri("");
            media3.ArtUri = new Uri("");
            playlistView.Media.Add(media3);

            var media4 = new Models.MediaModel();
            media4.Title = "Tourism";
            media4.MediaUri = new Uri("");
            media4.ArtUri = new Uri("");
            playlistView.Media.Add(media4);

            var media5 = new Models.MediaModel();
            media5.Title = "Aventador";
            media5.MediaUri = new Uri("");
            media5.ArtUri = new Uri("");
            playlistView.Media.Add(media5);

            var media6 = new Models.MediaModel();
            media6.Title = "Johnnie Walker";
            media6.MediaUri = new Uri("");
            media6.ArtUri = new Uri("");
            playlistView.Media.Add(media6);

            var media7 = new Models.MediaModel();
            media7.Title = "Cruise Tour";
            media7.MediaUri = new Uri("");
            media7.ArtUri = new Uri("");
            playlistView.Media.Add(media7);

            //single_female
            var media8 = new Models.MediaModel();
            media8.Title = "Pororo color";
            media8.MediaUri = new Uri("");
            media8.ArtUri = new Uri("");
            playlistView.Media.Add(media8);

            var media9 = new Models.MediaModel();
            media9.Title = "Returning to farm";
            media9.MediaUri = new Uri("");
            media9.ArtUri = new Uri("");
            playlistView.Media.Add(media9);

            var media10 = new Models.MediaModel();
            media10.Title = "피 땀 눈물";
            media10.MediaUri = new Uri("");
            media10.ArtUri = new Uri("");
            playlistView.Media.Add(media10);

            var media11 = new Models.MediaModel();
            media11.Title = "MAC Lip Swatches";
            media11.MediaUri = new Uri("");
            media11.ArtUri = new Uri("");
            playlistView.Media.Add(media11);

            var media12 = new Models.MediaModel();
            media12.Title = "CELINE NANO TOTE";
            media12.MediaUri = new Uri("");
            media12.ArtUri = new Uri("");
            playlistView.Media.Add(media12);

            var media13 = new Models.MediaModel();
            media13.Title = "GYM - Return to 20's";
            media13.MediaUri = new Uri("");
            media13.ArtUri = new Uri("");
            playlistView.Media.Add(media13);

            var media14 = new Models.MediaModel();
            media14.Title = "Luxury house";
            media14.MediaUri = new Uri("");
            media14.ArtUri = new Uri("");
            playlistView.Media.Add(media14);

            //couple
            var media15 = new Models.MediaModel();
            media15.Title = "Pororo Rainbow color";
            media15.MediaUri = new Uri("");
            media15.ArtUri = new Uri("");
            playlistView.Media.Add(media15);

            var media16 = new Models.MediaModel();
            media16.Title = "Returning to farm";
            media16.MediaUri = new Uri("");
            media16.ArtUri = new Uri("");
            playlistView.Media.Add(media16);

            var media17 = new Models.MediaModel();
            media17.Title = "Happymove";
            media17.MediaUri = new Uri("");
            media17.ArtUri = new Uri("");
            playlistView.Media.Add(media17);

            var media18 = new Models.MediaModel();
            media18.Title = "EURO Trip!!";
            media18.MediaUri = new Uri("");
            media18.ArtUri = new Uri("");
            playlistView.Media.Add(media18);

            var media19 = new Models.MediaModel();
            media19.Title = "Wedding Event";
            media19.MediaUri = new Uri("");
            media19.ArtUri = new Uri("");
            playlistView.Media.Add(media19);

            var media20 = new Models.MediaModel();
            media20.Title = "Enjoy the little things arount you";
            media20.MediaUri = new Uri("");
            media20.ArtUri = new Uri("");
            playlistView.Media.Add(media20);

            var media21 = new Models.MediaModel();
            media21.Title = "Cruise Tour";
            media21.MediaUri = new Uri("");
            media21.ArtUri = new Uri("");
            playlistView.Media.Add(media21);

            //group_child
            var media22 = new Models.MediaModel();
            media22.Title = "Aqua Planet";
            media22.MediaUri = new Uri("");
            media22.ArtUri = new Uri("");
            playlistView.Media.Add(media22);

            //group
            var media23 = new Models.MediaModel();
            media23.Title = "Returning to farm";
            media23.MediaUri = new Uri("");
            media23.ArtUri = new Uri("");
            playlistView.Media.Add(media23);

            var media24 = new Models.MediaModel();
            media24.Title = "에버랜드 이용 꿀팁!!";
            media24.MediaUri = new Uri("");
            media24.ArtUri = new Uri("");
            playlistView.Media.Add(media24);

            var media25 = new Models.MediaModel();
            media25.Title = "TOEIC";
            media25.MediaUri = new Uri("");
            media25.ArtUri = new Uri("");
            playlistView.Media.Add(media25);

            var media26 = new Models.MediaModel();
            media26.Title = "Fuel your Passion";
            media26.MediaUri = new Uri("");
            media26.ArtUri = new Uri("");
            playlistView.Media.Add(media26);

            var media27 = new Models.MediaModel();
            media27.Title = "효도하세요, 효도!";
            media27.MediaUri = new Uri("");
            media27.ArtUri = new Uri("");
            playlistView.Media.Add(media27);

            var media28 = new Models.MediaModel();
            media28.Title = "Cruise Tour";
            media28.MediaUri = new Uri("");
            media28.ArtUri = new Uri("");
            playlistView.Media.Add(media28);


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
            // remove the image and text from grid(it is right next to camera grid)  @jack
            imageInfoImage.Source = null;
            imageInfoTB.Text = "";
            imageInfoDescription.Text = "";

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
            int analyzedAge_Single = ((int)face.FaceAttributes.Age / 10) * 10;
            double averageAge = (imageWithFaces.DetectedFaces.Average(f => (double)f.FaceAttributes.Age));
            int analyzedAge_Group = (((int)imageWithFaces.DetectedFaces.Average(f => (double)f.FaceAttributes.Age)) / 10) * 10;

            var adImage = new BitmapImage();
            
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
                        if (averageAge < 10)
                        {
                            playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));

                            adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                            imageInfoImage.Source = adImage;

                            imageInfoTB.Text = "Gender: " + face.FaceAttributes.Gender + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                            imageInfoDescription.Text = "뽀로로에게 알파벳을 배워보자!!";
                        }
                        else if (averageAge >= 60)
                        {
                            playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));

                            adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                            imageInfoImage.Source = adImage;

                            imageInfoTB.Text = "Gender: " + face.FaceAttributes.Gender + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                            imageInfoDescription.Text = "한적한 곳에서 귀농 생활 어떠신가요?";
                        }
                        else
                        {
                            switch (analyzedAge_Single)
                            {
                                case 10:
                                    playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));

                                    averageAge = imageWithFaces.DetectedFaces.Average(f => (double)f.FaceAttributes.Age);

                                    adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                                    imageInfoImage.Source = adImage;

                                    imageInfoTB.Text = "Gender: " + face.FaceAttributes.Gender + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                                    imageInfoDescription.Text = "따분한 수업보단 원피스를 찾아 떠나는 것은 어떤가요?";
                                    break;
                                case 20:
                                    playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));

                                    averageAge= imageWithFaces.DetectedFaces.Average(f => (double)f.FaceAttributes.Age);

                                    adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                                    imageInfoImage.Source = adImage;

                                    imageInfoTB.Text = "Gender: " + face.FaceAttributes.Gender + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                                    imageInfoDescription.Text = "청춘이여, 지금 떠나세요!!";
                                    break;
                                case 30:
                                    playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/8b4077ab-b80a-4cd2-8aa5-468f63fe2795/Something_just_like_this.ism/manifest(format=m3u8-aapl)")));
                                    
                                    adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/8b4077ab-b80a-4cd2-8aa5-468f63fe2795/Something_just_like_this_000001.png");
                                    imageInfoImage.Source = adImage;

                                    imageInfoTB.Text = "Gender: " + face.FaceAttributes.Gender + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                                    imageInfoDescription.Text = "당신의 출근시간을 책임져 줄 남자의 자동차를 소개드립니다.";
                                    break;
                                case 40:
                                    playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));
                                    
                                    adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                                    imageInfoImage.Source = adImage;

                                    imageInfoTB.Text = "Gender: " + face.FaceAttributes.Gender + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                                    imageInfoDescription.Text = "직장에선 worker, 지인과는 Johnnie Walker!!";
                                    break;
                                case 50:
                                    playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));
                                    
                                    adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                                    imageInfoImage.Source = adImage;

                                    imageInfoTB.Text = "Gender: " + face.FaceAttributes.Gender + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                                    imageInfoDescription.Text = "평생일한 당신!! 크루즈 여행에 초대합니다~!!";
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    // when the person is female  @jack
                    else
                    {
                        if (averageAge < 10)
                        {
                            playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));
                            
                            adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                            imageInfoImage.Source = adImage;

                            imageInfoTB.Text = "Gender: " + face.FaceAttributes.Gender + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                            imageInfoDescription.Text = "이쁜 아이들은 뽀로로에게 영어를 배워봅시다.";
                        }
                        else if (averageAge >= 60)
                        {
                            playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));
                            
                            adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                            imageInfoImage.Source = adImage;

                            imageInfoTB.Text = "Gender: " + face.FaceAttributes.Gender + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                            imageInfoDescription.Text = "한적한 곳에서 귀농 생활 어떠신가요?";
                        }
                        else
                        {
                            switch (analyzedAge_Single)
                            {
                                case 10:
                                    playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));

                                    adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                                    imageInfoImage.Source = adImage;

                                    imageInfoTB.Text = "Gender: " + face.FaceAttributes.Gender + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                                    imageInfoDescription.Text = "오늘, 방탄소년단의 '피 땀 눈물'을 50%할인된 가격으로 만나보세요.";
                                    break;
                                case 20:
                                    playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));
                                    
                                    adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                                    imageInfoImage.Source = adImage;

                                    imageInfoTB.Text = "Gender: " + face.FaceAttributes.Gender + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                                    imageInfoDescription.Text = "키스를 부르는 입술, 오늘만 특가 할인!!";
                                    break;
                                case 30:
                                    playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));
                                    
                                    adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                                    imageInfoImage.Source = adImage;

                                    imageInfoTB.Text = "Gender: " + face.FaceAttributes.Gender + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                                    imageInfoDescription.Text = "당신의 품격을 한층 더 높여 드립니다.";
                                    break;
                                case 40:
                                    playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));
                                    
                                    adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                                    imageInfoImage.Source = adImage;

                                    imageInfoTB.Text = "Gender: " + face.FaceAttributes.Gender + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                                    imageInfoDescription.Text = "다시 20대의 몸매로 돌아가고 싶다면, 등록하세요!!";
                                    break;
                                case 50:
                                    playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));
                                    
                                    adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                                    imageInfoImage.Source = adImage;

                                    imageInfoTB.Text = "Gender: " + face.FaceAttributes.Gender + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                                    imageInfoDescription.Text = "꿈꾸던 집을 지금 분양하세요!";
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
                if (averageAge < 10)
                {
                    playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));

                    adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                    imageInfoImage.Source = adImage;

                    imageInfoTB.Text = "Type: " + "Couple of People" + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                    imageInfoDescription.Text = "귀여운 꼬마 커플에게는 재미있는 노래를 알려줄게요~!";
                }
                else if (averageAge >= 60)
                {
                    playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));

                    adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                    imageInfoImage.Source = adImage;

                    imageInfoTB.Text = "Type: " + "Couple of People" + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                    imageInfoDescription.Text = "한적한 곳에서 귀농 생활 어떠신가요?";
                }
                else
                {
                    switch (analyzedAge_Group)
                    {
                        case 10:
                            playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));

                            averageAge = imageWithFaces.DetectedFaces.Average(f => (double)f.FaceAttributes.Age);

                            adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                            imageInfoImage.Source = adImage;

                            imageInfoTB.Text = "Type: " + "Couple of People" + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                            imageInfoDescription.Text = "학생 커플 여러분, 취미로 의미있는 봉사활동 어떠신가요?";
                            break;
                        case 20:
                            playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));

                            averageAge = imageWithFaces.DetectedFaces.Average(f => (double)f.FaceAttributes.Age);

                            adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)");
                            imageInfoImage.Source = adImage;

                            imageInfoTB.Text = "Type: " + "Couple of People" + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                            imageInfoDescription.Text = "젊을때 떠나야죠!! [커플여행] 유럽 여행 어떠신가요~?";
                            break;
                        case 30:
                            playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/8b4077ab-b80a-4cd2-8aa5-468f63fe2795/Something_just_like_this.ism/manifest(format=m3u8-aapl)")));

                            adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/8b4077ab-b80a-4cd2-8aa5-468f63fe2795/Something_just_like_this_000001.png");
                            imageInfoImage.Source = adImage;

                            imageInfoTB.Text = "Type: " + "Couple of People" + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                            imageInfoDescription.Text = "결혼 적령기인 30대 커플, 이러한 이벤트는 어떠신가요?";
                            break;
                        case 40:
                            playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));

                            adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                            imageInfoImage.Source = adImage;

                            imageInfoTB.Text = "Type: " + "Couple of People" + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                            imageInfoDescription.Text = "아무리 일, 돈이 중요하지만 결코 사소한 행복을 놓치지 마세요.";
                            break;
                        case 50:
                            playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));

                            adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                            imageInfoImage.Source = adImage;

                            imageInfoTB.Text = "Type: " + "Couple of People" + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                            imageInfoDescription.Text = "크루즈 여행으로, 서로에게 함께 해온 시간을 기념하세요.";
                            break;
                        default:
                            break;
                    }
                }
            }
            // for the people with children  @jack
            else if (numberOfPeople > 1 && imageWithFaces.DetectedFaces.Any(f => f.FaceAttributes.Age <= 12) &&
                     imageWithFaces.DetectedFaces.Any(f => f.FaceAttributes.Age > 12))
            {
                // Group with at least one child
                //recommendation = this.kioskSettings.GenericRecommendations.FirstOrDefault(r => r.Id == "ChildWithOneOrMoreAdults");

                playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));

                adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                imageInfoImage.Source = adImage;

                imageInfoTB.Text = "Type: " + "Children" + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                imageInfoDescription.Text = "온가족이 떠나는 아쿠아플래닛!!";
            }
            else if (numberOfPeople > 1 && !imageWithFaces.DetectedFaces.Any(f => f.FaceAttributes.Age <= 12))
            {
                // Group of adults without children
                // recommendation = this.kioskSettings.GenericRecommendations.FirstOrDefault(r => r.Id == "TwoOrMoreAdults");

                if (averageAge >= 60)
                {
                    playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));

                    adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                    imageInfoImage.Source = adImage;

                    imageInfoTB.Text = "Type: " + "A Group of People" + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                    imageInfoDescription.Text = "공기 좋고, 물도 좋은 귀농 어떠세요?";
                }
                else
                {
                    switch (analyzedAge_Group)
                    {
                        case 10:
                            playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));

                            averageAge = imageWithFaces.DetectedFaces.Average(f => (double)f.FaceAttributes.Age);

                            adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                            imageInfoImage.Source = adImage;

                            imageInfoTB.Text = "Type: " + "A Group of People" + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                            imageInfoDescription.Text = "신나고 재밌는 에버랜드로 오세요~!!";
                            break;
                        case 20:
                            playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));

                            averageAge = imageWithFaces.DetectedFaces.Average(f => (double)f.FaceAttributes.Age);

                            adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                            imageInfoImage.Source = adImage;

                            imageInfoTB.Text = "Type: " + "A Group of People" + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                            imageInfoDescription.Text = "신발사이즈 토익점수에서 이번 기회에 벗어나세요!!";
                            break;
                        case 30:
                            playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/8b4077ab-b80a-4cd2-8aa5-468f63fe2795/Something_just_like_this.ism/manifest(format=m3u8-aapl)")));

                            adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/8b4077ab-b80a-4cd2-8aa5-468f63fe2795/Something_just_like_this_000001.png");
                            imageInfoImage.Source = adImage;

                            imageInfoTB.Text = "Type: " + "A Group of People" + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                            imageInfoDescription.Text = "여러분, 젊음이 가기전에 모든것을 즐기세요~!";
                            break;
                        case 40:
                            playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));

                            adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                            imageInfoImage.Source = adImage;

                            imageInfoTB.Text = "Type: " + "A Group of People" + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                            imageInfoDescription.Text = "아직 늦지 않았습니다. 오늘 전화하세요.";
                            break;
                        case 50:
                            playbackList.MoveTo((uint)playbackList.Items.ToList().FindIndex(i => (Uri)i.Source.CustomProperties["uri"] == new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars.ism/manifest(format=m3u8-aapl)")));

                            adImage.UriSource = new Uri("http://seteam.streaming.mediaservices.windows.net/e7b90ad1-38a4-442e-9357-7ecdac66bf21/A_Sky_Full_Of_Stars_000001.png");
                            imageInfoImage.Source = adImage;

                            imageInfoTB.Text = "Type: " + "A Group of People" + "\n" + "Avearage Age: " + averageAge + "\n" + "Num of People: " + numberOfPeople;
                            imageInfoDescription.Text = "다함께 떠나는 크루즈 여행, 여러분의 휴식을 책임집니다.";
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
