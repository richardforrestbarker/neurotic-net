using System.Collections.Generic;

namespace Neurotic.Benchmarks
{
    /// <summary>
    /// Configuration settings for neural network benchmarks
    /// </summary>
    public class BenchmarkSettings
    {
        public int InputCount { get; set; }
        public int OutputCount { get; set; }
        public int LayerCount { get; set; }
        public int NeuronsPerLayer { get; set; }
        public double Interconnectivity { get; set; }

        public BenchmarkSettings(int inputCount, int outputCount, int layerCount = 3, int neuronsPerLayer = 100, double interconnectivity = 0.5)
        {
            InputCount = inputCount;
            OutputCount = outputCount;
            LayerCount = layerCount;
            NeuronsPerLayer = neuronsPerLayer;
            Interconnectivity = interconnectivity;
        }

        /// <summary>
        /// MNIST: 28x28 grayscale images of handwritten digits (0-9)
        /// Note: NeuronsPerLayer must equal OutputCount for the current factory implementation
        /// </summary>
        public static BenchmarkSettings MNIST => new BenchmarkSettings(
            inputCount: 784,      // 28x28 pixels
            outputCount: 10,      // 10 digit classes (0-9)
            layerCount: 3,
            neuronsPerLayer: 10,  // Must match outputCount
            interconnectivity: 0.5
        );

        /// <summary>
        /// ImageNet: Large-scale image classification dataset
        /// Simplified to 224x224x3 RGB images for common CNN architectures
        /// Note: NeuronsPerLayer must equal OutputCount for the current factory implementation
        /// </summary>
        public static BenchmarkSettings ImageNet => new BenchmarkSettings(
            inputCount: 150528,   // 224x224x3 pixels
            outputCount: 1000,    // 1000 object classes
            layerCount: 5,
            neuronsPerLayer: 1000,  // Must match outputCount
            interconnectivity: 0.3
        );

        /// <summary>
        /// FERET: Face recognition dataset
        /// Simplified to 64x64 grayscale images
        /// Note: NeuronsPerLayer must equal OutputCount for the current factory implementation
        /// </summary>
        public static BenchmarkSettings FERET => new BenchmarkSettings(
            inputCount: 4096,     // 64x64 pixels
            outputCount: 100,     // Approximate number of subjects
            layerCount: 4,
            neuronsPerLayer: 100,  // Must match outputCount
            interconnectivity: 0.4
        );

        /// <summary>
        /// ORL: Olivetti Research Laboratory face database
        /// 112x92 grayscale images, 40 subjects
        /// Note: NeuronsPerLayer must equal OutputCount for the current factory implementation
        /// </summary>
        public static BenchmarkSettings ORL => new BenchmarkSettings(
            inputCount: 10304,    // 112x92 pixels
            outputCount: 40,      // 40 subjects
            layerCount: 4,
            neuronsPerLayer: 40,  // Must match outputCount
            interconnectivity: 0.4
        );
    }
}
