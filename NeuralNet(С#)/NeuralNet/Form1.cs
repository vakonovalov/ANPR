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
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using NeuralNetworkLibrary;

namespace NeuralNet
{
    public partial class Form1 : Form
    {
        NeuralNW network;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Neural network files (*.nw)|*.nw|All files (*.*)|*.*";
        }

        private void createMNISTDataSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DatsSet ds = new DatsSet();
            ds.ShowDialog();
        }

        private void Open_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                String strSrc = openFileDialog1.FileName;
                network.OpenNW(strSrc);
            }
        }

        private void save_Click(object sender, EventArgs e)
        {
            if (network != null)
            {
                folderBrowserDialog1.ShowDialog();
                String strSrc = folderBrowserDialog1.SelectedPath;
                network.SaveNW(strSrc + "\\network.nw");
            }
        }

        private void creaneNeuralNetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NeuralNetCreate f = new NeuralNetCreate();
            f.ShowDialog();
        }
    }
}
