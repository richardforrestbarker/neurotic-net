using System;
using System.Collections.Generic;
using Neurotic;
using Neurotic.Factory;
using NUnit.Framework;

namespace Tests
{
    public class ConvolutionNeuronTests
    {
        [Test]
        public void Neuron_CalculatesWithInputBiases()
        {
            var pipe1 = new IPipe();
            var pipe2 = new IPipe();
            pipe1.SetValue(2.0);
            pipe2.SetValue(3.0);

            var inputs = new Dictionary<IPipe, IBias>
            {
                { pipe1, new PassthroughBias() },
                { pipe2, new PassthroughBias() }
            };

            var outputPipe = new IPipe();
            var neuron = new ConvolutionNeuron(inputs, outputPipe);
            neuron.Calculate();

            Assert.AreEqual(5.0, outputPipe.GetValue());
        }

        [Test]
        public void Neuron_CalculatesWithConstantBiases()
        {
            var pipe1 = new IPipe();
            var pipe2 = new IPipe();
            pipe1.SetValue(2.0);
            pipe2.SetValue(3.0);

            var inputs = new Dictionary<IPipe, IBias>
            {
                { pipe1, new ConstantBias(5.0) },
                { pipe2, new ConstantBias(10.0) }
            };

            var outputPipe = new IPipe();
            var neuron = new ConvolutionNeuron(inputs, outputPipe);
            neuron.Calculate();

            Assert.AreEqual(15.0, outputPipe.GetValue()); // 5 + 10
        }

        [Test]
        public void Neuron_AppliesOutputBias()
        {
            var pipe1 = new IPipe();
            pipe1.SetValue(10.0);

            var inputs = new Dictionary<IPipe, IBias>
            {
                { pipe1, new PassthroughBias() }
            };

            var outputPipe = new IPipe();
            var neuron = new ConvolutionNeuron(inputs, outputPipe, new InvertBias());
            neuron.Calculate();

            Assert.AreEqual(-10.0, outputPipe.GetValue());
        }

        [Test]
        public void Neuron_OutputBiasSigmoid()
        {
            var pipe1 = new IPipe();
            pipe1.SetValue(0.0);

            var inputs = new Dictionary<IPipe, IBias>
            {
                { pipe1, new PassthroughBias() }
            };

            var outputPipe = new IPipe();
            var neuron = new ConvolutionNeuron(inputs, outputPipe, new SigmoidBias());
            neuron.Calculate();

            Assert.AreEqual(0.5, outputPipe.GetValue(), 0.0001);
        }

        [Test]
        public void Neuron_CanSetOutputBias()
        {
            var pipe1 = new IPipe();
            pipe1.SetValue(5.0);

            var inputs = new Dictionary<IPipe, IBias>
            {
                { pipe1, new PassthroughBias() }
            };

            var outputPipe = new IPipe();
            var neuron = new ConvolutionNeuron(inputs, outputPipe);
            
            neuron.setOutputBias(new ConstantBias(100.0));
            neuron.Calculate();

            Assert.AreEqual(100.0, outputPipe.GetValue());
        }

        [Test]
        public void Neuron_GetOutputBias()
        {
            var neuron = new ConvolutionNeuron();
            Assert.IsNotNull(neuron.getOutputBias());
            Assert.IsInstanceOf<PassthroughBias>(neuron.getOutputBias());

            var sigmoid = new SigmoidBias();
            neuron.setOutputBias(sigmoid);
            Assert.AreEqual(sigmoid, neuron.getOutputBias());
        }
    }
}
