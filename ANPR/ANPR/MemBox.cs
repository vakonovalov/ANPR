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
using AForge.Imaging;
using AForge.Imaging.Filters;

namespace NumberPlateDetector
{
    public class MemBox
    {
        private static Emgu.CV.UI.ImageBox streamBox;
        //rotation cases
        private static Emgu.CV.UI.ImageBox cropBox;
        private static Emgu.CV.UI.ImageBox pr1Box;
        private static Emgu.CV.UI.ImageBox pr2Box;
        private static System.Windows.Forms.TextBox angleBox;

        //normalization cases
        private static Emgu.CV.UI.ImageBox normBox;
        private static Emgu.CV.UI.ImageBox nr1Box;
        private static Emgu.CV.UI.ImageBox nr2Box;

        //recognize cases
        private static System.Windows.Forms.TextBox symBox;
        private static int state;

        public static void setStreamBox(Emgu.CV.UI.ImageBox str)
        {
            streamBox = str;
        }

        public static void setCropBox(Emgu.CV.UI.ImageBox str)
        {
            cropBox = str;
        }

        public static void setPr1Box(Emgu.CV.UI.ImageBox str)
        {
            pr1Box = str;
        }

        public static void setPr2Box(Emgu.CV.UI.ImageBox str)
        {
            pr2Box = str;
        }

        public static void setAngle(System.Windows.Forms.TextBox str)
        {
            angleBox = str;
        }

        public static void setNormBox(Emgu.CV.UI.ImageBox str)
        {
            normBox = str;
        }

        public static void setNr1Box(Emgu.CV.UI.ImageBox str)
        {
            nr1Box = str;
        }

        public static void setNr2Box(Emgu.CV.UI.ImageBox str)
        {
            nr2Box = str;
        }

        public static void setSymBox(System.Windows.Forms.TextBox sym)
        {
            symBox = sym;
        }

        public static System.Windows.Forms.TextBox  getAngle()
        {
            return angleBox;
        }

        public static Emgu.CV.UI.ImageBox getStreamBox()
        {
            return streamBox;
        }

        public static Emgu.CV.UI.ImageBox getCropBox()
        {
            return cropBox;
        }

        public static Emgu.CV.UI.ImageBox getPr1Box()
        {
            return pr1Box;
        }

        public static Emgu.CV.UI.ImageBox getPr2Box()
        {
            return pr2Box;
        }

        public static Emgu.CV.UI.ImageBox getNormBox()
        {
            return normBox;
        }

        public static Emgu.CV.UI.ImageBox getNr1Box()
        {
            return nr1Box;
        }

        public static Emgu.CV.UI.ImageBox getNr2Box()
        {
            return nr2Box;
        }

        public static void setState(int st)
        {
            state = st;
        }

        public static int getState()
        {
            return state;
        }
    }
}
