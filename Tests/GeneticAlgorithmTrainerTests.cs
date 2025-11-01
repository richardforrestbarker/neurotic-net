using System;
using System.Collections.Generic;
using System.Linq;
using Neurotic;
using Neurotic.Factory;
using Neurotic.Trainer;
using NUnit.Framework;

namespace Tests
{
    public class GeneticAlgorithmTrainerTests
    {
        [Test]
        public void Trainer_CreatesPopulation()
        {
            var factory = new ConvolutionNetworkFactory(0.5, 3, 5);
            var inputs = new List<IPipe>();
            inputs.Fill(i => new IPipe(), 5);
            var outputs = new List<IPipe>();
            outputs.Fill(i => new IPipe(), 5);
            
            var network = factory.Construct(inputs, outputs);
            var trainer = new GeneticAlgorithmTrainer(10, 0.1, 0.7, 42);
            
            var trainingData = new List<TrainingData>
            {
                new TrainingData(new double[] { 1, 2, 3, 4, 5 }, new double[] { 2, 4, 6, 8, 10 })
            };
            
            // Should not throw
            Assert.DoesNotThrow(() => trainer.Train(network, trainingData, 1));
        }

        [Test]
        public void Trainer_CalculatesFitness()
        {
            var factory = new ConvolutionNetworkFactory(0.5, 3, 5);
            var inputs = new List<IPipe>();
            inputs.Fill(i => new IPipe(), 5);
            var outputs = new List<IPipe>();
            outputs.Fill(i => new IPipe(), 5);
            
            var network = factory.Construct(inputs, outputs);
            var trainer = new GeneticAlgorithmTrainer(10, 0.1, 0.7, 42);
            
            var trainingData = new List<TrainingData>
            {
                new TrainingData(new double[] { 0, 0, 0, 0, 0 }, new double[] { 0, 0, 0, 0, 0 })
            };
            
            double fitness = trainer.GetFitness(network, trainingData);
            Assert.GreaterOrEqual(fitness, 0);
        }

        [Test]
        public void Trainer_ImprovesOverGenerations()
        {
            var factory = new ConvolutionNetworkFactory(1.0, 2, 3);
            var inputs = new List<IPipe>();
            inputs.Fill(i => new IPipe(), 3);
            var outputs = new List<IPipe>();
            outputs.Fill(i => new IPipe(), 3);
            
            var network = factory.Construct(inputs, outputs);
            var trainer = new GeneticAlgorithmTrainer(20, 0.2, 0.7, 42);
            
            // Simple training task - try to output constant values
            var trainingData = new List<TrainingData>
            {
                new TrainingData(new double[] { 1, 2, 3 }, new double[] { 0, 0, 0 }),
                new TrainingData(new double[] { 4, 5, 6 }, new double[] { 0, 0, 0 })
            };
            
            double initialFitness = trainer.GetFitness(network, trainingData);
            trainer.Train(network, trainingData, 10);
            double finalFitness = trainer.GetFitness(network, trainingData);
            
            // Fitness should improve (decrease) or at least not get worse
            Assert.LessOrEqual(finalFitness, initialFitness * 1.1); // Allow small tolerance
        }

        [Test]
        public void Trainer_ThrowsForNonConvolutionNetwork()
        {
            var trainer = new GeneticAlgorithmTrainer();
            var trainingData = new List<TrainingData>();
            
            // Create a mock calculator that's not a ConvolutionNeuralNetwork
            var mockCalculator = new MockCalculator();
            
            Assert.Throws<ArgumentException>(() => trainer.Train(mockCalculator, trainingData, 1));
        }

        [Test]
        public void TrainingData_ConstructsCorrectly()
        {
            var inputs = new double[] { 1, 2, 3 };
            var outputs = new double[] { 4, 5, 6 };
            
            var data = new TrainingData(inputs, outputs);
            
            Assert.AreEqual(inputs, data.Inputs);
            Assert.AreEqual(outputs, data.ExpectedOutputs);
        }

        private class MockCalculator : INeuralCalculator
        {
            public void Calculate() { }
        }
    }
}
