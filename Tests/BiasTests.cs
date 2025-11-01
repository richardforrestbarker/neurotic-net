using System;
using NUnit.Framework;
using Neurotic;

namespace Tests
{
    public class BiasTests
    {
        [Test]
        public void ConstantBias_ReturnsConstantValue()
        {
            var bias = new ConstantBias(5.0);
            Assert.AreEqual(5.0, bias.Bias(10.0, null));
            Assert.AreEqual(5.0, bias.Bias(0.0, null));
            Assert.AreEqual(5.0, bias.Bias(-10.0, null));
        }

        [Test]
        public void PassthroughBias_ReturnsInput()
        {
            var bias = new PassthroughBias();
            Assert.AreEqual(10.0, bias.Bias(10.0, null));
            Assert.AreEqual(0.0, bias.Bias(0.0, null));
            Assert.AreEqual(-10.0, bias.Bias(-10.0, null));
        }

        [Test]
        public void SigmoidBias_CalculatesCorrectly()
        {
            var bias = new SigmoidBias();
            Assert.AreEqual(0.5, bias.Bias(0.0, null), 0.0001);
            Assert.IsTrue(bias.Bias(10.0, null) > 0.99);
            Assert.IsTrue(bias.Bias(-10.0, null) < 0.01);
        }

        [Test]
        public void InvertBias_NegatesInput()
        {
            var bias = new InvertBias();
            Assert.AreEqual(-10.0, bias.Bias(10.0, null));
            Assert.AreEqual(0.0, bias.Bias(0.0, null));
            Assert.AreEqual(10.0, bias.Bias(-10.0, null));
        }

        [Test]
        public void ReciprocalBias_ReturnsReciprocal()
        {
            var bias = new ReciprocalBias();
            Assert.AreEqual(0.5, bias.Bias(2.0, null));
            Assert.AreEqual(2.0, bias.Bias(0.5, null));
            Assert.AreEqual(0.0, bias.Bias(0.0, null)); // Edge case for zero
        }

        [Test]
        public void ConstantBias_EqualsAndHashCode()
        {
            var bias1 = new ConstantBias(5.0);
            var bias2 = new ConstantBias(5.0);
            var bias3 = new ConstantBias(3.0);

            Assert.IsTrue(bias1.Equals(bias2));
            Assert.IsFalse(bias1.Equals(bias3));
            Assert.AreEqual(bias1.GetHashCode(), bias2.GetHashCode());
            Assert.AreNotEqual(bias1.GetHashCode(), bias3.GetHashCode());
        }

        [Test]
        public void PassthroughBias_EqualsAndHashCode()
        {
            var bias1 = new PassthroughBias();
            var bias2 = new PassthroughBias();

            Assert.IsTrue(bias1.Equals(bias2));
            Assert.AreEqual(bias1.GetHashCode(), bias2.GetHashCode());
        }

        [Test]
        public void BiasToString_ReturnsCorrectSignature()
        {
            var constant = new ConstantBias(5.0);
            Assert.AreEqual("Constant(5)", constant.ToString());

            var passthrough = new PassthroughBias();
            Assert.AreEqual("Passthrough(x)", passthrough.ToString());

            var sigmoid = new SigmoidBias();
            Assert.AreEqual("Sigmoid(x) = 1/(1+e^-x)", sigmoid.ToString());

            var invert = new InvertBias();
            Assert.AreEqual("Invert(x) = -x", invert.ToString());

            var reciprocal = new ReciprocalBias();
            Assert.AreEqual("Reciprocal(x) = 1/x", reciprocal.ToString());
        }

        [Test]
        public void FunctionBias_FactoryMethods()
        {
            var constant = FunctionBias.Constant(5.0);
            Assert.IsInstanceOf<ConstantBias>(constant);
            Assert.AreEqual(5.0, constant.Bias(0.0, null));

            var passthrough = FunctionBias.Passthrough();
            Assert.IsInstanceOf<PassthroughBias>(passthrough);
            Assert.AreEqual(10.0, passthrough.Bias(10.0, null));

            var sigmoid = FunctionBias.Sigmoid();
            Assert.IsInstanceOf<SigmoidBias>(sigmoid);

            var invert = FunctionBias.Invert();
            Assert.IsInstanceOf<InvertBias>(invert);

            var reciprocal = FunctionBias.Reciprocal();
            Assert.IsInstanceOf<ReciprocalBias>(reciprocal);
        }
    }
}
