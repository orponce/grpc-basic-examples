using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ex7ClientStreamingVideo
{
    internal class VideoDevice
    {
        // Flag that indicates that the video should be stopped
        public bool stop { get; set; } = false;

        // Is the video device running and showing the image?
        public bool isRunning { get; set; } = false;
    }
}
