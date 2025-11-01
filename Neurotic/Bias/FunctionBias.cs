using System;

namespace Neurotic
{
    public class FunctionBias : IBias
    {
        private readonly Func<double, INeuralCalculator, double> biasFunction;
        private readonly string functionName;
        private readonly string functionSignature;

        public FunctionBias(Func<double, INeuralCalculator, double> biasFunction, string functionName, string functionSignature = null)
        {
            this.biasFunction = biasFunction ?? throw new ArgumentNullException(nameof(biasFunction));
            this.functionName = functionName ?? throw new ArgumentNullException(nameof(functionName));
            this.functionSignature = functionSignature ?? functionName;
        }

        public double Bias(double input, INeuralCalculator caller)
        {
            return biasFunction(input, caller);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            FunctionBias other = (FunctionBias)obj;
            return functionName == other.functionName && 
                   functionSignature == other.functionSignature;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(functionName, functionSignature);
        }

        public override string ToString()
        {
            return functionSignature;
        }

        // Static factory methods for common bias functions
        public static FunctionBias Constant(double constantValue)
        {
            return new ConstantBias(constantValue);
        }

        public static FunctionBias Passthrough()
        {
            return new PassthroughBias();
        }

        public static FunctionBias Sigmoid()
        {
            return new SigmoidBias();
        }

        public static FunctionBias Invert()
        {
            return new InvertBias();
        }

        public static FunctionBias Reciprocal()
        {
            return new ReciprocalBias();
        }
    }

    public class ConstantBias : FunctionBias
    {
        private readonly double constantValue;

        public ConstantBias(double constantValue) 
            : base((input, caller) => constantValue, "Constant", $"Constant({constantValue})")
        {
            this.constantValue = constantValue;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            ConstantBias other = (ConstantBias)obj;
            return constantValue == other.constantValue;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine("Constant", constantValue);
        }
    }

    public class PassthroughBias : FunctionBias
    {
        public PassthroughBias() 
            : base((input, caller) => input, "Passthrough", "Passthrough(x)")
        {
        }
    }

    public class SigmoidBias : FunctionBias
    {
        public SigmoidBias() 
            : base((input, caller) => 1.0 / (1.0 + Math.Exp(-input)), "Sigmoid", "Sigmoid(x) = 1/(1+e^-x)")
        {
        }
    }

    public class InvertBias : FunctionBias
    {
        public InvertBias() 
            : base((input, caller) => -input, "Invert", "Invert(x) = -x")
        {
        }
    }

    public class ReciprocalBias : FunctionBias
    {
        public ReciprocalBias() 
            : base((input, caller) => input != 0 ? 1.0 / input : 0, "Reciprocal", "Reciprocal(x) = 1/x")
        {
        }
    }
}
