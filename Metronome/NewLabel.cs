using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Metronome
{
    public class NewLabel : Label
    {
        public event MouseClickEvent MCE;
        public event MouseMoveEvent MME;
        private int correction_x;
        private int correction_y;

        public NewLabel(int x = 0, int y = 0)
        {
            this.correction_x = 0;
            this.correction_y = 0;
            this.Location = new Point(x, y);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (MME != null)
                MME.Invoke(this.Location.X + this.correction_x + e.X, this.Location.Y + this.correction_y + e.Y);
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (MCE != null)
                MCE.Invoke(this.Location.X + this.correction_x + e.X, this.Location.Y + this.correction_y + e.Y);
        }
        public void GetCorrectPos(int x, int y)
        {
            this.correction_x = x;
            this.correction_y = y;
        }
        public int ReadCorrectXPos()
        {
            return this.correction_x;
        }
    }
}
