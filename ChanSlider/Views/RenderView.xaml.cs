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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChanSlider.Views
{
    public interface IRenderView
    {
        Duration AnimationDuration { get; set; }
        bool IsLoading { get; set; }
        bool IsSwitching { get; }

        event Action NextImage;

        void SetNextImage(ImageSource img);
        void SwitchImages();
    }

    public partial class RenderView : UserControl, IRenderView
    {
        public Duration AnimationDuration
        {
            get => doubleAnimation.Duration;
            set
            {
                doubleAnimation.Duration = value;
                doubleAnimationReversed.Duration = value;
            }
        }

        public bool IsLoading
        {
            get => lblLoading.Visibility == Visibility.Visible;
            set => lblLoading.Visibility = value ? Visibility.Visible : Visibility.Hidden;
        }

        public bool IsSwitching { get; private set; }

        public event Action NextImage;

        readonly DoubleAnimation doubleAnimation;
        readonly DoubleAnimation doubleAnimationReversed;
        bool reversed;

        public RenderView()
        {
            InitializeComponent();

            var easing = new SineEase() { EasingMode = EasingMode.EaseInOut };
            doubleAnimation = new DoubleAnimation(1.0, 0.0, Duration.Automatic, FillBehavior.HoldEnd) { EasingFunction = easing };
            doubleAnimationReversed = new DoubleAnimation(0.0, 1.0, Duration.Automatic, FillBehavior.HoldEnd) { EasingFunction = easing };

            doubleAnimation.Completed += DoubleAnimation_Completed;
            doubleAnimationReversed.Completed += DoubleAnimation_Completed;
        }

        private void DoubleAnimation_Completed(object sender, EventArgs e)
        {
            reversed = !reversed;

            NextImage?.Invoke();

            IsSwitching = false;
        }

        public void SetNextImage(ImageSource img)
        {
            if (!reversed)
                imgImageBot.Source = img;
            else
                imgImageTop.Source = img;
        }

        public void SwitchImages()
        {
            IsSwitching = true;

            if (!reversed)
                grdImageTop.BeginAnimation(OpacityProperty, doubleAnimation);
            else
                grdImageTop.BeginAnimation(OpacityProperty, doubleAnimationReversed);
        }
    }
}
