using Microsoft.ProjectOxford.Emotion.Contract;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IntelligentKiosk.Controls
{
    public sealed partial class EmotionMeterControl : UserControl
    {
        public EmotionMeterControl()
        {
            this.InitializeComponent();
            //this.DataContext = this;
        }

        public static readonly DependencyProperty EmotionNameProperty =
            DependencyProperty.Register(
            "EmotionName",
            typeof(string),
            typeof(EmotionMeterControl),
            new PropertyMetadata("")
            );

        public static readonly DependencyProperty EmotionValueProperty =
            DependencyProperty.Register(
            "EmotionValue",
            typeof(float),
            typeof(EmotionMeterControl),
            new PropertyMetadata(0)
            );

        public static readonly DependencyProperty MeterForegroundProperty =
            DependencyProperty.Register(
            "MeterForeground",
            typeof(SolidColorBrush),
            typeof(EmotionMeterControl),
            new PropertyMetadata(new SolidColorBrush(Colors.White))
            );

        public SolidColorBrush MeterForeground
        {
            get { return (SolidColorBrush)GetValue(MeterForegroundProperty); }
            set { SetValue(MeterForegroundProperty, (SolidColorBrush)value); }
        }

        public string EmotionName
        {
            get { return (string)GetValue(EmotionNameProperty); }
            set { SetValue(EmotionNameProperty, (string)value); }
        }
        public float EmotionValue
        {
            get { return (float)GetValue(EmotionValueProperty); }
            set { SetValue(EmotionValueProperty, (float)value); }
        }
    }
}
