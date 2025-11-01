using System;
using System.Collections.Generic;
using System.Linq;
using Neurotic.Factory;

namespace Neurotic.Trainer
{
    /// <summary>
    /// Genetic Algorithm trainer that evolves neural networks by modifying:
    /// - Neuron connections (which pipes connect to which neurons)
    /// - Bias functions (input and output biases)
    /// - Network topology
    /// </summary>
    public class GeneticAlgorithmTrainer : ITrainer
    {
        private readonly int populationSize;
        private readonly double mutationRate;
        private readonly double crossoverRate;
        private readonly Random random;

        private static readonly IBias[] AvailableBiases = new IBias[]
        {
            new PassthroughBias(),
            new SigmoidBias(),
            new InvertBias(),
            new ConstantBias(0.5),
            new ConstantBias(1.0),
            new ConstantBias(-1.0),
            new ReciprocalBias()
        };

        public GeneticAlgorithmTrainer(int populationSize = 50, double mutationRate = 0.1, double crossoverRate = 0.7, int? seed = null)
        {
            this.populationSize = populationSize;
            this.mutationRate = mutationRate;
            this.crossoverRate = crossoverRate;
            this.random = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        public void Train(INeuralCalculator network, ICollection<TrainingData> trainingData, int iterations)
        {
            if (!(network is ConvolutionNeuralNetwork convNetwork))
            {
                throw new ArgumentException("GeneticAlgorithmTrainer currently only supports ConvolutionNeuralNetwork");
            }

            // Create initial population
            var population = CreateInitialPopulation(convNetwork, trainingData);

            for (int generation = 0; generation < iterations; generation++)
            {
                // Evaluate fitness for all individuals
                var fitnessScores = population.Select(net => new
                {
                    Network = net,
                    Fitness = GetFitness(net, trainingData)
                }).OrderBy(x => x.Fitness).ToList();

                // Keep the best individuals (elitism)
                int eliteCount = Math.Max(1, populationSize / 10);
                var newPopulation = fitnessScores.Take(eliteCount).Select(x => CloneNetwork(x.Network)).ToList();

                // Generate rest of population through selection, crossover, and mutation
                while (newPopulation.Count < populationSize)
                {
                    var parent1 = SelectParent(fitnessScores);
                    var parent2 = SelectParent(fitnessScores);

                    ConvolutionNeuralNetwork offspring;
                    if (random.NextDouble() < crossoverRate)
                    {
                        offspring = Crossover(parent1, parent2);
                    }
                    else
                    {
                        offspring = CloneNetwork(parent1);
                    }

                    Mutate(offspring);
                    newPopulation.Add(offspring);
                }

                population = newPopulation;

                // Update the original network with the best individual
                CopyNetwork(fitnessScores.First().Network, convNetwork);
            }
        }

        public double GetFitness(INeuralCalculator network, ICollection<TrainingData> trainingData)
        {
            if (!(network is ConvolutionNeuralNetwork convNetwork))
            {
                return double.MaxValue;
            }

            double totalError = 0;
            foreach (var data in trainingData)
            {
                // Set inputs
                var inputPipes = convNetwork.getInput().ToArray();
                for (int i = 0; i < Math.Min(data.Inputs.Length, inputPipes.Length); i++)
                {
                    inputPipes[i].SetValue(data.Inputs[i]);
                }

                // Calculate
                convNetwork.Calculate();

                // Calculate error
                var outputPipes = convNetwork.getOutput().ToArray();
                for (int i = 0; i < Math.Min(data.ExpectedOutputs.Length, outputPipes.Length); i++)
                {
                    double error = outputPipes[i].GetValue() - data.ExpectedOutputs[i];
                    totalError += error * error; // Mean Squared Error
                }
            }

            return totalError / trainingData.Count;
        }

        private List<ConvolutionNeuralNetwork> CreateInitialPopulation(ConvolutionNeuralNetwork baseNetwork, ICollection<TrainingData> trainingData)
        {
            var population = new List<ConvolutionNeuralNetwork>();
            
            for (int i = 0; i < populationSize; i++)
            {
                var clone = CloneNetwork(baseNetwork);
                // Randomize initial biases
                foreach (var layer in clone)
                {
                    foreach (var neuron in layer)
                    {
                        RandomizeBiases(neuron);
                    }
                }
                population.Add(clone);
            }
            
            return population;
        }

        private ConvolutionNeuralNetwork SelectParent<T>(List<T> fitnessScores) where T : class
        {
            // Tournament selection
            int tournamentSize = 3;
            var tournament = new List<T>();
            
            for (int i = 0; i < tournamentSize; i++)
            {
                tournament.Add(fitnessScores[random.Next(fitnessScores.Count)]);
            }
            
            // Use dynamic to access the anonymous type properties
            dynamic best = tournament.OrderBy(x => ((dynamic)x).Fitness).First();
            return best.Network;
        }

        private ConvolutionNeuralNetwork Crossover(ConvolutionNeuralNetwork parent1, ConvolutionNeuralNetwork parent2)
        {
            var offspring = CloneNetwork(parent1);
            
            // Mix biases from both parents
            var p1Layers = parent1.ToArray();
            var p2Layers = parent2.ToArray();
            var offspringLayers = offspring.ToArray();
            
            for (int l = 0; l < Math.Min(p1Layers.Length, p2Layers.Length); l++)
            {
                var p1Neurons = p1Layers[l].ToArray();
                var p2Neurons = p2Layers[l].ToArray();
                var offspringNeurons = offspringLayers[l].ToArray();
                
                for (int n = 0; n < Math.Min(p1Neurons.Length, p2Neurons.Length); n++)
                {
                    // Randomly choose output bias from either parent
                    if (random.NextDouble() < 0.5)
                    {
                        offspringNeurons[n].setOutputBias(p1Neurons[n].getOutputBias());
                    }
                    else
                    {
                        offspringNeurons[n].setOutputBias(p2Neurons[n].getOutputBias());
                    }
                }
            }
            
            return offspring;
        }

        private void Mutate(ConvolutionNeuralNetwork network)
        {
            foreach (var layer in network)
            {
                foreach (var neuron in layer)
                {
                    // Mutate output bias
                    if (random.NextDouble() < mutationRate)
                    {
                        neuron.setOutputBias(AvailableBiases[random.Next(AvailableBiases.Length)]);
                    }
                    
                    // Mutate input biases
                    if (random.NextDouble() < mutationRate)
                    {
                        RandomizeBiases(neuron);
                    }
                }
            }
        }

        private void RandomizeBiases(ConvolutionNeuron neuron)
        {
            var inputs = new Dictionary<IPipe, IBias>();
            foreach (var pipe in neuron.getInput())
            {
                inputs[pipe] = AvailableBiases[random.Next(AvailableBiases.Length)];
            }
            neuron.setInputWithBiases(inputs);
        }

        private ConvolutionNeuralNetwork CloneNetwork(ConvolutionNeuralNetwork source)
        {
            var clone = new ConvolutionNeuralNetwork();
            var pipeMapping = new Dictionary<IPipe, IPipe>();
            
            // Clone all layers and neurons
            foreach (var layer in source)
            {
                var cloneLayer = new ConvolutionNeuralLayer();
                
                foreach (var neuron in layer)
                {
                    // Create new output pipe
                    var newOutput = new IPipe();
                    pipeMapping[neuron.getOutput()] = newOutput;
                    
                    var cloneNeuron = new ConvolutionNeuron(new Dictionary<IPipe, IBias>(), newOutput, neuron.getOutputBias());
                    cloneLayer.AddLast(cloneNeuron);
                }
                
                clone.AddLast(cloneLayer);
            }
            
            // Now wire up inputs using the pipe mapping
            var sourceLayersArray = source.ToArray();
            var cloneLayersArray = clone.ToArray();
            
            for (int l = 0; l < sourceLayersArray.Length; l++)
            {
                var sourceNeurons = sourceLayersArray[l].ToArray();
                var cloneNeurons = cloneLayersArray[l].ToArray();
                
                for (int n = 0; n < sourceNeurons.Length; n++)
                {
                    var inputs = new Dictionary<IPipe, IBias>();
                    foreach (var input in sourceNeurons[n].getInput())
                    {
                        IPipe mappedPipe;
                        if (pipeMapping.TryGetValue(input, out mappedPipe))
                        {
                            // This is an internal pipe from previous layer
                            var oldInputs = GetInputBiases(sourceNeurons[n]);
                            inputs[mappedPipe] = oldInputs.ContainsKey(input) ? oldInputs[input] : new PassthroughBias();
                        }
                        else
                        {
                            // This is an external input pipe (from network inputs)
                            pipeMapping[input] = input; // Use the same pipe
                            var oldInputs = GetInputBiases(sourceNeurons[n]);
                            inputs[input] = oldInputs.ContainsKey(input) ? oldInputs[input] : new PassthroughBias();
                        }
                    }
                    cloneNeurons[n].setInputWithBiases(inputs);
                }
            }
            
            return clone;
        }

        private Dictionary<IPipe, IBias> GetInputBiases(ConvolutionNeuron neuron)
        {
            return neuron.getInputWithBiases();
        }

        private void CopyNetwork(ConvolutionNeuralNetwork source, ConvolutionNeuralNetwork target)
        {
            var sourceLayersArray = source.ToArray();
            var targetLayersArray = target.ToArray();
            
            for (int l = 0; l < Math.Min(sourceLayersArray.Length, targetLayersArray.Length); l++)
            {
                var sourceNeurons = sourceLayersArray[l].ToArray();
                var targetNeurons = targetLayersArray[l].ToArray();
                
                for (int n = 0; n < Math.Min(sourceNeurons.Length, targetNeurons.Length); n++)
                {
                    targetNeurons[n].setOutputBias(sourceNeurons[n].getOutputBias());
                    
                    // Copy input biases
                    var inputs = new Dictionary<IPipe, IBias>();
                    foreach (var pipe in targetNeurons[n].getInput())
                    {
                        inputs[pipe] = new PassthroughBias(); // Default
                    }
                    targetNeurons[n].setInputWithBiases(inputs);
                }
            }
        }
    }
}
