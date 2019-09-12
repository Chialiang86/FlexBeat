using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.IO;



namespace Metronome
{
    public delegate void MouseClickEvent(int x, int y);

    public delegate void MouseMoveEvent(int x, int y);

    public enum Drum { bass = 0, snare = 1, hihat = 2, none = 3 }

    public enum Difficulty { easy = 0, normal = 1, hard = 2 }

    public enum ObjCode { table_edge = 0, table = 1, beatstream_bg = 2, beat_icon = 3, play_button = 4, stop_button = 5, signature = 6 }


    public class NewTimer : System.Windows.Forms.Timer
    {
        public int index { get; set; }
    }

    public struct Beaticon
    { 
        public bool active { get; set; }
        public Drum key { get; set; }
        public NewLabel img { get; set; }
        //public SoundPlayer sound_ { get; set; }
        public NewMediaPlayer sound_ { get; set; }
    }

    class mainwindow : Form
    {
        private delegate void UpdateMessageCallBack();
        private delegate void UpdateUICallBack();
        private delegate void UpdateAnimationCallBack();
        public event MouseClickEvent MCE;
        public event MouseMoveEvent MME;

        private const int time_constant = 60000; //one minate
        private const int type_num = 4;

        private static readonly string[] sig_mode = { "2/4", "3/4" , "4/4" , "4/4 swing" };
        private int speed ;
        private double[] vol;  
        private int beats_num;
        private int message_index;
        private int beats_icon_num;// ctrl by sig change 
        private int temp_beat;// ctrl by sig change , stop , timer tick , rst button
        private int temp_edit_beat;// ctrl by mouse move , mouse click , reset , sig change , stop , timer tick , rst button
        private int temp_drum_set;// ctrl by drumset change
        private int temp_style;// ctrl by style change
        private int temp_diff;// ctrl by style change
        private int timer_ctrl;
        private int name_counter;
        private int test_loop_counter;
        private int ani_counter;
        private int ani_fix_counter;
        private int[] test_diff_index;
        private int[] test_diff_max;
        private bool edit_ctrl;// ctrl by edit button , sig change ,
        private bool test_ctrl;// ctrl by test button , play button , stop button , edit button , rst button
        private bool animation_ctrl;
        private bool pause_ctrl;
        private bool[] setting_ctrl;


        private string[] sig_type;
        private string[] drum_set;
        private string[] style;
        private string[] message;
        private string[] diff_text;
        private string[] setting_text;
        private Drum[][][] test_stream = new Drum[3][][];
        private Drum temp_drum;

        private InputName input;
        private Animation ani;
        private FileInfo file_num_r;
        private FileInfo file_num_w;
        private FileInfo file_list;
        private StreamReader num_read;
        private StreamWriter num_write;
        private StreamWriter list_write;
        private ObjOutwardInfo info;
        private System.Random rand;
        private System.Threading.Timer play_timer;
        private System.Timers.Timer message_timer;
        private NumericUpDown beat_speed_nud;
        private Button edit_btn;
        private Button play_btn;
        private Button stop_btn;
        private Button rst_btn;
        private Button test_btn;
        private Button next_btn;
        private Button setting_btn;
        private ComboBox time_sig_com;
        private ComboBox drum_set_com;
        private ComboBox style_com;
        private ComboBox test_com;
        private ComboBox setting_com;
        private Button bass_btn;
        private Button snare_btn;
        private Button hihat_btn;
        private Button clear_btn;
        private NewPictureBox table;
        private NewLabel signature;
        private NewLabel table_box;
        private NewLabel beatstream_bg;
        private NewLabel speed_show;
        private NewLabel sig_show;
        private NewLabel drumset_show;
        private NewLabel style_show;
        private NewLabel message_show;
        private Bitmap background_img;
        private Bitmap beatstream_bg_img;
        private Bitmap table_img;
        private Bitmap table_edge_img;
        private Bitmap signature_img;
        private Bitmap setting_img;
        private Bitmap[][] drum_img = new Bitmap[3][];
        private Bitmap[][] drum_btn_img = new Bitmap[3][];
        private Bitmap[] play_img = new Bitmap[3];
        private Color[] drum_color = new Color[9];
        private Uri[] drum_path = new Uri[3];
        private NewMediaPlayer[] drum_sound = new NewMediaPlayer[3];

        private Beaticon[] beatstream = new Beaticon[16];
        public event MouseClickEvent mouse_pos;

       

        public mainwindow()
        {
            this.Width = 1200;
            this.Height = 880;

            //object construct

            this.info = new ObjOutwardInfo(this.Width, this.Height);
            this.beat_speed_nud = new NumericUpDown();
            this.time_sig_com = new ComboBox();
            this.drum_set_com = new ComboBox();
            this.style_com = new ComboBox();
            this.test_com = new ComboBox();
            this.setting_com = new ComboBox();
            this.edit_btn = new Button();
            this.play_btn = new Button();
            this.stop_btn = new Button();
            this.rst_btn = new Button();
            this.test_btn = new Button();
            this.next_btn = new Button();
            this.bass_btn = new Button();
            this.snare_btn = new Button();
            this.hihat_btn = new Button();
            this.clear_btn = new Button();
            this.play_timer = new System.Threading.Timer(play_timer_Tick);
            this.message_timer = new System.Timers.Timer();
            this.rand = new Random();
            for (int i = 0; i < 16; ++i)
            {
                this.beatstream[i] = new Beaticon();
                this.beatstream[i].img = new NewLabel(this.info.beatstream_pos.X + this.info.beats_size.Width * i, this.info.beatstream_pos.Y);
                this.beatstream[i].sound_ = new NewMediaPlayer();
                this.beatstream[i].sound_.index = i;
            }

            this.input = new InputName();
            this.input.Hide();
            this.input.ok_btn.Click += new EventHandler(NameDone);
            this.input.FormClosed += new FormClosedEventHandler(MainOpen);

            this.test_loop_counter = 0;
            this.ani_counter = 0;
            this.ani_fix_counter = 0;
            this.animation_ctrl = false;
            this.pause_ctrl = false;
            this.ani = new Animation();
            this.ani.Hide();
            
            this.file_num_r = new FileInfo(Application.StartupPath + @"\file\num.cyb");
            this.file_num_w = new FileInfo(Application.StartupPath + @"\file\num.cyb");
            this.file_list = new FileInfo(Application.StartupPath + @"\file\list.cyb");
            if(this.file_num_r.Exists)
            {
                this.num_read = this.file_num_r.OpenText();
                this.name_counter = Convert.ToInt32(this.num_read.ReadLine());
                this.num_read.Close();
            }
            this.num_write = this.file_num_w.CreateText();
            this.list_write = this.file_list.AppendText();


            //important info initialize
            this.speed = 80;
            this.beats_num = 4;
            this.temp_edit_beat = 15;
            this.beats_icon_num = 16;
            this.temp_beat = beats_icon_num - 1;
            this.edit_ctrl = true;
            this.test_ctrl = false;
            this.temp_drum = Drum.bass;

            double w = System.Convert.ToDouble(Properties.Rsc.guitar.Width);
            double h = System.Convert.ToDouble(Properties.Rsc.guitar.Height);
            if (h * this.Width / w > this.Height)
            {
                h = h * Convert.ToDouble(this.Width) / w;
                w = this.Width;
            }
            else
            {
                w = w * Convert.ToDouble(this.Height) / h;
                h = this.Height;
            }

            this.background_img    = new Bitmap(Properties.Rsc.guitar, new Size(System.Convert.ToInt32(w), System.Convert.ToInt32(h)));
            this.table_edge_img    = new Bitmap(Properties.Rsc.woodenwall, this.info.table_edge_size);
            this.table_img         = new Bitmap(Properties.Rsc.table, this.info.table_size);
            this.beatstream_bg_img = new Bitmap(Properties.Rsc.dark, this.info.beatstream_bg_size);
            this.signature_img     = new Bitmap(Properties.Rsc.signature, this.info.logo_size);
            this.setting_img       = new Bitmap(Properties.Rsc.setting, this.info.setting_img_size);
            this.play_img[0]       = new Bitmap(Properties.Rsc.play_img, this.info.icon_img_size);
            this.play_img[1]       = new Bitmap(Properties.Rsc.pause_img, this.info.icon_img_size);
            this.play_img[2]       = new Bitmap(Properties.Rsc.stop_img, this.info.icon_img_size);

            // info
            // this.drum_img[ style ][ DrumIndex(drum key) ]
            this.drum_img[0]    = new Bitmap[12];
            this.drum_img[1]    = new Bitmap[3];
            this.drum_img[2]    = new Bitmap[3];
            this.drum_img[0][0] = new Bitmap(Properties.Rsc._00bass_img , this.info.beats_icon_img_size);
            this.drum_img[0][1] = new Bitmap(Properties.Rsc._00snare_img, this.info.beats_icon_img_size);
            this.drum_img[0][2] = new Bitmap(Properties.Rsc._00hihat_img, this.info.beats_icon_img_size);
            this.drum_img[0][3] = new Bitmap(Properties.Rsc._01bass_img, this.info.beats_icon_img_size);
            this.drum_img[0][4] = new Bitmap(Properties.Rsc._01snare_img, this.info.beats_icon_img_size);
            this.drum_img[0][5] = new Bitmap(Properties.Rsc._01hihat_img, this.info.beats_icon_img_size);
            this.drum_img[0][6] = new Bitmap(Properties.Rsc._02bass_img, this.info.beats_icon_img_size);
            this.drum_img[0][7] = new Bitmap(Properties.Rsc._02snare_img, this.info.beats_icon_img_size);
            this.drum_img[0][8] = new Bitmap(Properties.Rsc._02hihat_img, this.info.beats_icon_img_size);
            this.drum_img[0][9] = new Bitmap(Properties.Rsc._03bass_img, this.info.beats_icon_img_size);
            this.drum_img[0][10] = new Bitmap(Properties.Rsc._03snare_img, this.info.beats_icon_img_size);
            this.drum_img[0][11] = new Bitmap(Properties.Rsc._03hihat_img, this.info.beats_icon_img_size);
            this.drum_img[1][0] = new Bitmap(Properties.Rsc._1bass_img, this.info.beats_icon_img_size);
            this.drum_img[1][1] = new Bitmap(Properties.Rsc._1snare_img, this.info.beats_icon_img_size);
            this.drum_img[1][2] = new Bitmap(Properties.Rsc._1hihat_img, this.info.beats_icon_img_size);
            this.drum_img[2][0] = new Bitmap(Properties.Rsc._2bass_img, this.info.beats_icon_img_size);
            this.drum_img[2][1] = new Bitmap(Properties.Rsc._2snare_img, this.info.beats_icon_img_size);
            this.drum_img[2][2] = new Bitmap(Properties.Rsc._2hihat_img, this.info.beats_icon_img_size);

            // info
            // drum_btn_img[ style ][ DrumIndex(drum key) ]
            this.drum_btn_img[0]     = new Bitmap[12];
            this.drum_btn_img[1]     = new Bitmap[3];
            this.drum_btn_img[2]     = new Bitmap[3];
            this.drum_btn_img[0][0]  = new Bitmap(Properties.Rsc._00bass_img, this.info.drum_img_size);
            this.drum_btn_img[0][1]  = new Bitmap(Properties.Rsc._00snare_img, this.info.drum_img_size);
            this.drum_btn_img[0][2]  = new Bitmap(Properties.Rsc._00hihat_img, this.info.drum_img_size);
            this.drum_btn_img[0][3]  = new Bitmap(Properties.Rsc._01bass_img, this.info.drum_img_size);
            this.drum_btn_img[0][4]  = new Bitmap(Properties.Rsc._01snare_img, this.info.drum_img_size);
            this.drum_btn_img[0][5]  = new Bitmap(Properties.Rsc._01hihat_img, this.info.drum_img_size);
            this.drum_btn_img[0][6]  = new Bitmap(Properties.Rsc._02bass_img, this.info.drum_img_size);
            this.drum_btn_img[0][7]  = new Bitmap(Properties.Rsc._02snare_img, this.info.drum_img_size);
            this.drum_btn_img[0][8]  = new Bitmap(Properties.Rsc._02hihat_img, this.info.drum_img_size);
            this.drum_btn_img[0][9]  = new Bitmap(Properties.Rsc._03bass_img, this.info.drum_img_size);
            this.drum_btn_img[0][10] = new Bitmap(Properties.Rsc._03snare_img, this.info.drum_img_size);
            this.drum_btn_img[0][11] = new Bitmap(Properties.Rsc._03hihat_img, this.info.drum_img_size);
            this.drum_btn_img[1][0]  = new Bitmap(Properties.Rsc._1bass_img, this.info.drum_img_size);
            this.drum_btn_img[1][1]  = new Bitmap(Properties.Rsc._1snare_img, this.info.drum_img_size);
            this.drum_btn_img[1][2]  = new Bitmap(Properties.Rsc._1hihat_img, this.info.drum_img_size);
            this.drum_btn_img[2][0]  = new Bitmap(Properties.Rsc._2bass_img, this.info.drum_img_size);
            this.drum_btn_img[2][1]  = new Bitmap(Properties.Rsc._2snare_img, this.info.drum_img_size);
            this.drum_btn_img[2][2]  = new Bitmap(Properties.Rsc._2hihat_img, this.info.drum_img_size);

            play_img[0] = new Bitmap(Properties.Rsc.play_img , this.info.icon_img_size);
            play_img[1] = new Bitmap(Properties.Rsc.pause_img, this.info.icon_img_size);
            play_img[2] = new Bitmap(Properties.Rsc.stop_img,  this.info.icon_img_size);

            for (int i = 0; i < 9; ++i)
            {
                drum_color[i] = new Color();
            }

            drum_color[0] = Color.FromArgb(210, 230, 210);// bass
            drum_color[1] = Color.FromArgb(210, 230, 230);// snare
            drum_color[2] = Color.FromArgb(255, 252, 193);// hihat
            drum_color[3] = Color.FromArgb(230, 220, 210);// clear
            drum_color[4] = Color.FromArgb(120, 230, 130);// play
            drum_color[5] = Color.FromArgb(190, 190, 190);// beatstream original
            drum_color[6] = Color.FromArgb(190, 231, 233);// label text
            drum_color[7] = Color.FromArgb(0, 0, 0);// button text
            drum_color[8] = Color.FromArgb(150, 150, 150);// button text

            this.sig_type = new string[4];
            this.sig_type[0] = "2/4";
            this.sig_type[1] = "3/4";
            this.sig_type[2] = "4/4";
            this.sig_type[3] = "4/4 Swing";

            this.temp_drum_set = 0;
            this.drum_set = new string[4];
            this.drum_set[0] = "Cajon1";
            this.drum_set[1] = "Cajon2";
            this.drum_set[2] = "Djembe";
            this.drum_set[3] = "Bongo";

            this.temp_style = 0;
            this.style = new string[3];
            this.style[0] = "default";
            this.style[1] = "jazz";
            this.style[2] = "唸出來";

            this.message_index = 0;
            this.message = new string[8];
            this.message[0] = "成韻盃 39th Comming Soon，報名要快喔!!!";
            this.message[1] = "初賽日期 : 11/30 (六)，決賽日期 : 12/14 (六)";
            this.message[2] = "初賽地點 : 成大電機系館靄雲廳";
            this.message[3] = "決賽地點 : 成大醫學院成杏廳";
            this.message[4] = "感謝 Savulu 擔任初賽評審!";
            this.message[5] = "感謝 柯泯薰 擔任決賽評審!";
            this.message[6] = "感謝 南西肯恩 擔任初賽評審!";
            this.message[7] = "感謝 洗耳恭聽 陶婉玲 擔任決賽評審!";

            this.setting_text = new string[3];
            this.setting_text[0] = "關閉/開啟 訊息";
            this.setting_text[1] = "關閉/開啟 成韻Logo";
            this.setting_text[2] = "更換新的背景圖";

            this.temp_diff = (int)Difficulty.easy;
            this.diff_text = new string[3];
            this.diff_text[0] = "easy";//   2/4
            this.diff_text[1] = "normal";// 4/4
            this.diff_text[2] = "hard";//   4/4 swing

            this.test_diff_index = new int[3] { 0, 0, 0 };
            this.test_diff_max   = new int[3] { 3, 3, 3 };
            this.test_stream[(int)Difficulty.easy]   = new Drum[this.test_diff_max[0]][];// 2/4
            this.test_stream[(int)Difficulty.normal] = new Drum[this.test_diff_max[1]][];// 4/4
            this.test_stream[(int)Difficulty.hard]   = new Drum[this.test_diff_max[2]][];// 4/4 swing
            // easy 2/4
            //q1
            this.test_stream[(int)Difficulty.easy][0] = new Drum[8]
                { Drum.bass , Drum.snare, Drum.hihat, Drum.bass ,
                  Drum.bass , Drum.none , Drum.snare, Drum.none };
            //q2
            this.test_stream[(int)Difficulty.easy][1] = new Drum[8]
                { Drum.bass , Drum.hihat, Drum.bass , Drum.bass ,
                  Drum.snare, Drum.none , Drum.snare, Drum.snare};
            //q3
            this.test_stream[(int)Difficulty.easy][2] = new Drum[8]
                { Drum.bass , Drum.none , Drum.hihat, Drum.snare,
                  Drum.hihat, Drum.hihat, Drum.bass , Drum.none};
            // normal 4/4
            //q1
            this.test_stream[(int)Difficulty.normal][0] = new Drum[16]
               { Drum.bass , Drum.none , Drum.bass , Drum.none ,
                 Drum.snare, Drum.none , Drum.hihat, Drum.bass ,
                 Drum.hihat, Drum.hihat, Drum.bass , Drum.none ,
                 Drum.snare, Drum.bass , Drum.none , Drum.bass };
            //q2
            this.test_stream[(int)Difficulty.normal][1] = new Drum[16]
               { Drum.bass , Drum.none , Drum.hihat, Drum.snare ,
                 Drum.hihat, Drum.hihat, Drum.bass , Drum.hihat ,
                 Drum.hihat, Drum.snare, Drum.hihat, Drum.hihat ,
                 Drum.snare, Drum.none , Drum.none , Drum.bass  };
            //q2
            this.test_stream[(int)Difficulty.normal][2] = new Drum[16]
               { Drum.bass , Drum.none , Drum.snare, Drum.bass  ,
                 Drum.bass , Drum.snare, Drum.hihat, Drum.snare ,
                 Drum.none , Drum.snare, Drum.none , Drum.snare ,
                 Drum.snare, Drum.none , Drum.snare, Drum.none  };
            // hard
            //q1
            this.test_stream[(int)Difficulty.hard][0] = new Drum[24]
               { Drum.bass , Drum.none , Drum.hihat, Drum.hihat, Drum.none , Drum.hihat,
                 Drum.snare, Drum.none , Drum.hihat, Drum.hihat, Drum.none , Drum.bass ,
                 Drum.hihat, Drum.none , Drum.hihat, Drum.bass , Drum.none , Drum.hihat,
                 Drum.snare, Drum.hihat, Drum.hihat, Drum.bass , Drum.hihat, Drum.bass };
            //q2
            this.test_stream[(int)Difficulty.hard][1] = new Drum[24]
               { Drum.bass , Drum.none , Drum.hihat, Drum.bass , Drum.none , Drum.bass ,
                 Drum.hihat, Drum.none , Drum.hihat, Drum.snare, Drum.none , Drum.bass ,
                 Drum.bass , Drum.none , Drum.snare, Drum.bass , Drum.none , Drum.bass ,
                 Drum.snare, Drum.hihat, Drum.snare, Drum.bass , Drum.snare, Drum.bass };
            //q2
            this.test_stream[(int)Difficulty.hard][2] = new Drum[24]
               { Drum.bass , Drum.none , Drum.hihat, Drum.snare, Drum.none , Drum.bass ,
                 Drum.bass , Drum.none , Drum.snare, Drum.hihat, Drum.none , Drum.snare,
                 Drum.hihat, Drum.none , Drum.snare, Drum.bass , Drum.none , Drum.snare,
                 Drum.snare, Drum.hihat, Drum.hihat, Drum.snare, Drum.none , Drum.bass };


            this.drum_path[0] = new Uri(Application.StartupPath.Substring(0, Application.StartupPath.Length ) + @"\Resources\" + drum_set[0] + @"\bass.wav");
            this.drum_path[1] = new Uri(Application.StartupPath.Substring(0, Application.StartupPath.Length ) + @"\Resources\" + drum_set[0] + @"\snare.wav");
            this.drum_path[2] = new Uri(Application.StartupPath.Substring(0, Application.StartupPath.Length ) + @"\Resources\" + drum_set[0] + @"\hihat.wav");
            /*
            var fileStream1 = File.Create(Application.StartupPath.Substring(0, Application.StartupPath.Length - 11) + @"Resources\bass.wav");
            var fileStream2 = File.Create(Application.StartupPath.Substring(0, Application.StartupPath.Length - 11) + @"Resources\snare.wav");
            var fileStream3 = File.Create(Application.StartupPath.Substring(0, Application.StartupPath.Length - 11) + @"Resources\hihat.wav");

            MessageBox.Show(fileStream1.Name);
            Properties.Rsc.bass.CopyTo(fileStream1);
            Properties.Rsc.snare.CopyTo(fileStream2);
            Properties.Rsc.hihat.CopyTo(fileStream3);

            drum_path[0] = new Uri(fileStream1.Name);
            drum_path[1] = new Uri(fileStream2.Name);
            drum_path[2] = new Uri(fileStream3.Name);*/

            /*
            drum_sound[0] = new SoundPlayer(Properties.Rsc.bass);
            drum_sound[1] = new SoundPlayer(Properties.Rsc.snare);
            drum_sound[2] = new SoundPlayer(Properties.Rsc.hihat);*/

            this.vol = new double[3];
            this.vol[(int)Drum.bass] = 10000000.00;
            this.vol[(int)Drum.snare] = 80000.00;
            this.vol[(int)Drum.hihat] = 0.5;
            for (int i = 0; i < drum_sound.Length; ++i)
            {
                drum_sound[i] = new NewMediaPlayer();
                drum_sound[i].Open(drum_path[i]);
                drum_sound[i] = drum_sound[i];
                drum_sound[i].Stop();
                drum_sound[i].Volume = this.vol[i];
               Application.DoEvents();
            }


            //table
            this.table = new NewPictureBox();
            this.table.Location = this.info.table_pos;
            this.table.Size = this.info.table_size;
            this.table.Image = this.table_img;
            this.table.Name = "table";
            this.table.BorderStyle = BorderStyle.FixedSingle;
            this.table.MCE += new MouseClickEvent(MouseClickPos);
            this.table.MME += new MouseMoveEvent(MouseMovePos);

            // table_edge
            this.table_box = new NewLabel();
            this.table_box.Location = this.info.table_edge_pos;
            this.table_box.Size = this.info.table_edge_size;
            this.table_box.Image = this.table_edge_img;
            this.table_box.Name = "table_box";
            this.table_box.MCE += new MouseClickEvent(MouseClickPos);
            this.table_box.MME += new MouseMoveEvent(MouseMovePos);

            // message_show
            this.message_show = new NewLabel();
            this.message_show.Location = this.info.message_show_pos;
            this.message_show.Size = this.info.table_message_size;
            this.message_show.Font = new Font("Corbel", this.info.font_size, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.message_show.ForeColor = drum_color[6];
            this.message_show.Text = this.message[0];
            this.message_show.TextAlign = ContentAlignment.MiddleCenter;
            this.message_show.Name = "message_show";
            this.message_show.TabIndex = 12;
            this.message_show.BackColor = Color.Transparent;
            this.message_show.MME += new MouseMoveEvent(MouseMovePos);
            this.message_show.MCE += new MouseClickEvent(MouseClickPos);


            // speed_show
            this.speed_show = new NewLabel();//(120,80)
            this.speed_show.Location = this.info.speed_show_pos;
            this.speed_show.Size = this.info.table_label_size;
            this.speed_show.Font = new Font("Corbel", this.info.font_size, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.speed_show.ForeColor = drum_color[6];
            this.speed_show.Text = "Speed ";
            this.speed_show.Name = "speed_show";
            this.speed_show.TabIndex = 13;
            this.speed_show.TextAlign = ContentAlignment.MiddleRight;
            this.speed_show.BackColor = Color.Transparent;
            this.speed_show.GetCorrectPos(info.table_pos.X, info.table_pos.Y);
            this.speed_show.MME += new MouseMoveEvent(MouseMovePos);
            this.speed_show.MCE += new MouseClickEvent(MouseClickPos);

            // beat_speed_nud
            this.beat_speed_nud.Font = new System.Drawing.Font("Corbel", this.info.font_size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.beat_speed_nud.Location = this.info.beat_speed_nud_pos;
            this.beat_speed_nud.Size = this.info.table_label_size;
            this.beat_speed_nud.AutoSize = false;
            this.beat_speed_nud.Maximum = new decimal(new int[] { 160, 0, 0, 0 });
            this.beat_speed_nud.Minimum = new decimal(new int[] { 30 , 0, 0, 0 });
            this.beat_speed_nud.Name = "beat_speed_nud";
            this.beat_speed_nud.TabIndex = 2;
            this.beat_speed_nud.Value = new decimal(new int[] { 80, 0, 0, 0 });
            this.beat_speed_nud.Text = System.Convert.ToString(this.speed);
            this.beat_speed_nud.TextAlign = HorizontalAlignment.Right;
            this.beat_speed_nud.ValueChanged += new System.EventHandler(this.beat_speed_nud_ValueChanged);

            // sig_show
            this.sig_show = new NewLabel();
            this.sig_show.Location = this.info.sig_show_pos;
            this.sig_show.Size = this.info.table_label_size;
            this.sig_show.Font = new Font("Corbel", this.info.font_size, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.sig_show.ForeColor = drum_color[6];
            this.sig_show.Text = "Signature ";
            this.sig_show.Name = "sig_show";
            this.sig_show.TabIndex = 14;
            this.sig_show.TextAlign = ContentAlignment.MiddleRight;
            this.sig_show.BackColor = Color.Transparent;
            this.sig_show.GetCorrectPos(info.table_pos.X, info.table_pos.Y);
            this.sig_show.MME += new MouseMoveEvent(MouseMovePos);
            this.sig_show.MCE += new MouseClickEvent(MouseClickPos);

            // time_sig_com
            this.time_sig_com.Font = new System.Drawing.Font("Corbel", this.info.font_size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            for (int i = 0; i < sig_mode.Length; ++i)
            {
                this.time_sig_com.Items.Add(sig_mode[i]);
            }
            this.time_sig_com.Location = this.info.time_sig_com_pos;
            this.time_sig_com.Size = this.info.table_label_size;
            this.time_sig_com.IntegralHeight = false;
            this.time_sig_com.Name = "time_sig_com";
            this.time_sig_com.Sorted = true;
            this.time_sig_com.TabIndex = 3;
            this.time_sig_com.Text = sig_mode[2];
            this.time_sig_com.SelectedIndexChanged += new System.EventHandler(this.time_sig_com_SelectedItemChanged);
            this.time_sig_com.DropDownStyle = ComboBoxStyle.DropDownList;

            //drumset_show
            this.drumset_show = new NewLabel();
            this.drumset_show.Location = this.info.drumset_show_pos;
            this.drumset_show.Size = this.info.table_label_size;
            this.drumset_show.Font = new Font("Corbel", this.info.font_size, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.drumset_show.ForeColor = drum_color[6];
            this.drumset_show.Text = "Drum Set";
            this.drumset_show.Name = "drumset_show";
            this.drumset_show.TabIndex = 14;
            this.drumset_show.TextAlign = ContentAlignment.MiddleRight;
            this.drumset_show.BackColor = Color.Transparent;
            this.drumset_show.GetCorrectPos(info.table_pos.X, info.table_pos.Y);
            this.drumset_show.MME += new MouseMoveEvent(MouseMovePos);
            this.drumset_show.MCE += new MouseClickEvent(MouseClickPos);


            // drum_set_com
            this.drum_set_com.Font = new System.Drawing.Font("Corbel", this.info.font_size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            for (int i = 0; i < this.drum_set.Length; ++i)
            {
                this.drum_set_com.Items.Add(drum_set[i]);
            }
            this.drum_set_com.Location = this.info.drumset_com_pos;
            this.drum_set_com.Size = this.info.table_label_size;
            this.drum_set_com.IntegralHeight = false;
            this.drum_set_com.Name = "drum_set_com";
            this.drum_set_com.Sorted = false;
            this.drum_set_com.TabIndex = 3;
            this.drum_set_com.Text = drum_set[0];
            this.drum_set_com.SelectedIndexChanged += new System.EventHandler(this.drum_set_com_SelectedItemChanged);
            this.drum_set_com.DropDownStyle = ComboBoxStyle.DropDownList;

            //style_show
            this.style_show = new NewLabel();
            this.style_show.Location = this.info.style_show_pos;
            this.style_show.Size = this.info.table_label_size;
            this.style_show.Font = new Font("Corbel", this.info.font_size, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.style_show.ForeColor = drum_color[6];
            this.style_show.Text = "Icon Style";
            this.style_show.Name = "style_show";
            this.style_show.TabIndex = 14;
            this.style_show.TextAlign = ContentAlignment.MiddleRight;
            this.style_show.BackColor = Color.Transparent;
            this.style_show.GetCorrectPos(this.table.Location.X, this.table.Location.Y);
            this.style_show.MME += new MouseMoveEvent(MouseMovePos);
            this.style_show.MCE += new MouseClickEvent(MouseClickPos);

            // style_com
            this.style_com.Font = new System.Drawing.Font("Corbel", this.info.font_size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            for (int i = 0; i < this.style.Length; ++i)
            {
                this.style_com.Items.Add(this.style[i]);
            }
            this.style_com.Location = this.info.style_com_pos;
            this.style_com.Size = this.info.table_label_size;
            this.style_com.IntegralHeight = false;
            this.style_com.Name = "style_com";
            this.style_com.Sorted = true;
            this.style_com.TabIndex = 3;
            this.style_com.Text = style[0];
            this.style_com.SelectedIndexChanged += new System.EventHandler(this.style_com_SelectedItemChanged);
            this.style_com.DropDownStyle = ComboBoxStyle.DropDownList;

            // edit_btn
            this.edit_btn.Font = new System.Drawing.Font("Corbel", this.info.font_size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.edit_btn.ForeColor = drum_color[7];
            this.edit_btn.Location = this.info.edit_btn_pos;
            this.edit_btn.Name = "edit_btn";
            this.edit_btn.Size = info.table_btn_size;
            this.edit_btn.TabIndex = 4;
            this.edit_btn.Text = "Done";
            this.edit_btn.UseVisualStyleBackColor = true;
            this.edit_btn.Click += new System.EventHandler(this.edit_btn_Click);

            // play_btn
            this.play_btn.Location = this.info.play_btn_pos;
            this.play_btn.Size = this.info.table_icon_size;
            this.play_btn.Name = "play_btn";
            this.play_btn.TabIndex = 4;
            this.play_btn.UseVisualStyleBackColor = true;
            this.play_btn.Image = play_img[0];
            this.play_btn.ImageAlign = ContentAlignment.MiddleCenter;
            this.play_btn.Click += new System.EventHandler(this.play_btn_Click);
            this.play_btn.FlatAppearance.BorderSize = 10;
            this.play_btn.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);

            // stop_btn
            this.stop_btn.Location = this.info.stop_btn_pos;
            this.stop_btn.Size = this.info.table_icon_size;
            this.stop_btn.Name = "stop_btn";
            this.stop_btn.TabIndex = 6;
            this.stop_btn.UseVisualStyleBackColor = true;
            this.stop_btn.Image = play_img[2];
            this.stop_btn.ImageAlign = ContentAlignment.MiddleCenter;
            this.stop_btn.Click += new System.EventHandler(this.stop_btn_Click);
            this.stop_btn.FlatAppearance.BorderSize = 10;
            this.stop_btn.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);

            // rst_btn
            this.rst_btn.Font = new System.Drawing.Font("Corbel", this.info.font_size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rst_btn.Location = this.info.rst_btn_pos;
            this.rst_btn.Size = this.info.table_btn_size;
            this.rst_btn.ForeColor = drum_color[7];
            this.rst_btn.Name = "rst_btn";
            this.rst_btn.TabIndex = 7;
            this.rst_btn.Text = "Reset";
            this.rst_btn.UseVisualStyleBackColor = true;
            this.rst_btn.Click += new System.EventHandler(this.rst_btn_Click);
            this.rst_btn.FlatAppearance.BorderSize = 10;
            this.rst_btn.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);

            // test_btn
            this.test_btn.Font = new System.Drawing.Font("Corbel", this.info.font_size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.test_btn.Location = this.info.test_btn_pos;
            this.test_btn.Size = this.info.table_btn_size;
            this.test_btn.ForeColor = drum_color[7];
            this.test_btn.Name = "test_btn";
            this.test_btn.TabIndex = 7;
            this.test_btn.Text = "Test";
            this.test_btn.UseVisualStyleBackColor = true;
            this.test_btn.Click += new System.EventHandler(this.test_btn_Click);
            this.test_btn.FlatAppearance.BorderSize = 10;
            this.test_btn.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
            this.test_btn.Show();


            // test_com
            this.test_com.Font = new System.Drawing.Font("Corbel", this.info.font_size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            for (int i = 0; i < this.diff_text.Length; ++i)
            {
                this.test_com.Items.Add(this.diff_text[i]);
            }
            this.test_com.Location = this.info.test_com_pos;
            this.test_com.Size = this.info.table_label_size;
            this.test_com.IntegralHeight = false;
            this.test_com.Name = "test_com";
            this.test_com.Text = "easy";
            this.test_com.TabIndex = 3;
            this.test_com.Text = style[0];
            this.test_com.SelectedIndexChanged += new System.EventHandler(this.test_com_SelectedItemChanged);
            this.test_com.DropDownStyle = ComboBoxStyle.DropDownList;
            this.test_com.Hide();


            // next_btn
            this.next_btn.Font = new System.Drawing.Font("Corbel", this.info.font_size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.next_btn.Location = this.info.next_btn_pos;
            this.next_btn.Size = this.info.table_btn_size;
            this.next_btn.ForeColor = drum_color[7];
            this.next_btn.Name = "test_btn";
            this.next_btn.TabIndex = 7;
            this.next_btn.Text = "Change";
            this.next_btn.UseVisualStyleBackColor = true;
            this.next_btn.Click += new System.EventHandler(this.next_btn_Click);
            this.next_btn.FlatAppearance.BorderSize = 10;
            this.next_btn.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
            this.next_btn.Hide();

            //setting_com
            this.setting_com.Font = new System.Drawing.Font("Corbel", this.info.font_size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            for (int i = 0; i < this.setting_text.Length; ++i)
            {
                this.setting_com.Items.Add(this.setting_text[i]);
            }
            this.setting_com.Location = this.info.setting_com_pos;
            this.setting_com.Size = this.info.setting_com_size;
            this.setting_com.IntegralHeight = false;
            this.setting_com.Name = "setting_com";
            this.setting_com.Sorted = false;
            this.setting_com.TabIndex = 3;
            this.setting_com.Text = this.setting_text[0];
            this.setting_com.DropDownStyle = ComboBoxStyle.DropDownList;
            this.setting_com.SelectedIndexChanged += new System.EventHandler(this.setting_com_SelectedItemChanged);
            this.setting_com.Hide();

            // setting_btn
            this.setting_ctrl = new bool[4] { true, true, true, false};
            this.setting_btn = new Button();
            this.setting_btn.Location = this.info.setting_btn_pos;
            this.setting_btn.Size = this.info.table_icon_size;
            this.setting_btn.Font = new Font("Corbel", this.info.font_size, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.setting_btn.TextAlign = ContentAlignment.TopRight;
            this.setting_btn.ForeColor = this.drum_color[7];
            this.setting_btn.Name = "setting_btn";
            this.setting_btn.TabIndex = 12;
            this.setting_btn.Image = this.setting_img;
            this.setting_btn.Click += new EventHandler(setting_btn_Click);


            //beatstream bg
            this.beatstream_bg = new NewLabel();
            this.beatstream_bg.Location = this.info.beatstream_bg_pos;
            this.beatstream_bg.Size = this.info.beatstream_bg_size;
            this.beatstream_bg.BorderStyle = BorderStyle.None;
            this.beatstream_bg.Image = this.beatstream_bg_img;
            this.beatstream_bg.ImageAlign = ContentAlignment.MiddleCenter;
            this.beatstream_bg.GetCorrectPos(info.table_pos.X, info.table_pos.Y);
            this.beatstream_bg.MCE += new MouseClickEvent(MouseClickPos);
            this.beatstream_bg.MME += new MouseMoveEvent(MouseMovePos);

            //signature
            this.signature = new NewLabel();
            this.signature.Location = this.info.logo_pos;
            this.signature.Size = this.info.logo_size;
            this.signature.BorderStyle = BorderStyle.None;
            this.signature.Image = this.signature_img;
            this.signature.ImageAlign = ContentAlignment.MiddleCenter;
            this.signature.BackColor = Color.Transparent;
            this.signature.GetCorrectPos(info.table_pos.X, info.table_pos.Y);
            this.signature.MCE += new MouseClickEvent(MouseClickPos);
            this.signature.MME += new MouseMoveEvent(MouseMovePos);

            //beatstream
            for (int i = 0; i < 16; ++i)
            {
                this.beatstream[i].active = false;
                this.beatstream[i].key = Drum.none;
                this.beatstream[i].img.Name = "beatstream";
                this.beatstream[i].img.Size = this.info.beats_size;
                this.beatstream[i].img.BackColor = drum_color[5];
                this.beatstream[i].img.BorderStyle = BorderStyle.FixedSingle;
                this.beatstream[i].img.ImageAlign = ContentAlignment.MiddleCenter;
                this.beatstream[i].img.BorderStyle = BorderStyle.Fixed3D;
                this.beatstream[i].img.GetCorrectPos(info.table_pos.X, info.table_pos.Y);
                this.beatstream[i].img.MCE += new MouseClickEvent(MouseClickPos);
                this.beatstream[i].img.MME += new MouseMoveEvent(MouseMovePos);
            }

            for (int i = 0; i < beats_icon_num; i += this.beats_num)
            {
                if ((i / this.beats_num) % 2 == 0)
                {
                    this.beatstream[i].active = true;
                    this.beatstream[i].key = Drum.bass;
                    this.beatstream[i].img.Image = this.drum_img[this.temp_style][DrumIndex(this.beatstream[i].key)];
                    this.beatstream[i].sound_ = drum_sound[(int)this.beatstream[i].key];
                    this.beatstream[i].sound_.Stop();
                    this.beatstream[i].sound_.Volume = this.vol[(int)this.beatstream[i].key];
                }
                else
                {
                    this.beatstream[i].active = true;
                    this.beatstream[i].key = Drum.snare;
                    this.beatstream[i].img.Image = this.drum_img[this.temp_style][DrumIndex(this.beatstream[i].key)];
                    this.beatstream[i].sound_ = drum_sound[(int)this.beatstream[i].key];
                    this.beatstream[i].sound_.Stop();
                    this.beatstream[i].sound_.Volume = this.vol[(int)this.beatstream[i].key];
                }
                this.beatstream[i].img.BackColor = drum_color[8];
            }

            
            // bass_btn
            this.bass_btn.BackColor = drum_color[0];
            this.bass_btn.Font = new System.Drawing.Font("Corbel", this.info.font_size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bass_btn.Location = this.info.bass_btn_pos;
            this.bass_btn.Size = this.info.drum_size;
            this.bass_btn.ForeColor = drum_color[7];
            this.bass_btn.Name = "bass_btn";
            this.bass_btn.TabIndex = 8;
            //this.bass_btn.Text = "Bass";
            this.bass_btn.TextAlign = ContentAlignment.BottomCenter;
            this.bass_btn.UseVisualStyleBackColor = false;
            this.bass_btn.Image = this.drum_btn_img[this.temp_style][DrumIndex(Drum.bass)];
            this.bass_btn.ImageAlign = ContentAlignment.MiddleCenter;
            this.bass_btn.Click += new System.EventHandler(this.bass_btn_Click);

            // snare_btn
            this.snare_btn.BackColor = drum_color[1];
            this.snare_btn.Font = new System.Drawing.Font("Corbel", this.info.font_size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.snare_btn.Location = this.info.snare_btn_pos;
            this.snare_btn.Size = this.info.drum_size;
            this.snare_btn.ForeColor = drum_color[7];
            this.snare_btn.Name = "snare_btn";
            this.snare_btn.TabIndex = 9;
            //this.snare_btn.Text = "Snare";
            this.snare_btn.TextAlign = ContentAlignment.BottomCenter;
            this.snare_btn.UseVisualStyleBackColor = false;
            this.snare_btn.Image = this.drum_btn_img[this.temp_style][DrumIndex(Drum.snare)];
            this.snare_btn.ImageAlign = ContentAlignment.MiddleCenter;
            this.snare_btn.Click += new System.EventHandler(this.snare_btn_Click);
            

            // hihat_btn
            this.hihat_btn.BackColor = drum_color[2];
            this.hihat_btn.Font = new System.Drawing.Font("Corbel", this.info.font_size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hihat_btn.Location = this.info.hihat_btn_pos;
            this.hihat_btn.Size = this.info.drum_size;
            this.hihat_btn.ForeColor = drum_color[7];
            this.hihat_btn.Name = "hihat_btn";
            this.hihat_btn.TabIndex = 10;
            //this.hihat_btn.Text = "Hi-hat";
            this.hihat_btn.TextAlign = ContentAlignment.BottomCenter;
            this.hihat_btn.UseVisualStyleBackColor = false;
            this.hihat_btn.Image = this.drum_btn_img[this.temp_style][DrumIndex(Drum.hihat)];
            this.hihat_btn.ImageAlign = ContentAlignment.MiddleCenter;
            this.hihat_btn.Click += new System.EventHandler(this.hihat_btn_Click);

            // clear_btn
            this.clear_btn.BackColor = drum_color[3];
            this.clear_btn.Font = new System.Drawing.Font("Corbel", this.info.font_size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clear_btn.Location = this.info.clear_btn_pos;
            this.clear_btn.Size = this.info.drum_size;
            this.clear_btn.ForeColor = drum_color[7];
            this.clear_btn.Name = "clear_btn";
            this.clear_btn.TabIndex = 11;
            this.clear_btn.Text = "Clear";
            this.clear_btn.TextAlign = ContentAlignment.MiddleCenter;
            this.clear_btn.UseVisualStyleBackColor = false;
            this.clear_btn.Click += new System.EventHandler(this.clear_btn_Click);

            // play_timer
            //this.play_timer.Elapsed += new System.Timers.ElapsedEventHandler(this.play_timer_Tick);
            //this.play_timer.Interval = Convert.ToDouble(time_constant) / Convert.ToDouble(speed * beats_num);
            //this.play_timer.Enabled = false;
            int interval = time_constant / (this.speed * this.beats_num);
            this.timer_ctrl = -1;
            this.play_timer.Change(this.timer_ctrl, interval);

            // message_timer
            this.message_timer.Elapsed += new System.Timers.ElapsedEventHandler(this.message_timer_Tick);
            this.message_timer.Interval = 5000;
            this.message_timer.Enabled = true;


            this.ResetTableControl();
            this.ResetControl();
            this.ResetTag();


            // mainwindow
            ((System.ComponentModel.ISupportInitialize)(this.beat_speed_nud)).BeginInit();
            this.SuspendLayout();
            this.BackgroundImage = this.background_img;
            this.DoubleBuffered = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(17F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = true;
            this.mouse_pos += new MouseClickEvent(MouseClickPos);
            this.Resize += new EventHandler(WindowChanged);
            this.MME += new MouseMoveEvent(MouseMovePos);
            this.MCE += new MouseClickEvent(MouseClickPos);
            this.FormClosing += new FormClosingEventHandler(ClosingSet);


            this.Font = new System.Drawing.Font("新細明體", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(136)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Name = "mainwindow";
            this.Text = "成韻盃 織音-Drum";
            ((System.ComponentModel.ISupportInitialize)(this.beat_speed_nud)).EndInit();
            //((System.ComponentModel.ISupportInitialize)(this.title_label)).EndInit();
            //this.ResumeLayout(false);
        }

        private void MainOpen(Object sender,FormClosedEventArgs e)
        {
            this.Enabled = true;
        }
        

        private void ResetControl()
        {
            if (this.Controls != null)
            {
                this.Controls.Clear();
            }
            this.Controls.Add(this.table);
            this.Controls.Add(this.table_box);
            this.Controls.Add(this.hihat_btn);
            this.Controls.Add(this.snare_btn);
            this.Controls.Add(this.bass_btn);
            this.Controls.Add(this.clear_btn);
            this.Controls.Add(this.signature);
            this.Controls.Add(this.message_show);
        }

        private void ResetTableControl()
        {
            for (int i = 0; i < beats_icon_num; ++i)
            {
                this.beatstream[i].img.Parent = this.table;
            }
            this.play_btn.Parent = this.table;
            this.stop_btn.Parent = this.table;
            this.edit_btn.Parent = this.table;
            this.rst_btn.Parent = this.table;
            this.test_btn.Parent = this.table;
            this.test_com.Parent = this.table;
            this.next_btn.Parent = this.table;
            this.speed_show.Parent = this.table;
            this.sig_show.Parent = this.table;
            this.beat_speed_nud.Parent = this.table;
            this.time_sig_com.Parent = this.table;
            this.drum_set_com.Parent = this.table;
            this.beatstream_bg.Parent = this.table;
            this.drumset_show.Parent = this.table;
            this.style_com.Parent = this.table;
            this.style_show.Parent = this.table;
            this.setting_btn.Parent = this.table;
            this.setting_com.Parent = this.table;

            //add controls
            if (this.table.Controls != null)
            {
                this.table.Controls.Clear();
            }
            for (int i = 0; i < this.beats_icon_num; ++i)
            {
                this.table.Controls.Add(this.beatstream[i].img);
            }
            this.table.Controls.Add(this.beatstream_bg);
            this.table.Controls.Add(this.speed_show);
            this.table.Controls.Add(this.beat_speed_nud);
            this.table.Controls.Add(this.sig_show);
            this.table.Controls.Add(this.time_sig_com);
            this.table.Controls.Add(this.drumset_show);
            this.table.Controls.Add(this.drum_set_com);
            this.table.Controls.Add(this.style_com);
            this.table.Controls.Add(this.style_show);

            this.table.Controls.Add(this.play_btn);
            this.table.Controls.Add(this.stop_btn);
            this.table.Controls.Add(this.edit_btn);
            this.table.Controls.Add(this.rst_btn);
            this.table.Controls.Add(this.test_btn);
            this.table.Controls.Add(this.test_com);
            this.table.Controls.Add(this.next_btn);
            this.table.Controls.Add(this.setting_btn);
            this.table.Controls.Add(this.setting_com);
        }

        private void WindowChanged(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                this.BackgroundImage = new Bitmap(this.BackgroundImage, new Size(this.Width, this.Height));
                if (this.timer_ctrl == 0/*this.play_timer.Enabled*/)
                {
                    //pause until all objects resize 
                    this.play_timer.Change(-1, -1);
                    this.ResetPosSize();
                    this.ResetImage();
                    this.ResetTag();
                    int interval = time_constant / (this.speed * this.beats_num);
                    this.play_timer.Change(this.timer_ctrl, interval);
                    this.play_btn.Image = new Bitmap(Properties.Rsc.pause_img, this.info.icon_img_size);
                    Application.DoEvents();
                }
                else
                {
                    this.ResetPosSize();
                    this.ResetImage();
                    this.ResetTag();
                    this.play_btn.Image = new Bitmap(Properties.Rsc.play_img, this.info.icon_img_size);
                    Application.DoEvents();
                }
            }
        }
        private void ResetPosSize()
        {
            double resize_ratio_x;
            double resize_ratio_y;
            float font_size;

            resize_ratio_x = Convert.ToDouble(this.Width) / double.Parse(this.Tag.ToString().Split(',')[0]);
            resize_ratio_y = Convert.ToDouble(this.Height) / double.Parse(this.Tag.ToString().Split(',')[1]);
            foreach (Control o in this.Controls)
            {
                if (o != null)
                {
                    o.Left = (int)(double.Parse(o.Tag.ToString().Split(',')[0]) * resize_ratio_x + 0.5);
                    o.Top = (int)(double.Parse(o.Tag.ToString().Split(',')[1]) * resize_ratio_y + 0.5);
                    o.Width = (int)(double.Parse(o.Tag.ToString().Split(',')[2]) * resize_ratio_x + 0.5);
                    o.Height = (int)(double.Parse(o.Tag.ToString().Split(',')[3]) * resize_ratio_y + 0.5);
                    font_size = (float)(double.Parse(o.Font.Size.ToString()) * resize_ratio_x);
                    o.Font = new Font("Corbel", font_size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    Application.DoEvents();
                }
            }
            this.table.WindowChanged(resize_ratio_x, resize_ratio_y);
            this.info.ResizeInfo(resize_ratio_x, resize_ratio_y);
        }

        private void ResetImage()
        {
            //okok
            this.sig_show.GetCorrectPos(this.table.Location.X, this.table.Location.Y);
            this.speed_show.GetCorrectPos(this.table.Location.X, this.table.Location.Y);
            this.drumset_show.GetCorrectPos(this.table.Location.X, this.table.Location.Y);
            this.style_show.GetCorrectPos(this.table.Location.X, this.table.Location.Y);

            this.table_edge_img = new Bitmap(Properties.Rsc.woodenwall, this.info.table_edge_size);
            this.table_img = new Bitmap(Properties.Rsc.table, this.info.table_size);
            this.beatstream_bg_img = new Bitmap(Properties.Rsc.dark, this.info.beatstream_bg_size);
            this.signature_img = new Bitmap(Properties.Rsc.signature, this.signature.Size);
            this.setting_img = new Bitmap(Properties.Rsc.setting, this.info.setting_img_size);
            Application.DoEvents();

            this.beatstream_bg.Image = this.beatstream_bg_img;
            this.table_box.Image = this.table_edge_img;
            this.table.Image = this.table_img;
            this.setting_btn.Image = this.setting_img;
            this.signature.Image = this.signature_img;
            this.signature.GetCorrectPos(this.table.Location.X, this.table.Location.Y);
            this.beatstream_bg.GetCorrectPos(this.table.Location.X, this.table.Location.Y);
            Application.DoEvents();

            this.drum_img[0][0]  = new Bitmap(Properties.Rsc._00bass_img, this.info.beats_icon_img_size);
            this.drum_img[0][1]  = new Bitmap(Properties.Rsc._00snare_img, this.info.beats_icon_img_size);
            this.drum_img[0][2]  = new Bitmap(Properties.Rsc._00hihat_img, this.info.beats_icon_img_size);
            this.drum_img[0][3]  = new Bitmap(Properties.Rsc._01bass_img, this.info.beats_icon_img_size);
            this.drum_img[0][4]  = new Bitmap(Properties.Rsc._01snare_img, this.info.beats_icon_img_size);
            this.drum_img[0][5]  = new Bitmap(Properties.Rsc._01hihat_img, this.info.beats_icon_img_size);
            this.drum_img[0][6]  = new Bitmap(Properties.Rsc._02bass_img, this.info.beats_icon_img_size);
            this.drum_img[0][7]  = new Bitmap(Properties.Rsc._02snare_img, this.info.beats_icon_img_size);
            this.drum_img[0][8]  = new Bitmap(Properties.Rsc._02hihat_img, this.info.beats_icon_img_size);
            this.drum_img[0][9]  = new Bitmap(Properties.Rsc._03bass_img, this.info.beats_icon_img_size);
            this.drum_img[0][10] = new Bitmap(Properties.Rsc._03snare_img, this.info.beats_icon_img_size);
            this.drum_img[0][11] = new Bitmap(Properties.Rsc._03hihat_img, this.info.beats_icon_img_size);
            this.drum_img[1][0]  = new Bitmap(Properties.Rsc._1bass_img, this.info.beats_icon_img_size);
            this.drum_img[1][1]  = new Bitmap(Properties.Rsc._1snare_img, this.info.beats_icon_img_size);
            this.drum_img[1][2]  = new Bitmap(Properties.Rsc._1hihat_img, this.info.beats_icon_img_size);
            this.drum_img[2][0]  = new Bitmap(Properties.Rsc._2bass_img, this.info.beats_icon_img_size);
            this.drum_img[2][1]  = new Bitmap(Properties.Rsc._2snare_img, this.info.beats_icon_img_size);
            this.drum_img[2][2]  = new Bitmap(Properties.Rsc._2hihat_img, this.info.beats_icon_img_size);
            Application.DoEvents();

            // info
            for (int i = 0; i < this.beats_icon_num; ++i)
            {
                if (this.beatstream[i].active)
                {
                    this.beatstream[i].img.Image = this.drum_img[this.temp_style][DrumIndex(this.beatstream[i].key)];
                    Application.DoEvents();
                }
                this.beatstream[i].img.GetCorrectPos(this.table.Location.X, this.table.Location.Y);
            }


            this.drum_btn_img[0][0]  = new Bitmap(Properties.Rsc._00bass_img, this.info.drum_img_size);
            this.drum_btn_img[0][1]  = new Bitmap(Properties.Rsc._00snare_img, this.info.drum_img_size);
            this.drum_btn_img[0][2]  = new Bitmap(Properties.Rsc._00hihat_img, this.info.drum_img_size);
            this.drum_btn_img[0][3]  = new Bitmap(Properties.Rsc._01bass_img, this.info.drum_img_size);
            this.drum_btn_img[0][4]  = new Bitmap(Properties.Rsc._01snare_img, this.info.drum_img_size);
            this.drum_btn_img[0][5]  = new Bitmap(Properties.Rsc._01hihat_img, this.info.drum_img_size);
            this.drum_btn_img[0][6]  = new Bitmap(Properties.Rsc._02bass_img, this.info.drum_img_size);
            this.drum_btn_img[0][7]  = new Bitmap(Properties.Rsc._02snare_img, this.info.drum_img_size);
            this.drum_btn_img[0][8]  = new Bitmap(Properties.Rsc._02hihat_img, this.info.drum_img_size);
            this.drum_btn_img[0][9]  = new Bitmap(Properties.Rsc._03bass_img, this.info.drum_img_size);
            this.drum_btn_img[0][10] = new Bitmap(Properties.Rsc._03snare_img, this.info.drum_img_size);
            this.drum_btn_img[0][11] = new Bitmap(Properties.Rsc._03hihat_img, this.info.drum_img_size);
            this.drum_btn_img[1][0]  = new Bitmap(Properties.Rsc._1bass_img, this.info.drum_img_size);
            this.drum_btn_img[1][1]  = new Bitmap(Properties.Rsc._1snare_img, this.info.drum_img_size);
            this.drum_btn_img[1][2]  = new Bitmap(Properties.Rsc._1hihat_img, this.info.drum_img_size);
            this.drum_btn_img[2][0]  = new Bitmap(Properties.Rsc._2bass_img, this.info.drum_img_size);
            this.drum_btn_img[2][1]  = new Bitmap(Properties.Rsc._2snare_img, this.info.drum_img_size);
            this.drum_btn_img[2][2]  = new Bitmap(Properties.Rsc._2hihat_img, this.info.drum_img_size);
            Application.DoEvents();

            // info
            this.bass_btn.Image = this.drum_btn_img[this.temp_style][DrumIndex(Drum.bass)];
            this.snare_btn.Image = this.drum_btn_img[this.temp_style][DrumIndex(Drum.snare)];
            this.hihat_btn.Image = this.drum_btn_img[this.temp_style][DrumIndex(Drum.hihat)];
            this.stop_btn.Image = new Bitmap(Properties.Rsc.stop_img, this.info.icon_img_size);
            Application.DoEvents();

            this.play_img[0] = new Bitmap(Properties.Rsc.play_img, this.info.icon_img_size);
            this.play_img[1] = new Bitmap(Properties.Rsc.pause_img, this.info.icon_img_size);
            this.play_img[2] = new Bitmap(Properties.Rsc.stop_img, this.info.icon_img_size);
            Application.DoEvents();
        }
        
        private void ResetTag()
        {
            this.Tag = this.Width + "," + this.Height;
            foreach (Control o in this.table.Controls)
            {
                o.Tag = "" + o.Left + "," + o.Top + ',' + o.Width + ',' + o.Height + ',' + o.Name;
            }
            foreach (Control o in this.Controls)
            {
                o.Tag = "" + o.Left + ',' + o.Top + ',' + o.Width + ',' + o.Height + ',' + o.Name;
            }
        }

        private void ClosingSet(object sender, System.ComponentModel.CancelEventArgs e)
        {
            lock (this)
            {
                this.play_timer.Dispose();
                this.message_timer.Dispose();
                this.num_write.WriteLine(this.name_counter.ToString());
                this.num_write.Close();
                this.list_write.Close();
                this.Dispose();
            }
        }

        private void Reset(string mode)
        {

            //beatstream
            for (int i = 0; i < beats_icon_num; ++i)
            {
                this.beatstream[i].active = false;
                this.beatstream[i].key = Drum.none;
                this.beatstream[i].img.Image = null;
                if (i % this.beats_num == 0)
                    this.beatstream[i].img.BackColor = drum_color[8];
                else
                    this.beatstream[i].img.BackColor = drum_color[5];
            }
            this.speed = 80;
            this.temp_beat = beats_icon_num - 1;
            this.temp_edit_beat = 0;
            this.edit_ctrl = true;

            int interval = time_constant / (this.speed * this.beats_num);
            this.timer_ctrl = -1;
            this.play_timer.Change(this.timer_ctrl, interval);

            this.animation_ctrl = false;
            this.ani_fix_counter = 0;
            this.test_loop_counter = 0;
            this.ani.change_index = 0;

            this.temp_drum = Drum.none;
            this.beat_speed_nud.Text = System.Convert.ToString(this.speed);
            if (temp_beat >= 0 && temp_beat < beats_icon_num && temp_edit_beat >= 0 && temp_edit_beat < beats_icon_num)
                if (temp_edit_beat % beats_num == 0)
                    this.beatstream[temp_edit_beat].img.BackColor = this.drum_color[8];
                else
                    this.beatstream[temp_edit_beat].img.BackColor = this.drum_color[5];

            if (mode == sig_mode[0]) // 2/4
            {
                for (int i = 0; i < beats_icon_num; i += this.beats_num)
                {
                    if ((i / this.beats_num) % 2 == 0)
                    {
                        this.beatstream[i].active = true;
                        this.beatstream[i].key = Drum.bass;
                        this.beatstream[i].img.Image = this.drum_img[this.temp_style][DrumIndex(this.beatstream[i].key)];
                        this.beatstream[i].sound_ = drum_sound[(int)this.beatstream[i].key];
                        this.beatstream[i].sound_.Stop();
                    }
                    else
                    {
                        this.beatstream[i].active = true;
                        this.beatstream[i].key = Drum.snare;
                        this.beatstream[i].img.Image = this.drum_img[this.temp_style][DrumIndex(this.beatstream[i].key)];
                        this.beatstream[i].sound_ = drum_sound[(int)this.beatstream[i].key];
                        this.beatstream[i].sound_.Stop();
                    }

                }
            }
            else if (mode == sig_mode[1]) // 3/4
            {
                for (int i = 0; i < beats_icon_num; i += this.beats_num)
                {
                    if ((i / this.beats_num) % 2 == 0)
                    {
                        this.beatstream[i].active = true;
                        this.beatstream[i].key = Drum.bass;
                        this.beatstream[i].img.Image = this.drum_img[this.temp_style][DrumIndex(this.beatstream[i].key)];
                        this.beatstream[i].sound_ = drum_sound[(int)this.beatstream[i].key];
                        this.beatstream[i].sound_.Stop();
                    }
                    else
                    {
                        this.beatstream[i].active = true;
                        this.beatstream[i].key = Drum.snare;
                        this.beatstream[i].img.Image = this.drum_img[this.temp_style][DrumIndex(this.beatstream[i].key)];
                        this.beatstream[i].sound_ = drum_sound[(int)this.beatstream[i].key];
                        this.beatstream[i].sound_.Stop();
                    }

                }
            }
            else if (mode == sig_mode[2]) // 4/4
            {
                for(int i = 0; i < beats_icon_num; i += this.beats_num)
                {
                    if((i / this.beats_num) % 2== 0)
                    {
                        this.beatstream[i].active = true;
                        this.beatstream[i].key = Drum.bass;
                        this.beatstream[i].img.Image = this.drum_img[this.temp_style][DrumIndex(this.beatstream[i].key)];
                        this.beatstream[i].sound_ = drum_sound[(int)this.beatstream[i].key];
                        this.beatstream[i].sound_.Stop();
                    }
                    else
                    {
                        this.beatstream[i].active = true;
                        this.beatstream[i].key = Drum.snare;
                        this.beatstream[i].img.Image = this.drum_img[this.temp_style][DrumIndex(this.beatstream[i].key)];
                        this.beatstream[i].sound_ = drum_sound[(int)this.beatstream[i].key];
                        this.beatstream[i].sound_.Stop();
                    }
                    
                }

            }
            else // 6/8
            {
                for (int i = 0; i < beats_icon_num; i += this.beats_num)
                {
                    if ((i / this.beats_num) % 2 == 0)
                    {
                        this.beatstream[i].active = true;
                        this.beatstream[i].key = Drum.bass;
                        this.beatstream[i].img.Image = this.drum_img[this.temp_style][DrumIndex(this.beatstream[i].key)];
                        this.beatstream[i].sound_ = drum_sound[(int)this.beatstream[i].key];
                        this.beatstream[i].sound_.Stop();
                    }
                    else
                    {
                        this.beatstream[i].active = true;
                        this.beatstream[i].key = Drum.snare;
                        this.beatstream[i].img.Image = this.drum_img[this.temp_style][DrumIndex(this.beatstream[i].key)];
                        this.beatstream[i].sound_ = drum_sound[(int)this.beatstream[i].key];
                        this.beatstream[i].sound_.Stop();
                    }

                }
            }
            
            this.edit_btn.Text = "Done";
            this.play_btn.Image = play_img[0];
            this.time_sig_com.Enabled = true;
            this.drum_set_com.Enabled = true;
            this.style_com.Enabled = true;
            this.bass_btn.Show();
            this.snare_btn.Show();
            this.hihat_btn.Show();
            this.clear_btn.Show();

            this.beatstream[temp_beat].img.BackColor = drum_color[5];
        }

        private void ComClick(object sender, EventArgs e)
        {
            
        }

        private void ResetSound(object sender, EventArgs e)
        {
            MessageBox.Show("Oops");
            NewTimer sound = sender as NewTimer;
            beatstream[sound.index].sound_.Stop();
        }


        private void message_timer_Tick(object sender, EventArgs e)
        {
            if(!this.IsDisposed)
                this.Invoke(new UpdateMessageCallBack(UpdateMessage));
        }

        private void UpdateMessage()
        {
            this.message_index = (this.message_index + 1 == this.message.Length) ? 0 : this.message_index + 1;
            this.message_show.Text = this.message[this.message_index];
        }

        private void play_timer_Tick(object sender)
        {
            if(this.animation_ctrl && (this.test_loop_counter == 0 || this.test_loop_counter == 2) && this.temp_beat == this.beats_icon_num - 1)
            {
                if(this.ani_fix_counter == 0)
                {
                    this.Invoke(new UpdateAnimationCallBack(UpdateAnimation));
                }
                this.ani_fix_counter = (this.ani_fix_counter + 1) % this.beats_num;
            }
            else
            {
                this.Invoke(new UpdateUICallBack(UpdateObject));
            }
        }
        private void UpdateAnimation()
        {
            this.Enabled = false;
            this.ani.Show();
            if (this.ani.change_index == 6)
            {
                this.animation_ctrl = false;
                this.Enabled = true;
            }
            if (this.test_loop_counter == 0)
                this.ani.NextFlip(false);
            if (this.test_loop_counter == 2)
                this.ani.NextFlip(true);
        }

        private void UpdateObject()
        {
            if (this.temp_beat % beats_num == 0)
                this.beatstream[this.temp_beat].img.BackColor = this.drum_color[8];
            else
                this.beatstream[this.temp_beat].img.BackColor = this.drum_color[5];
            this.temp_beat = (this.temp_beat + 1) % this.beats_icon_num;
            if (this.beatstream[this.temp_beat].active)
            {
                this.beatstream[temp_beat].sound_.Position = TimeSpan.Zero;
                this.beatstream[temp_beat].sound_.Play();
            }
            this.beatstream[this.temp_beat].img.BackColor = this.drum_color[4];
            if (this.test_ctrl && this.temp_beat == this.beats_icon_num - 1)
            {
                ++this.test_loop_counter;
                this.animation_ctrl = true;
            }
        }
        


        protected override void OnMouseClick(MouseEventArgs m)
        {
            base.OnMouseClick(m);
            this.MCE.Invoke(m.X , m.Y );
           
        }

        protected override void OnMouseMove(MouseEventArgs m)
        {
            base.OnMouseMove(m);
            this.MME.Invoke(m.X , m.Y );
        }

        public void MouseClickPos(int x,int y)
        {
            //okok
            int beat_index = (x - this.info.beatstream_xpos_revise) / this.info.beats_size.Width;
            if (this.edit_ctrl && beat_index >= 0 && beat_index < this.beats_icon_num)
            {
                this.temp_edit_beat = beat_index;
                beatstream[temp_edit_beat].key = temp_drum;
                beatstream[temp_edit_beat].active = true;
                if (beatstream[temp_edit_beat].key != Drum.none)
                {
                    beatstream[temp_edit_beat].active = true;
                    beatstream[temp_edit_beat].img.Image = new Bitmap(this.drum_img[this.temp_style][DrumIndex(this.beatstream[this.temp_edit_beat].key)],this.info.beats_icon_img_size);
                    beatstream[temp_edit_beat].sound_ = drum_sound[(int)beatstream[temp_edit_beat].key];
                    beatstream[temp_edit_beat].sound_.Stop();
                }
                else
                {
                    beatstream[temp_edit_beat].active = false;
                    beatstream[temp_edit_beat].img.Image = null;
                }

            }
        }

        public void MouseMovePos(int X,int Y)
        {
            //okok
            int beat_index = (X - this.info.beatstream_xpos_revise) / this.info.beats_size.Width;
            if (this.edit_ctrl && beat_index != this.temp_edit_beat && beat_index >= 0 && beat_index < this.beats_icon_num )
            {
                if (this.temp_edit_beat >= 0 && this.temp_edit_beat < this.beats_icon_num)
                {
                    if (temp_edit_beat % beats_num == 0 )
                        this.beatstream[temp_edit_beat].img.BackColor = this.drum_color[8];
                    else 
                        this.beatstream[temp_edit_beat].img.BackColor = this.drum_color[5];

                    this.temp_edit_beat = beat_index;
                    this.beatstream[temp_edit_beat].img.BackColor = this.drum_color[Convert.ToInt32(temp_drum)];
                }
            }
        }

        private void beat_speed_nud_ValueChanged(object sender, EventArgs e)
        {
            this.speed = System.Convert.ToInt32(beat_speed_nud.Value);
            int interval = time_constant / (this.speed * this.beats_num);
            this.timer_ctrl = -1;
            this.play_timer.Change(this.timer_ctrl, interval);

            this.play_btn.Image = play_img[0];
        }

        private void time_sig_com_SelectedItemChanged(object sender, EventArgs e)
        {
            this.speed = System.Convert.ToInt32(beat_speed_nud.Value);
            this.play_btn.Image = play_img[0];
            this.temp_beat = 0;
            this.temp_edit_beat = 0;
            for (int i = 0; i < beats_icon_num; ++i)
            {
                this.beatstream[i].img.Dispose();
                this.beatstream[i].sound_ = null;
            }

            if (time_sig_com.Text == sig_type[0])// 2/4
            {
                this.beats_num = 4;
                this.beats_icon_num = beats_num * 2;
                this.edit_ctrl = false;
                this.temp_drum = Drum.none;
                int interval = time_constant / (this.speed * this.beats_num);
                this.timer_ctrl = -1;
                this.play_timer.Change(this.timer_ctrl, interval);
            }
            else if (time_sig_com.Text == sig_type[1])// 3/4
            {
                this.beats_num = 4;
                this.beats_icon_num = beats_num * 3;
                this.edit_ctrl = false;
                this.temp_drum = Drum.none;
                int interval = time_constant / (this.speed * this.beats_num);
                this.timer_ctrl = -1;
                this.play_timer.Change(this.timer_ctrl, interval);
            }
            else if (time_sig_com.Text == sig_type[2])// 4/4
            {
                this.beats_num = 4;
                this.beats_icon_num = beats_num * 4;
                this.edit_ctrl = false;
                this.temp_drum = Drum.none;
                int interval = time_constant / (this.speed * this.beats_num);
                this.timer_ctrl = -1;
                this.play_timer.Change(this.timer_ctrl, interval);
            }
            else// 6/8
            {
                this.beats_num = 6;
                this.beats_icon_num = beats_num * 4;
                this.edit_ctrl = false;
                this.temp_drum = Drum.none;
                int interval = time_constant / (this.speed * this.beats_num);
                this.timer_ctrl = -1;
                this.play_timer.Change(this.timer_ctrl, interval);
            }
            this.RebuildBeatstream();//malloc empty obj
            this.ResetTableControl();//adjust and sort controls
            this.ResetTag();//assign new tag
            this.Reset(time_sig_com.Text);//reset info to basic beatstream rythm
        }
        

        private void drum_set_com_SelectedItemChanged(object sender, EventArgs e)
        {
            this.speed = System.Convert.ToInt32(beat_speed_nud.Value);
            this.play_btn.Image = this.play_img[0];
            //this.play_timer.Enabled = false;

            int interval = time_constant / (this.speed * this.beats_num);
            this.timer_ctrl = -1;
            this.play_timer.Change(this.timer_ctrl, interval);

            if (this.temp_beat % this.beats_num == 0)
                this.beatstream[this.temp_beat].img.BackColor = drum_color[8];
            else
                this.beatstream[this.temp_beat].img.BackColor = drum_color[5];
            this.temp_beat = beats_icon_num - 1;
            int index;
            for(index = 0; index < this.drum_set.Length; ++index)
            {
                if (this.drum_set_com.Text == this.drum_set[index])//origin
                {
                    this.temp_drum_set = index;
                    this.drum_path[0] = new Uri(Application.StartupPath.Substring(0, Application.StartupPath.Length) + @"\Resources\" + drum_set[index] + @"\bass.wav");
                    this.drum_path[1] = new Uri(Application.StartupPath.Substring(0, Application.StartupPath.Length) + @"\Resources\" + drum_set[index] + @"\snare.wav");
                    this.drum_path[2] = new Uri(Application.StartupPath.Substring(0, Application.StartupPath.Length) + @"\Resources\" + drum_set[index] + @"\hihat.wav");
                    for(int i = 0; i < this.drum_sound.Length; ++i)
                    {
                        this.drum_sound[i].Open(this.drum_path[i]);
                        this.drum_sound[i].Stop();
                        this.drum_sound[i].Volume = this.vol[i];
                    }
                    break;
                }
            }
            for(int i = 0; i < this.beats_icon_num; ++i)
            {
                if (beatstream[i].active)
                {
                    beatstream[i].sound_ = null;
                    beatstream[i].sound_ = drum_sound[Convert.ToInt32(beatstream[i].key)];
                    beatstream[i].sound_.Volume = this.vol[(int)this.beatstream[i].key];
                    beatstream[i].sound_.Stop();
                    this.beatstream[i].img.Image = this.drum_img[this.temp_style][DrumIndex(this.beatstream[i].key)];
                }
            }
            this.bass_btn.Image = this.drum_btn_img[this.temp_style][DrumIndex(Drum.bass)];
            this.snare_btn.Image = this.drum_btn_img[this.temp_style][DrumIndex(Drum.snare)];
            this.hihat_btn.Image = this.drum_btn_img[this.temp_style][DrumIndex(Drum.hihat)];
        }

        private void style_com_SelectedItemChanged(object sender, EventArgs e)
        {
            this.speed = System.Convert.ToInt32(beat_speed_nud.Value);
            this.play_btn.Image = play_img[0];

            int interval = time_constant / (this.speed * this.beats_num);
            this.timer_ctrl = -1;
            this.play_timer.Change(this.timer_ctrl, interval);

            if (this.temp_beat % this.beats_num == 0)
                this.beatstream[this.temp_beat].img.BackColor = drum_color[8];
            else
                this.beatstream[this.temp_beat].img.BackColor = drum_color[5];
            this.temp_beat = beats_icon_num - 1;

            for(int style_index = 0; style_index < this.style.Length; ++style_index)
            {
                if (style_com.Text == this.style[style_index])
                {
                    this.temp_style = style_index;
                    for (int i = 0; i < this.beats_icon_num; ++i)
                    {
                        if (this.beatstream[i].key != Drum.none)
                        {
                            this.beatstream[i].img.Image =
                                this.drum_img[this.temp_style][DrumIndex(this.beatstream[i].key)];
                        }
                    }
                    this.bass_btn.Image = this.drum_btn_img[style_index][DrumIndex(Drum.bass)];
                    this.snare_btn.Image = this.drum_btn_img[style_index][DrumIndex(Drum.snare)];
                    this.hihat_btn.Image = this.drum_btn_img[style_index][DrumIndex(Drum.hihat)];
                    break;
                }
            }
        }
        private void setting_com_SelectedItemChanged(object sender, EventArgs e)
        {
            if (this.setting_com.Text == this.setting_text[0])// close/open message
            {
                if (this.setting_ctrl[0])
                {
                    this.setting_ctrl[0] = false;
                    this.message_show.Hide();
                }
                else
                {
                    this.setting_ctrl[0] = true;
                    this.message_show.Show();
                }
            }
            else if(this.setting_com.Text == this.setting_text[1])// close/open signature
            {

                if (this.setting_ctrl[1])
                {
                    this.setting_ctrl[1] = false;
                    this.signature.Hide();
                }
                else
                {
                    this.setting_ctrl[1] = true;
                    this.signature.Show();
                }
            }
            else// background change
            {

            }
            this.setting_ctrl[this.setting_ctrl.Length - 1] = false;
            this.setting_com.Hide();
        }

        private void setting_btn_Click(object sender, EventArgs e)
        {
            if (this.setting_ctrl[this.setting_ctrl.Length - 1] == false)
            {
                this.setting_ctrl[this.setting_ctrl.Length - 1] = true;
                this.setting_com.Show();
                if (this.timer_ctrl == -1)
                    this.setting_com.Enabled = true;
                else
                    this.setting_com.Enabled = false;
            }
            else
            {
                this.setting_ctrl[this.setting_ctrl.Length - 1] = false;
                this.setting_com.Hide();
                if (this.timer_ctrl == -1)
                    this.setting_com.Enabled = true;
                else
                    this.setting_com.Enabled = false;
            }
        }

        private int DrumIndex(Drum key)
        {
            if(this.temp_style == 0)
                return this.temp_drum_set * 3 + (int)key;
            else
                return (int)key;
        }

        private void edit_btn_Click(object sender, EventArgs e)
        {
            if (edit_btn.Text == "Edit")
            {
                if (temp_edit_beat % beats_num == 0 && temp_edit_beat >= 0 && temp_edit_beat < beats_icon_num)
                    this.beatstream[temp_edit_beat].img.BackColor = this.drum_color[8];
                else if (temp_edit_beat >= 0 && temp_edit_beat < beats_icon_num)
                    this.beatstream[temp_edit_beat].img.BackColor = this.drum_color[5];
                if (this.timer_ctrl == -1)
                {
                    this.time_sig_com.Enabled = true;
                    this.drum_set_com.Enabled = true;
                    this.style_com.Enabled = true;
                }
                this.edit_ctrl = true;
                this.temp_drum = Drum.none;
                this.edit_btn.Text = "Done";
                this.bass_btn.Show();
                this.snare_btn.Show();
                this.hihat_btn.Show();
                this.clear_btn.Show();
            }
            else
            {
                this.edit_ctrl = false;
                this.temp_drum = Drum.none;
                this.edit_btn.Text = "Edit";
                if(this.timer_ctrl == 0)
                {
                    this.time_sig_com.Enabled = false;
                    this.drum_set_com.Enabled = false;
                    this.style_com.Enabled = false;
                }
                this.bass_btn.Hide();
                this.snare_btn.Hide();
                this.hihat_btn.Hide();
                this.clear_btn.Hide();
                if (temp_edit_beat % beats_num == 0 && temp_edit_beat >= 0 && temp_edit_beat < beats_icon_num)
                    this.beatstream[temp_edit_beat].img.BackColor = this.drum_color[8];
                else if (temp_edit_beat >= 0 && temp_edit_beat < beats_icon_num)
                    this.beatstream[temp_edit_beat].img.BackColor = this.drum_color[5];
            }
        }

        private void play_btn_Click(object sender, EventArgs e)
        {
            if (temp_edit_beat % beats_num == 0 && temp_edit_beat >= 0 && temp_edit_beat < beats_icon_num)
                this.beatstream[temp_edit_beat].img.BackColor = this.drum_color[8];
            else if (temp_edit_beat >= 0 && temp_edit_beat < beats_icon_num)
                this.beatstream[temp_edit_beat].img.BackColor = this.drum_color[5];
            if (temp_beat >= 0 && temp_beat < beats_icon_num && temp_edit_beat >= 0 && temp_edit_beat < beats_icon_num)
                if (temp_edit_beat % beats_num == 0)
                    this.beatstream[temp_edit_beat].img.BackColor = this.drum_color[8];
                else
                    this.beatstream[temp_edit_beat].img.BackColor = this.drum_color[5];
            if (this.timer_ctrl == -1)
            {
                if (this.setting_ctrl[this.setting_ctrl.Length - 1])
                    this.setting_com.Enabled = false;
                if (this.test_ctrl)
                {
                    this.animation_ctrl = true;
                    this.ani_fix_counter = 0;
                }
                this.play_btn.Image = play_img[1];
                this.play_btn.ImageAlign = ContentAlignment.MiddleCenter;
                int interval = time_constant / (this.speed * this.beats_num);
                this.timer_ctrl = 0;
                this.play_timer.Change(this.timer_ctrl, interval);
                this.time_sig_com.Enabled = false;
                this.drum_set_com.Enabled = false;
                this.style_com.Enabled = false;
                this.test_com.Enabled = false;
                this.next_btn.Enabled = false;
                this.edit_ctrl = false;
                this.edit_btn.Text = "Edit";
                this.bass_btn.Hide();
                this.snare_btn.Hide();
                this.hihat_btn.Hide();
                this.clear_btn.Hide();
            }
            else
            {
                if (this.setting_ctrl[this.setting_ctrl.Length - 1])
                    this.setting_com.Enabled = true;
                this.play_btn.Image = play_img[0];
                this.play_btn.ImageAlign = ContentAlignment.MiddleCenter;
                int interval = time_constant / (this.speed * this.beats_num);
                this.timer_ctrl = -1;
                this.play_timer.Change(this.timer_ctrl, interval);
                this.time_sig_com.Enabled = true;
                this.drum_set_com.Enabled = true;
                this.style_com.Enabled = true;
                this.test_com.Enabled = true;
                this.next_btn.Enabled = true;
                this.edit_btn.Text = "Done";
                this.edit_ctrl = true;
                this.bass_btn.Show();
                this.snare_btn.Show();
                this.hihat_btn.Show();
                this.clear_btn.Show();
            }
        }

        private void stop_btn_Click(object sender, EventArgs e)
        {
            this.play_btn.Image = play_img[0];
            if (temp_beat >= 0 && temp_beat < beats_icon_num && temp_beat >= 0 && temp_beat < beats_icon_num)
                if (temp_beat % beats_num == 0)
                    this.beatstream[temp_beat].img.BackColor = this.drum_color[8];
                else
                    this.beatstream[temp_beat].img.BackColor = this.drum_color[5];

            if (this.setting_ctrl[this.setting_ctrl.Length - 1])
                this.setting_com.Enabled = true;

            this.temp_beat = this.beats_icon_num - 1;
            int interval = time_constant / (this.speed * this.beats_num);
            this.timer_ctrl = -1;
            this.play_timer.Change(this.timer_ctrl, interval);
            this.animation_ctrl = false;
            this.ani.change_index = 0;
            this.ani_fix_counter = 0;
            this.test_loop_counter = 0;
            this.time_sig_com.Enabled = true;
            this.drum_set_com.Enabled = true;
            this.style_com.Enabled = true;
            this.test_com.Enabled = true;
            this.next_btn.Enabled = true;
            this.edit_ctrl = true;
            this.bass_btn.Show();
            this.snare_btn.Show();
            this.hihat_btn.Show();
            this.clear_btn.Show();
        }

        private void rst_btn_Click(object sender, EventArgs e)
        {
            Reset(time_sig_com.Text);
        }

        private void NameDone(Object sender, EventArgs e)
        {
            if(this.input.name_input.Text != "" && this.input.name_input.Text != "王大吉")
            {
                ++this.name_counter;
                this.list_write.WriteLine(this.input.name_input.Text);
                this.input.Hide();
            }
            else
            {
                MessageBox.Show("｢ " +this.input.name_input.Text + " 」 不可以使用喔!!!!", " 成韻小精靈",MessageBoxButtons.OK);
                this.input.name_input.Text = "";
            }
        }

        private void test_btn_Click(object sender, EventArgs e)
        {

            this.play_btn.Image = play_img[0];
            if (temp_beat >= 0 && temp_beat < beats_icon_num && temp_beat >= 0 && temp_beat < beats_icon_num)
                if (temp_beat % beats_num == 0)
                    this.beatstream[temp_beat].img.BackColor = this.drum_color[8];
                else
                    this.beatstream[temp_beat].img.BackColor = this.drum_color[5];

            this.temp_beat = this.beats_icon_num - 1;
            int interval = time_constant / (this.speed * this.beats_num);
            this.timer_ctrl = -1;
            this.play_timer.Change(this.timer_ctrl, interval);

            if (this.test_btn.Text == "End Test")
            {
                if (this.setting_ctrl[this.setting_ctrl.Length - 1])
                    this.setting_com.Enabled = true;
                this.test_btn.Text = "Test";
                this.edit_btn.Text = "Edit";
                this.animation_ctrl = false;
                this.ani.change_index = 0;
                this.ani_fix_counter = 0;
                this.test_loop_counter = 0;
                this.test_ctrl = false;
                this.test_com.Enabled = false;
                this.next_btn.Enabled = false;
                this.edit_ctrl = false;
                this.test_com.Hide();
                this.next_btn.Hide();
                this.bass_btn.Hide();
                this.snare_btn.Hide();
                this.hihat_btn.Hide();
                this.clear_btn.Hide();
                if(this.input.name_input.Text != "" && this.input.name_input.Text != "王大吉")
                {
                    MessageBox.Show(this.input.name_input.Text + " : \n\n" +
                        " 感謝您  祝您中獎!", "成韻小精靈", MessageBoxButtons.OK);
                }
            }
            else //Test
            {
                if(this.setting_ctrl[this.setting_ctrl.Length - 1])
                    this.setting_com.Enabled = true;
                this.input.name_input.Text = "王大吉";
                this.input.ShowDialog();
                this.test_btn.Text = "End Test";
                this.edit_btn.Text = "Done";
                this.animation_ctrl = false;
                this.ani.change_index = 0;
                this.ani_fix_counter = 0;
                this.test_loop_counter = 0;
                this.test_ctrl = true;
                this.test_com.Enabled = true;
                this.next_btn.Enabled = true; 
                this.edit_ctrl = true;
                this.test_com.Show();
                this.next_btn.Show();
                this.bass_btn.Show();
                this.snare_btn.Show();
                this.hihat_btn.Show();
                this.clear_btn.Show();
                this.temp_style = 2;//唸出來
                this.style_com.Enabled = false;
                this.style_com.Text = this.style[this.temp_style];
                this.style_com.Enabled = true;
                this.TestAssign(this.temp_diff);//easy
            }
        }

        private void next_btn_Click(object sender, EventArgs e)
        {
            if (this.test_ctrl)
            {
                if (this.test_diff_index[this.temp_diff] + 1 < this.test_diff_max[this.temp_diff])
                    this.test_diff_index[this.temp_diff] += 1;
                else
                    this.test_diff_index[this.temp_diff] = 0;
                this.time_sig_com.Enabled = true;
                this.temp_edit_beat = 0;
                this.temp_beat = this.beats_icon_num - 1;
                this.beat_speed_nud.Text = System.Convert.ToString(this.speed);
                this.UpdateQuestion();
            }
        }

        private void test_com_SelectedItemChanged(object sender, EventArgs e)
        {
            if (this.test_ctrl)
            {
                for (int test_i = 0; test_i < this.diff_text.Length; ++test_i)
                {
                    if (this.test_com.Text == this.diff_text[test_i])
                    {
                        this.TestAssign(test_i);
                        break;
                    }
                }
            }
        }

        private void TestAssign(int diff_i)
        {
            this.time_sig_com.Enabled = false;
            int interval;
            switch (diff_i)
            {
                case (int)Difficulty.easy:
                    this.temp_diff = (int)Difficulty.easy;
                    this.time_sig_com.Text = this.sig_type[0];
                    this.beats_num = 4;
                    this.beats_icon_num = beats_num * 2;
                    this.speed = 70;
                    interval = time_constant / (this.speed * this.beats_num);
                    this.timer_ctrl = -1;
                    this.play_timer.Change(this.timer_ctrl, interval);
                    break;
                case (int)Difficulty.normal:
                    this.temp_diff = (int)Difficulty.normal;
                    this.time_sig_com.Text = this.sig_type[2];
                    this.beats_num = 4;
                    this.beats_icon_num = beats_num * 4;
                    this.speed = 70;
                    interval = time_constant / (this.speed * this.beats_num);
                    this.timer_ctrl = -1;
                    this.play_timer.Change(this.timer_ctrl, interval);
                    break;
                case (int)Difficulty.hard:
                    this.temp_diff = (int)Difficulty.hard;
                    this.time_sig_com.Text = this.sig_type[3];
                    this.beats_num = 6;
                    this.beats_icon_num = beats_num * 4;
                    this.speed = 70;
                    interval = time_constant / (this.speed * this.beats_num);
                    this.timer_ctrl = -1;
                    this.play_timer.Change(this.timer_ctrl, interval);
                    break;
            }
            this.time_sig_com.Enabled = true;
            this.temp_edit_beat = 0;
            this.temp_beat = this.beats_icon_num - 1;
            this.beat_speed_nud.Text = System.Convert.ToString(this.speed);
            this.RebuildBeatstream();//to set new beatstreqm
            this.UpdateQuestion();//update to temp question of the difficulties
            this.ResetTableControl();//adjust and sort controls
            this.ResetTag();//assign new tag
            Application.DoEvents();
        }

        private void RebuildBeatstream()
        {
            this.beatstream = new Beaticon[beats_icon_num];
            this.info.ResizeBeatstreamInfo(this.beats_icon_num);

            for (int i = 0; i < this.beats_icon_num; ++i)
            {
                this.beatstream[i] = new Beaticon();
                this.beatstream[i].active = false;
                this.beatstream[i].key = Drum.none;
                this.beatstream[i].img = new NewLabel(this.info.beatstream_pos.X + this.info.beats_size.Width * i, this.info.beatstream_pos.Y);
                this.beatstream[i].img.Parent = this.table;
                this.beatstream[i].img.Size = this.info.beats_size;
                this.beatstream[i].img.Name = "beatstream";
                this.beatstream[i].img.BorderStyle = BorderStyle.Fixed3D;
                this.beatstream[i].img.BackColor = drum_color[5];
                this.beatstream[i].img.GetCorrectPos(this.table.Location.X, this.table.Location.Y);
                this.beatstream[i].img.MME += new MouseMoveEvent(MouseMovePos);
                this.beatstream[i].img.MCE += new MouseClickEvent(MouseClickPos);
                this.beatstream[i].sound_ = new NewMediaPlayer();
            }
        }

        private void UpdateQuestion()
        {
            int temp_diff_index;
            Drum drum;
            for (int i = 0; i < this.beats_icon_num; ++i)
            {
                temp_diff_index = this.test_diff_index[this.temp_diff];
                drum = this.test_stream[this.temp_diff][temp_diff_index][i];
                if (drum != Drum.none)
                {
                    this.beatstream[i].active = true;
                    this.beatstream[i].key = drum;
                    this.beatstream[i].img.Image = this.drum_img[this.temp_style][this.DrumIndex(drum)];//唸出來
                    this.beatstream[i].sound_ = this.drum_sound[(int)drum];
                    this.beatstream[i].sound_.Stop();
                }
                else
                {
                    this.beatstream[i].active = false;
                    this.beatstream[i].key = Drum.none;
                    this.beatstream[i].img.Image = null;
                    this.beatstream[i].sound_ = null;
                }
                if (i % this.beats_num == 0)
                    this.beatstream[i].img.BackColor = this.drum_color[8];
                else
                    this.beatstream[i].img.BackColor = this.drum_color[5];
            }
            this.bass_btn.Image  = this.drum_btn_img[this.temp_style][this.DrumIndex(Drum.bass)];
            this.snare_btn.Image = this.drum_btn_img[this.temp_style][this.DrumIndex(Drum.snare)];
            this.hihat_btn.Image = this.drum_btn_img[this.temp_style][this.DrumIndex(Drum.hihat)];
        }

        private void bass_btn_Click(object sender, EventArgs e)
        {
            temp_drum = Drum.bass;
        }

        private void snare_btn_Click(object sender, EventArgs e)
        {
            temp_drum = Drum.snare;
        }

        private void hihat_btn_Click(object sender, EventArgs e)
        {
            temp_drum = Drum.hihat;
        }

        private void clear_btn_Click(object sender, EventArgs e)
        {
            temp_drum = Drum.none;
        }
        
    }
    class Program
    {

        static void Main(string[] args)
        {
            mainwindow m = new mainwindow();
            m.ShowDialog();
        }
    }
}
