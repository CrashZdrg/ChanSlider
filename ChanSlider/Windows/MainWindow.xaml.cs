using ChanSlider.Views;
using System;
using System.Collections.Generic;
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

namespace ChanSlider.Windows
{
    interface IMainWindow
    {
        Dispatcher Dispatcher { get; }
        bool Running { get; set; }

        string Tags { get; set; }
        bool Fullscreen { get; set; }
        bool HighRes { get; set; }
        int Interval { get; set; }
        int AnimationDuration { get; set; }
        System.Collections.IEnumerable Apis { get; set; }
        int SelectedApi { get; set; }

        IRenderView RenderView { get; }

        event Action Closing;
        event Action Load;

        event Action Start;
        event Action Stop;

        event Action NextImage;
        event Action PrevImage;

        event Action GoPost;

        void TimerTick();
    }

    public partial class MainWindow : Window, IMainWindow
    {
        public bool Running { get; set; }

        public string Tags
        {
            get => txtTags.Text;
            set => txtTags.Text = value;
        }

        public bool Fullscreen
        {
            get => chkFullscreen.IsChecked == true;
            set => chkFullscreen.IsChecked = value;
        }

        public bool HighRes
        {
            get => chkHighRes.IsChecked == true;
            set => chkHighRes.IsChecked = value;
        }

        public int Interval
        {
            get => int.Parse(txtInterval.Text);
            set => txtInterval.Text = value.ToString();
        }

        public int AnimationDuration
        {
            get => int.Parse(txtAnimationDuration.Text);
            set => txtAnimationDuration.Text = value.ToString();
        }

        public System.Collections.IEnumerable Apis
        {
            get => cbxApis.ItemsSource;
            set => cbxApis.ItemsSource = value;
        }
        public int SelectedApi
        {
            get => cbxApis.SelectedIndex;
            set => cbxApis.SelectedIndex = value;
        }

        public IRenderView RenderView { get; private set; }

        public event Action Load;
        public new event Action Closing;

        public event Action Start;
        public event Action Stop;

        public event Action NextImage;
        public event Action PrevImage;

        public event Action GoPost;

        IFullscreenWindow fullscreenWindow;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += (s, e) => Load?.Invoke();
            base.Closing += (s, e) => Closing?.Invoke();

            btnStartStop.Click += BtnStartStop_Click;
            KeyDown += MainWindow_KeyDown;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Right:
                    NextImage?.Invoke();
                    break;
                case Key.Left:
                    PrevImage?.Invoke();
                    break;
                case Key.Space:
                    GoPost?.Invoke();
                    break;
            }
        }

        private void PreviewTextInput_Int(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }

        public void TimerTick()
        {
            NextImage?.Invoke();
        }

        private void FullscreenWindow_Closed()
        {
            DoStop();
        }

        private void BtnStartStop_Click(object sender, RoutedEventArgs e)
        {
            if (!Running)
                DoStart();
            else
                DoStop();
        }

        private void DoStart()
        {
            if (Running)
                return;

            Running = true;

            OptionsEnabled(false);
            btnStartStop.Content = "Stop";

            if (Fullscreen)
            {
                fullscreenWindow = new FullscreenWindow();
                fullscreenWindow.Closed += FullscreenWindow_Closed;
                fullscreenWindow.KeyDown += MainWindow_KeyDown;
                RenderView = fullscreenWindow.RenderView;
                fullscreenWindow.Show();
            }
            else
            {
                var renderView = new RenderView();
                cntRenderView.Content = renderView;
                RenderView = renderView;
            }

            Start?.Invoke();
        }

        private void DoStop()
        {
            if (!Running)
                return;

            Running = false;

            Stop?.Invoke();

            if (Fullscreen)
                fullscreenWindow.Close();
            else
                cntRenderView.Content = null;

            RenderView = null;

            btnStartStop.Content = "Start";
            OptionsEnabled(true);
        }

        private void OptionsEnabled(bool enable)
        {
            txtTags.IsEnabled = enable;
            chkFullscreen.IsEnabled = enable;
        }
    }
}
