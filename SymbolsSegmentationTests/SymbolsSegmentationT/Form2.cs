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
using Emgu.CV.UI;

namespace SymbolsSegmentationT
{
    public partial class Form2 : Form
    {
        private Form1 main;
        double[] yPoints;
        double[] ders;

        public Form2()
        {
            InitializeComponent();
        }

        public Form2(ref double[] yPts, ref double[] drs)
        {
            yPoints = yPts;
            ders = drs;
            InitializeComponent();
        }

        public void BuildIntensityHist()
        {
            chart1.Series[0].Points.Clear();
            chart2.Series[0].Points.Clear();
            for (int i = 0; i < yPoints.Length; i++)
            {
                chart1.Series[0].Points.AddY(yPoints[i]);
            }

            if (ders == null) return;

            for (int i = 0; i < yPoints.Length; i++)
            {
                chart2.Series[0].Points.AddY(ders[i]);
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            main = this.Owner as Form1;
            BuildIntensityHist();
        }

//        public void MyOldMethodForPlateHorizontalAndVerticalBoundaries()
//        {
//            int imgH = image.Height;
//            int imgW = image.Width;
//            List<double> yPoints = new List<double>(imgH);
//            List<double> xPoints = new List<double>(imgW);
//            List<double> xUpPoints = new List<double>(imgW);
//            List<double> xBotPoints = new List<double>(imgW);
//            List<int> derYMaxMinRange, derXMaxMinRange;
//            int i = 0;
//            List<double> ders;
            
//            double angle1, angle2;
//            double diag;
//            int xl = 0, xr = 0, lmax = 0, rmax = 0, lmin = 0, rmin = 0;
                    
//            Image<Gray, Byte> img = new Image<Gray, Byte>(image);
//            Image<Gray, Byte> grayImg = new Image<Gray, Byte>(image);

//            input.Image = img;
            
//            do
//            {
//                i++;

//                yPoints = IntensityHorHist(img.Data, 0, imgH, 0, imgW);
//                ders = Derivative(yPoints);

//                BuildIntensityHist(yPoints, ders);

//                derYMaxMinRange = DerivativeMaxMinRange(ders, 0, imgH / 2, imgH / 2, imgH);

//                lmax = derYMaxMinRange[2];
//                rmax = derYMaxMinRange[3];
//                lmin = derYMaxMinRange[4];
//                rmin = derYMaxMinRange[5];

//                xPoints = IntensityVerHist(img.Data, lmax, rmin, 0, imgW);
//                ders = Derivative(xPoints);

//                //BuildIntensityHist(xPoints, ders);

//                derXMaxMinRange = DerivativeMaxMinRange(ders, 0, imgW / 2, imgW / 2, imgW);
//                xl = derXMaxMinRange[0];
//                xr = derXMaxMinRange[1];
//                int width = xr - xl;

//                xUpPoints = IntensityVerHist(img.Data, lmax, rmax, 0, imgW);
//                xBotPoints = IntensityVerHist(img.Data, lmin, rmin, 0, imgW);

//                int indUp = xUpPoints.FindIndex(y => y == xUpPoints.Max());
//                int indBot = xBotPoints.FindIndex(y => y == xBotPoints.Max());

//                diag = Math.Sqrt(width * width + (lmax - rmax) * (lmax - rmax));
//                angle1 = (180 / Math.PI) * Math.Acos((width * width + diag * diag - (lmax - rmax) * (lmax - rmax)) / (2.0 * width * diag));// *(indUp > imgW / 2 ? 1 : -1);

//                diag = Math.Sqrt(width * width + (lmin - rmin) * (lmin - rmin));
//                angle2 = (180 / Math.PI) * Math.Acos((width * width + diag * diag - (lmin - rmin) * (lmin - rmin)) / (2.0 * width * diag));// *(indBot > imgW / 2 ? -1 : 1);
                                
//                //int angle = gradsDiffBot.FindIndex(y => y == gradsDiffBot.Max());

//                grayImg = img.Clone();

//                LineSegment2D horizontalBot = new LineSegment2D(new Point(0, lmax), new Point(imgW - 1, lmax));
//                grayImg.Draw(horizontalBot, new Gray(0), 2);

//                LineSegment2D horizontalUp = new LineSegment2D(new Point(0, rmin), new Point(imgW - 1, rmin));
//                grayImg.Draw(horizontalUp, new Gray(0), 2);

//                LineSegment2D vereticalLeft = new LineSegment2D(new Point(xl, 0), new Point(xl, imgH - 1));
//                grayImg.Draw(vereticalLeft, new Gray(0), 2);

//                LineSegment2D vereticalRight = new LineSegment2D(new Point(xr, 0), new Point(xr, imgH - 1));
//                grayImg.Draw(vereticalRight, new Gray(0), 2);

//                input.Image = grayImg;

//                img = img.Rotate((angle1 + angle2) / 2.0, img.GetAverage());
//                //Image<Bgr, Byte> imgs = HorizontalHist(img);
//                //derivativeAllColumns(img);

//                output.Image = img;
////                main.res = img;
//            }
//            //while (Math.Abs((angle1 + angle2) / 2.0) > 1);
//            while (i < 1);
////            main.res = img;
//        }
    }
}
