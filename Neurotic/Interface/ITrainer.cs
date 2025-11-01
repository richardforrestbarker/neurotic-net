using System.Collections.Generic;

namespace Neurotic
{
    /// <summary>
    /// Interface for training strategies that can optimize neural networks
    /// </summary>
    public interface ITrainer
    {
        /// <summary>
        /// Train a network using the provided training data
        /// </summary>
        /// <param name="network">The network to train</param>
        /// <param name="trainingData">Input-output pairs for training</param>
        /// <param name="iterations">Number of training iterations</param>
        void Train(INeuralCalculator network, ICollection<TrainingData> trainingData, int iterations);

        /// <summary>
        /// Get the current fitness/error of the network
        /// </summary>
        double GetFitness(INeuralCalculator network, ICollection<TrainingData> trainingData);
    }

    /// <summary>
    /// Represents a single training example with inputs and expected outputs
    /// </summary>
    public class TrainingData
    {
        public double[] Inputs { get; set; }
        public double[] ExpectedOutputs { get; set; }

        public TrainingData(double[] inputs, double[] expectedOutputs)
        {
            Inputs = inputs;
            ExpectedOutputs = expectedOutputs;
        }
    }
}
