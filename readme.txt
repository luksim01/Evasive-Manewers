Evasive Manewers

Controls:
	W to move forward.
	S to move backward.
	A to move left.
	D to move right.
	Left Shift to jump.
	Space to bark a move command, causing nearby sheep to flee.
	(Space can also be used to scare wolves hunting sheep away and to add a stray sheep into
	the herd).
	Tab to bark a jump command, causing herd to perform a staggered jump.	

Development Environment:
	Developed on MacBook Air, Apple M1.
	Developed using Unity with editor version 2022.3.9f1.
	Build Settings:
		Target Platform:	macOS
		Architecture:		Intel 64-bit + Apple silicon

Biggest Challenge:
	Leaderboard. The leaderboard required self study into how to create data persistence
	between the main game scene and the leaderboard scene, followed by data retention in
	a .json file to be reused when loading the leaderboard again. Adding functionality to
	use a new player leaderboard input, comparing against existing rankings, and reordering
	the rankings in the leaderboard and data retention file required long and tedious
	testing to check how different scores behaved in the leaderboard ranking and I would
	like to develop my skills in automating testing in future projects for similar
	scenarios.

Area of Effort:
	Encounters and encounter sequence. I tried to add a variety of encounters during the
	chase to ensure it is exciting and unique. I have five different encounters:
	1. Obstacle that herd and player must avoid, 
	2. Long obstacle that herd and player must jump over,
	3. Wolf hunting the player that the player must avoid,
	4. Wolf hunting the sheep that the player must interrupt,
	5. Stray sheep crossing the trail that the player recruit into their herd.
	The frequency of encounters also required careful consideration as too slow would be too
	easy and boring, but too fast and it is hard to manage and feels unfair.

References:
	I used elements of code from the below tutorials to help with data persistence and
	retention in the leaderboard script (Assets/Scripts/MemoryManager.cs).
	"Implement data persistence between scenes"
	https://learn.unity.com/tutorial/implement-data-persistence-between-scenes#
	"Implement data persistence between sessions"
	https://learn.unity.com/tutorial/implement-data-persistence-between-sessions#

Testing: GitHub and Discord webhook.
