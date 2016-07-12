using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SymbolRecognitionTraining
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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
