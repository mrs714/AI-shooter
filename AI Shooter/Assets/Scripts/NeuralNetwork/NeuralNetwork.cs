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
    float mutationValue;
    Layer[] layers;

    // For representation
    public int distanceBetweenLayers = 4;
    public int verticalDistance = 2;


    // Constructor with number of neurons in first layer, last layer, and hidden layers.
    public NeuralNetwork(int FirstLayerNumber, int LastLayerNumber, int HiddenLayersNumber, int NeuronsPerLayer, float MutationValue = 0.1f)
    {
        this.firstLayerNumber = FirstLayerNumber;
        this.lastLayerNumber = LastLayerNumber;
        this.hiddenLayersNumber = HiddenLayersNumber;
        this.neuronsPerLayer = NeuronsPerLayer;
        this.mutationValue = MutationValue;

        Initialize();
    }

    // Constructor to copy a neural network
    public NeuralNetwork(NeuralNetwork neuralNetwork)
    {
        this.firstLayerNumber = neuralNetwork.firstLayerNumber;
        this.lastLayerNumber = neuralNetwork.lastLayerNumber;
        this.hiddenLayersNumber = neuralNetwork.hiddenLayersNumber;
        this.neuronsPerLayer = neuralNetwork.neuronsPerLayer;
        this.mutationValue = neuralNetwork.mutationValue;

        Initialize();
        CopyNeuralNetwork(neuralNetwork);
    }

    // Methods: 

    // Copy a neural network
    public void CopyNeuralNetwork(NeuralNetwork neuralNetwork)
    {
        // For each layer
        for (int i = 0; i < layers.Length; i++)
        {
            // For each neuron
            for (int j = 0; j < layers[i].neurons.Length; j++)
            {
                // Copy each weight
                for (int k = 0; k < layers[i].neurons[j].weights.Length; k++)
                {
                    layers[i].neurons[j].weights[k] = neuralNetwork.layers[i].neurons[j].weights[k];
                }
                // Copy bias
                layers[i].neurons[j].bias = neuralNetwork.layers[i].neurons[j].bias;
            }
        }
    }

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
                    layers[i].neurons[j].weights[k] += Random.Range(-mutationValue, mutationValue);
                    layers[i].neurons[j].weights[k] = Mathf.Clamp(layers[i].neurons[j].weights[k], -1, 1);
                }
                // And the bias:
                layers[i].neurons[j].bias += Random.Range(-mutationValue, mutationValue);
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
        return (1 / (1 + Mathf.Exp(-x)) - 0.5f) * 2;
    } 

    // Checks if two neural networks are the same
    public bool IsEqual(NeuralNetwork other) {
        // For each layer:
        for (int i = 0; i < layers.Length; i++) {
            // For each neuron:
            for (int j = 0; j < layers[i].neurons.Length; j++) {
                // For each weight:
                for (int k = 0; k < layers[i].neurons[j].weights.Length; k++) {
                    // If the weights are different, return false:
                    if (layers[i].neurons[j].weights[k] != other.layers[i].neurons[j].weights[k]) {
                        return false;
                    }
                }
                // If the bias is different, return false:
                if (layers[i].neurons[j].bias != other.layers[i].neurons[j].bias) {
                    return false;
                }
            }
        }
        // If everything is the same, return true:
        return true;
    }

    // Save the neural network to a file
    public void SaveToFile(string path) {
        // Create a new file:
        System.IO.FileStream file = System.IO.File.Create(path);
        // Print FirstLayerNumber, LastLayerNumber, HiddenLayersNumber, NeuronsPerLayer, MutationValue in the first line:
        string line = firstLayerNumber + " " + lastLayerNumber + " " + hiddenLayersNumber + " " + neuronsPerLayer + " " + mutationValue + "\n";
        // After that, print the weights and biases of each neuron:
        for (int i = 0; i < layers.Length; i++) {
            for (int j = 0; j < layers[i].neurons.Length; j++) {
                for (int k = 0; k < layers[i].neurons[j].weights.Length; k++) {
                    line += layers[i].neurons[j].weights[k] + " ";
                }
                line += layers[i].neurons[j].bias + "\n";
            }
        }
        // Convert the string to bytes:
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(line);
        // Write the bytes to the file:
        file.Write(bytes, 0, bytes.Length);
        // Close the file:
        file.Close();
    }

    // Load the neural network from a file
    public void LoadFromFile(string path) {
        // Read the file:
        string line = System.IO.File.ReadAllText(path);
        // Split the string by lines:
        string[] lines = line.Split('\n');
        // Split the first line by spaces:
        string[] values = lines[0].Split(' ');
        // Get the values of FirstLayerNumber, LastLayerNumber, HiddenLayersNumber, NeuronsPerLayer, MutationValue:
        firstLayerNumber = int.Parse(values[0]);
        lastLayerNumber = int.Parse(values[1]);
        hiddenLayersNumber = int.Parse(values[2]);
        neuronsPerLayer = int.Parse(values[3]);
        mutationValue = float.Parse(values[4]);
        // Initialize the network:
        Initialize();
        // For each layer:
        int counter = 1;
        for (int i = 0; i < layers.Length; i++) {
            // For each neuron:
            for (int j = 0; j < layers[i].neurons.Length; j++) {
                // Split the line by spaces:
                values = lines[counter].Split(' ');
                // For each weight:
                for (int k = 0; k < layers[i].neurons[j].weights.Length; k++) {
                    // Get the weight:
                    layers[i].neurons[j].weights[k] = float.Parse(values[k]);
                }
                // Get the bias:
                layers[i].neurons[j].bias = float.Parse(values[values.Length - 1]);
                // Increase the counter:
                counter++;
            }
        }
    }

    // Creates the 3D representation of the NN from the prefabs "Neuron" and "Connection"
    public List<GameObject> Create3DRepresentation(GameObject NeuronPrefab, GameObject ConnectionPrefab, int distanceBetweenLayers = 2, int verticalDistance = 2) {

        distanceBetweenLayers = this.distanceBetweenLayers;
        verticalDistance = this.verticalDistance;

        // List of neurons:
        List<GameObject> neurons = new List<GameObject>();
        // List of connections:
        List<GameObject> connections = new List<GameObject>();

        // Max ammount of neurons in any layer:
        int maxNeurons = Mathf.Max(firstLayerNumber, lastLayerNumber, neuronsPerLayer);

        // Neurons
        for (int i = 0; i < layers.Length; i++) {
            // For each neuron:
            for (int j = 0; j < layers[i].neurons.Length; j++) {
                // Create a neuron:
                // Get the ammount of neurons in the layer we are in:
                int length = 0;

                if (i == 0) {
                    length = firstLayerNumber;
                }
                else if (i == hiddenLayersNumber + 1) {
                    length = lastLayerNumber;
                }
                else {
                    length = neuronsPerLayer;
                }  

                // Calculate the height depending on the max ammount of neurons for any layer, and the length of the layer we are in, centered around 2 in the y axis:
                float height = (maxNeurons - length) + j * verticalDistance;

                // Create a neuron centered in the y axis depending on the length of the layer and the max ammount of neurons:
                GameObject neuron = GameObject.Instantiate(NeuronPrefab, new Vector3(i * distanceBetweenLayers, height, 0), Quaternion.identity);

                // Add the neuron to the list:
                neurons.Add(neuron);
            }
        }

        // Connections
        int counter = 0;
        for (int i = 0; i < layers.Length - 1; i++) {
            int counter_layer = 0;
            // For each neuron:
            for (int j = 0; j < layers[i].neurons.Length; j++) {
                int counter_next = 0;
                // For each neuron in the next layer:
                for (int k = 0; k < layers[i + 1].neurons.Length; k++) {

                    // Create a connection between the neurons:
                    GameObject actual_neuron = neurons[counter];
                    GameObject next_neuron = neurons[layers[i].neurons.Length + counter_next + counter - counter_layer];

                    // Get the heights of both neurons
                    float height_actual = actual_neuron.transform.position.y;
                    float height_next = next_neuron.transform.position.y;
                    float height = (height_next + height_actual) / 2;

                    // Create the connection:
                    GameObject connection = GameObject.Instantiate(ConnectionPrefab, new Vector3((i * distanceBetweenLayers) + (distanceBetweenLayers/2), height, 0), Quaternion.identity);
                    
                    // Rotate 90 degrees in the x axis:
                    connection.transform.Rotate(0, 0, 90);

                    float diff_height = (int)(height_next - height_actual);
                    float diagonal = Mathf.Round(Mathf.Sqrt(distanceBetweenLayers * distanceBetweenLayers + diff_height * diff_height) * 1000) / 1000f;
                    float angle = Mathf.Asin((diff_height/diagonal));

                    // Rotate the connection to point to the next neuron:
                    connection.transform.Rotate(0, 0, angle * Mathf.Rad2Deg);

                    // Calculate the space to fill
                    float diff_distance = diagonal/(2);

                    // Adjust the scale based on the calculated height difference
                    connection.transform.localScale = new Vector3(0.1f, diff_distance, 0.1f);

                    // Add the connection to the list:
                    connections.Add(connection);
                    
                    counter_next++;
                }
                counter_layer++;
                counter++;
            }
        }

        // Return the list of neurons so they can be moved later:
        // Join the list of neurons and connections:
        List<GameObject> objects = new List<GameObject>();
        objects.AddRange(neurons);
        objects.AddRange(connections);
        return objects;
    }

    // Draws the neural network on the top right of the camera
    public void DrawNetwork(List<GameObject> objects) {
        // Calculate the ammount of neurons and connections:
        int neuronsCount = firstLayerNumber + lastLayerNumber + hiddenLayersNumber * neuronsPerLayer;

        List<GameObject> neurons = objects.GetRange(0, neuronsCount);
        List<GameObject> connections = objects.GetRange(neuronsCount, objects.Count - neuronsCount);

        // Give each neuron a color depending on its value, from red to green:
        int count = 0;
        for (int i = 0; i < layers.Length; i++) {
            for (int j = 0; j < layers[i].neurons.Length; j++) {

                // Gradient from red to green:
                float value = layers[i].neurons[j].value;
                Color color = new Color(1 - value, value, 0);
                neurons[count].GetComponent<Renderer>().material.color = color;

                count++;
            }
        }

        // Now the connections:
        count = 0;
        for (int i = 0; i < layers.Length - 1; i++) {
            for (int j = 0; j < layers[i].neurons.Length; j++) {
                for (int k = 0; k < layers[i + 1].neurons.Length; k++) {
                    // Get the connection:
                    GameObject connection = connections[count];

                    // Set color depending on weight:
                    float weight = layers[i].neurons[j].weights[k];
                    weight = (weight + 1) / 2; // Scale from {-1, 1} to {0, 1}
                    Color color = new Color(1 - weight, weight, 0);
                    connection.GetComponent<Renderer>().material.color = color;

                    count++;
                }
            }
        }
    }
}
