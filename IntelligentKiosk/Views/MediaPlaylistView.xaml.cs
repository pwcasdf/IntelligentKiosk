using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace IntelligentKiosk.Views
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MediaPlaylistView : UserControl
    {
        ObservableCollection<Models.MediaModel> media = new ObservableCollection<Models.MediaModel>();

        public MediaPlaylistView()
        {
            this.InitializeComponent();

            // Bind the list view to the songs collection
            listView.ItemsSource = Media;
        }

        /// <summary>
        /// The selected index of the playlist
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                return listView.SelectedIndex;
            }
            set
            {
                listView.SelectedIndex = value;
            }
        }

        /// <summary>
        /// Raised when an item is clicked
        /// </summary>
        public event ItemClickEventHandler ItemClick
        {
            add { listView.ItemClick += value; }
            remove { listView.ItemClick -= value; }
        }

        /// <summary>
        /// Raised when the selection changes
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged
        {
            add { listView.SelectionChanged += value; }
            remove { listView.SelectionChanged -= value; }
        }

        /// <summary>
        /// A collection of songs in the list to be displayed to the user
        /// </summary>
        public ObservableCollection<Models.MediaModel> Media
        {
            get
            {
                return media;
            }
        }

        public Models.MediaModel GetSongById(Uri id)
        {
            return media.Single(s => s.MediaUri == id);
        }

        public int GetSongIndexById(Uri id)
        {
            return media.ToList().FindIndex(s => s.MediaUri == id);
        }
    }
}
