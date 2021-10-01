using System;
using System.Collections.Generic;
using System.Linq;
using Neurotic;
using Neurotic.Factory;
using NUnit.Framework;

namespace Tests
{
    public class ConvolutionNetworkFactoryTests
    {


        ConvolutionNetworkFactory factory;
        ICollection<IPipe> inps = new List<IPipe>();
        ICollection<IPipe> outps = new List<IPipe>();
        [SetUp]
        public void Setup()
        {
            factory = new ConvolutionNetworkFactory(.5, 19, 5);
            inps = new List<IPipe>();
            inps.Fill((i) => new IPipe(), 5, (i, p) => p.SetValue(i));
            outps.Fill((i) => new IPipe(), 5, (i, p) => p.SetValue(i));
        }
        [Test]
        public void FactoryConstructsWithNoNullNeurons()
        {
            ConvolutionNeuralNetwork network = factory.Construct(inps, outps);
            Assert.True(network.getInput().Any());
            Assert.True(network.getOutput().Any());
            Assert.True(network.getInput().All(p => p != null));
            Assert.True(network.getOutput().All(p => p != null));
        }
        [Test]
        public void OutputPipesAreReferencedAsInputPipes()
        {
            ConvolutionNeuralNetwork network = factory.Construct(inps, outps);

            var firstNeuronOutput = network.ElementAt(0).ElementAt(0).getOutput();
            var secondNeuronOutput = network.ElementAt(0).ElementAt(1).getOutput();

            Assert.True(network.ElementAt(1).ElementAt(0).getInput().Any(p => object.ReferenceEquals(p, firstNeuronOutput)));

            Assert.True(network.ElementAt(1).ElementAt(0).getInput().Any(p => p.Equals(firstNeuronOutput)));
            Assert.True(network.ElementAt(1).ElementAt(1).getInput().Any(p => p.Equals(secondNeuronOutput)));
        }
        [Test]
        public void InterconectivityMustBe0To1()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => factory = new ConvolutionNetworkFactory(-0.01, 20, 5));
            Assert.Throws<ArgumentOutOfRangeException>(() => factory = new ConvolutionNetworkFactory(1.01, 20, 5));
        }

        [Test]
        public void LayerCountGreaterThan0()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => factory = new ConvolutionNetworkFactory(.5, -1, 5));
        }
        [Test]
        public void NetworkCellCountGreaterThan0()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => factory = new ConvolutionNetworkFactory(.5, 20, -1));
        }
    }
}