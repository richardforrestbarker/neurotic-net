# neurotic-net

A lightweight, extensible foundation for building neural networks in a purely interface-based style. neurotic-net exposes small composable interfaces for Networks, Neurons, Pipes, Biases, and Trainers so you can mix-and-match implementations, chain and group models, and create novel connection topologies without being forced into a monolithic class hierarchy.

## Key Ideas

- **Interface-first design**: Every element — networks, neurons, layers — implements simple interfaces so they can be composed, swapped, and tested independently.
- **Explicit data flow**: Values travel through `IPipe` objects; networks and neurons control how they read inputs and write outputs.
- **Modular topology**: Networks define how neurons are connected; neurons define the transform that maps inputs to outputs via bias functions.
- **Flexible bias functions**: Neurons use `IBias` implementations to transform inputs and outputs, enabling complex activation patterns.
- **Decoupled training**: Trainers are separate interfaces so you can apply different optimization strategies (like genetic algorithms) to the same model.

## Why Use neurotic-net

- **Flexibility**: Create feedforward, convolutional, or experimental hybrid topologies by implementing or combining provided network types.
- **Testability**: Small interfaces and single-responsibility components make unit testing easy.
- **Extensibility**: Add new bias functions, new connection strategies, or new trainers without changing existing code.
- **Composability**: Chain networks together or nest networks inside networks to build complex models from simple parts.

## Core Concepts and Interfaces

### IPipe
Represents a single channel that holds a double value that can be read or written by components.

**Used for:**
- Network inputs and outputs
- Neuron outputs (every neuron exposes a single IPipe output)
- Layer-level wiring

**API:**
```csharp
public class IPipe
{
    public void SetValue(double value);
    public double GetValue();
}
```

### IBias
Represents a transformation function applied to neuron inputs and outputs. Each bias function takes a double input and returns a double output.

**API:**
```csharp
public interface IBias
{
    double Bias(double input, INeuralCalculator caller);
}
```

**Provided implementations:**
- `PassthroughBias` - Returns the value unchanged: `f(x) = x`
- `SigmoidBias` - Sigmoid activation: `f(x) = 1/(1+e^-x)`
- `InvertBias` - Negates the value: `f(x) = -x`
- `ConstantBias` - Returns a constant: `f(x) = c`
- `ReciprocalBias` - Returns reciprocal: `f(x) = 1/x`

All bias functions can be created using the `FunctionBias` class or its static factory methods.

### INeuralCalculator
The root interface for all computational components in the system. Networks, neurons, and layers implement this.

**API:**
```csharp
public interface INeuralCalculator
{
    void Calculate();
}
```

### ConvolutionNeuron
Implements a neuron that:
- Receives a dictionary mapping `IPipe` inputs to their corresponding `IBias` functions
- Computes a weighted sum where each input is transformed by its bias function
- Applies an output bias to the result
- Writes the final result to a single `IPipe` output

**API:**
```csharp
public class ConvolutionNeuron : INeuralInterfaceInputAggregate
{
    public ConvolutionNeuron(Dictionary<IPipe, IBias> inputs, IPipe output, IBias outputBias);
    public void Calculate();
    public ICollection<IPipe> getInput();
    public Dictionary<IPipe, IBias> getInputWithBiases();
    public IPipe getOutput();
    public IBias getOutputBias();
    public void setInputWithBiases(Dictionary<IPipe, IBias> inputsWithBiases);
    public void setOutputBias(IBias bias);
}
```

### ConvolutionNeuralNetwork
A multi-layer neural network implementation built from `ConvolutionNeuralLayer` objects.

**API:**
```csharp
public class ConvolutionNeuralNetwork : LinkedList<ConvolutionNeuralLayer>, INeuralInterfaceIOAggregate
{
    public void Calculate();
    public ICollection<IPipe> getInput();
    public ICollection<IPipe> getOutput();
}
```

### ITrainer
Interface for training strategies that can optimize neural networks.

**API:**
```csharp
public interface ITrainer
{
    void Train(INeuralCalculator network, ICollection<TrainingData> trainingData, int iterations);
    double GetFitness(INeuralCalculator network, ICollection<TrainingData> trainingData);
}
```

### GeneticAlgorithmTrainer
Evolves neural networks using genetic algorithms by modifying:
- Neuron connections (which pipes connect to which neurons)
- Bias functions (input and output biases)
- Network topology through mutation and crossover

**Features:**
- Population-based evolution
- Tournament selection
- Configurable mutation and crossover rates
- Elitism to preserve best solutions
- Mean Squared Error fitness function

## Quick Start

### Installation

Clone the repository and build:

```bash
git clone https://github.com/richardforrestbarker/neurotic-net.git
cd neurotic-net
dotnet build
```

### Basic Usage

```csharp
using Neurotic;
using Neurotic.Factory;

// Create a network factory
var factory = new ConvolutionNetworkFactory(
    interconectivity: 0.5,  // 50% connectivity between layers
    layerCount: 3,          // 3 layers
    cellCount: 5            // 5 neurons per layer
);

// Create input and output pipes
var inputs = new List<IPipe>();
for (int i = 0; i < 5; i++)
    inputs.Add(new IPipe());

var outputs = new List<IPipe>();
for (int i = 0; i < 5; i++)
    outputs.Add(new IPipe());

// Construct the network
var network = factory.Construct(inputs, outputs);

// Set input values
inputs[0].SetValue(1.0);
inputs[1].SetValue(2.0);
// ... set remaining inputs

// Calculate network outputs
network.Calculate();

// Read output values
double result = outputs[0].GetValue();
```

### Using Bias Functions

```csharp
// Create different bias functions
var passthrough = new PassthroughBias();
var sigmoid = new SigmoidBias();
var constant = new ConstantBias(5.0);

// Or use factory methods
var biasFunc = FunctionBias.Sigmoid();

// Apply to a value
double transformed = biasFunc.Bias(2.0, null);  // Returns ~0.88
```

### Training with Genetic Algorithm

```csharp
using Neurotic.Trainer;

// Create training data
var trainingData = new List<TrainingData>
{
    new TrainingData(
        inputs: new double[] { 1.0, 2.0 },
        expectedOutputs: new double[] { 0.0, 0.0 }
    ),
    // Add more training examples...
};

// Create and configure trainer
var trainer = new GeneticAlgorithmTrainer(
    populationSize: 50,
    mutationRate: 0.1,
    crossoverRate: 0.7
);

// Train the network
trainer.Train(network, trainingData, iterations: 100);

// Evaluate fitness
double fitness = trainer.GetFitness(network, trainingData);
```

## Examples

See the `Examples` project for complete working examples:

```bash
dotnet run --project Examples/Examples.csproj
```

The examples demonstrate:
1. Basic network construction and evaluation
2. Using different bias functions
3. Training a network with genetic algorithms

## Project Structure

```
neurotic-net/
├── Neurotic/                    # Main library
│   ├── Interface/               # Core interfaces (IPipe, IBias, ITrainer, etc.)
│   ├── Bias/                    # Bias function implementations
│   ├── Factory/                 # Network factories
│   │   └── Convolution/        # Convolutional network implementation
│   └── Trainer/                 # Training algorithms
│       └── GeneticAlgorithmTrainer.cs
├── Tests/                       # Unit tests
└── Examples/                    # Example usage
```

## Design Notes and Best Practices

- **Keep IPipe small and predictable** — It's the primitive of data transfer.
- **Bias functions enable flexibility** — Use different bias combinations to create complex activation patterns.
- **Favor composition over inheritance** — Prefer creating new networks by combining existing ones rather than subclassing.
- **Clear separation of concerns** — Neurons handle computation, networks handle topology, trainers handle optimization.

## Testing

Run the test suite:

```bash
dotnet test
```

Current test coverage includes:
- IPipe functionality and equality
- All bias function implementations
- Neuron calculation with various bias configurations
- Network factory and construction
- Genetic algorithm trainer evolution

## Extending the Library

### Adding New Bias Functions

Implement `IBias` or extend `FunctionBias`:

```csharp
public class CustomBias : FunctionBias
{
    public CustomBias() 
        : base(
            (input, caller) => Math.Tanh(input),
            "Tanh",
            "Tanh(x)"
        )
    {
    }
}
```

### Adding New Network Topologies

Implement `INeuralInterfaceIOAggregate`:

```csharp
public class CustomNetwork : INeuralInterfaceIOAggregate
{
    public void Calculate() { /* implementation */ }
    public ICollection<IPipe> getInput() { /* implementation */ }
    public ICollection<IPipe> getOutput() { /* implementation */ }
    // ... other required methods
}
```

### Adding New Trainers

Implement `ITrainer`:

```csharp
public class BackpropTrainer : ITrainer
{
    public void Train(INeuralCalculator network, 
                     ICollection<TrainingData> trainingData, 
                     int iterations)
    {
        // Implement backpropagation
    }
    
    public double GetFitness(INeuralCalculator network, 
                            ICollection<TrainingData> trainingData)
    {
        // Calculate loss/fitness
    }
}
```

## Contributing

- Please open issues for bugs, feature requests, or design discussions.
- Fork the repo and send pull requests. Keep changes small and focused.
- Add unit tests and documentation for new features.

## License

[Add license information here]
