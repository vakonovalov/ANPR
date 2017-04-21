using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
using FANNCSharp;
#if FANN_FIXED
using FANNCSharp.Fixed;
using DataType = System.Int32;
#elif FANN_DOUBLE
using FANNCSharp.Double;
using DataType = System.Double;
#else
using FANNCSharp.Float;
using DataType = System.Single;
#endif
using AForge.Imaging;
using AForge.Imaging.Filters;

namespace ANPR
{
    class Recognitor
    {
        private int framePass;
        private Int64 frameCounter;
        private int state;
        private Capture capture;
        private CascadeClassifier cascadeClassifier;
        /*событие о том что получено изображение  + дегелат события*/
        private delegate void workerPointer(Image<Bgr, Byte> data);
        private AutoResetEvent pick;
        private ConcurrentQueue<Image<Bgr, Byte>> queue; 
        private Thread backgroundThread;
        private static int lockFlag = 0;
        private PlateProcessor plateProcessor;

        public Recognitor()
        {
            cascadeClassifier = new CascadeClassifier("./cascade.xml");
            framePass = 1;
            frameCounter = 1;
            queue = new ConcurrentQueue<Image<Bgr, Byte>>();
            backgroundThread = new Thread(BackgroundFrameHandler);
            backgroundThread.IsBackground = true;
            backgroundThread.Priority = ThreadPriority.Lowest;
            pick = new AutoResetEvent(false);
            state = 0;
            plateProcessor = new PlateProcessor();
        }

        public void Run()
        {
            setState(1);
            String videoFile = ChooseFile();
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
                capture.ImageGrabbed += ImageGrabbedEventHandler;
                backgroundThread.Start();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }
        }

        public String ChooseFile()
        {
            const string message = "Choose video file";
            const string caption = "Video file";
            MessageBox.Show(message, caption,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Question);

            OpenFileDialog FBD = new OpenFileDialog();
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                return FBD.FileName;
            }
            return "";
        }

        public void ImageGrabbedEventHandler(object sender, EventArgs e)
        {
            Mat mat = new Mat();
            Image<Bgr, Byte> frame;
            capture.Retrieve(mat);
            frame = mat.ToImage<Bgr, Byte>();
            //ThreadPool.QueueUserWorkItem(new WaitCallback(frameHandler), frame);  
            AddToQueueCurrentFrame(frame);
           // frameWorker(frame);
            //frameCounter++;
           // frame.Dispose();
            MemBox.getDisplayForm().streamBox.Image = frame;
        }

        /*Процедура обработки кадров*/
        public void AddToQueueCurrentFrame(dynamic frame)
        {
            if (frameCounter % framePass == 0)
            {
                if (Interlocked.CompareExchange(ref lockFlag, 1, 0) == 0)
                {
                    queue.Enqueue(frame);
                    pick.Set();
                }
            }
            frameCounter++;
            //MemBox.getDisplayForm().streamBox.Image = frame;
        }
        
        protected void BackgroundFrameHandler()
        {
            Image<Bgr, Byte> currFrame;
            while (state == 1)
            {
                pick.WaitOne();
                currFrame = null;
                while (queue.TryDequeue(out currFrame)) 
                {
                    ProcessFrame(currFrame);
                }
            }
        }

        public void ProcessFrame(Image<Bgr, Byte> currFrame)
        {
            Image<Bgr, Byte> ROI_frame;
            Image<Bgr, Byte> origFrame;
            Rectangle[] platesDetected;

            origFrame = currFrame;
            //origFrame.Save("./frames/" + frameCounter.ToString() + ".bmp");

            platesDetected = cascadeClassifier.DetectMultiScale(
                           origFrame.Convert<Gray, Byte>(), //Исходное изображение
                           1.1,  //Коэффициент увеличения изображения
                           5,   //Группировка предварительно обнаруженных событий. Чем их меньше, тем больше ложных тревог
                           new Size(15, 15), //Минимальный размер
                           new Size(200, 200)); //Максимальный 
        
            ROI_frame = origFrame;
            for (int i = 0; i < platesDetected.Length; i++)
            {
                ROI_frame = origFrame.Copy(platesDetected[i]);
                //origFrame.Draw(facesDetected2[i], new Bgr(Color.Blue), 2);
                Image<Bgr, Byte> rotateImg = plateProcessor.ProcessPlate(ROI_frame.ToBitmap());//rotationPlate(ROI_frame);
                MemBox.getDisplayForm().crop.Image = rotateImg;
                //Image<Bgr, Byte> normImg = normalizePlate(rotateImg);
                //MemBox.getDisplayForm().normBox.Image = normImg;
                //normImg.Save("./plates/" + frameCounter.ToString() + "." + (i+1).ToString() + ".bmp");
                //MulticlassSupportVectorMachine machine = MulticlassSupportVectorMachine.Load("MachineForSymbol.machineforsymbol");
                //int output = machine.Compute(BitmapToDouble(ROI_frame.ToBitmap()).ToArray());

            }
            Interlocked.Decrement(ref lockFlag);
            //            MulticlassSupportVectorMachine machine = MulticlassSupportVectorMachine.Load("MachineForSymbol");
            //            double[] input = BitmapToDouble(ROI_frame.ToBitmap()).ToArray();
            //            int output = machine.Compute(input);
        }

        public void setState(int st)
        {
            state = st;
        }

        public int getState()
        {
            return state;
        }

        public Capture getCapture()
        {
            return this.capture;
        }      
    }
}
