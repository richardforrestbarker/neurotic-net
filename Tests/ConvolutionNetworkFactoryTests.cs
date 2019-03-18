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
            factory = new ConvolutionNetworkFactory(.5, 20, 5);
            inps = new List<IPipe>();
            inps.Fill((i) => new IPipe(), 5, (i,p) => p.SetValue(i));
            outps.Fill((i) => new IPipe(), 5, (i, p) => p.SetValue(i));
        }
        [Test]
        public void FactoryConstructsWithNoNullNeurons()
        {
            INeuralInterface network = factory.Construct(inps, outps);
            Assert.True(network.getInPipes().Any());
            Assert.True(network.getOutPipes().Any());
            Assert.True(network.getInPipes().All(p => p != null));
            Assert.True(network.getOutPipes().All(p => p != null));
        }

        [Test]
        public void InterconectivityMustBe0To1()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => factory = new ConvolutionNetworkFactory(-0.01, 20, 5));
            Assert.Throws<ArgumentOutOfRangeException>(() => factory = new ConvolutionNetworkFactory(1.01, 20, 5));
            Assert.Pass();
        }

        [Test]
        public void LayerCountGreaterThan0()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => factory = new ConvolutionNetworkFactory(.5, -1, 5));
            Assert.Pass();
        }
        [Test]
        public void NetworkCellCountGreaterThan0()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => factory = new ConvolutionNetworkFactory(.5, 20, -1));
            Assert.Pass();
        }
    }
}