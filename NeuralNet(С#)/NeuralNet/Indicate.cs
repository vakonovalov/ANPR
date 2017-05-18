using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace NeuralNet
{
    public static class Indicate
    {
        public static PictureBox pic1 = null;
        public static PictureBox pic2 = null;
        public static PictureBox pic3 = null;

        public static void set(PictureBox p, bool mode)
        {
            if (mode)
            {
                p.BackColor = Color.Green;
                Application.DoEvents();
            }
            else 
            {
                p.BackColor = Color.Red;
                Application.DoEvents();
            }
        }
    }
}
