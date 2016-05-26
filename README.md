<b>Purpose</b>

These C# scripts are part of a two-alternative forced-choice categorization of objects task built in Unity3D. This is designed to train players to distinguish categories of objects in a game environment to make the task more engaging for participants and to display 3D objects.

<b>Task Description</b>

The program begins by welcoming the player to the game and requires the subject and session number be entered. This is followed by a confirmation screen for verification of subject and session number entered. 

Once confirmed, the task begins with one object shown as the exemplar for a particular category in the center of the screen. Pairs of objects will then be shown at the top of the screen, one pair at a time. These objects will move from the top of the screen in a straight line to the bottom at a consistent rate of speed while each object randomly rotates around it's own center of mass. After an adjustable amount of time has passed, another pair will be visible at the top of the screen and will traverse toward the bottom. 

If a pair of objects reach the bottom before the player makes a selection as to which of the pair is the same category as the example, all of the objects stop moving towards the bottom (but continues rotating) until the player makes a selection. An adjustable number of pairs (or trials) are presented for each category example (making a block of trials) before the game continues to a new category example (block) with different sets of pairs (trials).

<b>List of Files</b>
<ul>
	<li>Done_DestroyByBoundary.cs</li>
	<li>Done_DestroyByContact.cs</li>
	<li>Done_GameController3.cs</li>
	<li>Done_Mover.cs</li>
	<li>Done_PauseBoundary.cs</li>
	<li>Done_PlayerController2.cs</li>
	<li>Done_RandomRotator.cs</li>
	<li>LoadOnClick.cs</li>
	<li>ObjectPoolerScript.cs</li>
	<li>OutputFile.cs</li>
</ul>
<b>Install procedure</b>
<ul>
	<li>Install Unity3D on computer</li>
	<li>Import/add 3D objects for game</li>
	<li>Each script should be attached to a game object</li>
	<li>Scripts that should have their own invisible game objects: LoadOnClick, Done_GameController3, ObjectPoolerScript, OutputFile</li>
	<li>Scripts that should be attached to the player object: Done_PlayerController2, Done_Mover</li>
	<li>Scripts that should be attached to all of the enemy/target objects: Done_RandomRotator, Done_DestroyByContact</li>
	<li>Scripts that should be attached to the game boundaries: Done_DestroyByBoundary, Done_PauseBoundary</li>
</ul>
<b>How to Use</b>

Options can be set, adjusted and tested in the Unity3D editor. Once all scripts, objects and editor options are set up the game needs to be compiled in Unity3D in order to run as its own executable.

<b>Brief description of each file</b> 
<ul>
	<li>Done_DestroyByBoundary.cs : De-activates game objects leave play area, resets feedback.</li>
	<li>Done_DestroyByContact.cs : Determines what should happen when player object and selected object colliders make contact.</li>
	<li>Done_ GameController3.cs : Loads game, monitors game state, manages game timing and what happens when players run out of time.</li>
	<li>Done_Mover.cs : Controls movement of game objects (that aren’t the player object) by checking game state in the game controller.</li>
	<li>Done_PauseBoundary.cs : Monitors whether or not the game needs to pause for a response or because of an incorrect response.</li>
	<li>Done_PlayerController2.cs : Controls player object movement direction, speed, and position. Also defines functions that alter the player object based on game state.</li>
	<li>Done_RandomRotator.cs : Controls the angle and speed of game object rotation.</li>
	<li>LoadOnClick.cs : Gets and sets session information to be passed from initial prompt screen into game.</li>
	<li>ObjectPoolerScript.cs : Manages pools of game objects. Game objects are stored in asset bundles, which are loaded based on session information gathered in LoadOnClick.cs. One massive pool is created for one instance of all game objects. It then adds/sets required object properties such as attaching the game object scripts and colliders. From the one big pool, smaller pools are formed to list each time a specific game object will be needed as a lure or example. Also has functions for randomly selecting and returning game objects for game play.</li>
	<li>OutputFile.cs : Creates a text file to save game play information. Defines functions for writing to file for game controller to update as game progresses.</li>
</ul>
<b>Features</b>
<ul>
	<li>Space Invader-type arcade game orientation of game objects</li>
	<li>Accuracy feedback: visual and auditory</li>
	<li>If there is an incorrect response, all objects stay in their current locations for an adjustable period of time so the player has additional time to study the remaining correct object compared to the category example</li>
	<li>Each time the player makes a correct response their score increases</li>
	<li>At incremental levels of points the amount of time the task waits to show new pairs are the top of the screen decreases, so over time more stimuli are on the screen at a time</li>
	<li>Adjustable number of breaks where the score and speed are reset back to the starting values</li>
	<li>Adjustable timer for each block, continues to count down while pausing after incorrect response</li>
	<li>If the timer runs out, all remaining pairs are removed from the screen and the next block starts with the new category example, so the player will lose opportunities for points if they run out of time</li>
	<li>Uses object pooling to improve resource management</li>
	<li>Imports game objects using asset bundles</li>
</ul>
<b>Contributors</b>

Many of these scripts started as scripts from the Unity Space Shooter tutorial (link: https://unity3d.com/learn/tutorials/projects/space-shooter-tutorial), which were then altered by varying degrees for the categorization game. This project was made with John Rohrlich and the assistance of Levi Davis and Justin Vallelonga. 
