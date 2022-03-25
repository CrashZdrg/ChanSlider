using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChanSlider.Utils
{
    class LogFile : StreamWriter
    {
        public LogFile(string path)
            : base(new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite), leaveOpen: false)
        {
            AutoFlush = true;
        }
    }
}
