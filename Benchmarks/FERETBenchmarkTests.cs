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
    /// FERET benchmark: Tests network on face recognition
    /// Dataset: 64x64 grayscale face images
    /// </summary>
    [TestFixture]
    public class FERETBenchmarkTests
    {
        private const string DatasetPath = "./data/feret";

        [Test]
        public void FERET_NetworkCanBeCreated()
        {
            var settings = BenchmarkSettings.FERET;
            
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
        public void FERET_NetworkCanCalculate()
        {
            var settings = BenchmarkSettings.FERET;
            
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
        public void FERET_NetworkCanBeTrainedWithSyntheticData()
        {
            var settings = BenchmarkSettings.FERET;
            
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
                int subject = i % 10;
                outputData[subject] = 1.0;
                
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
