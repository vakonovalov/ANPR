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
    class Recognitor
    {
        int ii = 5;
        private Capture capture; //Камера
        String cascadeFileName; //Каскад детектора
        CascadeClassifier cascadeClassifier; //Каскад
        Thread thread;

        public Recognitor()
        {
            cascadeFileName = "cascade.xml";
            cascadeClassifier = new CascadeClassifier(cascadeFileName); //Каскад
        }

        public void Run()
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

                capture.ImageGrabbed += processFrame;
                capture.Start();

                //thread = new Thread(new ParameterizedThreadStart(worker));

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }
        }

        public void worker(dynamic frame)
        {
            frame = (Image<Bgr, Byte>)frame;
            Mat mat = new Mat();
            Image<Bgr, Byte> ROI_frame;
            //capture.Retrieve(mat); //Полученный кадр
            frame = mat.ToImage<Bgr, Byte>();
            //frame = frame.Resize(0.3, Inter.Cubic);
            //Хаар работает с ЧБ изображением
            //Детектируем
            Rectangle[] facesDetected2;
           // if (ii < 10)
          //  {
           //     ii++;
           // }
            //else
           // {
               // ii = 0;
                facesDetected2 = cascadeClassifier.DetectMultiScale(
                                   frame.Convert<Gray, Byte>(), //Исходное изображение
                                   1.1,  //Коэффициент увеличения изображения
                                   5,   //Группировка предварительно обнаруженных событий. Чем их меньше, тем больше ложных тревог
                                   new Size(15, 15), //Минимальный размер
                    //new Size(200, 200));
                                   new Size(600, 600)); //Максимальный размер
                //Выводим всё найденное
                //MSER
                ROI_frame = frame;

                for (int i = 0; i < facesDetected2.Length; i++)
                {

                    ROI_frame = frame.Copy(facesDetected2[i]);
                    frame.Draw(facesDetected2[i], new Bgr(Color.Blue), 2);
                    Image<Bgr, Byte> rotateImg = rotationPlate(ROI_frame);
                    Image<Bgr, Byte> normImg = normalizePlate(rotateImg);

                    //MulticlassSupportVectorMachine machine = MulticlassSupportVectorMachine.Load("MachineForSymbol.machineforsymbol");
                    //int output = machine.Compute(BitmapToDouble(ROI_frame.ToBitmap()).ToArray());

                }

            //}
            //            MulticlassSupportVectorMachine machine = MulticlassSupportVectorMachine.Load("MachineForSymbol");
            //            double[] input = BitmapToDouble(ROI_frame.ToBitmap()).ToArray();
            //            int output = machine.Compute(input);
          //  VideoImage.Image = frame;
         //   if (MemBox.getState() != 1) return;
        }

        //Процедура обработки видео и поика таблички с номером
        public void processFrame(object sender, EventArgs e)
        {
            Emgu.CV.UI.ImageBox VideoImage = MemBox.getStreamBox();
            Emgu.CV.UI.ImageBox crop = MemBox.getCropBox();
            Mat mat = new Mat();
            Image<Bgr, Byte> ROI_frame;
            capture.Retrieve(mat); //Полученный кадр
            Image<Bgr, Byte> frame = mat.ToImage<Bgr, Byte>();

            //if (thread.ThreadState != ThreadState.Running) 
            //{
            //    thread.Start(frame);
           // }

            MemBox.getStreamBox().Image = frame;
        }

        Image<Bgr, byte> rotationPlate(Image<Bgr, byte> image)
        {
            Image<Gray, Byte> sobel_frame;
            Image<Gray, Byte> gray_image = image.Convert<Gray, Byte>();

            gray_image._EqualizeHist();
            sobel_frame = gray_image;


            Matrix<byte> kernelNoizeV = new Matrix<byte>(new Byte[5, 5] {   { 0, 0, 0, 0, 0 }, 
                                                                            { 0, 0, 0, 0, 0 }, 
                                                                            { 0, 1, 1, 1, 0 },
                                                                            { 0, 0, 0, 0, 0 },
                                                                            { 0, 0, 0, 0, 0 }});

            Matrix<byte> kernelNoizeH = new Matrix<byte>(new Byte[5, 5] {   { 0, 0, 0, 0, 0 }, 
                                                                            { 0, 0, 1, 0, 0 }, 
                                                                            { 0, 0, 1, 0, 0 },
                                                                            { 0, 0, 1, 0, 0 },
                                                                            { 0, 0, 0, 0, 0 }});


            Matrix<byte> kernel = new Matrix<byte>(new Byte[7, 7] {   { 0, 0, 0, 0, 0, 0, 0 }, 
                                                                      { 0, 0, 0, 0, 0, 0, 0 }, 
                                                                      { 0, 0, 1, 1, 1, 0, 0 }, 
                                                                      { 0, 0, 1, 1, 1, 0, 0 }, 
                                                                      { 0, 0, 1, 1, 1, 0, 0 }, 
                                                                      { 0, 0, 0, 0, 0, 0, 0 }, 
                                                                      { 0, 0, 0, 0, 0, 0, 0 }});

            sobel_frame = sobel_frame.SmoothGaussian(3);
            //Морфологическое открытие
            sobel_frame = sobel_frame.MorphologyEx(MorphOp.Open, kernel, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());
            sobel_frame = sobel_frame.MorphologyEx(MorphOp.Erode, kernel, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());

            MemBox.getPr1Box().Image = sobel_frame;

            //Оператор Собеля
            sobel_frame = sobel_frame.ThresholdAdaptive(new Gray(255.0), AdaptiveThresholdType.GaussianC, ThresholdType.Binary, 31, new Gray(-10.0));

            sobel_frame = sobel_frame.MorphologyEx(MorphOp.Close, kernel, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());

            MemBox.getPr2Box().Image = sobel_frame;

            LineSegment2D[] lines = null;
            /*ВЫделение горизонтальных прямых (widthLine >= 0/5*imageLine)*/
            lines = sobel_frame.HoughLinesBinary(1, Math.PI / 200, image.Width / 2, image.Width / 2, 5)[0];
            if (lines != null && lines.Length > 0)
            {
                double angle = 0;
                LineSegment2D avr = new LineSegment2D();
                foreach (LineSegment2D seg in lines)
                {
                    avr.P1 = new Point(avr.P1.X + seg.P1.X, avr.P1.Y + seg.P1.Y);
                    avr.P2 = new Point(avr.P2.X + seg.P2.X, avr.P2.Y + seg.P2.Y);
                    //image.Draw(seg, new Bgr(255, 0, 0), 1);
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
               // MemBox.getAngle().Text = Convert.ToString(Math.Round(angle, 3));
                image = image.Rotate(angle, new Bgr(0, 0, 0));
                MemBox.getCropBox().Image = image;
            }
            return image;
        }

        Image<Bgr, byte> normalizePlate(Image<Bgr, byte> image)
        {
            Image<Bgr, Byte> img = image;
            MemBox.getNormBox().Image = img; 

            return  img;
        }


        private static List<double> BitmapToDouble(Bitmap bmp)
        {
            //При обучении нужно, чтобы все изображения были единого размера. База которую мы привели позволяет обучатся на размере 34*60. Тут мы её немножко ужимаем, для скорости работы.
            ResizeNearestNeighbor filter = new ResizeNearestNeighbor(17, 30);
            BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.ReadWrite, bmp.PixelFormat);
            List<double> res = new List<double>();
            int width = bitmapData.Width;
            int height = bitmapData.Height;
            int stride = bitmapData.Stride;
            int offset = stride - width * 3;

            unsafe
            {
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
