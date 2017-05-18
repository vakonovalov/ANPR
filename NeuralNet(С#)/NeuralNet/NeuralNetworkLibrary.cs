using System;
using System.Linq;
using System.Collections.Generic;
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
        public static double LogLikelihood(double[] y, double[] o)
        {
            int ind = 0;
            while (y[ind] < 0.01)
            {
                ind++;
            }
            return -Math.Log(o[ind]);
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

    [Serializable]
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

        public abstract void GenerateWeights(bool wiseMode = true);

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
        public override void GenerateWeights(bool wiseMode = true)
        {
            Random rnd = new Random();
            double d, bound;
            d = 1.0 / (double)inputCount;
            bound = Math.Sqrt(3.0 * d);

            for (int i = 0; i < InputCount; i++)
            {
                for (int j = 0; j < OutputCount; j++)
                {
                    weights[j, i] = (wiseMode) ? 2 * bound * rnd.NextDouble() - bound : ((double)rnd.Next(0, 1000) / 1000.0) - 0.5;
                }
            }

            for (int j = 0; j < OutputCount; j++)
            {
                biases[j] = 0;
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
            Vector<double> derSigmoid = z.Map<double>(f => ActivationFunctions.Sigmoid(f) * (1 - ActivationFunctions.Sigmoid(f)));
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

            this.input = a.ToArray();
            inputWeighted = z.ToArray();
            z = z.Map<double>(f => ActivationFunctions.Sigmoid(f));
            OutputActivations = z.ToArray();
        }

        public override void FindGradient()
        {
            Vector<double> a = Vector<double>.Build.DenseOfArray(input);
            Vector<double> d = Vector<double>.Build.DenseOfArray(delta);

            gradWeights = d.OuterProduct(a).ToArray();
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
        public override void GenerateWeights(bool wiseMode = true)
        {
            Random rnd = new Random();
            double d, bound;
            d = 1.0 / (double)(inputCount*inputCount);
            bound = Math.Sqrt(3.0 * d);

            for (int i = 0; i < InputCount; i++)
            {
                for (int j = 0; j < OutputCount; j++)
                {
                    weights[j, i] = (wiseMode) ? 2 * bound * rnd.NextDouble() - bound : ((double)rnd.Next(0, 1000) / 1000.0) - 0.5;
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

            this.input = a.ToArray();
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

            gradWeights = d.OuterProduct(a).ToArray();
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
        private int countLayers = 0, countInput, countOutput;
        private Matrix<double>[] gradWeightsAux;
        private Vector<double>[] gradBiasesAux;
        private Matrix<double>[] cache;
        private int[] indices;
        public List<double> errors = new List<double>();
        
        /* создает полносвязанную сеть из n слоев. 
           sizex - размерность вектора входных параметров
           layers - массив слоев. значение элементов массива - количество нейронов в слое               
         */
        public NeuralNW(int sizeInput, List<AbstractLayerNW> layers)
        {
            countLayers = layers.Count;
            countInput = sizeInput;
            countOutput = layers[layers.Count - 1].OutputCount;

            this.layers = new AbstractLayerNW[layers.Count];

            for (int i = 0; i < countLayers; i++)
            {
                this.layers[i] = layers[i];
            }

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
        
        //перемешивание индексов для формирования случайных mini batch
        private int[] Shuffle(int count)
        {
            Random r = new Random();
            int[] indices = new int[count];
            int n = count;

            for (int i = 0; i < n; i++ )
            {
                indices[i] = i;
            }

            while (n > 1) 
            {  
                n--;
                int k = r.Next(n + 1);
                int value = indices[k];
                indices[k] = indices[n];
                indices[n] = value;  
            }  
            return indices;
        }
       
        // открывает нс
        public static NeuralNW OpenNW(string filename)
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

            return nw;
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

            for (int i = countLayers - 2; i >= 0; i--)
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
            return CostFunctions.LogLikelihood(y, layers[countLayers - 1].OutputActivations);
        }

        /* инициализирует сеть, изменяя ее весовые коэффициэнты. 
           x, y - обучающая пара.
         */
        //Первый индекс у массивов - номер входного примера
        public void LearnNWInit(double[][] input, double[][] trueOutput) 
        {
            gradWeightsAux = new Matrix<double>[countLayers];
            gradBiasesAux = new Vector<double>[countLayers];
            cache = new Matrix<double>[countLayers];

            for (int l = 0; l < countLayers; l++)
            {
                gradWeightsAux[l] = Matrix<double>.Build.Dense(layers[l].GradWeights.GetLength(0), layers[l].GradWeights.GetLength(1), 0);
                gradBiasesAux[l] = Vector<double>.Build.Dense(layers[l].GradBiases.Length, 0);
                cache[l] = Matrix<double>.Build.Dense(layers[l].GradWeights.GetLength(0), layers[l].GradWeights.GetLength(1), 0);
            }

            indices = Shuffle(input.Length);
        }

        /* обучает сеть, изменяя ее весовые коэффициэнты. (стандартный метод) 
         *   x, y - обучающая пара. 
         *   klern - скорость обучаемости
         */
        //Первый индекс у массивов - номер входного примера
        public void LearnNWStep(double[][] input, double[][] trueOutput, double klearn)
        {
            int i, j, k, l;

            for (l = 0; l < countLayers; l++)
            {
                gradWeightsAux[l].Clear();
                gradBiasesAux[l].Clear();
            }
            
            Matrix<double> gW;
            Vector<double> gB;

            for (i = 0; i < input.Length; i++)
            {
                CalculateOutput(input[i]);
                CalculateDelta(trueOutput[i]);
                CalculateGrad();

                for (l = 0; l < countLayers; l++)
                {
                    gW = Matrix<double>.Build.DenseOfArray(layers[l].GradWeights);
                    gB = Vector<double>.Build.DenseOfArray(layers[l].GradBiases);

                    gradWeightsAux[l] = gradWeightsAux[l].Add(gW);
                    gradBiasesAux[l] = gradBiasesAux[l].Add(gB);
                }
            }

            //Adagrad
            for (l = 0; l < countLayers; l++)
            {
                cache[l] = cache[l].Add(gradWeightsAux[l].PointwisePower(2));
            }

            Matrix<double> w;
            Vector<double> b;

            for (l = 0; l < countLayers; l++)
            {
                w = Matrix<double>.Build.DenseOfArray(layers[l].Weights);
                b = Vector<double>.Build.DenseOfArray(layers[l].Biases);

                //w = w.Subtract(gradWeightsAux[l].Multiply(klearn / input.Length));
                w = w.Subtract(gradWeightsAux[l].Multiply(klearn / input.Length).PointwiseDivide(cache[l].PointwiseSqrt().Add(0.00001)));
                b = b.Subtract(gradBiasesAux[l].Multiply(klearn / input.Length));

                layers[l].Weights = w.ToArray();
                layers[l].Biases = b.ToArray();
            }

            return;
        }

        /* обучает сеть, изменяя ее весовые коэффициэнты. (разбитие обучающей выборки на партии) 
         *   x, y - обучающая пара. 
         *   klern - скорость обучаемости
         *   miniBatchSize - размер партии тренировочных изображений
         *   decayRate - скорость снижения значений в cache
         */
        //Первый индекс у массивов - номер входного примера
        public void LearnNWStep(double[][] input, double[][] trueOutput, double klearn, int miniBatchSize, double decayRate = 0.99)
        {
            if (miniBatchSize < 1 || miniBatchSize > input.Length)
            {
                return;
            }

            int i, j, k, l;
            Matrix<double> gW;
            Vector<double> gB;

            for (l = 0; l < countLayers; l++)
            {
                gradWeightsAux[l].Clear();
                gradBiasesAux[l].Clear();
            }

            int batchCount = input.Length / miniBatchSize;
            
            if (input.Length % miniBatchSize > 0)
            {
                batchCount++;
            }

            int begin, end, batchIndex;
            Random r = new Random();
            batchIndex = r.Next(batchCount);
            begin = batchIndex * miniBatchSize;
            end = (batchIndex + 1) * miniBatchSize;

            if (end > input.Length)
            {
                end = input.Length;
            }

            for (i = begin; i < end; i++)
            {
                CalculateOutput(input[indices[i]]);
                CalculateDelta(trueOutput[indices[i]]);
                CalculateGrad();

                for (l = 0; l < countLayers; l++)
                {
                    gW = Matrix<double>.Build.DenseOfArray(layers[l].GradWeights);
                    gB = Vector<double>.Build.DenseOfArray(layers[l].GradBiases);

                    gradWeightsAux[l] = gradWeightsAux[l].Add(gW);
                    gradBiasesAux[l] = gradBiasesAux[l].Add(gB);
                }
            }

            //Adagrad
            for (l = 0; l < countLayers; l++)
            {
                //cache[l] = cache[l].Add(gradWeightsAux[l].PointwisePower(2));
                cache[l] = cache[l].Multiply(decayRate) + (gradWeightsAux[l].PointwisePower(2).Multiply(1.0 - decayRate));
            }

            Matrix<double> w;
            Vector<double> b;

            for (l = 0; l < countLayers; l++)
            {
                w = Matrix<double>.Build.DenseOfArray(layers[l].Weights);
                b = Vector<double>.Build.DenseOfArray(layers[l].Biases);

//                w = w.Subtract(gradWeightsAux[l].Multiply(klearn / miniBatchSize));
                w = w.Subtract(gradWeightsAux[l].Multiply(klearn / miniBatchSize).PointwiseDivide(cache[l].PointwiseSqrt().Add(0.00001)));

                b = b.Subtract(gradBiasesAux[l].Multiply(klearn / miniBatchSize));

                layers[l].Weights = w.ToArray();
                layers[l].Biases = b.ToArray();
            }

            return;
        }

        public void LearnNWError(double[][] input, double[][] trueOutput, double accuracyLimit, out double errorSum, out double accuracyRate)
        {
            errorSum = 0;
            double error, max;
            int success = 0;
            int indTrue, indExp;

            for (int i = 0; i < input.Length; i++)
            {
                CalculateOutput(input[i]);
                error = CalculateError(trueOutput[i]);
                errorSum += error;

                max = NetOut()[0];
                indExp = 0;
                for (int j = 1; j < NetOut().Length; j++)
                {
                    if (NetOut()[j] > max)
                    {
                        max = NetOut()[j];
                        indExp = j;
                    }
                }

                max = trueOutput[i][0];
                indTrue = 0;
                for (int j = 1; j < trueOutput[i].Length; j++)
                {
                    if (trueOutput[i][j] > max)
                    {
                        max = trueOutput[i][j];
                        indTrue = j;
                    }
                }                                

                //if (error < accuracyLimit)
                if (indExp == indTrue)
                {
                    success++;
                }
            }

            accuracyRate = (double)success / (double)input.Length;
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
