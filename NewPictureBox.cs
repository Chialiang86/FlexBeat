using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Metronome
{
    public class NewPictureBox : PictureBox
    {
        public event MouseClickEvent MCE;
        public event MouseMoveEvent MME;
        public int correction_x;
        public int correction_y;

        public NewPictureBox(int x = 0, int y = 0)
        {
            this.Location = new Point(x, y);
        }
        public void GetCorrectPos(int x, int y)
        {
            this.correction_x = x;
            this.correction_y = y;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (MME != null)
                MME.Invoke(this.Location.X + e.X, this.Location.Y + e.Y);
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (MCE != null)
                MCE.Invoke(this.Location.X + e.X, this.Location.Y + e.Y);
        }
        public void WindowChanged(double resize_ratio_x, double resize_ratio_y)
        {
            string type;
            float font_size;
            foreach (Control o in this.Controls)
            {
                type = o.ToString().Split(',')[0].Split('.')[1];
                o.Left = (int)(double.Parse(o.Tag.ToString().Split(',')[0]) * resize_ratio_x + 0.5);
                o.Top = (int)(double.Parse(o.Tag.ToString().Split(',')[1]) * resize_ratio_y + 0.5);
                o.Width = (int)(double.Parse(o.Tag.ToString().Split(',')[2]) * resize_ratio_x + 0.5);
                o.Height = (int)(double.Parse(o.Tag.ToString().Split(',')[3]) * resize_ratio_y + 0.5);
                font_size = (float)(double.Parse(o.Font.Size.ToString()) * resize_ratio_x);
                o.Font = new Font("Corbel", font_size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                Application.DoEvents();
            }
        }
    }
}
