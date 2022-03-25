using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ChanSlider.Models
{
    class ApiItemMdl
    {
        public string PostUrl { get; set; }
        public Uri Url { get; set; }
        public BitmapSource Source { get; set; }
    }
}
