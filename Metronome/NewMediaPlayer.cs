using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Metronome
{
    public class NewMediaPlayer : System.Windows.Media.MediaPlayer
    {
        public System.Timers.Timer to_stop;
        private delegate void SelfMuted();
        public int index { get; set; }
        public NewMediaPlayer()
        {
            this.to_stop = new System.Timers.Timer();
            this.to_stop.Enabled = false;
            this.to_stop.Interval = 580;
            this.index = 2;
        }
    }
}
