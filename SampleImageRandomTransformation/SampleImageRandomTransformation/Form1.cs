using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.Features2D;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using Emgu.CV.UI;

namespace SampleImageRandomTransformation
{
    public partial class Form1 : Form
    {
        Random rand;
        TransformOperation tr;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tr = new TransformOperation();
            rand = new Random();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Bmp files (*.bmp)|*.bmp|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                String strSrc = openFileDialog1.FileName;
                pictureBox1.Image = new Bitmap(strSrc);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Choose image");
                return;
            }

            Bitmap image = new Bitmap(pictureBox1.Image);
            Image<Gray, Byte> img = new Image<Gray, Byte>(image);

            img = tr.Scaling(img);

            img = tr.Skew(img);

            img = tr.BorderTrim(img, 40, 52);

            double d = rand.NextDouble();
            if (d <= 0.5)
            {
                img = tr.MorphTransform(img, Morph.DILATE);
            }
            else
            {
                img = tr.MorphTransform(img, Morph.ERODE);
            }

            img = tr.RandomNoise(img);

            pictureBox2.Image = img.ToBitmap();
        }
    }
}
