using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChanSlider.Models
{
    class ConfigMdl : Utils.JsonFile
    {
        public string Tags { get; set; } = "rating:s order:random";
        public int Api { get; set; }
        public bool Fullscreen { get; set; } = true;
        public bool HighRes { get; set; }
        public int AnimationDurationMs { get; set; }
        public int IntervalS { get; set; } = 5;
    }
}
