using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using NeuralNet;

namespace NeuralNetworkLibrary
{
    public static class ActivationFunctions
    {
        public static double Sigmoid(double value)
        {
            return 1.0 / (1.0 + Math.Exp(-value));
        }

        public static double Softmax(int index, double[] inputWeighted)
        {
            double sum = 0;

            for (int i = 0; i < inputWeighted.Length; i++) 
            {
                sum += Math.Exp(inputWeighted[i]);
            }

            return Math.Exp(inputWeighted[index]) / sum;
        }
    }

    public static class CostFunctions
    {
        //define the log-likelihood cost function C = -ln(a)
        public static double LogLikelihood(double value)
        {
            return -Math.Log(value);
        }

        public static double MeanSquareError(double[] y, double[] o)
        {
            double kerr = 0;
            for (int i = 0; i < y.Length; i++)
            {
                kerr += Math.Pow(y[i] - o[i], 2);
            }
            return 0.5*kerr;
        }       
    }

    public abstract class AbstractLayerNW
    {
        protected int inputCount;
        protected int outputCount;
        protected double[,] weights;
        protected double[] biases;
        protected double[] delta;
        protected double[] outputActivations;
        protected double[] inputWeighted;
        protected double[] input;
        protected double[,] gradWeights;
        protected double[] gradBiases;
        protected double[] gradInput;

        public int OutputCount
        {
            get { return outputCount; }
            set { outputCount = value; }
        }

        public int InputCount
        {
            get { return inputCount; }
            set { inputCount = value; }
        }

        public double[] OutputActivations
        {
            get { return outputActivations; }
            set { outputActivations = value; }
        }

        public double[] InputWeighted
        {
            get
            {
                return inputWeighted;
            }

            set
            {
                inputWeighted = value;
            }
        }

        public double[] Delta
        {
            get
            {
                return delta;
            }

            set
            {
                delta = value;
            }
        }

        public double[,] Weights
        {
            get
            {
                return weights;
            }

            set
            {
                weights = value;
            }
        }

        public double[] Biases
        {
            get
            {
                return biases;
            }

            set
            {
                biases = value;
            }
        }

        public double[] Input
        {
            get
            {
                return input;
            }

            set
            {
                input = value;
            }
        }

        public double[,] GradWeights
        {
            get
            {
                return gradWeights;
            }

            set
            {
                gradWeights = value;
            }
        }

        public double[] GradBiases
        {
            get
            {
                return gradBiases;
            }

            set
            {
                gradBiases = value;
            }
        }

        public double[] GradInput
        {
            get { return gradInput; }
            set { gradInput = value; }
        }
        
        public abstract void GenerateWeights();

        protected abstract void Init();

        //Перерузка для слоя который имеет входы и выходы
        public abstract void BackPropagate(double[] gradOutput);

        //Перерузка для последнего слоя (слой без выходов)
//        public abstract void BackPropagate(double[] networkOutput);

        public abstract void FeedForward(double[] input);

        public abstract void FindGradient();

        public abstract double this[int row, int col] { get; set; }
    }

    //Полносвязный слой 
    [Serializable]
    public class FullyConnectedLayer : AbstractLayerNW
    {
       // Конструктор с параметрами. передается количество входных и выходных нейронов
        public FullyConnectedLayer(int countX, int countY)
        {
            InputCount = countX;
            OutputCount = countY;
            Init();
        }

        // Выделяет память под веса
        protected override void Init()
        {
            weights = new double[OutputCount, InputCount];
            biases = new double[OutputCount];
            delta = new double[OutputCount];
            outputActivations = new double[OutputCount];
            inputWeighted = new double[OutputCount];
            input = new double[InputCount];
            gradWeights = new double[OutputCount, InputCount];
            gradBiases = new double[OutputCount];
            gradInput = new double[InputCount];
        }

        // Заполняем веса случайными числами для входного изображения с интенсивностями 0-255
        public override void GenerateWeights()
        {
            Random rnd = new Random();
            for (int i = 0; i < InputCount; i++)
            {
                for (int j = 0; j < OutputCount; j++)
                {
                   weights[i, j] = 0.02346038*rnd.NextDouble() - 0.011730188;
                }
            }
        }

        //public override void BackPropagate(double[] prevErrors, double[,] prevWeights)
        //{
        //    Matrix<double> w = Matrix<double>.Build.DenseOfArray(prevWeights);
        //    Vector<double> d = Vector<double>.Build.DenseOfArray(prevErrors);
        //    Vector<double> z = Vector<double>.Build.DenseOfArray(inputWeighted);
        //    Vector<double> derSigmoid = z.Map<double>(f => f * (1 - f));

        //    w = w.Transpose();
        //    delta = (w.Multiply(d).PointwiseMultiply(derSigmoid)).ToArray();
        //}

        //public override void BackPropagate(double[] networkOutput)
        //{
        //    Vector<double> g = Vector<double>.Build.DenseOfArray(networkOutput);
        //    Vector<double> z = Vector<double>.Build.DenseOfArray(inputWeighted);
        //    Vector<double> derSigmoid = z.Map<double>(f => f * (1 - f));

        //    delta = (g.PointwiseMultiply(derSigmoid)).ToArray();
        //}

        public override void BackPropagate(double[] gradOutput)
        {
            Matrix<double> w = Matrix<double>.Build.DenseOfArray(Weights);
            Vector<double> z = Vector<double>.Build.DenseOfArray(inputWeighted);
            Vector<double> gradOut = Vector<double>.Build.DenseOfArray(gradOutput);
            Vector<double> derSigmoid = z.Map<double>(f => f*(1-f));
            Vector<double> d;

            d = gradOut.PointwiseMultiply(derSigmoid);           
            w = w.Transpose();
            
            gradInput = w.Multiply(d).ToArray();
            delta = d.ToArray();
        }
        

        public override void FeedForward(double[] input)
        {
            Matrix<double> w = Matrix<double>.Build.DenseOfArray(weights);
            Vector<double> a = Vector<double>.Build.DenseOfArray(input);
            Vector<double> b = Vector<double>.Build.DenseOfArray(biases);
            Vector<double> z = w.Multiply(a) + b;

            input = a.ToArray();
            inputWeighted = z.ToArray();
            z = z.Map<double>(f => ActivationFunctions.Sigmoid(f));
            OutputActivations = z.ToArray();
        }

        public override void FindGradient()
        {
            Vector<double> a = Vector<double>.Build.DenseOfArray(input);
            Vector<double> d = Vector<double>.Build.DenseOfArray(delta);

            gradWeights = a.OuterProduct(d).ToArray();
            gradBiases = delta;
        }

        public override double this[int row, int col]
        {
            get { return weights[row, col]; }
            set { weights[row, col] = value; }
        }
    }

    //[Serializable]
    //public class ConvMaxPoolLayer : AbstractLayerNW
    //{
    //    private int featureMapsCount;

    //    // Конструктор с параметрами. передается количество входных и выходных нейронов
    //    public ConvMaxPoolLayer(int countX, int countY)
    //    {
    //        InputCount = countX;
    //        OutputCount = countY;
    //        Init();
    //    }

    //    // Выделяет память под веса
    //    /// <summary>
    //    /// !!!!!!Напоминание нужно подавать на вход количество feature maps
    //    /// </summary>
    //    protected override void Init()
    //    {
    //        weights = new double[OutputCount, InputCount];
    //    }

    //    // Заполняем веса случайными числами
    //    public override void GenerateWeights()
    //    {
    //        Random rnd = new Random();
    //        for (int i = 0; i < InputCount; i++)
    //        {
    //            for (int j = 0; j < OutputCount; j++)
    //            {
    //                weights[i, j] = rnd.NextDouble() - 0.5;
    //            }
    //        }
    //    }

    //    protected override double ActivationFunction(double value)
    //    {
    //        return 1.0 / (1.0 + Math.Exp(-value));
    //    }

    //    //public override void BackPropagate(double[] prevErrors, double[,] prevWeights)
    //    //{
    //    //    Matrix<double> w = Matrix<double>.Build.DenseOfArray(prevWeights);
    //    //    Vector<double> d = Vector<double>.Build.DenseOfArray(prevErrors);
    //    //    Vector<double> z = Vector<double>.Build.DenseOfArray(inputWeighted);
    //    //    Vector<double> derSigmoid = z.Map<double>(f => f * (1 - f));

    //    //    w = w.Transpose();
    //    //    delta = (w.Multiply(d).PointwiseMultiply(derSigmoid)).ToArray();
    //    //}

    //    public override void BackPropagate(double[] networkOutput)
    //    {
    //        Vector<double> g = Vector<double>.Build.DenseOfArray(networkOutput);
    //        Vector<double> z = Vector<double>.Build.DenseOfArray(inputWeighted);
    //        Vector<double> derSigmoid = z.Map<double>(f => f * (1 - f));

    //        delta = (g.PointwiseMultiply(derSigmoid)).ToArray();
    //    }

    //    public override void FeedForward(double[] input)
    //    {
    //        //OutputActivations = LinealAlgebraOperationts.Convolution();

            
    //        Matrix<double> w = Matrix<double>.Build.DenseOfArray(weights);
    //        Vector<double> a = Vector<double>.Build.DenseOfArray(input);
    //        Vector<double> b = Vector<double>.Build.DenseOfArray(biases);
    //        Vector<double> z = w.Multiply(a) + b;

    //        input = a.ToArray();
    //        inputWeighted = z.ToArray();
    //        z = z.Map<double>(f => ActivationFunction(f));
    //        OutputActivations = z.ToArray();
    //    }

    //    public override void FindGradient()
    //    {
    //        Vector<double> a = Vector<double>.Build.DenseOfArray(input);
    //        Vector<double> d = Vector<double>.Build.DenseOfArray(delta);

    //        gradWeights = a.OuterProduct(d).ToArray();
    //        gradBiases = delta;
    //    }

    //    public override double this[int row, int col]
    //    {
    //        get { return weights[row, col]; }
    //        set { weights[row, col] = value; }
    //    }
    //}


    //SoftMax слой (выходной)

    [Serializable]
    public class SoftMaxLayer : AbstractLayerNW
    {
         // Конструктор с параметрами. передается количество входных и выходных нейронов
        public SoftMaxLayer(int countX, int countY)
        {
            InputCount = countX;
            OutputCount = countY;
            Init();
        }

        // Выделяет память под веса
        protected override void Init()
        {
            weights = new double[OutputCount, InputCount];
            biases = new double[OutputCount];
            delta = new double[OutputCount];
            outputActivations = new double[OutputCount];
            inputWeighted = new double[OutputCount];
            input = new double[InputCount];
            gradWeights = new double[OutputCount, InputCount];
            gradBiases = new double[OutputCount];
            gradInput = new double[InputCount];
        }

        // Заполняем веса случайными числами
        public override void GenerateWeights()
        {
            for (int i = 0; i < InputCount; i++)
            {
                for (int j = 0; j < OutputCount; j++)
                {
                    weights[i, j] = 0;
                }
            }
            for (int j = 0; j < OutputCount; j++)
            {
                biases[j] = 0;
            }
        }

        public override void BackPropagate(double[] gradOutput)
        {
            Matrix<double> w = Matrix<double>.Build.DenseOfArray(Weights);
            Vector<double> z = Vector<double>.Build.DenseOfArray(inputWeighted);
            Vector<double> gradOut = Vector<double>.Build.DenseOfArray(gradOutput);

            w = w.Transpose();

            gradInput = w.Multiply(gradOut).ToArray();
            delta = gradOutput;
        }
        
        public override void FeedForward(double[] input)
        {
            Matrix<double> w = Matrix<double>.Build.DenseOfArray(weights);
            Vector<double> a = Vector<double>.Build.DenseOfArray(input);
            Vector<double> b = Vector<double>.Build.DenseOfArray(biases);
            Vector<double> z = w.Multiply(a) + b;

            input = a.ToArray();
            inputWeighted = z.ToArray();

            for (int i = 0; i < z.Count; i++)
            {
                z[i] = ActivationFunctions.Softmax(i, inputWeighted);
            }
            OutputActivations = z.ToArray();
        }

        public override void FindGradient()
        {
            Vector<double> a = Vector<double>.Build.DenseOfArray(input);
            Vector<double> d = Vector<double>.Build.DenseOfArray(delta);

            gradWeights = a.OuterProduct(d).ToArray();
            gradBiases = delta;
        }

        public override double this[int row, int col]
        {
            get { return weights[row, col]; }
            set { weights[row, col] = value; }
        }
    }

    [Serializable]
    public class NeuralNW
    {
        private AbstractLayerNW[] layers;
        int countLayers = 0, countInput, countOutput;

        /* создает полносвязанную сеть из n слоев. 
           sizex - размерность вектора входных параметров
           layers - массив слоев. значение элементов массива - количество нейронов в слое               
         */
        public NeuralNW(int sizeInput, List<AbstractLayerNW> layers)
        {
            countLayers = layers.Count;
            countInput = sizeInput;
            countOutput = layers[layers.Count - 1].OutputCount;

            // заполняем веса слоя случайнымичислами
            for (int i = 0; i < countLayers; i++)
            {
                this.layers[i].GenerateWeights();
            }
        }

        // открывает нс
        public NeuralNW(string filename)
        {
           OpenNW(filename);
        }

        // открывает нс
        public void OpenNW(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
            NeuralNW nw;
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                nw = (NeuralNW)formatter.Deserialize(fs);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("failed to deserialize. reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }

            this.layers = nw.layers;
            this.countLayers = nw.countLayers;
            this.countInput = nw.countInput;
            this.countOutput = nw.countOutput;
        }

        // сохраняет нс
        public void SaveNW(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Create);

            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, this);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("failed to serialize. reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
        }

        //Feed forward for each layer
        public void CalculateOutput(double[] input)
        {
            layers[0].FeedForward(input);
            for (int i = 1; i < countLayers; i++)
            {
                layers[i].FeedForward(layers[i - 1].OutputActivations);
            }
        }

        public void CalculateDelta(double[] trueOutput)
        {
            double[] networkOutput = new double[countOutput];

            for (int i = 0; i < countOutput; i++)
            {
                networkOutput[i] = layers[countLayers - 1].OutputActivations[i] - trueOutput[i];
            }

            layers[countLayers - 1].BackPropagate(networkOutput);

            for (int i = countLayers - 2; i > 0; i--)
            {
                layers[i].BackPropagate(layers[i + 1].GradInput);
            }
        }

        public void CalculateGrad()
        {
            for (int i = 0; i < countLayers; i++)
            {
                layers[i].FindGradient();
            }
        }

        public double[] NetOut(int index)
        {
            return layers[index].OutputActivations;
        }

        public double[] NetOut()
        {
            return layers[countLayers - 1].OutputActivations;
        }

        // возвращает ошибку (метод наименьших квадратов)
        public double CalculateError(double[] y)
        {
            return CostFunctions.MeanSquareError(y, layers[countLayers - 1].OutputActivations);
        }

        /* обучает сеть, изменяя ее весовые коэффициэнты. 
           x, y - обучающая пара. klern - скорость обучаемости
           в качестве результата метод возвращает ошибку 0.5(y-outy)^2 */
        //Первый индекс у массивов - номер входного примера
        public double LearnNW(double[][] input, double[][] trueOutput, double klearn, int miniBatchSize)
        {
            int i, j, k, l;
            int[] indices = new int[miniBatchSize];

            Matrix<double>[] gradWeights = new Matrix<double>[countLayers];
            Vector<double>[] gradBiases = new Vector<double>[countLayers];

            Matrix<double> gW;
            Vector<double> gB;

            for (l = 0; l < countLayers; l++)
            {
                gradWeights[l] = Matrix<double>.Build.Dense(layers[l].GradWeights.GetLength(0), layers[l].GradWeights.GetLength(1), 0);
                gradBiases[l] = Vector<double>.Build.Dense(layers[l].GradBiases.Length, 0);
            }

            for (i = 0; i < input.Length; i++)
            {
                CalculateOutput(input[i]);
                CalculateDelta(trueOutput[i]);
                CalculateGrad();

                for (l = 0; l < countLayers; l++)
                {
                    gW = Matrix<double>.Build.DenseOfArray(layers[l].GradWeights);
                    gB = Vector<double>.Build.DenseOfArray(layers[l].GradBiases);

                    gradWeights[l] = gradWeights[l].Add(gW);
                    gradBiases[l] = gradBiases[l].Add(gB);
                }
            }

            Matrix<double> w;
            Vector<double> b;

            for (l = 0; l < countLayers; l++)
            {
                w = Matrix<double>.Build.DenseOfArray(layers[l].Weights);
                b = Vector<double>.Build.DenseOfArray(layers[l].Biases);

                w = w.Subtract(gradWeights[l].Multiply(klearn / input.Length));
                b = b.Subtract(gradBiases[l].Multiply(klearn / input.Length));

                layers[l].Weights = w.ToArray();
                layers[l].Biases = b.ToArray();
            }

            return CalculateError(trueOutput[i]);
        }

        public int CountLayers
        {
            get { return countLayers; }
        }

        public AbstractLayerNW[] Layers
        {
            get { return layers; }
            set { layers = value; }
        }
    }
    
}
