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
    static class MemBox
    {
        private static Emgu.CV.UI.ImageBox streamBox;
        private static Emgu.CV.UI.ImageBox cropBox;
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

        public static void setSymBox(System.Windows.Forms.TextBox sym)
        {
            symBox = sym;
        }

        public static Emgu.CV.UI.ImageBox getStreamBox()
        {
            return streamBox;
        }

        public static Emgu.CV.UI.ImageBox getCropBox()
        {
            return cropBox;
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
