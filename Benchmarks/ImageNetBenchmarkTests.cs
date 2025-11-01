using System;
using System.Collections.Generic;
using System.Linq;
using Neurotic;
using Neurotic.Factory;
using Neurotic.Trainer;
using NUnit.Framework;

namespace Neurotic.Benchmarks
{
    /// <summary>
    /// ImageNet benchmark: Tests network on large-scale image classification
    /// Dataset: 224x224x3 RGB images, 1000 object classes
    /// </summary>
    [TestFixture]
    public class ImageNetBenchmarkTests
    {
        private const string DatasetPath = "./data/imagenet";

        [Test]
        public void ImageNet_NetworkCanBeCreated()
        {
            var settings = BenchmarkSettings.ImageNet;
            
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
        public void ImageNet_NetworkCanCalculate()
        {
            var settings = BenchmarkSettings.ImageNet;
            
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
        public void ImageNet_NetworkCanBeTrainedWithSyntheticData()
        {
            var settings = BenchmarkSettings.ImageNet;
            
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
            
            // Use only a few samples due to large input size
            for (int i = 0; i < 5; i++)
            {
                var inputData = new double[settings.InputCount];
                var outputData = new double[settings.OutputCount];
                
                // Random input
                for (int j = 0; j < settings.InputCount; j++)
                {
                    inputData[j] = random.NextDouble();
                }
                
                // One-hot encoded output for random class
                int classIndex = random.Next(settings.OutputCount);
                outputData[classIndex] = 1.0;
                
                trainingData.Add(new TrainingData(inputData, outputData));
            }

            var trainer = new GeneticAlgorithmTrainer(
                populationSize: 5,
                mutationRate: 0.2,
                crossoverRate: 0.7,
                seed: 42
            );

            // Training should not throw
            Assert.DoesNotThrow(() => trainer.Train(network, trainingData, iterations: 1));

            // Fitness should be calculated
            double fitness = trainer.GetFitness(network, trainingData);
            Assert.IsTrue(!double.IsNaN(fitness) && !double.IsInfinity(fitness));
        }
    }
}
