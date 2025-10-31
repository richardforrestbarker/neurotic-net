neurotic-net
===========

A lightweight, extensible foundation for building neural networks in a purely interface-based style. neurotic-net exposes small composable interfaces for Networks, Neurons, Pipes, Trainers and Testers so you can mix-and-match implementations, chain and group models, and create novel connection topologies without being forced into a monolithic class hierarchy.

Key ideas
- Interface-first design: every element — networks, neurons, layers — implements simple interfaces so they can be composed, swapped, and tested independently.
- Explicit data flow: values travel through IPipe objects; networks and neurons control how they read inputs and write outputs.
- Modular topology: networks define how neurons are connected; neurons define the transform that maps inputs to a single output IPipe.
- Decoupled training and testing: trainers and testers are separate interfaces so you can apply different optimization or evaluation strategies to the same model.

Why use neurotic-net
- Flexibility: create feedforward, recurrent, grouped, or experimental hybrid topologies by implementing or combining provided network types.
- Testability: small interfaces and single-responsibility components make unit testing easy.
- Extensibility: add new neuron activation functions, new connection strategies, or new trainers without changing existing code.
- Composability: chain networks together or nest networks inside networks to build complex models from simple parts.

Core concepts and interfaces
- INeuralInterface
  - The root interface for all components in the system. Networks, neurons, and higher-order components implement this.
  - Responsibilities: lifecycle methods (initialize, reset), connection/registration APIs, and possibly a tick/execute method depending on the implementation.
  - Rationale: treating networks and neurons uniformly allows networks to contain networks, layers to contain layers, and for composition to remain simple.

- IPipe
  - Represents a single channel that holds a value (or a vector) that can be read or written by components.
  - Used for:
    - Network inputs and outputs
    - Neuron outputs (every neuron exposes a single IPipe output)
    - Layer-level wiring
  - Rationale: abstracting the value container separates data transport from computation, which enables different storage/backing strategies (e.g., shared memory, value-copying, lazy evaluation, or delayed/streaming pipes).

- INetwork (or Network implementations)
  - Declares the number of inputs and outputs and how neurons are arranged and connected.
  - Responsibilities: wiring neurons and IPipes together, exposing top-level input/output pipes, and defining a model’s evaluation pass or activation schedule.
  - Implementations can provide different connection strategies (see “Network types” below).

- INeuron (or Neuron implementations)
  - Implements the computational unit. Each neuron:
    - Receives an array (or collection) of IPipe inputs
    - Computes a function over them (activation/potential)
    - Writes its result into a single IPipe output
  - Many neuron types are provided, each named for the function they compute (see “Neuron types” below).

- ITrainer
  - Interface for trainers that mutate networks to reduce loss over data.
  - Responsibilities: define training loops, compute gradients or update rules, schedule epochs/batches, and expose hooks for monitoring or callbacks.
  - Trainers are separate from networks to allow using different training strategies on the same network topology.

- ITester
  - Interface for running evaluations of trained models.
  - Responsibilities: run datasets through models, compute metrics, generate reports, and optionally produce confusion matrices or detailed per-sample diagnostics.

Provided implementations (examples)
Note: the exact names and classes in the repo may differ — these are example categories to flesh out a README. Update names to reflect the repository’s actual types when you finalize the README.

- Neuron types
  - LinearNeuron — computes a weighted sum of inputs (weights can be stored alongside the neuron or supplied externally).
  - BiasNeuron — outputs a constant bias value to other neurons.
  - ActivationNeuron — applies an activation function (sigmoid, tanh, ReLU, leaky-ReLU, softmax variants).
  - CompositeNeuron — composes multiple sub-neurons or sub-networks to produce an output (useful for custom non-linearities).
  - RecurrentNeuron — maintains internal state across activation ticks (for simple RNN-style behavior).

- Network types
  - FullyConnectedNetwork — each neuron in layer L connects to each neuron in layer L+1 (classic feedforward).
  - LayeredNetwork — exposes explicit layer boundaries and provides layer-wise wiring and operations.
  - SparseNetwork — allows custom sparse adjacency lists for experiments with sparsity or pruning.
  - RecurrentNetwork — supports cycles and recurrent connections; must provide an activation schedule to handle state and unfolding.
  - CompositeNetwork — nests sub-networks, enabling model composition (e.g., encoder + decoder inside a single network object).

Typical data flow
1. A Network exposes N input IPipes and M output IPipes.
2. External code writes input values into the Network’s input IPipes.
3. The Network executes its activation/evaluation routine:
   - Either it calls each neuron in turn (topological order for feedforward models),
   - Or it invokes a scheduling policy for recurrent/iterative models.
4. Each Neuron reads its IPipe inputs, computes its function, and writes to its output IPipe.
5. The Network copies or maps internal neuron outputs into its top-level output IPipes for consumption.

Usage examples
- High-level pseudocode (adapt to your repo’s API):

  - Create a network:
    - Instantiate a Network implementation (e.g., FullyConnectedNetwork)
    - Configure layers and neurons (add neurons, set activation types, connect pipes)
  - Set inputs:
    - Write values to the network’s input IPipes
  - Run the network:
    - Call the network’s Evaluate(), Tick(), or Execute() method
  - Read outputs:
    - Read values from the network’s output IPipes

- Training:
  - Instantiate an ITrainer implementation (e.g., BackpropTrainer)
  - Provide training data and loss function
  - Call trainer.Train(network, data, options)
  - Use ITester to evaluate performance on a validation set

API surface (suggested)
Document the key methods and properties on the main interfaces so users know what to implement:
- INeuralInterface
  - Initialize(), Reset(), Tick() / Evaluate()
- IPipe
  - Value { get; set; } or Read(), Write(value)
- INetwork
  - Inputs { get; }, Outputs { get; }
  - AddNeuron(INeuron), Connect(source, target), Evaluate()
- INeuron
  - Inputs { get; }
  - Output { get; }
  - Compute()
- ITrainer
  - Train(INetwork, IDataSet, TrainOptions)
  - Step(), SaveCheckpoint()
- ITester
  - Test(INetwork, IDataSet) -> TestResult

Design notes and best practices
- Keep IPipe small and predictable — it’s the primitive of data transfer.
- Keep neurons stateless where possible or explicit about state — if using recurrent neurons, document the state semantics and how to reset between sequences.
- Favor composition over inheritance: prefer creating new networks by combining existing ones rather than subclassing heavy network classes.
- Provide clear lifecycle methods so users know when to initialize weights, reset state, or clean up ephemeral resources.

Testing and benchmarks
- Unit tests should focus on:
  - IPipe semantics (concurrency, copy vs shared behavior)
  - Neuron compute correctness for each activation type
  - Network wiring correctness (data flows from inputs to outputs as expected)
  - Trainer convergence on toy datasets (e.g., XOR, linear regression)
- Benchmarks:
  - Evaluate memory usage and evaluation speed for large numbers of neurons.
  - Compare different connection strategies (fully connected vs sparse) for throughput.

Extending the library
- Adding new neuron types:
  - Implement INeuron, expose an IPipe output, and document inputs shape/requirements.
- Adding new network topologies:
  - Implement INetwork, define how to wire neuron inputs to outputs, and provide an evaluation schedule.
- Adding trainers/testers:
  - Implement ITrainer/I Tester and provide examples using small datasets.

Examples & snippets
(Include concrete code snippets from your repo here — constructor names, method names, and sample code will make it easier for users to get started. If you want, I can auto-generate example snippets matching the actual types in your code; point me to the files or allow me to read the repo.)

Contributing
- Please open issues for bugs, feature requests, or design discussions.
- Fork the repo and send pull requests. Keep changes small and focused.
- Add unit tests and documentation for new features.

License
- 