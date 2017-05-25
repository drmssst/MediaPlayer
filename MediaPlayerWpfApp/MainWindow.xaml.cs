using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ViewModel viewModel = new ViewModel();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void Video01SourceButtonClick(object sender, RoutedEventArgs e)
        {            
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Video files (*.avi;*.mp4;*.mpg;*.mpeg)|*.avi;*.mp4;*.mpg;*.mpeg|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                tbxVideo01Source.Text = openFileDialog.FileName;
                tbxVideo01Source.Focus();
                btnVideo01Source.Focus();
            }
        }

        private void VideoSourceTextBox_LostFocus(byte playerId, object sender, RoutedEventArgs e)
        {
            Button playBtn = (playerId == 1)?Video01PlayButton:null;
            Button pauseBtn = (playerId == 1)?Video01PauseButton:null;
            Button stopBtn = (playerId == 1)?Video01StopButton:null;

            playBtn.IsEnabled = true;
            playBtn.Visibility = Visibility.Visible;

            pauseBtn.IsEnabled = false;
            pauseBtn.Visibility = Visibility.Hidden;

            stopBtn.IsEnabled = false;
        }
        private void PlayButton_Click(byte playerId, object sender, RoutedEventArgs e)
        {
            Button playBtn = (playerId == 1)?Video01PlayButton:null;
            Button pauseBtn = (playerId == 1)?Video01PauseButton:null;
            Button stopBtn = (playerId == 1)?Video01StopButton:null;
            VideoPlayer videoPlayer = (playerId == 1)?viewModel.Player1:null;

            playBtn.IsEnabled = false;
            playBtn.Visibility = Visibility.Hidden;

            pauseBtn.IsEnabled = true;
            pauseBtn.Visibility = Visibility.Visible;

            stopBtn.IsEnabled = true;

            videoPlayer.StartTimer();
        }
        private void PauseButton_Click(byte playerId, object sender, RoutedEventArgs e)
        {
            Button playBtn = (playerId == 1)?Video01PlayButton:null;
            Button pauseBtn = (playerId == 1)?Video01PauseButton:null;
            Button stopBtn = (playerId == 1)?Video01StopButton:null;
            VideoPlayer videoPlayer = (playerId == 1)?viewModel.Player1:null;

            playBtn.IsEnabled = true;
            playBtn.Visibility = Visibility.Visible;

            pauseBtn.IsEnabled = false;
            pauseBtn.Visibility = Visibility.Hidden;

            stopBtn.IsEnabled = true;

            videoPlayer.Timer.Stop();
        }
        private void StopButton_Click(byte playerId, object sender, RoutedEventArgs e)
        {
            Button playBtn = (playerId == 1)?Video01PlayButton:null;
            Button pauseBtn = (playerId == 1)?Video01PauseButton:null;
            Button stopBtn = (playerId == 1)?Video01StopButton:null;
            VideoPlayer videoPlayer = (playerId == 1)?viewModel.Player1:null;

            playBtn.IsEnabled = true;
            playBtn.Visibility = Visibility.Visible;

            pauseBtn.IsEnabled = false;
            pauseBtn.Visibility = Visibility.Hidden;

            stopBtn.IsEnabled = false;

            videoPlayer.Timer.Stop();
            videoPlayer.TotalSecondsPassed = TimeSpan.Zero;
        }

        private void Video01SourceTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            VideoSourceTextBox_LostFocus(1, sender, e);
        }
        private void Video01PlayButton_Click(object sender, RoutedEventArgs e)
        {
            PlayButton_Click(1, sender, e);
        }
        private void Video01PauseButton_Click(object sender, RoutedEventArgs e)
        {
            PauseButton_Click(1, sender, e);
        }
        private void Video01StopButton_Click(object sender, RoutedEventArgs e)
        {
            StopButton_Click(1, sender, e);
        }
    }

    public class ViewModel
    {
        public VideoPlayer Player1 { get; set; }
        public VideoPlayer Player2 { get; set; }
        
        public ViewModel()
        {
            Player1 = new VideoPlayer();
            Player2 = new VideoPlayer();            
        }
    }

    public class VideoPlayer : INotifyPropertyChanged
    {
        private TimeSpan _TotalSecondsPassed = new TimeSpan(0,0,0,0,0);
        private int _TimerIntervalMS = 1000;

        public Video Video { get; set; }
        public Slider Slider { get; set; }
        public DispatcherTimer Timer { get; set; }
        public bool IsGrouped { get; set; }
        public int TimerIntervalMS { get { return _TimerIntervalMS; } }
        public TimeSpan TotalSecondsPassed
        {
            get { return _TotalSecondsPassed; }
            set {
                if (_TotalSecondsPassed != value)
                {
                    _TotalSecondsPassed = value;
                    NotifyPropertyChanged("TotalSecondsPassed");
                } }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (propertyName != String.Empty && !String.IsNullOrWhiteSpace(propertyName))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

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
            TotalSecondsPassed += TimeSpan.FromMilliseconds(TimerIntervalMS);
            if (!(Video.Source != null) && (Video.Duration != null)
                && !Slider.UserIsDraggingSlider)
            {
                Slider.MinimumMS = 0.0;
                Slider.MaximumMS = Video.Duration.TotalMilliseconds;
                Slider.ValueMS = Video.Position.TotalMilliseconds;
            }
        }
        public void StartTimer()
        {
            Timer.Interval = TimeSpan.FromMilliseconds(TimerIntervalMS);
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
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class Slider
    {
        //public TimeSpan Duration { get; set; }
        public double MinimumMS { get; set; }
        public double MaximumMS { get; set; }
        public double ValueMS { get; set; }
        public bool UserIsDraggingSlider { get; set; }
    }
}

