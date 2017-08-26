using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using NeuralNetworkLibrary;

namespace NeuralNet
{

    enum Lamps
    { 
        init =      1,
        propagate = 2,
        error =     3
    }

    public partial class Form1 : Form
    {
        private String trainFilesDir = System.IO.Directory.GetCurrentDirectory() + @"\train folder";
        private NeuralNW network = null;

        /* 0 - TestSetImageFile
         * 1 - TestSetLabelFile
         * 2 - TraningSetImageFile
         * 3 - TraningSetLabelFile
         */
        private String[] TrainFiles = null;
        private String labels = "0123456789ABCЕНKMPTХУ";
        private double[][] inputTraining = null;
        private double[][] outputTraining = null;
        private double[][] inputTest = null;
        private double[][] outputTest = null;
        private int imgWidth = 0;
        private int imgHeight = 0;
        private int clsCount = 0;
        private bool netReady;
        private int setIterator = 0;
        private bool isRunned;
                
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FormFieldInit();
        }

        private void FormFieldInit()
        {
            openFileDialog1.Filter = "Neural network files (*.nw)|*.nw|All files (*.*)|*.*";
            
            dataGridView1.Rows.Add("", "");
            textBox2.Text = "0,1";
            textBox3.Text = "10";
            textBox4.Text = "10";
            imageWidth.Text = "40";
            imageHeight.Text = "52";
            clsCountBox.Text = "21";
            acc.Text = "0,95";
            Indicate.pic1 = pictureBox1;
            Indicate.pic2 = pictureBox2;
            Indicate.pic3 = pictureBox3;
            if (Directory.Exists(trainFilesDir))
            {
                LoadTrainFiles(trainFilesDir);
            }
            else 
            {
                textBox1.Text = @"C:\";
            }
        }

        private void Led(Lamps lamp, bool mode)
        {
            switch (lamp)
            {
                case Lamps.init:
                    Indicate.set(Indicate.pic1, mode);
                    break;
                case Lamps.propagate:
                    Indicate.set(Indicate.pic2, mode);
                    break;
                case Lamps.error:
                    Indicate.set(Indicate.pic3, mode);
                    break;
                default:
                    break;
            }
            Application.DoEvents();
        }

        private void createMNISTDataSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DatsSet ds = new DatsSet();
            ds.ShowDialog();
        }

        private void Open_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Neural network files (*.nw)|*.nw|All files (*.*)|*.*";
            if (!int.TryParse(clsCountBox.Text, out clsCount))
            {
                MessageBox.Show("Задано некорректное колличество классов");
                return;
            }

            if (!int.TryParse(imageWidth.Text, out imgWidth) || !int.TryParse(imageHeight.Text, out imgHeight))
            {
                MessageBox.Show("Некорректный размер изображения");
                return;
            }
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                String strSrc = openFileDialog1.FileName;
                network = NeuralNW.OpenNW(strSrc);
            }

        }

        private void save_Click(object sender, EventArgs e)
        {
            if (network != null)
            {
                folderBrowserDialog1.ShowDialog();
                String strSrc = folderBrowserDialog1.SelectedPath;
                network.SaveNW(strSrc + "\\network.nw");
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void creaneNeuralNetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //NeuralNetCreate f = new NeuralNetCreate();
            //f.ShowDialog();
            CreateNetwork();
        }

        private void CreateNetwork()
        {
            if (!int.TryParse(clsCountBox.Text, out clsCount))
            {
                MessageBox.Show("Задано некорректное колличество классов");
                return;
            }

            if (!int.TryParse(imageWidth.Text, out imgWidth) || !int.TryParse(imageHeight.Text, out imgHeight))
            {
                MessageBox.Show("Некорректный размер изображения");
                return;
            }

            FullyConnectedLayer full1 = new FullyConnectedLayer(imgWidth * imgHeight, 300);
            FullyConnectedLayer full2 = new FullyConnectedLayer(300, 100);
            SoftMaxLayer soft3 = new SoftMaxLayer(100, clsCount);

            List<AbstractLayerNW> layers = new List<AbstractLayerNW>(3){full1, full2, soft3};

            network = new NeuralNW(imgWidth * imgHeight, layers);
        }

        private bool LoadTrainFiles(String directory)
        {
            if (directory == "" || directory == null)
            {
                return false;
            }

            FileInfo[] fInfo = new DirectoryInfo(directory).GetFiles("*.idx");

            if (fInfo.Length != 4)
            {
                MessageBox.Show("Не найдены файлы .idx (необходимо 4 файла)");
                return false;
            }

            textBox1.Text = folderBrowserDialog1.SelectedPath;

            TrainFiles = new String[4];
            TrainFiles[0] = fInfo[0].FullName;
            TrainFiles[1] = fInfo[1].FullName;
            TrainFiles[2] = fInfo[2].FullName;
            TrainFiles[3] = fInfo[3].FullName;
            textBox1.Text = directory;
            return true;
        }

        private void button1_Click(object sender, EventArgs e) 
        {
            if (TrainFiles != null)
            {
                DialogResult d = MessageBox.Show("Файлы для тренировки уже выбраны, выбрать новые?", "Note", MessageBoxButtons.OKCancel);
                if (d == DialogResult.OK)
                {
                    TrainFiles = null;
                }
                else
                {
                    return;
                }
            }

            folderBrowserDialog1.ShowDialog();
            String src = folderBrowserDialog1.SelectedPath;

            LoadTrainFiles(src);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Bmp files (*.bmp)|*.bmp|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                picture.Image = null;
                String strSrc = openFileDialog1.FileName;
                picture.Image = new Bitmap(strSrc);
            }

        }

        private Byte[] ImgToByteArray(Image<Gray, Byte> img)
        {
            Byte[] data = new Byte[img.Data.Length];

            System.Buffer.BlockCopy(img.Data, 0, data, 0, img.Data.Length);
            return data;
        }

        private Image<Gray, Byte> ByteArrayToImg(Byte[] data, int width, int height, int channels = 1)
        {
            Byte[,,] imgData = new Byte[height, width, channels];

            System.Buffer.BlockCopy(data, 0, imgData, 0, data.Length);

            Image<Gray, Byte> img = new Image<Gray, Byte>(imgData);
            return img;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button3_Click(this, null);
            if (picture.Image == null)
            {
                MessageBox.Show("Выберите изображение");
                return;
            }

            Image<Gray, Byte> img = new Image<Gray, Byte>(new Bitmap(picture.Image));

            Byte[] data = ImgToByteArray(img);

            double[] normData = new double[img.Data.Length];

            for (int j = 0; j < data.Length; j++)
            {
                normData[j] = (data[j] / 255.0) - 0.5;
            }

            network.CalculateOutput(normData);
            double[] res = network.NetOut();

            for (int i = 0; i < res.Length; i++)
            {
                dataGridView1.Rows.Add(i, res[i].ToString("0.00000000"));
                dataGridView1.Rows[i].Cells[0].Value = labels[i];
                dataGridView1.Rows[i].Cells[1].Value = res[i].ToString("0.########");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
        }

        //init train data
        private void DataInit()
        {
            if (TrainFiles == null)
            {
                MessageBox.Show("Файлы для тренировки не выбраны");
                return;
            }

            if (network == null)
            {
                MessageBox.Show("Сеть не создана");
                return;
            }

            if (netReady == true)
            {
                DialogResult d = MessageBox.Show("Сеть не пуста, инициализировать?", "Note", MessageBoxButtons.OKCancel);
                if (d != DialogResult.OK)
                return;
            }

            if (imgWidth == 0 || imgHeight == 0)
            {
                MessageBox.Show("Задайте размер входных изображений");
                return;
            }

            Led(Lamps.init, true);

            IdxFileParser trainParser = new IdxFileParser(TrainFiles[2], 0, imgWidth, imgHeight);
            IdxFileParser trainLabelParser = new IdxFileParser(TrainFiles[3]);
            IdxFileParser testParser = new IdxFileParser(TrainFiles[0], 0, imgWidth, imgHeight);
            IdxFileParser testLabelParser = new IdxFileParser(TrainFiles[1]);

            int classCount = network.NetOut().Count();
            int countTraining = trainParser.dim(0);
            int sizeTraining = trainParser.itemSize();
            inputTraining = new double[countTraining][];
            outputTraining = new double[countTraining][];

            int countTest = testParser.dim(0);
            int sizeTest = testParser.itemSize();
            inputTest = new double[countTest][];
            outputTest = new double[countTest][];

            for (int i = 0; i < countTraining; i++)
            {
                inputTraining[i] = new double[sizeTraining];
                outputTraining[i] = new double[classCount];
            }

            for (int i = 0; i < countTest; i++)
            {
                inputTest[i] = new double[sizeTest];
                outputTest[i] = new double[classCount];
            }

            //inputTraining data init
            Byte[] dataInputTraining = new Byte[sizeTraining];
            for (int i = 0; i < countTraining; i++)
            {
                trainParser.readData(i, ref dataInputTraining);

                for (int j = 0; j < sizeTraining; j++)
                {
                    inputTraining[i][j] = (dataInputTraining[j] / 255.0) - 0.5;
                }
            }

            //outputTraining data init
            Byte[] dataOutputTraining = new Byte[1];
            for (int i = 0; i < countTraining; i++)
            {
                trainLabelParser.readData(i, ref dataOutputTraining);

                for (int j = 0; j < classCount; j++)
                {
                    outputTraining[i][j] = 0;
                }

                outputTraining[i][dataOutputTraining[0]] = 1;
            }

            //inputTest data init
            Byte[] dataInputTest = new Byte[sizeTest];
            for (int i = 0; i < countTest; i++)
            {
                testParser.readData(i, ref dataInputTest);

                for (int j = 0; j < sizeTest; j++)
                {
                    inputTest[i][j] = (dataInputTest[j] / 255.0) - 0.5;
                }
            }

            //outputTest data init
            Byte[] dataOutputTest = new Byte[1];
            for (int i = 0; i < countTest; i++)
            {
                testLabelParser.readData(i, ref dataOutputTest);

                for (int j = 0; j < classCount; j++)
                {
                    outputTest[i][j] = 0;
                }

                outputTest[i][dataOutputTest[0]] = 1;
            }

            network.LearnNWInit(inputTraining, outputTraining);
            netReady = true;

            Led(Lamps.init, false);
        }

        //start
        private void button4_Click(object sender, EventArgs e)
        {
            if (network == null)
            {
                MessageBox.Show("Сеть не создана");
                return;
            }

            if (netReady == false)
            {
                MessageBox.Show("Сеть не инициализирована");
                return;
            }

            if (TrainFiles == null)
            {
                MessageBox.Show("Файлы для тренировки не выбраны");
                return;
            }

            double errorSum, accuracyRate = 0;

            double klearn = double.Parse(textBox2.Text);
            int batchSize = int.Parse(textBox3.Text);

            int epoch = int.Parse(textBox4.Text);

            double accRate = double.Parse(acc.Text);

            isRunned = true;
            for (int i = 0; i < epoch; i++)
            {
                Led(Lamps.propagate, true);
                network.LearnNWStep(inputTraining, outputTraining, klearn, batchSize);
                Led(Lamps.propagate, false);

                if (i % 10 == 0 || i == epoch - 1)
                {
                    Led(Lamps.error, true);
                    network.LearnNWError(inputTraining, outputTraining, 0.7, out errorSum, out accuracyRate);
                    log.Items.Add("IterTraining: " + i + " Error: " + errorSum + " Accuracy: " + accuracyRate);

                    network.LearnNWError(inputTest, outputTest, 0.7, out errorSum, out accuracyRate);
                    network.errors.Add(errorSum);

                    log.Items.Add("IterTest: " + i + " Error: " + errorSum + " Accuracy: " + accuracyRate);
                    Led(Lamps.error, false);
                }

                network.SaveNW(trainFilesDir + "\\network.nw");
                log.TopIndex = log.Items.Count - 1;
                Application.DoEvents();

                //if (accuracyRate > accRate)
                  //  break;

                if (!isRunned)
                {
                    break;
                }
            }
            network.SaveNW(trainFilesDir + "\\network.nw");
        }

        //stop
        private void button5_Click(object sender, EventArgs e)
        {
            isRunned = false;
            log.Items.Add("Обучение остановлено пользователем");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            log.Items.Clear();
        }

        //data initialization
        private void initClick(object sender, EventArgs e)
        {
            DataInit();
        }

        private Image<Gray, Byte> GetImageFromTestSet(int id)
        {
            Image<Gray, Byte> img;
            imgWidth = int.Parse(imageWidth.Text);
            imgHeight = int.Parse(imageHeight.Text);

            IdxFileParser testParser = new IdxFileParser(TrainFiles[0], 0, imgWidth, imgHeight);
            int sizeTest = testParser.itemSize();
            int countTest = testParser.dim(0);
            Byte[] data = new Byte[sizeTest];

            if (id >= 0 && id < countTest)
            {
                testParser.readData(id, ref data);
                img = new Image<Gray, byte>(new Size(imgWidth, imgHeight));
                System.Buffer.BlockCopy(data, 0, img.Data, 0, img.Data.Length);
                picture.Image = img.ToBitmap();
                return img;
            }
            return null;
        }

        //load image from testDataFile
        private void nextID(object sender, EventArgs e)
        {
            if(TrainFiles == null)
            {
                MessageBox.Show("Загрузите файл с тестовыми данными");
                return;
            }
            if (setIterator != 9999)
            {
                setIterator++;
            }

            GetImageFromTestSet(setIterator);
        }

        private void prevID(object sender, EventArgs e)
        {
            if (TrainFiles == null)
            {
                MessageBox.Show("Загрузите файл с тестовыми данными");
                return;
            }

            if (setIterator != 0)
            {
                setIterator--;
            }

            GetImageFromTestSet(setIterator);
        }

        //Build error graph
        private void button9_Click(object sender, EventArgs e)
        {
            if (network != null)
            {
                if (network.errors.Count > 0)
                {
                    ErrorGraph f = new ErrorGraph(network.errors);
                    f.ShowDialog();
                }
            }
        }
    }
}
