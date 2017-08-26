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
using Tesseract;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;


namespace SymbolsSegmentationT
{
    public partial class Form1 : Form
    {
        private int regPos = -1;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            symbols.Text = "1.bmp";
        }

        public TesseractEngine TesseractClasifierBuild()
        {
            TesseractEngine engine = new TesseractEngine("./", "eng", EngineMode.Default);
            engine.SetVariable("tessedit_char_whitelist", "ABCKMEPTXY0123456789");
            engine.SetVariable("tessedit_char_blacklisk", "");
            engine.SetVariable("save_blob_choices", "1");
            engine.SetVariable("tessedit_pageseg_mode", "7");
            engine.SetVariable("tessedit_dump_pageseg_images", "1");
            return engine;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap image = new Bitmap(@"D:\test_plates\" + symbols.Text);

            Stopwatch sWatch = new Stopwatch();
            sWatch.Start();

            imageBox1.Image = new Image<Gray, Byte>(image);

            Image<Gray, Byte> plate = NormalizePlate(image);

            imageBox2.Image = plate;

            Image<Gray, Byte>[] symb = SymbolsSegmentation(plate);

            ImageList imgs = new ImageList();
            imgs.ImageSize = new Size(28, 28);

            listView1.Items.Clear();
            listView1.LargeImageList = null;
            listView1.LargeImageList = imgs;
            
            double[] giss = null;

            int pos = 0;
            for (int i = 0; i < symb.Length; i++)
            {
                Image<Gray, Byte> img = symb[i].Clone();
                pos += img.Width;

                //symb[i]._EqualizeHist();
                //CvInvoke.GaussianBlur(img, img, new Size(0, 0), 9);
                //CvInvoke.AddWeighted(symb[i], 100.0, img, -99.0, 0, img);
                //CvInvoke.Threshold(img, img, 0, 255, ThresholdType.Otsu & ThresholdType.Binary);
                //img = NormalizeForPlateBorders(img, 10, 50, 0);
                

                //List<Rectangle> rects = Bwareaopen(img.Clone());
                //img._SmoothGaussian(1);

                //leftRight = ClarifyLeftRightLines(img.ToBitmap());

                //border = CalculateLeftRightBorders(ref img, rects, leftRight);

                //LineSegment2D verical = new LineSegment2D(new Point(leftRight[0], 0), new Point(leftRight[1], img.Height - 1));
                //img.Draw(verical, new Gray(0), 1);

                int cutPos = -1;

                //right
                giss = IntensityVerHist(img.Data, 0, img.Height - 1, img.Width / 2, img.Width - 1);


                for (int j = 0; j < giss.Length; j++)
                {
                    if (giss[j] > 240)
                    {
                        cutPos = j;
                        break;
                    }
                }

                if (cutPos != -1)
                {
                    img = img.Copy(new Rectangle(0, 0, img.Width / 2 + cutPos, img.Height));
                    img = img.Resize(28, 28, Inter.Nearest);
                }

                //left
                giss = IntensityVerHist(img.Data, 0, img.Height - 1, 0, img.Width / 2);
                cutPos = -1;

                for (int j = 0; j < giss.Length; j++)
                {
                    if (giss[j] > 240)
                    {
                        cutPos = j;
                    }
                }

                if (cutPos != -1)
                {
                    img = img.Copy(new Rectangle(cutPos, 0, img.Width - cutPos, img.Height - 1));
                    img = img.Resize(28, 28, Inter.Nearest);
                }

                //down
                giss = IntensityHorHist(img.Data, img.Height / 2, img.Height - 1, 0, img.Width - 1);
                cutPos = -1;

                for (int j = 0; j < giss.Length; j++)
                {
                    if (giss[j] > 240)
                    {
                        cutPos = j;
                        break;
                    }
                }

                if (cutPos != -1)
                {
                    img = img.Copy(new Rectangle(0, 0, img.Width, img.Height / 2 + cutPos));
                    img = img.Resize(28, 28, Inter.Nearest);
                }

                
                //up
                giss = IntensityHorHist(img.Data, 0, img.Height / 2, 0, img.Width - 1);
                cutPos = -1;

                if (pos >= regPos)
                {
                    for (int j = 0; j < giss.Length; j++)
                    {
                        if (giss[j] > 240)
                        {
                            cutPos = j;
                        }
                    }
                }
                else                 
                {
                    for (int j = giss.Length - 1; j > 0; j--)
                    {
                        if (giss[j] > 220)
                        {
                            cutPos = j;
                        }
                    }              
                }

                if (cutPos != -1)
                {
                    img = img.Copy(new Rectangle(0, 0 + cutPos, img.Width - 1, img.Height - cutPos));
                    img = img.Resize(28, 28, Inter.Nearest);
                }  

                img.Save("D:\\rez\\" + i.ToString() + ".bmp");
                imgs.Images.Add(img.ToBitmap());
                ListViewItem item = new ListViewItem();
                item.ImageIndex = i;
                listView1.Items.Add(item);
            }

            //BuildHist(xPoints, null);
            //BuildHist(xPoints, null);
            //BuildHist(xPoints, null);
            //BuildHist(xPoints, null);


            sWatch.Stop();
            string sec1 = ((double)sWatch.ElapsedTicks / (double)Stopwatch.Frequency).ToString();
            listBox1.Items.Add("Full time: " + sec1);
            image.Dispose();
            plate.Dispose();
        }
        
        public void BuildHist(double[] yPts, double[] drs)
        {
            Form2 f = new Form2(ref yPts, ref drs);
            f.Owner = this;
            f.Show();
        }

        public double[] Derivative(double[] points)
        {
            double[] ders = new double[points.Length];
            ders[0] = points[1] - points[0];

            for (int i = 1; i < points.Length - 1; i++)
            {
                ders[i] = (points[i + 1] - points[i - 1]) / 2.0;
            }

            ders[points.Length - 1] = points[points.Length - 1] - points[points.Length - 2];

            return ders;
        }

        public List<double> CalculateNumberArea(Image<Gray, Byte> origImg, double step, int angleInterval)
        {
            int imgH = origImg.Height;
            int imgW = origImg.Width;
            double[] yPoints = null;
            List<double> gradsDiffBot = new List<double>((int)(angleInterval / step) + 1);
            List<double> gradsDiffUp = new List<double>((int)(angleInterval / step) + 1);
            List<int> yMaxGradBot = new List<int>((int)(angleInterval / step) + 1);
            List<int> yMaxGradUp = new List<int>((int)(angleInterval / step) + 1);
            double[] ders = null;
            int i;
            double k;
            int gradMaxY = 0;

            double dif = 0, maxdiff = 0;

            Image<Gray, Byte> img;

            int mid = (angleInterval / 2);

            for (k = -mid; k <= mid; k += step)
            {
                img = origImg.Rotate((double)k, new Gray(0));

                yPoints = IntensityHorHist(img.Data, 0, imgH, 0, imgW);

                ders = Derivative(yPoints);

                maxdiff = 0;
                for (i = imgH / 2; i < imgH; i++)
                {
                    dif = Math.Abs(ders[i]);
                    if (dif > maxdiff)
                    {
                        maxdiff = dif;
                        gradMaxY = i;
                    }
                }
                yMaxGradBot.Add(gradMaxY);
                gradsDiffBot.Add(maxdiff);

                maxdiff = 0;
                for (i = imgH / 2; i > 0; i--)
                {
                    dif = Math.Abs(ders[i]);
                    if (dif > maxdiff)
                    {
                        maxdiff = dif;
                        gradMaxY = i;
                    }
                }
                yMaxGradUp.Add(gradMaxY);
                gradsDiffUp.Add(maxdiff);
            }

            int angle = gradsDiffBot.FindIndex(y => y == gradsDiffBot.Max());
            img = origImg.Rotate((double)(angle*step - mid), new Gray(0));

            List<double> res = new List<double>(3);
            res.Add((double)angle*step - mid);
            res.Add(yMaxGradBot[angle]);
            res.Add(yMaxGradUp[angle]);
            
            if(checkBox1.CheckState == CheckState.Checked)
            {
                BuildHist(yPoints, ders);
            }
            
            return res;
        }
        
        /*horizontal Deskew image at a given angle*/
        public Image<Gray, Byte> Deskew(Image<Gray, Byte> img, double angle)
        {
            int imgH = img.Height;
            int imgW = img.Width;
            double[] xPoints = new double[imgW];
            double maxOrig, maxSkew;

            Image<Gray, Byte> image = img.Clone();

            image.ConvertFrom<Gray, float>(image.Sobel(1, 0, 3));
            image = image.AbsDiff(new Gray(128));
            image = image.Mul(2.0);

            xPoints = IntensityVerHist(image.Data, 0, imgH, 0, imgW);
            maxOrig = xPoints.Max();                                  
            
            
            Image<Gray, Byte> auxImg = img.Clone();

            Size sz = new Size();
            RotationMatrix2D rotate_matrix = RotationMatrix2D.CreateRotationMatrix(new PointF(auxImg.Width / 2, auxImg.Height / 2), angle, auxImg.Size, out sz);           
            Matrix<double> matrix = new Matrix<double>(rotate_matrix.Rows, rotate_matrix.Cols, rotate_matrix.NumberOfChannels);
            rotate_matrix.CopyTo(matrix);

            matrix.Data[0, 0] = 1;
            matrix.Data[1, 1] = 1;
            matrix.Data[0, 1] = Math.Tan(angle);

            image = auxImg.WarpAffine(matrix.Mat, Inter.Cubic, Warp.Default, BorderType.Replicate, new Gray(0));

            image.ConvertFrom<Gray, float>(image.Sobel(1, 0, 3));
            image = image.AbsDiff(new Gray(128));
            image = image.Mul(2.0);

            xPoints = IntensityVerHist(image.Data, 0, imgH, 0, imgW);
            maxSkew = xPoints.Max();

            if (maxOrig >= maxSkew)
            {
                return img;
            }
            else
            {
                return auxImg.WarpAffine(matrix.Mat, Inter.Cubic, Warp.Default, BorderType.Replicate, new Gray(0));
            }
        }

        /*Make symbols partition*/
        public int[] SymbolsPartition(Image<Gray, Byte> img)
        {
            Image<Gray, Byte> auxSharpImg = img.Clone();
            Image<Gray, Byte> auxSharpImg1 = img.Clone();
            
            CvInvoke.GaussianBlur(img, auxSharpImg, new Size(0, 0), 9);
            CvInvoke.AddWeighted(img, 100.0, auxSharpImg, -99.0, 0, auxSharpImg);

            auxSharpImg._SmoothGaussian(1);
            
            CvInvoke.Threshold(auxSharpImg, auxSharpImg, 0, 255, ThresholdType.Otsu & ThresholdType.Binary);

            if (auxSharpImg.Height < 30)
            {
                auxSharpImg._SmoothGaussian(3);
            }
            else
            {
                auxSharpImg = NormalizeForPlateBorders(auxSharpImg, 1, 50, 0);
                auxSharpImg._SmoothGaussian(15);
            }

            double[] xPts = IntensityVerHist(auxSharpImg.Data, 0, auxSharpImg.Height, 0, auxSharpImg.Width);
            double[] ders = Derivative(xPts);

            List<int> dersX = new List<int>();

            double vm, va, vb;
            double cw, cx;
            double aux;
            int xm, xl, xr;

            vm = xPts.Max();
            va = xPts.Average();
            vb = 2 * va - vm;

            cx = 0.7;
            cw = 0.1;

            do
            {
                xm = 0;
                while (xPts[xm] != xPts.Max())
                {
                    xm++;
                }
                                
                xl = xm - 1;
                while (xl > 0 && ders[xl] > 0)
                {
                    xl--;
                }

                xr = xm + 1;
                while (xr < auxSharpImg.Width - 1  && ders[xr] < 0)
                {
                    xr++;
                }

                aux = xPts[xm];
                for (int i = xl + 1; i < xr; i++)
                {
                    xPts[i] = 0;
                }
                if (aux > cw * vm)
                {
                    bool logic = true;
                    foreach (int ind in dersX)
                    {
                        if (xm > 0.75 * auxSharpImg.Width && Math.Abs(xm - ind) < auxSharpImg.Width * 0.061)
                        {
                            logic = false;
                        }

                        if (xm < 0.75 * auxSharpImg.Width && Math.Abs(xm - ind) < auxSharpImg.Width * 0.07)
                        {
                            logic = false;
                        }
                    }

                    if (logic)
                    {
                        dersX.Add(xm);
                    }
                }
            }
            while (xPts.Max() > 0);
            
            return dersX.ToArray();
        }

        public List<Rectangle> Bwareaopen(Image<Gray, Byte> img)
        {
            Image<Bgr, Byte> plate = new Image<Bgr, byte>(img.ToBitmap());
            Mat hr = new Mat();
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            List<Rectangle> rectangles = new List<Rectangle>();

            CvInvoke.FindContours(img, contours, hr, RetrType.Tree, ChainApproxMethod.ChainApproxSimple);

            double area;
            for (int i = 0; i < contours.Size; i++)
            {
                area = CvInvoke.ContourArea(contours[i]);
                if (area > img.Height * img.Width * Resources.areaLimit)
                {
                    MCvScalar color = new MCvScalar(0, 0, 255);
                    Rectangle rect = CvInvoke.BoundingRectangle(contours[i]);
                    rectangles.Add(rect);
                    plate.Draw(rect, new Bgr(0, 0, 255), 2);
                    
               }
            }
            return rectangles;
        }

        public int[] CalculateLeftRightBorders(ref Image<Gray, Byte> img, List<Rectangle> rects, int[] x)
        {
            int[] result = new int[2];
            int ind1 = 0, ind2 = 0;
            List<int> ProbBorders = new List<int>(rects.Count*2 + x.Length);
            int idealWidth;
            int min = img.Width;
            int aux;

            idealWidth = (int)(img.Height * Resources.heightWidthCropRatio);
            
            foreach (Rectangle rec in rects) 
            {
                ProbBorders.Add(rec.Left);
                ProbBorders.Add(rec.Right);
            }

            ProbBorders.Add(x[0]);
            ProbBorders.Add(x[1]);

            for (int i = 0; i < ProbBorders.Count; i++)
            {
                for (int j = 0; j < ProbBorders.Count; j++)
                {
                    aux = Math.Abs(Math.Abs(ProbBorders[i] - ProbBorders[j]) - idealWidth);
                    if (aux < min)
                    {
                        min = aux;
                        ind1 = i;
                        ind2 = j;
                    }
                }
            }

            if (ProbBorders[ind1] > ProbBorders[ind2])
            {
                result[0] = ProbBorders[ind2];
                result[1] = ProbBorders[ind1];               
            }
            else 
            {
                //соотношение ширина/винт 0.05538
                result[0] = ProbBorders[ind1];
                result[1] = ProbBorders[ind2]; 
            }

            result[0] = result[0] + (int)(idealWidth * Resources.screwRaito);
            result[1] = result[1] - (int)(idealWidth * Resources.screwRaito);
            

            //Rectangle rect = rects[i];
            //img.Draw(rect, new Bgr(0, 0, 255), 2);
            return result;
        }
                
        /*Clear small connected component*/
        public Image<Gray, Byte> ConnectedCompsNoiseClearGray(Image<Gray, Byte> img, int area, bool invert, int smooth)
        {
            Mat outputMat = new Mat();
            Mat stats = new Mat();
            Mat centroids = new Mat();
            
            if (smooth > 0)
                img._SmoothGaussian(smooth);

            if (invert)
            {
                img._Not();
            }

            int s = CvInvoke.ConnectedComponentsWithStats(img.Mat, outputMat, stats, centroids, LineType.FourConnected);
            
            int[] statsArr = new int[s*5];
            stats.CopyTo<int>(statsArr);

            List<Color> colors = new List<Color>(s);
            Random r = new Random();
            colors.Add(Color.FromArgb(0, 0, 0));
            for (int i = 1; i < s; i++)
            {
                if (statsArr[(i+1) * 5 - 1] < area)
                {
                    colors.Add(Color.FromArgb(0, 0, 0));
                }
                else
                {
                    colors.Add(Color.FromArgb(255, 255, 255));
                }
            }

            Image<Bgr, Byte> imgss = outputMat.ToImage<Bgr, Byte>();
            int ind;
            
            for (int i = 0; i < imgss.Height; i++)
                for (int j = 0; j < imgss.Width; j++)
                {
                    ind = imgss.Data[i, j, 0];
                    imgss.Data[i, j, 0] = colors[ind].B;
                    imgss.Data[i, j, 1] = colors[ind].G;
                    imgss.Data[i, j, 2] = colors[ind].R;
                }

            if (invert)
                return (imgss.Convert<Gray, byte>()).Not();              
            return imgss.Convert<Gray, byte>();
        }

        public Image<Gray, Byte> ConnectedComponentsDeleteAllExceptMax(Image<Gray, Byte> img)
        {
            Mat outputMat = new Mat();
            Mat stats = new Mat();
            Mat centroids = new Mat();

            img._Not();

            int s = CvInvoke.ConnectedComponentsWithStats(img.Mat, outputMat, stats, centroids, LineType.EightConnected);

            int[] statsArr = new int[s * 5];
            stats.CopyTo<int>(statsArr);

            List<Byte> intensity = new List<Byte>(s);
            intensity.Add(255);
            double max = 0;
            int index = 0;
            for (int i = 1; i < s; i++)
            {
                if (statsArr[(i + 1) * 5 - 1] > max)
                {
                    max = statsArr[(i + 1) * 5 - 1];
                    index = i;
                }
            }

            for (int i = 1; i < s; i++)
            {
                if (i == index)
                {
                    intensity.Add(0);
                }
                else
                {
                    intensity.Add(255);
                }                    
            }

            Image<Gray, Byte> imgss = outputMat.ToImage<Gray, Byte>();
            int cl;

            for (int i = 0; i < imgss.Height; i++)
                for (int j = 0; j < imgss.Width; j++)
                {
                    cl = imgss.Data[i, j, 0];
                    imgss.Data[i, j, 0] = intensity[cl];
                }

            return imgss;
        }
        
        /*Clear small connected component with different colours for classes*/
        public Image<Bgr, Byte> ConnectedCompsNoiseClearColor(Image<Gray, Byte> img, int area, bool invert, int smooth)
        {
            Mat outputMat = new Mat();
            Mat stats = new Mat();
            Mat centroids = new Mat();

            if (smooth > 0)
                img._SmoothGaussian(smooth);

            if (invert)
            {
                img._Not();
            }

            int s = CvInvoke.ConnectedComponentsWithStats(img.Mat, outputMat, stats, centroids, LineType.FourConnected);

            int[] statsArr = new int[s * 5];
            stats.CopyTo<int>(statsArr);

            List<Color> colors = new List<Color>(s);
            Random r = new Random();
            colors.Add(Color.FromArgb(0, 0, 0));
            for (int i = 1; i < s; i++)
            {
                if (statsArr[(i + 1) * 5 - 1] < area)
                {
                    colors.Add(Color.FromArgb(0, 0, 0));
                }
                else
                {
                    colors.Add(Color.FromArgb(50 + r.Next(200), 50 + r.Next(200), 50 + r.Next(200)));
                }
            }

            Image<Bgr, Byte> imgss = outputMat.ToImage<Bgr, Byte>();
            int ind;

            for (int i = 0; i < imgss.Height; i++)
                for (int j = 0; j < imgss.Width; j++)
                {
                    ind = imgss.Data[i, j, 0];
                    imgss.Data[i, j, 0] = colors[ind].B;
                    imgss.Data[i, j, 1] = colors[ind].G;
                    imgss.Data[i, j, 2] = colors[ind].R;
                }

            return imgss;
        }

        /*Return intensity values of*/
        public double[] IntensityVerHist(Byte[, ,] pixels, int startV, int endV, int startH, int endH)
        {
            int i, j, k;
            double mean;
            double[] points = new double[endH - startH];

            for (i = startH, k = 0; i < endH; i++, k++)
            {
                mean = 0;
                for (j = startV; j < endV; j++)
                {
                    mean += pixels[j, i, 0];
                }

                mean /= endV - startV;
                points[k] = mean;
            }
            return points;
        }

        public double[] IntensityHorHist(Byte[, ,] pixels, int startV, int endV, int startH, int endH)
        {
            int i, j, k;
            double mean;
            double[] points = new double[endV - startV];

            for (i = startV, k = 0; i < endV; i++, k++)
            {
                mean = 0;
                for (j = startH; j < endH; j++)
                {
                    mean += pixels[i, j, 0];
                }

                mean /= endH - startH;
                points[k] = mean;
            }
            return points;
        }

        public int[] ClarifyUpBottomLines(Bitmap img)
        {
            int imgH = img.Height;
            int imgW = img.Width;
            double[] yUpPoints = new double[imgH];
            double[] yBotPoints = new double[imgH];
            Image<Gray, Byte> origImage = new Image<Gray, byte>(img);
            Image<Gray, Byte> image = new Image<Gray, byte>(img);

            image.ConvertFrom<Gray, float>(image.Sobel(0, 1, 3));
            image = image.Sub(new Gray(128));
            image = image.Mul(2.0);

            yUpPoints = IntensityHorHist(image.Data, 0, imgH / 2, 0, imgW);

            int indUp = 0;
            while (yUpPoints[indUp] != yUpPoints.Max())
            {
                indUp++;
            }
            
            image = origImage.Clone();
            image.ConvertFrom<Gray, float>(image.Sobel(0, 1, 3));
            image = image.Add(new Gray(128));
            image._Not();
            image = image.Mul(2.0);

            yBotPoints = IntensityHorHist(image.Data, imgH / 2 + 1, imgH, 0, imgW);

            int indBot = 0;
            while (yBotPoints[indBot] != yBotPoints.Max())
            {
                indBot++;
            }
            indBot += imgH / 2;

            int[] res = new int[2];
            res[0] = indUp;
            res[1] = indBot;

            return res;
        }

        public int[] ClarifyLeftRightLines(Bitmap img)
        {
            int imgH = img.Height;
            int imgW = img.Width;
            double[] xLeftPoints = new double[imgW];
            double[] xRightPoints = new double[imgW];
            Image<Gray, Byte> origImage = new Image<Gray, byte>(img);
            Image<Gray, Byte> image = new Image<Gray, byte>(img);

            image.ConvertFrom<Gray, float>(image.Sobel(1, 0, 3));
            image = image.Sub(new Gray(128));
            image = image.Mul(2.0);

            xLeftPoints = IntensityVerHist(image.Data, 0, imgH, 0, imgW / 2);
           
            int indLeft = 0;
            while (xLeftPoints[indLeft] != xLeftPoints.Max())
            {
                indLeft++;
            }

            image = origImage.Clone();
            image.ConvertFrom<Gray, float>(image.Sobel(1, 0, 3));
            image = image.Add(new Gray(128));
            image._Not();
            image = image.Mul(2.0);

            xRightPoints = IntensityVerHist(image.Data, 0, imgH, imgW / 2 + 1, imgW);

            int indRight = 0;
            while (xRightPoints[indRight] != xRightPoints.Max())
            {
                indRight++;
            }

            indRight += imgW / 2;

            int[] res = new int[2];
            res[0] = indLeft;
            res[1] = indRight;
            return res;
        }

        /*Normalize plate borders (use ConnectedCompsNoiseClearGray)*/
        public Image<Gray, Byte> NormalizeForPlateBorders(Image<Gray, Byte> img, int areaNormal, int areaInvert, int smooth)
        {
            Image<Gray, Byte> auxImg = img.Clone();
            Image<Gray, Byte> auxSharpImg = img.Clone();

            CvInvoke.GaussianBlur(auxImg, auxSharpImg, new Size(0, 0), 9);
            CvInvoke.AddWeighted(auxImg, 100.0, auxSharpImg, -99.0, 0, auxSharpImg);

            CvInvoke.Threshold(auxSharpImg, auxSharpImg, 0, 255, ThresholdType.Otsu & ThresholdType.Binary);
            
            auxSharpImg = ConnectedCompsNoiseClearGray(auxSharpImg.Clone(), areaNormal, false, smooth);
            auxSharpImg = ConnectedCompsNoiseClearGray(auxSharpImg.Clone(), areaInvert, true, smooth);
            
            return auxSharpImg;
        }

        /*Draw vertical or horizontal black boundary lines on image (0 - horiz 1 - vert)*/
        public void AddBoundaryLines(ref Image<Gray, Byte> img, bool direction)
        {
            int heigth = img.Height;
            int width = img.Width;

            if (direction)
            {
                Swap(ref heigth, ref width);
            }
            LineSegment2D blackLineUp = new LineSegment2D(new Point(0, 0), new Point(width, 0));
            img.Draw(blackLineUp, new Gray(0), 1);

            LineSegment2D blackLineBot = new LineSegment2D(new Point(0, heigth), new Point(width, heigth));
            img.Draw(blackLineBot, new Gray(0), 1);
        }

        /*Border check*/
        public int CheckOverstepping(int heigth, int var)
        {
            if (var >= heigth)
            {
                return heigth - 1;
            }

            if (var < 0)
            {
                return 0;
            }

            return var;

        }

        public Image<Gray, Byte> NormalizePlate(Bitmap img)
        {
            Image<Gray, Byte> source = new Image<Gray, Byte>(img);
            Image<Gray, Byte> sourceCloneCropped = source.Clone();
            Image<Gray, Byte> auxImg = source.Clone();
            Image<Gray, Byte> auxSharpImg = source.Clone();
            List<double> imgCropData;

            CvInvoke.GaussianBlur(auxImg, auxSharpImg, new Size(0, 0), 9);
            CvInvoke.AddWeighted(auxImg, 100.0, auxSharpImg, -99.0, 0, auxSharpImg);

            Stopwatch sWatchLocal = new Stopwatch();
            sWatchLocal.Start();

            imgCropData = CalculateNumberArea(auxSharpImg, Resources.step, Resources.angleInterval);

            double firstAngle = imgCropData[0];

            sourceCloneCropped = source.Rotate(imgCropData[0], new Gray(0));

            int up = CheckOverstepping(sourceCloneCropped.Height, (int)imgCropData[2] - Resources.cropRate);
            int bot = CheckOverstepping(sourceCloneCropped.Height - up, (int)imgCropData[1] - (int)imgCropData[2] + Resources.cropRate * 2);
            sourceCloneCropped = sourceCloneCropped.Copy(new Rectangle(0, up, source.Width, bot));

            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            //Angle Clarify
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            auxImg = sourceCloneCropped.Clone();
            auxSharpImg = sourceCloneCropped.Clone();

            CvInvoke.GaussianBlur(auxImg, auxSharpImg, new Size(0, 0), 9);
            CvInvoke.AddWeighted(auxImg, 100.0, auxSharpImg, -99.0, 0, auxSharpImg);

            imgCropData = CalculateNumberArea(auxSharpImg, 0.5, 6);

            sourceCloneCropped = sourceCloneCropped.Rotate(imgCropData[0], new Gray(0));

            up = CheckOverstepping(sourceCloneCropped.Height, (int)imgCropData[2] - Resources.cropRate);
            bot = CheckOverstepping(sourceCloneCropped.Height - up, (int)imgCropData[1] - (int)imgCropData[2] + Resources.cropRate * 2);
            sourceCloneCropped = sourceCloneCropped.Copy(new Rectangle(0, up, source.Width, bot));

            ////////////////////////Clarify Up and Bot 
            auxImg = sourceCloneCropped.Clone();
            auxSharpImg = sourceCloneCropped.Clone();

            if (auxSharpImg.Height < 30)
            {
                auxSharpImg = NormalizeForPlateBorders(sourceCloneCropped, 10, 5, 0);
                auxSharpImg._SmoothGaussian(3);
            }
            else
            {
                auxSharpImg = NormalizeForPlateBorders(sourceCloneCropped, 200, 1, 0);
                auxSharpImg._SmoothGaussian(11);
            }

            int[] upBot = ClarifyUpBottomLines(auxSharpImg.ToBitmap());

            ////////////////////////Show Horizontal Lines
            if (checkBox2.CheckState == CheckState.Checked)
            {
                Image<Bgr, Byte> colorImg = ShowHorizontalCroppedLines(auxSharpImg, upBot);
                colorImg._EqualizeHist();
                streamBox.Image = colorImg;
            }
            else
            {
                streamBox.Image = null;
            }

            sourceCloneCropped = sourceCloneCropped.Copy(new Rectangle(0, upBot[0], source.Width, upBot[1] - upBot[0]));
            sourceCloneCropped = Deskew(sourceCloneCropped, (firstAngle / 180.0) * Math.PI);

            ////////////////////////Clarify Left and right 
            auxImg = sourceCloneCropped.Clone();
            auxSharpImg = sourceCloneCropped.Clone();
            CvInvoke.GaussianBlur(auxImg, auxSharpImg, new Size(0, 0), 9);
            CvInvoke.AddWeighted(auxImg, 100.0, auxSharpImg, -99.0, 0, auxSharpImg);

            ////////////////////////Add up/bot black LINES
            AddBoundaryLines(ref auxSharpImg, false);

            List<Rectangle> rects = Bwareaopen(auxSharpImg.Clone());
            auxSharpImg._SmoothGaussian(31);
            int[] leftRight;
            leftRight = ClarifyLeftRightLines(auxSharpImg.ToBitmap());

            int[] border = CalculateLeftRightBorders(ref auxSharpImg, rects, leftRight);

            ////////////////////////Show Vertical Lines
            if (checkBox2.CheckState == CheckState.Checked)
            {
                Image<Bgr, Byte> colorImg = ShowVerticalCroppedLines(auxSharpImg, border);
                colorImg._EqualizeHist();
                imageBox3.Image = colorImg;
            }
            else 
            {
                imageBox3.Image = null;
            }
            
            sourceCloneCropped = sourceCloneCropped.Copy(new Rectangle(border[0], 0, border[1] - border[0], sourceCloneCropped.Height));

            sWatchLocal.Stop();
            string sec = ((double)sWatchLocal.ElapsedTicks / (double)Stopwatch.Frequency).ToString();
            listBox1.Items.Add("Rotate: " + sec);
            return sourceCloneCropped;
        }

        public Image<Gray, Byte>[] SymbolsSegmentation(Image<Gray, Byte> cropPlate)
        {
            Image <Gray, Byte> auxImg = cropPlate.Clone();
            Image <Gray, Byte> auxSharpImg = cropPlate.Clone();

            int[] dersX = SymbolsPartition(auxImg);

            Array.Sort(dersX);

            for (int i = 0; i < dersX.Length; i++ )
            {
                if (dersX[i] > 0.65 * auxSharpImg.Width)
                {
                    regPos = dersX[i];
                    break;
                }
            }
          
            CvInvoke.GaussianBlur(auxImg, auxSharpImg, new Size(0, 0), 9);
            CvInvoke.AddWeighted(auxImg, 100.0, auxSharpImg, -99.0, 0, auxSharpImg);
                       
            Image<Bgr, Byte> segmentsImage = new Image<Bgr, Byte>(auxSharpImg.Clone().ToBitmap());
            
            for (int i = 0; i < dersX.Length; i++)
            {
                LineSegment2D verical = new LineSegment2D(new Point(dersX[i], 0), new Point(dersX[i], segmentsImage.Height - 1));
                segmentsImage.Draw(verical, new Bgr(0, 255, 0), 1);
            }

            LineSegment2D verical1 = new LineSegment2D(new Point(regPos, 0), new Point(regPos, segmentsImage.Height - 1));
            segmentsImage.Draw(verical1, new Bgr(255, 255, 0), 1);

            textBox1.Text = dersX.Length.ToString();

            if (checkBox2.CheckState == CheckState.Checked)
            {
                imageBox4.Image = segmentsImage;
            }
            else
            {
                imageBox4.Image = segmentsImage;
            }
            return SymbolsCutter(cropPlate, dersX);
        }

        private Image<Gray, Byte>[] SymbolsCutter(Image<Gray, Byte> img, int[] lines)
        {
            Image<Gray, Byte>[] symbols;
            List<int> lns = lines.ToList();
            List<Image<Gray, Byte>> symbs;
            lns.Sort();
            if (lns[0] == 0)
            {
                lns.Remove(lns.First());
            }

            if (lns.Last() == img.Width - 1)
            {
                lns.Remove(lns.Last());
            }

            symbols = new Image<Gray, Byte>[lns.Count + 1];
            symbols[0] = img.Copy(new Rectangle(0, 0, lns[0], img.Height));

            for (int i = 0; i < lns.Count - 1; i++)
            {
                if (lns[i] >= regPos)
                {
                    symbols[i + 1] = img.Copy(new Rectangle(lns[i], 0, lns[i + 1] - lns[i], (int)(img.Height * 0.7)));
                }
                else 
                {
                    symbols[i + 1] = img.Copy(new Rectangle(lns[i], 0, lns[i + 1] - lns[i], img.Height));
                }
            }

            symbols[lns.Count] = img.Copy(new Rectangle(lns[lns.Count - 1], 0, img.Width - lns[lns.Count - 1], (int)(img.Height * 0.7)));

            symbs = symbols.ToList();

            for (int i = 0; i < symbols.Length; i++)
            {
                if (symbols[i].Width < (img.Width * Resources.smallNumbersWidthRatio) * 1.1)
                {
                    symbs.RemoveAt(i);
                }
            }

            for (int i = 0; i < symbs.Count; i++)
            {
                Image<Gray, Byte> source = symbs[i].Clone();

                symbs[i]._EqualizeHist();
    
                double thresh = CvInvoke.Threshold(symbs[i], source, 0, 255, ThresholdType.Otsu | ThresholdType.Binary);
                
                double tr1  = CvInvoke.Threshold(symbs[i], symbs[i], thresh, 255, ThresholdType.Trunc);
              
                thresh = CvInvoke.Threshold(symbs[i], source, 0, 255, ThresholdType.Otsu | ThresholdType.Binary);
                CvInvoke.Threshold(symbs[i], symbs[i], thresh, 255, ThresholdType.Otsu | ThresholdType.Binary);

                double area = symbs[i].Width * symbs[i].Height * 0.02;

                symbs[i] = ConnectedCompsNoiseClearGray(symbs[i].Clone(), (int)area, true, 0);

                //symbs[i] = symbs[i].SmoothMedian(3);
                //CvInvoke.MorphologyEx(symbs[i], symbs[i], MorphOp.Erode, Resources.verLineKernel3x3, new Point(-1, -1), 1, BorderType.Replicate, new MCvScalar(-3));


            }
            return symbs.ToArray();
        }

        private Byte[] ImgToByteArray(Image<Gray, Byte> img)
        {
            Byte[] data = new Byte[img.Data.Length];

            System.Buffer.BlockCopy(img.Data, 0, data, 0, img.Data.Length);
            return data;
        }

        private Image<Gray, Byte> ByteArrayToImg(Byte[] data, int width, int height, Image<Gray, Byte> im, int channels = 1)
        {
            Byte[, ,] imgData = new Byte[height, width, channels];

            System.Buffer.BlockCopy(data, 0, imgData, 0, data.Length - 1);

            Image<Gray, Byte> img = new Image<Gray, Byte>(imgData);
            return img;
        }     
                        
        public Image<Bgr, Byte> ShowHorizontalCroppedLines(Image<Gray, Byte> img, int[] upBot)
        {
            Image<Bgr, Byte> colorImg = new Image<Bgr, byte>(img.Clone().ToBitmap());

            LineSegment2D horizontalBot = new LineSegment2D(new Point(0, upBot[0]), new Point(img.Width - 1, upBot[0]));
            colorImg.Draw(horizontalBot, new Bgr(0, 255, 0), 1);
            LineSegment2D horizontalUp = new LineSegment2D(new Point(0, upBot[1]), new Point(img.Width - 1, upBot[1]));
            colorImg.Draw(horizontalUp, new Bgr(0,255, 0), 1);
            return colorImg;
        }

        public Image<Bgr, Byte> ShowVerticalCroppedLines(Image<Gray, Byte> img, int[] border)
        {
            Image<Bgr, Byte> colorImg = new Image<Bgr, byte>(img.Clone().ToBitmap());

            LineSegment2D verticalLeft = new LineSegment2D(new Point(border[0], 0), new Point(border[0], img.Height));
            colorImg.Draw(verticalLeft, new Bgr(0, 255, 0), 1);

            LineSegment2D verticalRight = new LineSegment2D(new Point(border[1], 0), new Point(border[1], img.Height));
            colorImg.Draw(verticalRight, new Bgr(0, 255, 0), 1);

            return colorImg;
        }

        public void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        //NOT USED
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //public Bitmap AlphaTrimmedMeanFilter(Image<Gray, Byte> img)
        //{
        //    int[,] arr = new int[img.Height, img.Width];
        //    for (int i = 0; i < img.Height; i++)
        //    {
        //        for (int j = 0; j < img.Width; j++)
        //        {
        //            arr[i, j] = img.Data[i, j, 0];
        //        }
        //    }

        //    int n = 3;
        //    int d = 0;
        //    int width = img.Width;
        //    int height = img.Height;
        //    int[,] result = new int[height, width];

        //    int mid = (Resources.angleInterval / 2);

        //    for (int i = 0; i < height; i++)
        //    {
        //        for (int j = 0; j < width; j++)
        //        {

        //            List<double> value = new List<double>();
        //            for (int x = -1 * n / 2; x <= n / 2; x++)
        //            {
        //                for (int y = -1 * n / 2; y <= n / 2; y++)
        //                {
        //                    if (i + x >= 0 && i + x < height && j + y >= 0 && j + y < width)
        //                        value.Add(arr[i + x, j + y]);
        //                    else
        //                        value.Add(0);
        //                }
        //            }
        //            value.Sort();
        //            int sum = 0;
        //            for (int k = d / 2; k < n * n - d / 2; k++)
        //                sum += (int)value[k];
        //            result[i, j] = sum / (n * n - d);
        //        }
        //    }

        //    Image<Gray, Byte> imgResult = img.Clone();

        //    for (int i = 0; i < height; i++)
        //    {
        //        for (int j = 0; j < width; j++)
        //        {
        //            imgResult.Data[i, j, 0] = (Byte)(result[i, j]);
        //        }
        //    }
        //    return imgResult.ToBitmap();

        //}

        //public List<int> DerivativeMaxMinRange(List<double> ders, int maxl, int maxr, int minl, int minr)
        //{
        //    int i;
        //    double max = 0, min = 0;
        //    int imax = 0, imin = 0, lmax, rmax, lmin, rmin;

        //    for (i = maxl; i < maxr; i++)
        //    {
        //        if (ders[i] > max)
        //        {
        //            max = ders[i];
        //            imax = i;
        //        }
        //    }

        //    for (i = minl; i < minr; i++)
        //    {
        //        if (ders[i] < min)
        //        {
        //            min = ders[i];
        //            imin = i;
        //        }
        //    }

        //    lmax = imax;
        //    while (ders[lmax] > 0 && lmax > 0)
        //    {
        //        lmax--;
        //    }

        //    rmax = imax;
        //    while (ders[rmax] > 0 && rmax < ders.Count)
        //    {
        //        rmax++;
        //    }

        //    lmin = imin;
        //    while (ders[lmin] < 0 && lmin > 0)
        //    {
        //        lmin--;
        //    }

        //    rmin = imin;
        //    while (ders[rmin] < 0 && rmin < ders.Count - 1)
        //    {
        //        rmin++;
        //    }

        //    List<int> res = new List<int>(6);

        //    res.Add(imax);
        //    res.Add(imin);
        //    res.Add(lmax);
        //    res.Add(rmax);
        //    res.Add(lmin);
        //    res.Add(rmin);

        //    return res;
        //}

        //public void DerivativeAllColumns(Image<Bgr, Byte> image)
        //{
        //    Bitmap img = image.ToBitmap();
        //    int i, j;
        //    int imgH = img.Height;
        //    int imgW = img.Width;
        //    List<List<double>> ders = new List<List<double>>(imgW);

        //    for (i = 0; i < imgW; i++)
        //    {
        //        ders.Add(new List<double>(imgH));
        //    }

        //    for (i = 0; i < imgW; i++)
        //    {
        //        ders[i].Add(img.GetPixel(i, 1).B - img.GetPixel(i, 0).B);

        //        for (j = 1; j < imgH - 1; j++)
        //        {
        //            ders[i].Add((double)(img.GetPixel(i, j + 1).B - img.GetPixel(i, j - 1).B) / 2.0);
        //        }

        //        ders[i].Add(img.GetPixel(i, imgH - 1).B - img.GetPixel(i, imgH - 2).B);
        //    }

        //    Bgr color = new Bgr(Color.Red);

        //    double max = 0, min = 0, res = 0;
        //    int index;

        //    for (i = 0; i < imgW - 1; i++)
        //    {
        //        max = ders[i].Max();
        //        min = ders[i].Min();
        //        if (Math.Abs(max) > Math.Abs(min))
        //        {
        //            res = max;
        //        }
        //        else
        //        {
        //            res = min;
        //        }

        //        index = ders[i].FindIndex(y => y == res);
        //        /*for (j = 0; j < imgH - 1; j++)
        //        {
        //            //chart2.Series[0].Points.AddY(ders[i][j]);
                    
        //        }*/

        //        Cross2DF cr = new Cross2DF(new PointF(i, index), 0, 0);
        //        image.Draw(cr, color, 1);
        //    }
        //}

        //Find Rectangles with symbols inside (FindContours & BoundingRectangle)
        //public Image<Bgr, Byte> FindChars(Image<Gray, Byte> img)
        //{
        //    Image<Bgr, Byte> plate = new Image<Bgr, byte>(img.ToBitmap());
        //    Mat hr = new Mat();
        //    VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();

        //    CvInvoke.FindContours(img, contours, hr, RetrType.Tree, ChainApproxMethod.ChainApproxNone);

        //    for (int i = 0; i < contours.Size; i++)
        //    {
        //        MCvScalar color = new MCvScalar(0, 0, 255);
        //        Rectangle rect = CvInvoke.BoundingRectangle(contours[i]);
        //        if (rect.Height >= plate.Height * 0.45 && rect.Width < plate.Width * 0.19)
        //            plate.Draw(rect, new Bgr(0, 0, 255), 2);
        //    }
        //    return plate;
        //}

        //public Image<Bgr, Byte> ShowPathes(Image<Gray, Byte> img, Image<Bgr, Byte> img1, int[] dersX)
        //{
        //    Image<Bgr, Byte> colorImg;

        //    if (img1 != null)
        //    {
        //        colorImg = new Image<Bgr, byte>(img1.Clone().ToBitmap());
        //    }
        //    else 
        //    {
        //        colorImg = new Image<Bgr, byte>(img.Clone().ToBitmap());
        //    }
        //    List<Way> pathes = new List<Way>();

        //    for (int i = 0; i < dersX.Length; i++)
        //    {
        //          Way path = new Way(img, 1.0, 1.4, dersX[i]);
        //          path.AstarImpl();
        //          pathes.Add(path);
        //    }

        //    for (int i = 0; i < dersX.Length; i++)
        //    {
        //        for (int k = 0; k < pathes[i].points.Count; k++)
        //        {
        //            Point p = pathes[i].points[k];
        //            colorImg.Draw(new Cross2DF(new PointF(p.X, p.Y), 0, 0), new Bgr(0, 0, 255), 1);
        //        }
        //    }

        //    return colorImg;
        //}

        //Clear small connected component
        //public Image<Gray, Byte> ConnectedCompsNoiseClearGray(Image<Gray, Byte> img, int area, bool invert, int smooth)
        //{
        //    Mat outputMat = new Mat();
        //    Mat stats = new Mat();
        //    Mat centroids = new Mat();

        //    if (smooth > 0)
        //        img._SmoothGaussian(smooth);

        //    if (invert)
        //    {
        //        img._Not();
        //    }

        //    int s = CvInvoke.ConnectedComponentsWithStats(img.Mat, outputMat, stats, centroids, LineType.FourConnected);

        //    int[] statsArr = new int[s * 5];
        //    stats.CopyTo<int>(statsArr);

        //    List<Color> colors = new List<Color>(s);
        //    Random r = new Random();
        //    colors.Add(Color.FromArgb(0, 0, 0));
        //    for (int i = 1; i < s; i++)
        //    {
        //        if (statsArr[(i + 1) * 5 - 1] < area)
        //        {
        //            colors.Add(Color.FromArgb(0, 0, 0));
        //        }
        //        else
        //        {
        //            colors.Add(Color.FromArgb(255, 255, 255));
        //        }
        //    }

        //    Image<Bgr, Byte> imgss = outputMat.ToImage<Bgr, Byte>();
        //    int ind;

        //    for (int i = 0; i < imgss.Height; i++)
        //        for (int j = 0; j < imgss.Width; j++)
        //        {
        //            ind = imgss.Data[i, j, 0];
        //            imgss.Data[i, j, 0] = colors[ind].B;
        //            imgss.Data[i, j, 1] = colors[ind].G;
        //            imgss.Data[i, j, 2] = colors[ind].R;
        //        }

        //    if (invert)
        //        return (imgss.Convert<Gray, byte>()).Not();
        //    return imgss.Convert<Gray, byte>();
        //}
    }
}