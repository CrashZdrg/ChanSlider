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
using System.Windows.Shapes;

namespace ChanSlider.Windows
{
    interface IFullscreenWindow
    {
        IRenderView RenderView { get; }

        event Action Closed;
        event KeyEventHandler KeyDown;

        void Show();
        void Close();
    }

    public partial class FullscreenWindow : Window, IFullscreenWindow
    {
        public IRenderView RenderView => renderView;

        public new event Action Closed;

        public FullscreenWindow()
        {
            InitializeComponent();

            base.Closed += (s, e) => Closed?.Invoke();

            KeyDown += FullscreenWindow_KeyDown;
        }

        private void FullscreenWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Close();
                    break;
            }
        }
    }
}
