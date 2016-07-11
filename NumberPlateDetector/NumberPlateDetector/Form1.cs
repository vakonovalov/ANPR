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
using Accord.Statistics.Kernels;
using AForge.Imaging;
using AForge.Imaging.Filters;


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
            Image<Bgr, Byte> ROI_frame;
            capture.Retrieve(mat); //Полученный кадр
            Image<Bgr, Byte> frame = mat.ToImage<Bgr, Byte>();
            //frame = frame.Resize(0.5, Inter.Cubic);
            //Хаар работает с ЧБ изображением
            //Детектируем
            Rectangle[] facesDetected2 = cascadeClassifier.DetectMultiScale(
                    frame.Convert<Gray, Byte>(), //Исходное изображение
                    1.1,  //Коэффициент увеличения изображения
                    5,   //Группировка предварительно обнаруженных событий. Чем их меньше, тем больше ложных тревог
                    new Size(5, 5), //Минимальный размер совы
                    Size.Empty); //Максимальный размер совы
            //Выводим всё найденное
            ROI_frame = frame;
            foreach (Rectangle f in facesDetected2)
            {
                ROI_frame = frame.Copy(f);
                frame.Draw(f, new Bgr(Color.Blue), 2);
            }
            ROI_frame = PlateRotation(ROI_frame);
            VideoImage.Image = ROI_frame;
        }

        Image<Bgr, byte> PlateRotation(Image<Bgr, byte> image)
        {
            Image<Gray, Byte> sobel_frame;
            Image<Gray, Byte> gray_image = image.Convert<Gray, Byte>();

            gray_image._EqualizeHist();
            sobel_frame = gray_image.Sobel(0, 1, 3).AbsDiff(new Gray(0.0)).Convert<Gray, Byte>();//.AbsDiff(new Gray(0.0));
            //VideoImage.Image = sobel_frame;
            //CvInvoke.WaitKey();
           // sobel_frame = gray_image.Sobel(0, 1, 3).Add(gray_image.Sobel(1, 0, 3)).AbsDiff(new Gray(0.0)).Convert<Gray, Byte>();//.ThresholdBinary(new Gray(10.0), new Gray(255.0));
           // sobel_frame = sobel_frame.ThresholdAdaptive(new Gray(255.0), AdaptiveThresholdType.GaussianC, ThresholdType.Binary, 31, new Gray(-50.0));
              sobel_frame = sobel_frame.ThresholdAdaptive(new Gray(255.0), AdaptiveThresholdType.MeanC, ThresholdType.Binary, 31, new Gray(-30.0));

            //VideoImage.Image = sobel_frame;
            //CvInvoke.WaitKey();


            LineSegment2D[] lines = null;
            lines = sobel_frame.HoughLinesBinary(1, Math.PI / 180, image.Width / 5, image.Width / 2, 5)[0];
            if (lines != null && lines.Length > 0)
            {
                double angle = 0;
                LineSegment2D avr = new LineSegment2D();
                foreach (LineSegment2D seg in lines)
                {
                    avr.P1 = new Point(avr.P1.X + seg.P1.X, avr.P1.Y + seg.P1.Y);
                    avr.P2 = new Point(avr.P2.X + seg.P2.X, avr.P2.Y + seg.P2.Y);
                    //image.Draw(seg, new Bgr(255, 0, 0), 1);
                    //VideoImage.Image = image;
                    //CvInvoke.WaitKey();
                }
                avr.P1 = new Point(avr.P1.X / lines.Length, avr.P1.Y / lines.Length);
                avr.P2 = new Point(avr.P2.X / lines.Length, avr.P2.Y / lines.Length);
                LineSegment2D horizontal = new LineSegment2D(avr.P1, new Point(avr.P2.X, avr.P1.Y));

                //image.Draw(new LineSegment2D(avr.P1, new Point(avr.P2.X, avr.P1.Y)), new Bgr(0, 255, 0), 2);
                //image.Draw(avr, new Bgr(0, 255, 0), 2);

                double c = horizontal.P2.X - horizontal.P1.X;
                double a = Math.Abs(horizontal.P2.Y - avr.P2.Y);
                double b = Math.Sqrt(c * c + a * a);
                angle = (a / b * (180 / Math.PI)) * (horizontal.P2.Y > avr.P2.Y ? 1 : -1);
                image = image.Rotate(angle, new Bgr(0, 0, 0));
            }
            return image;
        }

        //void SymbolRecognition()
        //{
        //    //Набор входных изображений, развёрнутых в одномерные массивы
        //    double[][] inputs;
        //    //Набор ответов чем являются входные изображения
        //    int[] outputs;
        //    //"Размазанность" гауссианы при обучении. Чем ниже значение, тем больше "обобщения" делает SVM
        //    double sigma = 12;
        //    //Количество классов при обучении. В номерах 10 цифр, 12 букв, + 1 класс с отрицательной выборкой
        //    int classCount = 23;
        //    MulticlassSupportVectorLearning teacher = null;
        //    //Параметры распознающей машины: длина массива на каждую фотографию, параметр ядра обучения, количество классов
        //    //sigma - единственный настраиваемый параметр обучения. Я ставил где-то 10-20, изменялась точность незначительно.
        //    MulticlassSupportVectorMachine machine = new MulticlassSupportVectorMachine(width * height, new Gaussian(sigma), classCount);
        //    //Инициализация обучения
        //    teacher = new MulticlassSupportVectorLearning(machine, inputs, outputs);
        //    teacher.Algorithm = (svm, classInputs, classOutputs, i, j) => new SequentialMinimalOptimization(svm, classInputs, classOutputs) { CacheSize = 0 };
        //    teacher.Run();
        //    machine.Save("MachineForSymbol");
        //}

        private static List<double> test(string str)
        {
            Bitmap bmp = new Bitmap(str);
            //При обучении нужно, чтобы все изображения были единого размера. База которую мы привели позволяет обучатся на размере 34*60. Тут мы её немножко ужимаем, для скорости работы.
            ResizeNearestNeighbor filter = new ResizeNearestNeighbor(17, 30);
            int count = 0;
            BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            List<double> res = new List<double>();
            int width = bitmapData.Width;
            int height = bitmapData.Height;
            int stride = bitmapData.Stride;
            int offset = stride - width * 3;

            unsafe { 
                byte* ptr = (byte*)bitmapData.Scan0.ToPointer();
                double summ = 0;
                for (int y = 0; y < bitmapData.Height; y++)
                {
                    for (int x = 0; x < bitmapData.Width; x++, ptr += 3)
                    {
                        //Можно загрузить ЧБ изображения, но тут предполагается, что работаем с цветными изображениями
                        res.Add((ptr[0] + ptr[1] + ptr[2]) / (3 * 255.0));
                        summ += (ptr[0] + ptr[1] + ptr[2]);
                    }
                    ptr += offset;
                }
                summ = summ / (3 * 255.0 * bitmapData.Height * bitmapData.Width);
                //Бинаризуем по среднему цвету изображения
                for (int i = 0; i < res.Count; i++)
                {
                    if (res[i] < summ)
                        res[i] = 0;
                    else
                        res[i] = 1;
                }
            }
            bmp.UnlockBits(bitmapData);
            return res;
        }


    }
}
