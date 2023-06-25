using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetwork
{
    private class Neuron
    {
        public Neuron(float Value = 0, float[] Weights = null, float Bias = 0)
        {
            this.value = Value;
            this.weights = Weights;
            this.bias = Bias;
        }
        public float value;
        public float[] weights;
        public float bias;
    }

    private class Layer
    {
        public Neuron[] neurons;
    }

    //Neural network. Inputs: number of neurons in first layer, last layer, hidden layers, neurons per layer. Outputs: value of neurons in last layer.
    //Weights are randomly generated at start.
    int firstLayerNumber;
    int lastLayerNumber;
    int hiddenLayersNumber;
    int neuronsPerLayer;
    Layer[] layers;


    //Constructor with number of neurons in first layer, last layer, and hidden layers.
    public NeuralNetwork(int FirstLayerNumber, int LastLayerNumber, int HiddenLayersNumber, int NeuronsPerLayer)
    {
        this.firstLayerNumber = FirstLayerNumber;
        this.lastLayerNumber = LastLayerNumber;
        this.hiddenLayersNumber = HiddenLayersNumber;
        this.neuronsPerLayer = NeuronsPerLayer;

        Initialize();
    }

    // Methods: 

    // Initialize the network
    public void Initialize() {
        layers = new Layer[hiddenLayersNumber + 2];

        //create layers
        for (int i = 0; i < layers.Length; i++) {
            layers[i] = new Layer();
        }

        //populate them
        for (int i = 0; i < layers.Length; i++) {
            //Create ammount of neurons depending on the layer:
            if (i == 0) {
                layers[i].neurons = new Neuron[firstLayerNumber];
            }
            else if (i == hiddenLayersNumber + 1) {
                layers[i].neurons = new Neuron[lastLayerNumber];
            }
            else {
                layers[i].neurons = new Neuron[neuronsPerLayer];
            }

            //populate neurons, the layer before the last has a different number of weights, the last one has none:
            for (int j = 0; j < layers[i].neurons.Length; j++) {
                int numberOfWeights = i != hiddenLayersNumber ? neuronsPerLayer : lastLayerNumber;
                float[] weights = new float[numberOfWeights];
                for (int k = 0; k < numberOfWeights; k++) {
                    weights[k] = Random.Range(-1f, 1f);
                }
                layers[i].neurons[j] = new Neuron(0, weights, Random.Range(-1f, 1f));
            }
        }
    }

    // Clear neuron values
    public void ClearValues() {
        for (int i = 0; i < layers.Length; i++) {
            for (int j = 0; j < layers[i].neurons.Length; j++) {
                layers[i].neurons[j].value = 0;
            }
        }
    }

    // Evolve the network
    public void Mutate() {
        // For each layer:
        for (int i = 0; i < layers.Length; i++) {
            // For each neuron:
            for (int j = 0; j < layers[i].neurons.Length; j++) {
                // For each weight:
                for (int k = 0; k < layers[i].neurons[j].weights.Length; k++) {
                    // Mutate the weight:
                    layers[i].neurons[j].weights[k] += Random.Range(-0.1f, 0.1f);
                    layers[i].neurons[j].weights[k] = Mathf.Clamp(layers[i].neurons[j].weights[k], -1, 1);
                }
                // And the bias:
                layers[i].neurons[j].bias += Random.Range(-0.1f, 0.1f);
                layers[i].neurons[j].bias = Mathf.Clamp(layers[i].neurons[j].bias, -1, 1);
            }
        }
    }

    // Calculate values
    public float[] Calculate(float[] inputs)
    {
        ClearValues();
        // Assign weights to first layer:
        for (int i = 0; i < firstLayerNumber; i++) {
            layers[0].neurons[i].value = inputs[i];
        }

        // For each hidden layer, and the last one:
        for (int i = 1; i <= hiddenLayersNumber + 1; i++)
        {
            // For each neuron in the layer:
            for (int j = 0; j < layers[i].neurons.Length; j++)
            {
                // We take the previous layer and calculate the value of the neuron:
                foreach (Neuron neuron in layers[i - 1].neurons)
                {
                    layers[i].neurons[j].value += Mathf.Clamp((neuron.value * neuron.weights[j]) + neuron.bias, -1, 1) ;
                }
                // We apply the activation function:
                layers[i].neurons[j].value = Sigmoid(layers[i].neurons[j].value);
            }
        }

        // Put the values of the last layer into a float array:
        float[] outputs = new float[lastLayerNumber];
        for (int i = 0; i < lastLayerNumber; i++) {
            outputs[i] = layers[hiddenLayersNumber + 1].neurons[i].value;
        }

        return  outputs;
    }

    // Activation function: sigmoid
    float Sigmoid(float x) {
        return 2 / (1 + Mathf.Exp(-x)) - 0.5f;
    } 
}
