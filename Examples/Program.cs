using Neurotic;
using Neurotic.Factory;
using Neurotic.Trainer;

namespace Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Neurotic-Net Examples ===\n");

            // Example 1: Basic Network Construction
            BasicNetworkExample();

            Console.WriteLine();

            // Example 2: Using Different Bias Functions
            BiasFunctionsExample();

            Console.WriteLine();

            // Example 3: Training with Genetic Algorithm
            GeneticAlgorithmExample();

            Console.WriteLine("\nAll examples completed!");
        }

        static void BasicNetworkExample()
        {
            Console.WriteLine("Example 1: Basic Network Construction");
            Console.WriteLine("--------------------------------------");

            // Create a network with 5 inputs, 5 outputs, 3 layers, 5 cells per layer, 50% interconnectivity
            var factory = new ConvolutionNetworkFactory(0.5, 3, 5);

            // Create input and output pipes
            var inputs = new List<IPipe>();
            for (int i = 0; i < 5; i++)
            {
                inputs.Add(new IPipe());
            }

            var outputs = new List<IPipe>();
            for (int i = 0; i < 5; i++)
            {
                outputs.Add(new IPipe());
            }

            // Construct the network
            var network = factory.Construct(inputs, outputs);

            // Set input values
            inputs[0].SetValue(1.0);
            inputs[1].SetValue(2.0);
            inputs[2].SetValue(3.0);
            inputs[3].SetValue(4.0);
            inputs[4].SetValue(5.0);

            // Calculate
            network.Calculate();

            // Read outputs
            Console.WriteLine("Input values: [1.0, 2.0, 3.0, 4.0, 5.0]");
            Console.WriteLine("Output values:");
            for (int i = 0; i < outputs.Count; i++)
            {
                Console.WriteLine($"  Output[{i}]: {outputs[i].GetValue():F4}");
            }
        }

        static void BiasFunctionsExample()
        {
            Console.WriteLine("Example 2: Using Different Bias Functions");
            Console.WriteLine("------------------------------------------");

            // Demonstrate different bias functions
            double testValue = 2.0;

            var passthrough = new PassthroughBias();
            var sigmoid = new SigmoidBias();
            var invert = new InvertBias();
            var constant = new ConstantBias(5.0);
            var reciprocal = new ReciprocalBias();

            Console.WriteLine($"Input value: {testValue}");
            Console.WriteLine($"Passthrough: {passthrough.Bias(testValue, null):F4} - {passthrough}");
            Console.WriteLine($"Sigmoid: {sigmoid.Bias(testValue, null):F4} - {sigmoid}");
            Console.WriteLine($"Invert: {invert.Bias(testValue, null):F4} - {invert}");
            Console.WriteLine($"Constant: {constant.Bias(testValue, null):F4} - {constant}");
            Console.WriteLine($"Reciprocal: {reciprocal.Bias(testValue, null):F4} - {reciprocal}");

            // Using factory methods
            var biasFromFactory = FunctionBias.Sigmoid();
            Console.WriteLine($"\nUsing factory method: {biasFromFactory.Bias(0.0, null):F4}");
        }

        static void GeneticAlgorithmExample()
        {
            Console.WriteLine("Example 3: Training with Genetic Algorithm");
            Console.WriteLine("-------------------------------------------");

            // Create a small network
            var factory = new ConvolutionNetworkFactory(1.0, 2, 2);
            var inputs = new List<IPipe>();
            for (int i = 0; i < 2; i++)
            {
                inputs.Add(new IPipe());
            }

            var outputs = new List<IPipe>();
            for (int i = 0; i < 2; i++)
            {
                outputs.Add(new IPipe());
            }

            var network = factory.Construct(inputs, outputs);

            // Create training data - try to learn to output zeros
            var trainingData = new List<TrainingData>
            {
                new TrainingData(new double[] { 1.0, 2.0 }, new double[] { 0.0, 0.0 }),
                new TrainingData(new double[] { 3.0, 4.0 }, new double[] { 0.0, 0.0 }),
                new TrainingData(new double[] { -1.0, -2.0 }, new double[] { 0.0, 0.0 })
            };

            // Create trainer with seed for reproducibility
            var trainer = new GeneticAlgorithmTrainer(
                populationSize: 20,
                mutationRate: 0.2,
                crossoverRate: 0.7,
                seed: 42
            );

            // Measure initial fitness
            double initialFitness = trainer.GetFitness(network, trainingData);
            Console.WriteLine($"Initial fitness (MSE): {initialFitness:F4}");

            // Train the network
            Console.WriteLine("Training for 20 generations...");
            trainer.Train(network, trainingData, 20);

            // Measure final fitness
            double finalFitness = trainer.GetFitness(network, trainingData);
            Console.WriteLine($"Final fitness (MSE): {finalFitness:F4}");
            Console.WriteLine($"Improvement: {((initialFitness - finalFitness) / initialFitness * 100):F2}%");

            // Test the trained network
            Console.WriteLine("\nTesting trained network:");
            foreach (var data in trainingData)
            {
                for (int i = 0; i < data.Inputs.Length; i++)
                {
                    inputs[i].SetValue(data.Inputs[i]);
                }
                network.Calculate();

                Console.Write($"Input: [{string.Join(", ", data.Inputs.Select(x => x.ToString("F1")))}] ");
                Console.Write($"=> Output: [{string.Join(", ", outputs.Select(o => o.GetValue().ToString("F4")))}] ");
                Console.WriteLine($"(Expected: [{string.Join(", ", data.ExpectedOutputs.Select(x => x.ToString("F1")))}])");
            }
        }
    }
}

