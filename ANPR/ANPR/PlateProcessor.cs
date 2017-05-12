using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;

namespace ANPR
{
    public class PlateProcessor
    {
#if PLATE_PROCESSOR_INTERMEDIATE_OUTPUT
        Image<Bgr, Byte> colorImgUpBotLines;
        Image<Bgr, Byte> colorImgLeftRightLines;
#endif
        public PlateProcessor()
        {
            
        }

        public Image<Bgr, Byte> ProcessPlate(Bitmap image)
        {
            Image<Gray, Byte> plate = NormalizePlate(image);

            return SymbolsSegmentation(plate);
        }

        private double[] Derivative(double[] points)
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

        private List<double> CalculateNumberArea(Image<Gray, Byte> origImg, double step, int angleInterval)
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
            img = origImg.Rotate((double)(angle * step - mid), new Gray(0));

            List<double> res = new List<double>(3);
            res.Add((double)angle * step - mid);
            res.Add(yMaxGradBot[angle]);
            res.Add(yMaxGradUp[angle]);

            return res;
        }

        /*horizontal Deskew image at a given angle*/
        private Image<Gray, Byte> Deskew(Image<Gray, Byte> img, double angle)
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
        private int[] SymbolsPartition(Image<Gray, Byte> img)
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
                while (xr < auxSharpImg.Width - 1 && ders[xr] < 0)
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

        private List<Rectangle> Bwareaopen(Image<Gray, Byte> img)
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

        private int[] CalculateLeftRightBorders(ref Image<Gray, Byte> img, List<Rectangle> rects, int[] x)
        {
            int[] result = new int[2];
            int ind1 = 0, ind2 = 0;
            List<int> ProbBorders = new List<int>(rects.Count * 2 + x.Length);
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
        private Image<Gray, Byte> ConnectedCompsNoiseClearGray(Image<Gray, Byte> img, int area, bool invert, int smooth)
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

        /*Clear small connected component with different colours for classes*/
        private Image<Bgr, Byte> ConnectedCompsNoiseClearColor(Image<Gray, Byte> img, int area, bool invert, int smooth)
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
        private double[] IntensityVerHist(Byte[, ,] pixels, int startV, int endV, int startH, int endH)
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

        private double[] IntensityHorHist(Byte[, ,] pixels, int startV, int endV, int startH, int endH)
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

        private int[] ClarifyUpBottomLines(Bitmap img)
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

        private int[] ClarifyLeftRightLines(Bitmap img)
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

        /*Normalize plate borders (use ConnectedCompsNoiseClearGray and MorphOperation.Close)*/
        private Image<Gray, Byte> NormalizeForPlateBorders(Image<Gray, Byte> img, int areaNormal, int areaInvert, int smooth)
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
        private void AddBoundaryLines(ref Image<Gray, Byte> img, bool direction)
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
        private int CheckOverstepping(int heigth, int var)
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

        private Image<Gray, Byte> NormalizePlate(Bitmap img)
        {
            Image<Gray, Byte> source = new Image<Gray, Byte>(img);
            Image<Gray, Byte> sourceCloneCropped = source.Clone();
            Image<Gray, Byte> auxImg = source.Clone();
            Image<Gray, Byte> auxSharpImg = source.Clone();
            List<double> imgCropData;

            CvInvoke.GaussianBlur(auxImg, auxSharpImg, new Size(0, 0), 9);
            CvInvoke.AddWeighted(auxImg, 100.0, auxSharpImg, -99.0, 0, auxSharpImg);

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

            
#if PLATE_PROCESSOR_INTERMEDIATE_OUTPUT
            ////////////////////////Show Horizontal Lines
            colorImgUpBotLines = ShowHorizontalCroppedLines(auxSharpImg, upBot);
            colorImgUpBotLines._EqualizeHist();
#endif

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
            
#if PLATE_PROCESSOR_INTERMEDIATE_OUTPUT
            ////////////////////////Show Vertical Lines
            colorImgLeftRightLines = ShowVerticalCroppedLines(auxSharpImg, border);
            colorImgLeftRightLines._EqualizeHist();
#endif

            sourceCloneCropped = sourceCloneCropped.Copy(new Rectangle(border[0], 0, border[1] - border[0], sourceCloneCropped.Height));

            return sourceCloneCropped;
        }

        private Image<Bgr, Byte> SymbolsSegmentation(Image<Gray, Byte> cropPlate)
        {
            Image<Gray, Byte> auxImg = cropPlate.Clone();
            Image<Gray, Byte> auxSharpImg = cropPlate.Clone();

            int[] dersX = SymbolsPartition(auxImg);

            CvInvoke.GaussianBlur(auxImg, auxSharpImg, new Size(0, 0), 9);
            CvInvoke.AddWeighted(auxImg, 100.0, auxSharpImg, -99.0, 0, auxSharpImg);

            Image<Bgr, Byte> segmentsImage = new Image<Bgr, Byte>(auxSharpImg.Clone().ToBitmap());

            for (int i = 0; i < dersX.Length; i++)
            {
                LineSegment2D verical = new LineSegment2D(new Point(dersX[i], 0), new Point(dersX[i], segmentsImage.Height - 1));
                segmentsImage.Draw(verical, new Bgr(0, 255, 0), 1);
            }

            return segmentsImage;
        }

        private Image<Gray, Byte>[] SymbolsCutter(Image<Gray, Byte> img, int[] lines)
        {
            Image<Gray, Byte>[] symbols;
            List<int> lns = lines.ToList();
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

            for (int i = 0; i < lns.Count; i++)
            {
                symbols[i + 1] = img.Copy(new Rectangle(lns[i], 0, lns[i + 1], img.Height));   
            }

            symbols[lns.Count] = img.Copy(new Rectangle(lns[lns.Count - 1], 0, img.Width, img.Height));

            return symbols;
        }

        private Image<Bgr, Byte> ShowHorizontalCroppedLines(Image<Gray, Byte> img, int[] upBot)
        {
            Image<Bgr, Byte> colorImg = new Image<Bgr, byte>(img.Clone().ToBitmap());

            LineSegment2D horizontalBot = new LineSegment2D(new Point(0, upBot[0]), new Point(img.Width - 1, upBot[0]));
            colorImg.Draw(horizontalBot, new Bgr(0, 255, 0), 1);
            LineSegment2D horizontalUp = new LineSegment2D(new Point(0, upBot[1]), new Point(img.Width - 1, upBot[1]));
            colorImg.Draw(horizontalUp, new Bgr(0, 255, 0), 1);
            return colorImg;
        }

        private Image<Bgr, Byte> ShowVerticalCroppedLines(Image<Gray, Byte> img, int[] border)
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
    }
}
