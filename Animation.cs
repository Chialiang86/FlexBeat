using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Metronome
{
    class Animation : Form
    {
        //public delegate void NextFlip(bool state);// 0:test , 1:start
        private PictureBox img_show;
        private Label img_label;
        private Bitmap[][] num_img;
        public int change_index;

        public Animation()
        {
            this.Font = new Font("Corbel", 14, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.Size = new Size(800, 800);
            this.Text = "Name Input";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Load += new EventHandler(Loading);
            
            this.img_show = new NewPictureBox();
            this.img_show.Location = new Point(50,50);
            this.img_show.Size = new Size(700, 700);
            this.img_show.Name = "img_show";
            this.img_show.BorderStyle = BorderStyle.FixedSingle;

            this.img_label = new Label();
            this.img_label.Font = new Font("Corbel", 14, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.img_label.Location = new Point(0, 0);
            this.img_label.Size = new Size(700, 700);
            this.img_label.ImageAlign = ContentAlignment.MiddleCenter;
            this.img_label.Parent = this.img_show;
            this.img_label.BackColor = Color.Transparent;
            this.img_show.Controls.Add(this.img_label);


            this.num_img = new Bitmap[2][];
            this.num_img[0] = new Bitmap[6];
            this.num_img[1] = new Bitmap[6];
            this.num_img[0][0] = new Bitmap(Properties.Rsc.num00, new Size(700, 200));
            this.num_img[0][1] = new Bitmap(Properties.Rsc.num00, new Size(700, 200));
            this.num_img[0][2] = new Bitmap(Properties.Rsc.num01, new Size(700, 300));
            this.num_img[0][3] = new Bitmap(Properties.Rsc.num02, new Size(300, 700));
            this.num_img[0][4] = new Bitmap(Properties.Rsc.num03, new Size(300, 700));
            this.num_img[0][5] = new Bitmap(Properties.Rsc.num04, new Size(300, 700));
            this.num_img[1][0] = new Bitmap(Properties.Rsc.num10, new Size(700, 200));
            this.num_img[1][1] = new Bitmap(Properties.Rsc.num10, new Size(700, 200));
            this.num_img[1][2] = new Bitmap(Properties.Rsc.num11, new Size(700, 300));
            this.num_img[1][3] = new Bitmap(Properties.Rsc.num12, new Size(300, 700));
            this.num_img[1][4] = new Bitmap(Properties.Rsc.num13, new Size(300, 700));
            this.num_img[1][5] = new Bitmap(Properties.Rsc.num14, new Size(300, 700));

            this.Controls.Add(this.img_show);
        }
        public void NextFlip(bool state)
        {
            if(this.change_index < 6 && this.change_index >= 0)
            {
                this.img_label.Image = this.num_img[Convert.ToInt32(state)][this.change_index];
                ++this.change_index;
            }
            else
            {
                this.change_index = 0;
                this.Hide();
            }
        }

        public void Loading(Object sender,EventArgs e)
        {
            this.BackColor = Color.Maroon;
            this.TransparencyKey = Color.Maroon;
        }
    }


}
