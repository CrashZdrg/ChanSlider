using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChanSlider
{
    static class App
    {
        [STAThread]
        static void Main()
        {
            var window = new Windows.MainWindow();
            _ = new Presenters.MainPresenter(window);
            new Application().Run(window);
        }
    }
}
