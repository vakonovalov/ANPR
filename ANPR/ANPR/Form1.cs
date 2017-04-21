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
//using Accord.Statistics.Kernels;
using AForge.Imaging;
using AForge.Imaging.Filters;


namespace ANPR
{
    public partial class Form1 : Form
    {
        Recognitor rec;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            actionLog.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            MemBox.setDisplayForm(this);
        }

        private void captureStateController(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;

            if (rec != null)
            {
                switch (clickedButton.Name)
                {
                    case "pause":
                        {
                            rec.getCapture().Pause();
                            logWriter("Воспроизведение приостановлено");
                        }
                        break;
                    case "play":
                        {
                            if (rec.getState() == 0) 
                            {
                                rec = null;
                                rec = new Recognitor();
                                rec.Run();
                                rec.getCapture().Start();
                            }
                            else 
                            {
                                rec.getCapture().Start();
                            }
                            
                            logWriter("Воспроизведение возобновлено");
                            break;
                        }
                    case "stop":
                        {
                            if (rec.getState() == 0)
                            {
                                break;
                            }
                            else
                            {
                                rec.getCapture().Stop();
                                rec.getCapture().Dispose();
                                rec.setState(0);
                                rec = null;
                                logWriter("Воспроизведение завершено");
                                streamBox.Image = null;
                            }
                            break;
                        }
                    case "sourcePlay":
                        {
                            if (rec.getState() == 0)
                            {
                                rec = new Recognitor();
                                rec.Run();
                            }
                            else 
                            {
                                break;
                            }
                            
                            break;
                        }
                    default:
                        break;
                };
            }
            else if (clickedButton.Name.CompareTo("sourcePlay") == 0) 
            {
                rec = new Recognitor();
                rec.Run();
            }
        }

        private void play_Click(object sender, EventArgs e)
        {
            captureStateController(sender, e);
            MemBox.setState(1);
        }

        private void stop_Click(object sender, EventArgs e)
        {
            captureStateController(sender, e);
            MemBox.setState(0);
        }

        private void pause_Click(object sender, EventArgs e)
        {
            captureStateController(sender, e);
            MemBox.setState(2);
        }

        private void sourcePlay_Click(object sender, EventArgs e)
        {
            captureStateController(sender, e);
            MemBox.setState(3);  
        }

        private void logWriter(String str) 
        {
            actionLog.Items.Add(str);
        }  
    }
}
