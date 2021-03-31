using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Metronome
{
    class ObjOutwardInfo
    {
        public readonly Size original_windows_size = new Size(1800, 1300);
        public float font_size;
        private int total_lengh;
        public int beatstream_xpos_revise;

        public Size windows_size;
        public Size table_btn_size;
        public Size table_label_size;
        public Size table_message_size;
        public Size table_icon_size;
        public Size drum_img_size;
        public Size beats_icon_img_size;
        public Size setting_img_size;
        public Size icon_img_size;
        public Size drum_size;
        public Size beats_size;
        public Size setting_com_size;

        //table info
        public Point table_edge_pos;
        public Size table_edge_size;
        public Point table_pos;
        public Size table_size;
        public Point beatstream_pos;
        public Size beatstream_size;
        public Point beatstream_bg_pos;
        public Size beatstream_bg_size;

        public Point style_show_pos;
        public Point style_com_pos;
        public Point speed_show_pos;
        public Point beat_speed_nud_pos;
        public Point sig_show_pos;
        public Point time_sig_com_pos;
        public Point drumset_show_pos;
        public Point drumset_com_pos;
        public Point message_show_pos;
        public Point play_btn_pos;
        public Point stop_btn_pos;
        public Point edit_btn_pos;
        public Point rst_btn_pos;
        public Point test_btn_pos;
        public Point test_com_pos;
        public Point next_btn_pos;
        public Point setting_com_pos;
        public Point setting_btn_pos;

        //other
        public Point bass_btn_pos;
        public Point snare_btn_pos;
        public Point hihat_btn_pos;
        public Point clear_btn_pos;
        public Point logo_pos;
        public Size logo_size;
        public Point animation_screen_pos;
        public Size animation_screen_size;
        

        public ObjOutwardInfo(int windows_w = 1800, int windows_h = 1300, int beatstream_w = 1440)
        {
            //this.resize_ratio_x = Convert.ToDouble(original_windows_size.Width) / Convert.ToDouble(windows_w);
            //this.resize_ratio_y = Convert.ToDouble(original_windows_size.Height) / Convert.ToDouble(windows_h);
            this.windows_size = new Size(1800, 1300);

            this.font_size = 12.5f;
            this.total_lengh = 1440;
            this.table_btn_size = new Size(140, 84);
            this.table_label_size = new Size(170, 92);
            this.table_icon_size = new Size(84, 84);
            this.icon_img_size = new Size(40, 40);
            this.beats_size = new Size(90, 90);
            this.beats_icon_img_size = new Size(56, 56);
            this.setting_img_size = new Size(60, 60);
            this.drum_size = new Size(300, 300);
            this.drum_img_size = new Size(160, 160);
            this.setting_com_size = new Size(310, 92);

            //table info
            this.table_edge_pos = new Point(120, 80);
            this.table_edge_size = new Size(1560, 450);
            this.table_pos = new Point(140, 100);
            this.table_size = new Size(1520, 410);
            this.beatstream_pos = new Point(40, 140);
            this.beatstream_size = new Size(this.total_lengh, 90);
            this.beatstream_bg_pos = new Point(32, 132);
            this.beatstream_bg_size = new Size(this.total_lengh + 16, 106);
            this.beatstream_xpos_revise = this.table_pos.X + this.beatstream_pos.X;
            this.speed_show_pos = new Point(1284 - 7 * (this.table_label_size.Width + 10), 20);
            this.beat_speed_nud_pos = new Point(1284 - 6 * (this.table_label_size.Width + 10), 42);
            this.sig_show_pos = new Point(1284 - 5 * (this.table_label_size.Width + 10), 20);
            this.time_sig_com_pos = new Point(1284 - 4 * (this.table_label_size.Width + 10), 42);
            this.drumset_show_pos = new Point(1284 - 3 * (this.table_label_size.Width + 10), 20);
            this.drumset_com_pos = new Point(1284 - 2 * (this.table_label_size.Width + 10), 42);
            this.style_show_pos = new Point(1284 - 1 * (this.table_label_size.Width + 10), 20);
            this.style_com_pos = new Point(1284, 42);
            this.play_btn_pos = new Point(this.beatstream_pos.X + 6, 280);
            this.stop_btn_pos = new Point(this.beatstream_pos.X + 96, 280);
            this.edit_btn_pos = new Point(this.beatstream_pos.X + 200, 280);
            this.rst_btn_pos = new Point(this.beatstream_pos.X + 348, 280);
            this.test_btn_pos = new Point(this.beatstream_pos.X + 496, 280);
            this.next_btn_pos = new Point(this.beatstream_pos.X + 644, 280);
            this.test_com_pos = new Point(this.beatstream_pos.X + 792, 295);
            this.setting_com_pos = new Point(1070, 295);
            this.setting_btn_pos = new Point(1393, 280);

            //animation
            this.animation_screen_pos = new Point(200, 200);
            this.animation_screen_size = new Size(1400, 900);

            //other
            this.bass_btn_pos = new Point(210, 700);
            this.snare_btn_pos = new Point(570, 700);
            this.hihat_btn_pos = new Point(930, 700);
            this.clear_btn_pos = new Point(1290, 700);
            this.message_show_pos = new Point(700, 540);
            this.table_message_size = new Size(1000, 84);
            this.logo_pos = new Point(20, this.windows_size.Height - 250);
            this.logo_size = new Size(360, 120);

            double initial_ratio_x = Convert.ToDouble(windows_w) / Convert.ToDouble(this.windows_size.Width);
            double initial_ratio_y = Convert.ToDouble(windows_h) / Convert.ToDouble(this.windows_size.Height);

            //general
            this.InitialRevise(ref this.font_size, initial_ratio_x);
            this.InitialRevise(ref this.total_lengh, initial_ratio_x);
            this.InitialRevise(ref this.table_btn_size, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.table_label_size, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.table_message_size, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.table_icon_size, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.icon_img_size, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.beats_size, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.beats_icon_img_size, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.drum_size, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.drum_img_size, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.setting_img_size, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.setting_com_size, initial_ratio_x, initial_ratio_y);

            // table
            this.InitialRevise(ref this.table_edge_pos, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.table_edge_size, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.table_pos, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.table_size, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.beatstream_pos, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.beatstream_size, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.beatstream_bg_pos, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.beatstream_bg_size, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.beatstream_xpos_revise, initial_ratio_x);

            this.InitialRevise(ref this.setting_com_pos, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.message_show_pos, initial_ratio_x, initial_ratio_y);

            this.InitialRevise(ref this.speed_show_pos, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.beat_speed_nud_pos, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.sig_show_pos, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.time_sig_com_pos, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.drumset_show_pos, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.drumset_com_pos, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.style_show_pos, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.style_com_pos, initial_ratio_x, initial_ratio_y);

            this.InitialRevise(ref this.play_btn_pos, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.stop_btn_pos, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.edit_btn_pos, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.rst_btn_pos, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.test_btn_pos, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.test_com_pos, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.next_btn_pos, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.setting_btn_pos, initial_ratio_x, initial_ratio_y);

            //animation
            this.InitialRevise(ref this.animation_screen_pos, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.animation_screen_size, initial_ratio_x, initial_ratio_y);

            // other
            this.InitialRevise(ref this.bass_btn_pos, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.snare_btn_pos, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.hihat_btn_pos, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.clear_btn_pos, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.logo_pos, initial_ratio_x, initial_ratio_y);
            this.InitialRevise(ref this.logo_size, initial_ratio_x, initial_ratio_y);
        }
        public void InitialRevise(ref Point get, double rx, double ry)
        {
            get.X = Convert.ToInt32(rx * get.X);
            get.Y = Convert.ToInt32(ry * get.Y);
        }
        public void InitialRevise(ref Size get, double rx, double ry)
        {
            get.Width = Convert.ToInt32(rx * get.Width);
            get.Height = Convert.ToInt32(ry * get.Height);
        }
        public void InitialRevise(ref int get, double rx)
        {
            get = Convert.ToInt32(rx * get);
        }
        public void InitialRevise(ref float get, double rx)
        {
            get = Convert.ToInt32(rx * get);
        }

        public void ResizeBeatstreamInfo(int new_num)
        {
            this.beats_size.Width = this.total_lengh / new_num;
        }

        public void ResizeInfo(double resize_ratio_x, double resize_ratio_y)
        {
            this.beatstream_xpos_revise = Convert.ToInt32(resize_ratio_x * this.beatstream_xpos_revise);
            this.total_lengh = Convert.ToInt32(resize_ratio_x * this.total_lengh);
            this.drum_img_size.Width = Convert.ToInt32(resize_ratio_x * this.drum_img_size.Width);
            this.drum_img_size.Height = Convert.ToInt32(resize_ratio_y * this.drum_img_size.Height);
            this.beats_icon_img_size.Width = Convert.ToInt32(resize_ratio_x * this.beats_icon_img_size.Width);
            this.beats_icon_img_size.Height = Convert.ToInt32(resize_ratio_y * this.beats_icon_img_size.Height);
            this.icon_img_size.Width = Convert.ToInt32(resize_ratio_x * this.icon_img_size.Width);
            this.icon_img_size.Height = Convert.ToInt32(resize_ratio_y * this.icon_img_size.Height);
            this.setting_img_size.Width = Convert.ToInt32(resize_ratio_x * this.setting_img_size.Width);
            this.setting_img_size.Height = Convert.ToInt32(resize_ratio_y * this.setting_img_size.Height);
            this.beats_size.Width = Convert.ToInt32(resize_ratio_x * this.beats_size.Width);
            this.beats_size.Height = Convert.ToInt32(resize_ratio_y * this.beats_size.Height);
            this.table_edge_size.Width = Convert.ToInt32(resize_ratio_x * this.table_edge_size.Width);
            this.table_edge_size.Height = Convert.ToInt32(resize_ratio_y * this.table_edge_size.Height);
            this.table_size.Width = Convert.ToInt32(resize_ratio_x * this.table_size.Width);
            this.table_size.Height = Convert.ToInt32(resize_ratio_y * this.table_size.Height);
            this.beatstream_pos.X = Convert.ToInt32(resize_ratio_x * this.beatstream_pos.X);
            this.beatstream_pos.Y = Convert.ToInt32(resize_ratio_y * this.beatstream_pos.Y);
            this.beatstream_size.Width = Convert.ToInt32(resize_ratio_x * this.beatstream_size.Width);
            this.beatstream_size.Height = Convert.ToInt32(resize_ratio_y * this.beatstream_size.Height);
            this.beatstream_bg_size.Width = Convert.ToInt32(resize_ratio_x * this.beatstream_bg_size.Width);
            this.beatstream_bg_size.Height = Convert.ToInt32(resize_ratio_y * this.beatstream_bg_size.Height);
        }
    }
}
