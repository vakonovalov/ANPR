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
using System.Drawing.Imaging;

namespace SampleImageRandomTransformation
{
    public enum Morph
    {
        ERODE,
        DILATE
    }

    public class TransformOperation
    {
        private Image<Gray, Byte> image;
        private Random r;

        public TransformOperation(Image<Gray, Byte> img)
        {
            image = img;
            r = new Random();
        }

        public TransformOperation()
        {
            r = new Random();
        }

        //Scaling an image within (img.Size() - {1~3}) pixel
        public Image<Gray, Byte> Scaling(Image<Gray, Byte> img)
        {
            Image<Gray, Byte> imgCopy = img;  
            int wDelta = r.Next(1, 4);
            int hDelta = r.Next(1, 4);

            imgCopy = imgCopy.Resize(img.Width - wDelta, img.Height - hDelta, Inter.Nearest);

            return imgCopy;
        }

        //Skewing an image to [-15; +15] angle
        public Image<Gray, Byte> Skew(Image<Gray, Byte> img)
        {
            Image<Gray, Byte> imgCopy = img;

            int angle = r.Next(-15, 15);

            imgCopy = imgCopy.Rotate(angle, new Gray(255));
            return imgCopy;
        }

        //Trim border of image and Scaling to initial
        public Image<Gray, Byte> BorderTrim(Image<Gray, Byte> img, int w, int h)
        {
            Image<Gray, Byte> imgCopy = img;
            double crop = r.NextDouble();
            if (crop < 0.5)
            {
                crop = 1;
            }
            else 
            {
                crop = 2;
            }

            imgCopy = img.Copy(new Rectangle(0 + (int)crop, 0 + (int)crop, img.Width - (int)crop, img.Height - (int)crop));
            imgCopy = imgCopy.Resize(w, h, Inter.Nearest);
            return imgCopy;
        }

        //Erode or Dilate at given window of image
        public Image<Gray, Byte> MorphTransform(Image<Gray, Byte> img, Morph op)
        {
            Image<Gray, Byte> imgCopy = img;

            if (op == Morph.DILATE)
            {
                imgCopy._Dilate(1);
            }
            else 
            {
                imgCopy._Erode(1);
            }
    
            return imgCopy;
        }

        /*Make noise at image
         * Rules
         * 1. If current pixel is a white, and he has at least one "black neighbor", 
         * then with 0,2 probabilyty he can turn black too.
         * 2. If current pixel is a black, and he has at least one "black neighbor", 
         * then with 0,1 probabilyty he can turn while.
         */
        public Image<Gray, Byte> RandomNoise(Image<Gray, Byte> img)
        {
            Image<Gray, Byte> imgCopy = img;

            for (int i = 1; i < img.Data.GetLength(0) - 2; i++)
            {
                for (int j = 1; j < img.Data.GetLength(1) - 2; j++)
                {
                    if (imgCopy.Data[i, j, 0] > 125)
                    {
                        if (!IsBlackNeighbor(imgCopy.Data, i, j))
                        {
                            double p = r.NextDouble();
                            if (p <= 0.2)
                            {
                                imgCopy.Data[i, j, 0] = 0;
                            }
                        }
                    }
                    else 
                    {
                        if (!IsBlackNeighbor(imgCopy.Data, i, j))
                        {
                            double p = r.NextDouble();
                            if (p <= 0.1)
                            {
                                imgCopy.Data[i, j, 0] = 255;
                            }
                        }
                    }
                }
            }

                return imgCopy;
        }

        private bool IsBlackNeighbor(Byte[, ,] data, int x, int y, int chanel = 0, int threshold = 125)
        {
            if (data[x + 1, y + 1, chanel] < threshold)
            {
                return false;
            }

            if (data[x - 1, y - 1, chanel] < threshold)
            {
                return false;
            }

            if (data[x - 1, y + 1, chanel] < threshold)
            {
                return false;
            }

            if (data[x + 1, y - 1, chanel] < threshold)
            {
                return false;
            }

            if (data[x, y + 1, chanel] < threshold)
            {
                return false;
            }

            if (data[x, y - 1, chanel] < threshold)
            {
                return false;
            }

            if (data[x + 1, y, chanel] < threshold)
            {
                return false;
            }

            if (data[x - 1, y, chanel] < threshold)
            {
                return false;
            }

            return true;
        }

        public Image<Gray, Byte> Image
        {
            get { return image; }
            set { image = value; }
        }

    }
}
