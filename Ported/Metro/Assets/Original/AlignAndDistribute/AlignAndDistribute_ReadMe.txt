////////////////////////////////////
Align + Distribute Panel for Unity3D
////////////////////////////////////
By DcTurner

Thanks for downloading this Unity extension, I hope you find it useful. If you have any questions, bugs, comments, please send them to mrdcturner@gmail.com, or find me on twitter (@DcTurner).

----------
How to use
Once the package is included in your Unity project, click Window > Layout Panel. When you have more than 1 GameObject selected, the panel will become active.

UPDATE TO V1.1
Thanks to some great user feedback, I have made the tool usable via the Editor menu bar. Under GameObject, you will find two new options;
1. "Align" with sub-menu options based on the axis you are aligning to.

and

2. "Distribute" with sub-menu options for the axis you wish to distribute along.
----------
Extras
I have also included a set of simple C# extensions that I use to  make positioning objects faster. To use these extensions, please include this line at the top of your C# class;

using WorkFast;

This will allow you to use the following methods on GameObject and Transform variables.

setX(float newXValue, bool useLocalPositioning);
setZ(float newYValue, bool useLocalPositioning);
setZ(float newZValue, bool useLocalPositioning);

Example usage
gameObject.setX(100); // moves the gameObject to x:100
transform.setX(200, true); // moves the transform to x:200 in local space
-----------
That's it!

Keep an eye out for updates and new tools from me :)
Thanks again for your download :)

DcTurner