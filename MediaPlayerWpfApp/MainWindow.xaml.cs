using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MediaPlayerWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public VideoPlayer VideoPlayer1 = new VideoPlayer();
        public VideoPlayer VideoPlayer2 = new VideoPlayer();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void VideoSource01_ButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Video files (*.avi;*.mp4;*.mpg;*.mpeg)|*.avi;*.mp4;*.mpg;*.mpeg|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                tbxVideoSource.Text = openFileDialog.FileName;
                tbxVideoSource.Focus();
            }
        }

        private void Video01PlayButton_Click(object sender, RoutedEventArgs e)
        {
            Video01Element.Play();
        }
        private void Video01StopButton_Click(object sender, RoutedEventArgs e)
        {
            Video01Element.Stop();
        }
    }
    public class VideoPlayer
    {
        public Video Video { get; set; }
        public Slider Slider { get; set; }
        public DispatcherTimer Timer { get; set; }
        public bool IsGrouped { get; set; }

        public VideoPlayer()
        {
            Video = new Video();
            Slider = new Slider();
            IsGrouped = true;

            Timer = new DispatcherTimer();
            Timer.Tick += TimerTick;
        }
        private void TimerTick(object sender, EventArgs e)
        {
            if(!(Video.Source != null) && (Video.Duration != null)
                && !Slider.UserIsDraggingSlider)
            {
                Slider.MinimumMS = 0.0;
                Slider.MaximumMS = Video.Duration.TotalMilliseconds;
                Slider.ValueMS = Video.Position.TotalMilliseconds;
            }
        }
        public void StartTimer(int intervalMS = 1000)
        {
            Timer.Interval = TimeSpan.FromMilliseconds(intervalMS);
            Timer.Start();
        }
        public void FlipGroupedStatus()
        {
            IsGrouped = !IsGrouped;
        }
    }

    public class Video : INotifyPropertyChanged
    {
        private double _FramesPerSecond;
        private TimeSpan _Duration;
        private TimeSpan _Position;
        private Uri _Source;

        public double FramesPerSecond {
            get { return _FramesPerSecond; }
            set
            {
                if(_FramesPerSecond != value)
                {
                    _FramesPerSecond = value;
                    NotifyPropertyChanged("FramesPerSecond");
                }
            }
        }
        public TimeSpan Duration
        {
            get { return _Duration; }
            set
            {
                if (_Duration != value)
                {
                    _Duration = value;
                    NotifyPropertyChanged("Duration");
                }
            }
        }
        public TimeSpan Position
        {
            get { return _Position; }
            set
            {
                if (_Position != value)
                {
                    _Position = value;
                    NotifyPropertyChanged("Position");
                }
            }
        }
        public Uri Source
        {
            get { return _Source; }
            set
            {
                if(_Source != value)
                {
                    _Source = value;
                    NotifyPropertyChanged("Source");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if(propertyName != String.Empty && !String.IsNullOrWhiteSpace(propertyName))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }        
    }

    public class Slider
    {
        public TimeSpan Duration { get; set; }
        public double MinimumMS { get; set; }
        public double MaximumMS { get; set; }
        public double ValueMS { get; set; }
        public bool UserIsDraggingSlider { get; set; }
    }
}

