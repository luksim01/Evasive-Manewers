Treverse

Controls:
	W to move forward.
	S to move backward.
	A to rotate left.
	D to rotate right.
	I to interact with outlined objects.
	P to enter/exit drone camera view.

Development Environment:
	Developed on MacBook Air, Apple M1.
	Developed using Unity with editor version 2023.1.11f1.
	Build Settings:
		Target Platform:	macOS
		Architecture:		Intel 64-bit + Apple silicon

Biggest Challenge:
	Interactive object outline. Required handling of the outline material that was added to
	an object when in the player's interactive line of sight range and radius. This object
	could be destroyed while outlined and the sequence of events needed careful handling
	to prevent errors of accessing an object's properties after it was destroyed.
	Self study was also required for glowing shader creation and shader property
	manipulation to create a glowing outline effect.

Area of Effort:
	Water manager. To optimise the exploration of the large map, a pooled water tile
	coordination system was created. I developed this system to be configurable to an NxN
	grid of water tiles, loading the tiles around the player and coordinating the tiles
	furthest behind the player's movement to positions furthest in front of the player's
	movement. With enough water tiles and with water tiles that were large enough, popping
	couldn't be seen in front of the player, creating an endless sea effect. The water tiles
	used Perlin noise to offset vertices of the mesh to create waves, this type of noise
	allowed water tiles to be placed side by side to create a smooth transition between
	tiles. 
	A generic buoyant point was created that affected the parent object it was assigned to
	based on the wave offsets of the water tiles. This added bounce and rotation to the 
	parent object creating a floating effect that was applied to the kayak and reused for any 
	other floating objects such as bottles, buoys, and energy connectors.

References:
	I used elements of code from below tutorial to help with development of a generic buoyant 
	point script (Assets/Scripts/Movement/BuoyantPoint.cs - FindTurbulenceUsing() method).
	"How to Set Up Dynamic Water Physics and Boat Movement in Unity | Ship Buoyancy Tutorial"
	https://www.youtube.com/watch?v=eL_zHQEju8s 
