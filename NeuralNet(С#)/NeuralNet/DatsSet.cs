using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using System.Runtime.InteropServices;


namespace NeuralNet
{
    public partial class DatsSet : Form
    {
        public DatsSet()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
       
        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void DatsSet_Load(object sender, EventArgs e)
        {
            textBox3.Text = "10";
            textBox3.Enabled = false;
            textBox1.Text = @"C:\";
            textBox2.Text = @"C:\";
            imageHeight.Text = "52";
            imageWidth.Text = "40";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            textBox1.Text = folderBrowserDialog1.SelectedPath;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            textBox2.Text = folderBrowserDialog1.SelectedPath;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            String strSrc = textBox2.Text;
            FileInfo[] fInfo = new DirectoryInfo(strSrc).GetFiles("*.bmp");

            if (fInfo.Length == 0)
            {
                MessageBox.Show("Не найдено файлов *.bmp");
                return;
            }

            int width = int.Parse(imageWidth.Text);
            int height = int.Parse(imageHeight.Text);

            IdxFileParser trainParser = new IdxFileParser(textBox1.Text + "\\TraningSetImageFile.idx", 0, width, height);
            IdxFileParser trainLabelParser = new IdxFileParser(textBox1.Text + "\\TraningSetLabelFile.idx");
            IdxFileParser testParser = null;
            IdxFileParser testLabelParser = null;

            double percent = 0;

            if (checkBox1.Checked)
            {
                testParser = new IdxFileParser(textBox1.Text + "\\TestSetImageFile.idx", 0, width, height);
                testLabelParser = new IdxFileParser(textBox1.Text + "\\TestSetLabelFile.idx");                
                percent = double.Parse(textBox3.Text) / 100.0;
                bool check = (percent > 0 && percent <= 0.5);

                if (!check)
                {
                    MessageBox.Show("Incorrect percent value");
                    return;
                }               
            }

            Random r = new Random();
            for (int i = 0; i < fInfo.Length; i++)
            {
                Image<Gray, Byte> img = new Image<Gray, Byte>(fInfo[i].FullName);
                Byte[] data = new Byte[img.Data.Length];
                Byte[] label = new Byte[1];

                System.Buffer.BlockCopy(img.Data, 0, data, 0, img.Data.Length);
                label[0] = Decimal.ToByte(numericUpDown1.Value);
                if (r.NextDouble() < percent)
                {
                    testParser.appendData(data);
                    testLabelParser.appendData(label);
                }
                else
                {
                    trainParser.appendData(data);
                    trainLabelParser.appendData(label);
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                textBox3.Enabled = true;
            else
                textBox3.Enabled = false;
        }

    }
}
