using System;
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
using System.Threading;
using System.Drawing.Imaging;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
//using Accord.Statistics.Kernels;
using AForge.Imaging;
using AForge.Imaging.Filters;


namespace NumberPlateDetector
{
    public partial class Form1 : Form
    {
        Recognitor rec;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MemBox.setStreamBox(VideoImage);
            MemBox.setCropBox(crop);
            MemBox.setPr1Box(pr1);
            MemBox.setPr2Box(pr2);
            textBox1.Text = "dasdasd";
            MemBox.setAngle(textBox1);
            MemBox.setNormBox(normBox);
            MemBox.setNr1Box(nr1Box);
            MemBox.setNr1Box(nr2Box);
            MemBox.setSymBox(symbols);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            rec = new Recognitor();
            MemBox.setState(1);
            rec.Run();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MemBox.setState(0);
        }

        private void label4_Click(object sender, EventArgs e) 
        {
        
        }
    }
}
