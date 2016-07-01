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
using System.IO;
using System.Threading;

namespace SampleGeneratorApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        List<string> St; //Адреса всех файлов
        Image<Bgr, Byte> CurentIm; //Текущее изображение
        volatile int currentimg; //Номер текущего изображения в папке
        string fileadress; //Адрес файла с описанием
        string folder_from;
        string folder_to;
        

        private void Form1_Load(object sender, EventArgs e)
        {
            St = new List<string>();
            currentimg = 0;
            folder_from = null;
            folder_to = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folder_from == null)
            {
                const string message = "Select original image folder!";
                const string caption = "Original image";
                MessageBox.Show(message, caption,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Question);
                return;
            }

            if (folder_to == null)
            {
                const string message = "Select final image folder!";
                const string caption = "Final image";
                MessageBox.Show(message, caption,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Question);
                return;
            }

            button1.Visible = false;
            progressBar1.Visible = true;

            DirectoryInfo dir_from = new DirectoryInfo(folder_from);
            FileInfo[] FI = dir_from.GetFiles();
            for (int l = 0; l < FI.Length; l++) //Запишем все изображения из папки в лист
                if ((FI[l].Extension == ".jpg") || (FI[l].Extension == ".jpeg") || (FI[l].Extension == ".bmp") || (FI[l].Extension == ".png"))
                {
                    St.Add(FI[l].FullName);
                }

            System.IO.DirectoryInfo dir_to = new DirectoryInfo(folder_to);

            foreach (FileInfo file in dir_to.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in dir_to.GetDirectories())
            {
                dir.Delete(true);
            }
            for (int i = 0; i < FI.Length; i++)
            {
                System.IO.File.Copy(St[i], folder_to + "\\" + i + ".bmp");
            }

            string s = folder_to + ".dat";
            if (File.Exists(s))
                File.Delete(s);
            fileadress = s;

            progressBar1.Maximum = St.Count;
            for (currentimg = 0; currentimg < St.Count; currentimg++)
            {
                progressBar1.Value = currentimg;
                progressBar1.Refresh();
                if (File.Exists(St[currentimg]))
                {
                    string path = "";
                    CurentIm = new Image<Bgr, Byte>(St[currentimg]);
                    if (radioButton1.Checked)
                    {
                        path = folder_to.Substring(folder_to.LastIndexOf("\\") + 1, folder_to.Length - folder_to.LastIndexOf("\\") - 1) + "\\" + currentimg +
                            St[currentimg].Substring(St[currentimg].LastIndexOf("."), St[currentimg].Length - St[currentimg].LastIndexOf(".")) + "  1  " + "0 0 " + CurentIm.Width + " " + CurentIm.Height + "\r\n";
                    }
                    if (radioButton2.Checked)
                    {
                        path = folder_to.Substring(folder_to.LastIndexOf("\\") + 1, folder_to.Length - folder_to.LastIndexOf("\\") - 1) + "\\" + currentimg +
                            St[currentimg].Substring(St[currentimg].LastIndexOf("."), St[currentimg].Length - St[currentimg].LastIndexOf(".")) + "\r\n";
                    }
                    File.AppendAllText(fileadress, path);
                }
            }

            progressBar1.Visible = false;
            St.Clear();
            CurentIm = null;
            currentimg = 0;
            fileadress = null;
            folder_from = null;
            folder_to = null;
            button1.Visible = true;
        }

        //Откроем папку с исходными изображениями
        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            FBD.SelectedPath = Environment.CurrentDirectory;
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                folder_from = FBD.SelectedPath;
            }
        }

        //Откроем папку, в которою хотим сохранить изображениями
        private void button3_Click(object sender, EventArgs e)
        {
            const string message = "Caution! All files in selected folder would be deleted";
            const string caption = "Warning";
            MessageBox.Show(message, caption,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Question);

            FolderBrowserDialog FBD = new FolderBrowserDialog();
            FBD.SelectedPath = Environment.CurrentDirectory;
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                folder_to = FBD.SelectedPath;
            }
        }
    }
}
