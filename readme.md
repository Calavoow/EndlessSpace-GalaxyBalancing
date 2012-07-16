Balanced Galaxy Generation Mod
=======================

The goal of this mod is to find a simple solution so that a reasonably balanced galaxy can be generated.

###Current Approach
The current approach is to calculate a certain score for each player and then apply balancing on the result.
This could be in the form of adjust a few planets in the neighbourhood of a player to the regeneration of a
new galaxy till a balanced galaxy is generated.


##Getting Started
To get you started on editing you will first need a C# editor. I used Visual C# 2010 Express which is freely
available at [its site](http://www.microsoft.com/visualstudio/en-us/products/2010-editions/visual-csharp-express).
When you have installed your C# editor, clone the files from this Git repository to your local machine and open
the Amplitude.GalaxyGenerator.sln solution inside the GalaxyGenerator Sources folder. Then you are set to go!

I have built a test harness so that you do not need to compile the .dll and replace the one in your game to 
test your modifications. You can simply run the GalaxyGeneratorTest.cs inside the GalaxyGeneratorTest project.


<hr />
###Permission
I have received permission from AmpliMath (one of the developers of Endless Space) that I could upload the
source publicly to GitHub.