# AI-shooter
Unity project to create and train an agent with a neural network using a genetic algorithm to do diverse tasks, like avoiding enemies or leading projectiles correctly.
<br><br><br>

*Main Objectives:*
<br>
This project has been done with two main objectives in mind. The first was to get a more comprehensive look at some of Unity's features, such as instantiating prefabs, creating an intuitive Unity UI through sliders, selectors and such, building robust, self-contained classes, with well-implemented OOP, delving into C# handling of private, protected, and public types among others...
The second, more important objective, was to build a neural network and implement a genetic algorithm by hand. While I had previously worked with intelligent agents, it was through the use of MLAgents during my research project. 
Now, having done the research, and having a deeper understanding of the nature of both NN and genetic algorithms, I wanted to build them from the ground up to strengthen my knowledge.
<br><br><br>

*How to use the project?*
<br>
This repository contains the full folder of the Unity project (Unity version - 2022.1.23f1). After opening it like any other project, the scene contains two main things: the folder of the scene objects, which contains the plane that makes up the floor, the camera, and the light, and the TrainingManager, an empty object which contains the script that handles the spawning of all the objects for the training. The manager has the main settings which can be changed:
- Training: Whether the manager is active or not. If the box is checked, everything will be spawned according to the settings.
- Number of enemies: How many enemies will be in every training zone.
- Number of Objectives: How many objectives will be in every training zone.
- Number of zones per side: How many zones will be in one side (n zones per side -> n*n training zones in total).
- Zone Size: The size of each training zone.
- Type of training: What behaviour is trained (Implemented: Basic and Runner. Not implemented: shooter and both (shooter and runner). The _basic_ training overrides the number of enemies and objectives. The _runner_ aims to train a player which evades enemies while getting objectives. The _shooter_ aims to train a player which accurately shoots moving targets. _both_ aims to train a player which has both behaviors simultaneously).
- Seconds per generation: How many seconds each of the generations will last before evaluating the fitness.
- Mutation value: Each generation, the fittest players will pass on their neurons to their offspring, which will be mutated (their weights and bias, which have values from -1 to 1) with a value from _-mutationValue_ to _+mutationValue_.
- Enemy, objective, player and wall prefab: slots to put the prefabs of each element, so as not be limited to one type of enemy or reward/objective, for example.
<br><br><br>

*What comes after this?*
<br>
I intend the classes I developed in this process (the neural network and manager, mainly) to be the backbone of a future project, which will aim to simulate a typical environment, containing plants, preys and predators, both of which will evolve "naturally".
