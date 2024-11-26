Mini Project hand-in for
Programmering af interaktive 3D verdener

Project Name: Spore Civilization stage but worse
Name: Alexander Frederik Johansen
Student Number: 20234892
Link to Project: https://github.com/ajohan23/spore_civilization_stage_but_worse 

Overview of the game:
This game is an rts that takes inspiration from Spore and specifically the civilization stage. The player takes control of the blue nation and must build spice towers to earn money to fund tanks and boats needed to attack and defend against other cities. The player commands their units and moves the camera around the playing field with the mouse. The game features 14 computer-controlled nations that all fight for power against the player and each other.
The mains parts of the game:
  •	Camera – top down perspective and can be moved with the mouse.
  •	Tanks and boats – the main way the player interacts with the world. Can be selected and given orders by right and left clicking respectively
  •	Cities – can be taken over with vehicles, produces money and can construct vehicles
  •	Spice geysers – can be claimed with vehicles and generate money
  •	Money – the player and computer nations start with money to purchase vehicles. More money is earned throughout the game
  •	World – split into 3 continents with water in between where only boats can fare
Game features:
  •	14 AI nations who responds to threats and seeks to expand their borders
  •	Two types of terrain and two vehicle classes to traverse them
  •	Economy system that requires the player to expand

How Parts of the Course were Utilized:
The script that controls the camera uses affine transformations to move and zoom. A raycast is used to select and give orders to cites and vehicles. Vehicles use OverlapSphere to detect nearby enemies. Vehicles fire rockets that use the unity physics engine to detect what they hit. All vehicles navigate using NavMesh that is split in two to have land and water surfaces that only allow the matching vehicle type to travel there. The world was built with the terrain tool used to form the land mass of the different continents while a simple cube is used as the sea. The terrain is painted with different textures to give a sense of different landscape types. Particles were used to represent spice geysers, rockets and explosions with different materials to create a spice cloud and fire effect. The rockets and explosions use a script to play a sound clip randomly selected from a predefined list when they spawn. The buttons for buying tanks and boats use a shader made in shader graph. They also have an icon made with a billboard. The vehicles are represented by an imported tank model and boat model respectively. The materials on the cities and vehicles change color to represent the nation they belong to. The ui consists of a single element of text that displays how much money the player currently has and is updated continuously. The color of the element also changes to match the color of the nation the player plays as.

Project Parts:
  •	Scripts
    o	AINationController – gives orders the vehicles of each computer-controlled nation
    o	BoatButton – inherits from PhysicalButton, used to buy a boat in a city
    o	ButtonBillboard – rotates the button icons to always face the camera
    o	CameraController – used for moving and zooming the camera
    o	CarButton – inherits from PhysicalButton, used to buy a car in a city
    o	City – tracks the health of a city and what nation controls it
    o	Health – an interface used by VehicleController to track health and handle death
    o	MoneyDisplayer – updates ui elements to show how much money the player has
    o	Nation – tracks what cities and vehicles belon to the nation and how much money the nation has
    o	NationsManager – stores each nation instance and provides methos for accessing them
    o	Order – interface and implementations for the different types of orders that can be given to vehicles, like move orders and attack orders
    o	PhysicalButton  - an abstract class used to generalize the interaction with the Car- and BoatButton scripts
    o	RandomSound – chooses a random sound from a list on awake and plays it
    o	Rocket – controls the flight path of the rocket and keeps track of the target and who shot the rocket
    o	Selectable – interface used for objects that can be selected i.e. vehicle and cities
    o	Selector – takes input from the mouse buttons and selects vehicles and cities with left click and issues orders to the selected vehicle on right click depending on what was clicked on
    o	SpiceGeyser – controls whether it is built or not, who owns it and provides the owner with money
    o	VehicleController –the vehicle around to execute orders and attack nearby enemies
  •	Models
    o	Boat model from https://www.fab.com/listings/95c49644-7f73-4b26-af85-992c52acf92d 
    o	Tank model from https://www.fab.com/listings/fa6f1c5a-86ba-42c8-a817-804a11e0ff8a 
    o	spice geysers and cities made with primitives
  •	Materials
    o	Dirt, Grass, Water, Rock and Sand materials from https://ambientcg.com/ 
    o	Cites and spice geyser basic unity materials
  •	Scenes
    o	The game consists of one scene
  •	Testing
    o	The game is tested on Windows
  •	Sounds
    o	Explosion sounds - https://freesound.org/people/Selector/sounds/250200/ and https://freesound.org/people/Mr%20Sensible/sounds/14742/ 

Time Management
Task:	Time spent (in hours)
Setting up Unity, making a project in GitHub:	1
Research and conceptualization of game idea:	1.5
Searching for 3D models for boats and tanks:	0.5
Searching for materials for terrain and water:	1
Making Terrain and water:	2
Making camera movement controls: 1
Coding selection system:	3
Coding order system:	8
Coding and debugging ai nations:	8
Building city prefab:	0.5
Building vehicle prefabs:	0.5
Making UI elements with TextMesh Pro:	0.5
Creating particle effects for spice geysers, rockets and explosions:	1
Coding sound randomizer for explosions:	0.5
Hologram button shader:	1
Coding rocket logic:	0.5
Building NavMesh:	2
Code documentation:	1.5
Placing cities and geysers:	1.5
Making readme:	0.5
Total:	36

Used Resources
•	Hologram effect tutorial -  https://www.youtube.com/watch?v=KGGB5LFEejg 
