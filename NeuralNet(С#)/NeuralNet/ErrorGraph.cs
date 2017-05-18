using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeuralNet
{
    public partial class ErrorGraph : Form
    {
        public ErrorGraph(List<double> pts)
        {
            InitializeComponent();
            BuildGraph(pts);
        }

        private void ErrorGraph_Load(object sender, EventArgs e)
        {

        }

        private void BuildGraph(List<double> pts)
        {
            for (int i = 0; i < pts.Count; i++ )
            {
                chart1.Series[0].Points.AddY(pts[i]);
            }  
        }
    }
}
