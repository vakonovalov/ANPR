using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.Features2D;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using Emgu.CV.UI;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing.Imaging;

namespace QualityControl
{
    public partial class Form1 : Form
    {
        private dynamic box = null;
        private Int64 frameCounter;
        private Capture videoGrabber;
        private List<string> sample = new List<string>();
        //private Image<Bgr, Byte> frame;
        public TaskScheduler taskScheduler;

        object locker = new object();
        int st = 1;

        public Form1()
        {
            InitializeComponent();
        }

        private String getSampleSource()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = Environment.CurrentDirectory;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                return fbd.SelectedPath;
            }
            return "";
        }

        private String getGrabberSource()
        {
            const string message = "Выберите файл с видео";
            const string caption = "Select file";
            MessageBox.Show(message, caption,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Question);

            OpenFileDialog fbd = new OpenFileDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                return fbd.FileName;
            }
            return "";
        }

        public void initGrabber()
        {
            String source = getGrabberSource();
            if (source != "")
            {
                videoGrabber = new Capture(source);
            }
            else
            {
                MessageBox.Show("Error", "Initial capture error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void initSample()
        {
            String source = "D:\\plates";//getSampleSource();
            if (source != "")
            {
                DirectoryInfo dir_from = new DirectoryInfo(source);
                FileInfo[] fi = dir_from.GetFiles();
                for (int l = 0; l < fi.Length; l++)
                    if ((fi[l].Extension == ".jpg") || (fi[l].Extension == ".jpeg") || (fi[l].Extension == ".bmp") || (fi[l].Extension == ".png"))
                    {
                        sample.Add(fi[l].FullName);
                    }
            }
            else
            {
                MessageBox.Show("Error", "Initial sample error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string createBox(string s)
        {
            listBox1.Items.Add(s);
            return "";
        }


        public Image<Bgr, Byte> imgs(Image<Bgr, Byte> s)
        {
            pictureBox1.Image = s.ToBitmap();
            return s;
        }

        public ImageBox addIMB(ImageBox s)
        {
            s.Name = "sda";
            s.Width = 100;
            s.Height = 50;
            s.SizeMode = PictureBoxSizeMode.Normal;
            s.Visible = true;
            s.BackColor = Color.Black;
            panel1.Controls.Add(s);
            return s;
        }



        private void fingChild(Int64 fNum)
        {
            List<string> plates = sample.FindAll(s => s.Substring(s.LastIndexOf('\\') + 1, s.IndexOf('.') - s.LastIndexOf('\\') - 1) == fNum.ToString());
            ImageList plateImages = new ImageList();

            for (int i = 0; i < plates.Count; i++)
            {
                Task<Image<Bgr, Byte>> task = Task.Run(() =>
                {
                    Image<Bgr, Byte> img = new Image<Bgr, byte>(plates[i]);
                    return img;
                }).ContinueWith(t => imgs(t.Result), taskScheduler);

                /*Task<ImageBox> task = Task.Run(() =>
                {
                    ImageBox imb = new ImageBox();
                    Image<Bgr, Byte> img = new Image<Bgr, byte>(plates[i]);
                    imb.Image = img;
                    return imb;
                }).ContinueWith(t => addIMB(t.Result), taskScheduler);*/
                int j = panel1.Controls.Count;
            }
        }

        public void grabedFrameHandler(object sender, EventArgs e)
        {

            Image<Gray, Byte> frame;
            frameCounter++;
            Mat m = new Mat();
            videoGrabber.Retrieve(m);
            frame = m.ToImage<Gray, Byte>();
            framesStream.Image = frame;
            fingChild(frameCounter);
            frame.Dispose();
            /*for (; ; )
            {
                Image<Bgr, Byte> frame;
                frameCounter++;
                frame = videoGrabber.QueryFrame().ToImage<Bgr, Byte>();
                framesStream.Image = frame;
                fingChild(frameCounter);
                framesStream.Refresh();
                Application.DoEvents();
                frame.Dispose();

            }*/

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            frameCounter = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            initGrabber();
            initSample();
            taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            videoGrabber.ImageGrabbed += grabedFrameHandler;
            videoGrabber.Start();

            //grabedFrameHandler();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
            {
                if (st == 1)
                {
                    videoGrabber.Pause();
                    st = 0;
                }
                else if (st == 0)
                {
                    videoGrabber.Start();
                    st = 1;
                }
            }
        }
    }
}
