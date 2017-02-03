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
        private static Form1 displayForm;
        private static int state;

        public static void setState(int st)
        {
            state = st;
        }

        public static int getState()
        {
            return state;
        }

        public static void setDisplayForm(Form1 form)
        {
             displayForm = form;
        }

        public static Form1 getDisplayForm()
        {
            return displayForm;
        }
    }
}
