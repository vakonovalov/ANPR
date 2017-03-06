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
    enum State 
    {
        TRUE_POSITIVE  = 0,
        TRUE_NEGATIVE  = 1,
        FALSE_POSITIVE = 2,
        FALSE_NEGATIVE = 3
    }
}

namespace QualityControl
{
    public partial class Form1 : Form
    {
        private int isProgress;
        private Int64 frameCounter;
        private Capture videoGrabber;
        private List<string> sample = new List<string>();
        private StreamWriter report;
        private string reportsFolder= @".\reports\";
        private Int64 maxFramesCount; 


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

        private String getReportSource()
        {
            const string message = "Выберите файл отчета";
            const string caption = "Select file";
            MessageBox.Show(message, caption,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Question);

            OpenFileDialog fbd = new OpenFileDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string ext = Path.GetExtension(fbd.FileName);
                if (ext != ".txt") return null;
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
                maxFramesCount = Convert.ToInt64(videoGrabber.GetCaptureProperty(CapProp.FrameCount));
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

        private string getShortName(string s)
        {

            return s.Substring(s.LastIndexOf('\\') + 1, s.Length - s.LastIndexOf('\\') - 1);
        }
        
        private void fingChild(Int64 fNum)
        {
            List<string> plates = sample.FindAll(s => s.Substring(s.LastIndexOf('\\') + 1, s.IndexOf('.') - s.LastIndexOf('\\') - 1) == fNum.ToString());
            ImageList plateImages = new ImageList();
            ImageList imgs = new ImageList();

            imgs.ImageSize = new Size(67, 22);

            platesList.Items.Clear();

            for (int i = 0; i < plates.Count; i++)
            {
                    imgs.Images.Add(Image.FromFile(plates[i]));
                    ListViewItem it = new ListViewItem();
                    it.ImageIndex = i;
                    it.Text = getShortName(plates[i]);
                    platesList.Items.Add(it);
            }

            if (checkBox2.Checked && plates.Count == 0)
            {
                if (fNum >= Convert.ToInt64(textBoxStart.Text) && fNum <= Convert.ToInt64(textBoxEnd.Text))
                {
                    report.WriteLine(fNum.ToString() + " " + ((int)State.FALSE_NEGATIVE).ToString());
                    report.Flush();
                    isProgress = 1;
                    return;
                }
            }
            
            if (plates.Count != 0)
            {
                isProgress = 0;
                Next.Enabled = true;
                panel1.Enabled = true;
            }

            if (plates.Count == 0)
            {
                isProgress = 0;
                Next.Enabled = true;
                panel1.Enabled = true;
            }
            platesList.SmallImageList = imgs;
        }

        private StreamWriter getStream()
        {
            string file = reportsFolder + "report " + (DateTime.Now).ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + ".txt";
            if (!File.Exists(file))
            {
                using (File.Create(file));
                report = new StreamWriter(file);
            }
            else 
            {
                File.Delete(file);
                using (File.Create(file));               
                report = new StreamWriter(file); 
            }
            return report;  
        }

        public void grabedFrameHandler()
        {
            while (isProgress == 1 && frameCounter < maxFramesCount)
            {
                Image<Bgr, Byte> frame;
                frame = videoGrabber.QueryFrame().ToImage<Bgr, Byte>();
                frameCounter = Convert.ToInt64(videoGrabber.GetCaptureProperty(CapProp.PosFrames));
                textBox3.Text = frameCounter.ToString();
                framesStream.Image = frame;
                fingChild(frameCounter);
                framesStream.Refresh();
                Application.DoEvents();
                frame.Dispose();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            frameCounter = 0;
            isProgress = 0;
            platesList.View = View.Details;
            platesList.Columns.Add("Plates", 200);
            initGrabber();
            initSample();
            panel1.Enabled = false;
            //Next.Enabled = false;
            hScrollBar1.Maximum = 108;
            getStream();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (frameCounter != maxFramesCount)
            {
                switch (isProgress)
                {
                    case 0:
                        {
                            isProgress = 1;
                            panel1.Enabled = false;
                            Next.Enabled = false;
                            grabedFrameHandler();
                            break;
                        }
                    case 1:
                        {
                            panel1.Enabled = true;
                            Next.Enabled = true;
                            isProgress = 0;
                            break;
                        }
                    default:
                        break;
                }
            }
        }

        private void Add_Click(object sender, EventArgs e)
        {
            if (textBox1.TextLength > 0)
            {
                if (checkBox1.Checked)
                {
                    report.WriteLine(textBox1.Text + " " + ((int)State.FALSE_POSITIVE).ToString());
                    report.Flush();
                }
                else
                {
                    report.WriteLine(textBox1.Text + " " + ((int)State.TRUE_POSITIVE).ToString());
                    report.Flush();
                }
            }
        }

        private void platesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (platesList.SelectedItems.Count > 0)
            {
                if (platesList.SelectedItems[0]!= null)
                {
                    textBox1.Text = platesList.SelectedItems[0].Text;
                }
                else
                {
                    textBox1.Text = "";
                }
            }
        }

        private void Jump_Click(object sender, EventArgs e)
        {
            if (textBox2.TextLength > 0)
            {
                Int64 cadr = Convert.ToInt64(textBox2.Text);
                videoGrabber.SetCaptureProperty(CapProp.PosFrames, cadr);
            }
            button1_Click(this, null);
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int perc = hScrollBar1.Value;
            videoGrabber.SetCaptureProperty(CapProp.PosFrames, maxFramesCount * (perc / 100.0));
        }

        private List<int> metricsCalculate()
        {
            string file = getReportSource();
            if (file == null) 
                return null;
            List<int> counts = new List<int>(sizeof(State));
            for (int i = 0; i < sizeof(State); ++i)
            {
                counts.Add(0);
            }
                
            string[] data = File.ReadAllLines(file);
            for (int i = 0; i < data.Length; i++)
            {
                State num = (State)int.Parse(data[i].Substring(data[i].IndexOf(' ')));
                switch (num)
                {
                    case State.TRUE_POSITIVE:
                        counts[(int)num] += 1;
                        break;
                    case State.TRUE_NEGATIVE:
                        counts[(int)num] += 1;
                        break;
                    case State.FALSE_POSITIVE:
                        counts[(int)num] += 1;
                        break;
                    case State.FALSE_NEGATIVE:
                        counts[(int)num] += 1;
                        break;
                    default:
                        break;
                }
            }
            return counts;
        }

        private bool checkValues(List <int> metrics)
        {
            if (metrics[(int)State.TRUE_POSITIVE] + metrics[(int)State.FALSE_POSITIVE] == 0)
                return false;
            if (metrics[(int)State.TRUE_POSITIVE] + metrics[(int)State.FALSE_NEGATIVE] == 0)
                return false;
            return true;
        }
          
        private void button2_Click(object sender, EventArgs e)
        {
            List <int> metrics = metricsCalculate();
            double precision, recall, F;

            double TP = metrics[(int)State.TRUE_POSITIVE];
            double TN = metrics[(int)State.TRUE_NEGATIVE];
            double FP = metrics[(int)State.FALSE_POSITIVE];
            double FN = metrics[(int)State.FALSE_NEGATIVE];

            if (!checkValues(metrics)) return;

            precision = TP / (TP + FP);
            recall = TP / (TP + FN);
            if (precision + recall == 0)
                return;
            F = 2 * (precision * recall) / (precision + recall);

            textBox4.Text = precision.ToString();
            textBox5.Text = recall.ToString();
            textBox6.Text = Math.Round(F, 6).ToString();
        }


    }
}
