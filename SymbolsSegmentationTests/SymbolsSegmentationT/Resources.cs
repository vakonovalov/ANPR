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
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;


namespace SymbolsSegmentationT
{
    static class Resources
    {
        static public Matrix<byte> diskKernel3x3 = new Matrix<byte>(new byte[3, 3] {{ 0, 1, 0 }, 
                                                                                    { 1, 0, 1 }, 
                                                                                    { 0, 1, 0 }});

        static public Matrix<byte> horLineKernel3x3 = new Matrix<byte>(new byte[3, 3] {{ 0, 0, 0 }, 
                                                                                       { 1, 1, 1 }, 
                                                                                       { 0, 0, 0 }});

        static public Matrix<byte> verLineKernel3x3 = new Matrix<byte>(new byte[3, 3] {{ 0, 1, 0 }, 
                                                                                       { 0, 1, 0 }, 
                                                                                       { 0, 1, 0 }});

        static public Matrix<byte> verticalKernel5x5 = new Matrix<byte>(new byte[5, 5] {{ 0, 0, 1, 0, 0}, 
                                                                                        { 0, 0, 1, 0, 0}, 
                                                                                        { 0, 0, 1, 0, 0},
                                                                                        { 0, 0, 1, 0, 0}, 
                                                                                        { 0, 0, 1, 0, 0}});

        static public Matrix<byte> verLineKernel7x7 = new Matrix<byte>(new byte[7, 7] { { 0, 0, 0, 1, 0, 0, 0}, 
                                                                                        { 0, 0, 0, 1, 0, 0, 0}, 
                                                                                        { 0, 0, 0, 1, 0, 0, 0},
                                                                                        { 0, 0, 0, 1, 0, 0, 0}, 
                                                                                        { 0, 0, 0, 1, 0, 0, 0},
                                                                                        { 0, 0, 0, 1, 0, 0, 0},
                                                                                        { 0, 0, 0, 1, 0, 0, 0}});


        static public Matrix<byte> horizontalKernel5x5 = new Matrix<byte>(new byte[5, 5] {{ 0, 0, 0, 0, 0}, 
                                                                                          { 0, 0, 0, 0, 0}, 
                                                                                          { 1, 1, 1, 1, 1},
                                                                                          { 0, 0, 0, 0, 0}, 
                                                                                          { 0, 0, 0, 0, 0}});

        //plates ~520x90
        static public double heightWidthFullRatio = 5.47;
        static public double heightWidthCropRatio = 5.2947;
        //public double heightWidthCropRatio = 4.7679;
        static public double screwRaito = 0;
        //characters ~76x50
        static public double bigNumbersHeightRatio = 0.8;
        static public double charactersHeightRatio = 0.61;
        static public double charactersWidthRatio = 0.0994;
        static public double bigNumbersWidthRatio = 0.08615;
        static public double smallNumbersWidthRatio = 0.03678;
        //limit of area rectangles
        static public double areaLimit = 0.082;

        static public int cropRate = 10;
        static public int angleInterval = 40;
        static public double step = 2;
    }
}
