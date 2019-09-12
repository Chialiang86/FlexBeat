using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Metronome
{
    class InputName : Form
    {
        public Label input_message;
        public TextBox name_input;
        public Button ok_btn;
        public InputName()
        {
            this.Font = new Font("Corbel", 14, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.Size = new Size(600, 600);
            this.Text = "Name Input";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.DoubleBuffered = true;
            this.BackgroundImage = new Bitmap(Properties.Rsc.acoustic, this.Size);
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.Fixed3D;


            this.input_message = new Label();
            this.name_input = new TextBox();
            this.ok_btn = new Button();

            this.input_message.Parent = this;
            this.input_message.Font = new System.Drawing.Font("Corbel", 14, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.input_message.Location = new Point(100, 100);
            this.input_message.Size = new Size(400, 100);
            this.input_message.Text = "Please input your name.";
            this.input_message.ForeColor = Color.FromArgb(19, 31, 33);
            this.input_message.BackColor = Color.Transparent;

            this.name_input.Parent = this;
            this.name_input.Font = new System.Drawing.Font("Corbel", 14, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.name_input.Location = new Point(150, 230);
            this.name_input.Size = new Size(300, 100);
            this.name_input.ForeColor = Color.FromArgb(19, 31, 33);
            this.name_input.Multiline = false;
            this.name_input.Text = "王大吉";
            this.name_input.ImeMode = ImeMode.On;

            this.ok_btn.Font = new System.Drawing.Font("Corbel", 14, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ok_btn.Location = new Point(225, 340);
            this.ok_btn.Size = new Size(150, 60);
            this.ok_btn.Text = "OK";
            this.ok_btn.ForeColor = Color.FromArgb(19, 31, 33);

            this.Controls.Add(this.input_message);
            this.Controls.Add(this.name_input);
            this.Controls.Add(this.ok_btn);
        }

    }
}
