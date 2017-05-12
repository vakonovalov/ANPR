using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NeuralNet
{
    static public class LinealAlgebraOperationts
    {
        static public double[,] Convolution(double[,] matrix, double[,] kernel)
        {
            int i, j, k, l;
            int wMatrix = matrix.GetLength(1);
            int hMatrix = matrix.GetLength(0);
            int wKernel = kernel.GetLength(1);
            int hKernel = kernel.GetLength(0);
            double sum = 0;
            double[,] resMatrix = new double[hMatrix - hKernel + 1, wMatrix - wKernel + 1];

            for (i = 0; i < hMatrix - hKernel; i++)
            {
                for (j = 0; j < wMatrix - wKernel; j++)
                {
                    sum = 0;
                    for (k = 0; k < hKernel; k++)
                    {
                        for (l = 0; l < wKernel; l++)
                        {
                            sum += matrix[i + k, j + l] * kernel[k, l]; 
                        }
                    }
                    resMatrix[i, j] = sum;
                }
            }
            return resMatrix;
        }

        static public double[] Convolution(double[] matrix, double[,] kernel, int hMatrix, int wMatrix)
        {
            int i, j, k, l;
            int wKernel = kernel.GetLength(1);
            int hKernel = kernel.GetLength(0);
            double sum = 0;
            double[] resMatrix = new double[(hMatrix - hKernel + 1) * (wMatrix - wKernel + 1)];

            for (i = 0; i < hMatrix - hKernel; i++)
            {
                for (j = 0; j < wMatrix - wKernel; j++)
                {
                    sum = 0;
                    for (k = 0; k < hKernel; k++)
                    {
                        for (l = 0; l < wKernel; l++)
                        {
                            sum += matrix[(i + k) * hMatrix + (j + l)] * kernel[k, l];
                        }
                    }
                    resMatrix[i * hMatrix + j] = sum;
                }
            }
            return resMatrix;
        }
    }
}
