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


namespace NumberPlateDetector
{
    public partial class Form1 : Form
    {
        private Capture capture; //Камера
        String cascadeFileName; //Каскад детектора
        CascadeClassifier cascadeClassifier; //Каскад

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cascadeFileName = "cascade.xml";
            cascadeClassifier = new CascadeClassifier(cascadeFileName); //Каскад
            Run(); //Запускаем камеру
        }
        
        void Run()
        {
            string videoFile = "";
            const string message = "Choose video file";
            const string caption = "Video file";
            MessageBox.Show(message, caption,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Question);

            OpenFileDialog FBD = new OpenFileDialog();
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                videoFile = FBD.FileName;
            }

            try
            {
                if (videoFile != "")
                {
                    capture = new Capture(videoFile);
                }
                else
                {
                    capture = new Capture();
                }
                capture.ImageGrabbed += ProcessFrame;
                capture.Start();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }
        }

        //Процедура обработки видео и поика таблички с номером
        void ProcessFrame(object sender, EventArgs e)
        {
            Mat mat = new Mat();
            capture.Retrieve(mat); //Полученный кадр
            Image<Bgr, Byte> frame = mat.ToImage<Bgr, Byte>();
            //frame = frame.Resize(0.5, Inter.Cubic);
            //Хаар работает с ЧБ изображением
            //Детектируем
            Rectangle[] facesDetected2 = cascadeClassifier.DetectMultiScale(
                    frame.Convert<Gray, Byte>(), //Исходное изображение
                    2,  //Коэффициент увеличения изображения
                    8,   //Группировка предварительно обнаруженных событий. Чем их меньше, тем больше ложных тревог
                    new Size(5, 5), //Минимальный размер совы
                    Size.Empty); //Максимальный размер совы
            //Выводим всё найденное
            foreach (Rectangle f in facesDetected2)
            {
                frame.Draw(f, new Bgr(Color.Blue), 2);
            }

            VideoImage.Image = frame;

        }
    }
}
