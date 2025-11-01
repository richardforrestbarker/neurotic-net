using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Neurotic;
using Neurotic.Factory;
using Neurotic.Trainer;
using NUnit.Framework;

namespace Neurotic.Benchmarks
{
    /// <summary>
    /// MNIST benchmark: Tests network on handwritten digit recognition
    /// Dataset: 28x28 grayscale images of digits 0-9
    /// </summary>
    [TestFixture]
    public class MNISTBenchmarkTests
    {
        private const string DatasetPath = "./data/mnist";
        private const int TrainingSamples = 100; // Limited for testing purposes
        private const int TestSamples = 20;

        [Test]
        public void MNIST_NetworkCanBeCreated()
        {
            var settings = BenchmarkSettings.MNIST;
            
            // Create input and output pipes
            var inputs = new List<IPipe>();
            for (int i = 0; i < settings.InputCount; i++)
                inputs.Add(new IPipe());

            var outputs = new List<IPipe>();
            for (int i = 0; i < settings.OutputCount; i++)
                outputs.Add(new IPipe());

            // Create network
            var factory = new ConvolutionNetworkFactory(
                settings.Interconnectivity,
                settings.LayerCount,
                settings.NeuronsPerLayer
            );
            var network = factory.Construct(inputs, outputs);

            Assert.IsNotNull(network);
            Assert.IsTrue(network.getOutput().Count > 0);
        }

        [Test]
        public void MNIST_NetworkCanCalculate()
        {
            var settings = BenchmarkSettings.MNIST;
            
            var inputs = new List<IPipe>();
            for (int i = 0; i < settings.InputCount; i++)
                inputs.Add(new IPipe());

            var outputs = new List<IPipe>();
            for (int i = 0; i < settings.OutputCount; i++)
                outputs.Add(new IPipe());

            var factory = new ConvolutionNetworkFactory(
                settings.Interconnectivity,
                settings.LayerCount,
                settings.NeuronsPerLayer
            );
            var network = factory.Construct(inputs, outputs);

            // Set random input values
            var random = new Random(42);
            foreach (var input in inputs)
            {
                input.SetValue(random.NextDouble());
            }

            // Calculate should not throw
            Assert.DoesNotThrow(() => network.Calculate());

            // All outputs should have values
            foreach (var output in outputs)
            {
                Assert.IsTrue(!double.IsNaN(output.GetValue()));
            }
        }

        [Test]
        public void MNIST_NetworkCanBeTrainedWithSyntheticData()
        {
            var settings = BenchmarkSettings.MNIST;
            
            var inputs = new List<IPipe>();
            for (int i = 0; i < settings.InputCount; i++)
                inputs.Add(new IPipe());

            var outputs = new List<IPipe>();
            for (int i = 0; i < settings.OutputCount; i++)
                outputs.Add(new IPipe());

            var factory = new ConvolutionNetworkFactory(
                settings.Interconnectivity,
                settings.LayerCount,
                settings.NeuronsPerLayer
            );
            var network = factory.Construct(inputs, outputs);

            // Create synthetic training data (simplified patterns)
            var trainingData = new List<TrainingData>();
            var random = new Random(42);
            
            for (int i = 0; i < 10; i++)
            {
                var inputData = new double[settings.InputCount];
                var outputData = new double[settings.OutputCount];
                
                // Random input
                for (int j = 0; j < settings.InputCount; j++)
                {
                    inputData[j] = random.NextDouble();
                }
                
                // One-hot encoded output
                int digit = i % 10;
                outputData[digit] = 1.0;
                
                trainingData.Add(new TrainingData(inputData, outputData));
            }

            var trainer = new GeneticAlgorithmTrainer(
                populationSize: 10,
                mutationRate: 0.2,
                crossoverRate: 0.7,
                seed: 42
            );

            // Training should not throw
            Assert.DoesNotThrow(() => trainer.Train(network, trainingData, iterations: 2));

            // Fitness should be calculated
            double fitness = trainer.GetFitness(network, trainingData);
            Assert.IsTrue(!double.IsNaN(fitness) && !double.IsInfinity(fitness));
        }
    }
}
