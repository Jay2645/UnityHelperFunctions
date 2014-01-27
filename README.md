# Unity Helper Functions

A bunch of helper functions for Unity, written in C#. Note that this is a work in progress -- although most things have implementations, some only have a skeleton implementation. Full implementations and more complete code documentation will come later.

Contained within:

## Achievement System

*Partially implemented:* Listens to all events and provides a framework for unlocking achievements when certain requirements are met. Override the `Achievement` class to specify your own achievement logic. Eventually planned to allow for an easy hook into the Steamworks SDK. Base classes found in the `Achievements` folder.

## Audio Manager

A way to easily control the volume of various sound effects. Replacement for the `AudioSource` class. Allows you to pass any arbitrary AudioClip to play, get the presently playing clip, and more. The `MusicManager` class additionally allows behavior like shuffling and looping. The `AmbientManager` allows the user to set ambient sounds (for instance, birds chirping in the background) to a separate volume to allow users to easily change volume in the settings menu. Base classes found in the `Audio` folder.

## Clickable Objects

An easy way to make objects clickable. Additionally allows for behaviors like having a context menu open when click or having a description pop-up that opens when the user mouses over the object. Base classes found in the `Clickable` folder.

## Command Pattern

*Partially implemented:* An implementation of the [Command Pattern](http://en.wikipedia.org/wiki/Command_pattern). Allows for delayed execution of actions and an easy way to undo/redo actions. Base classes found in the `Commands` folder.

## Global Constants

A way to manage all your constant values from a single class, so multiple classes can reference the same constant value. Allows for easy notifications for classes inheriting from `Subject` or `Observer`: just pass a string from GlobalConsts. If you wish to change the string, you only need to change it in one file rather than many. Base classes found in the `Consts` folder.

## Debugging

A replacement console that disables itself when you shift from debug to release for improved performance. Also included: an ingame console that can be open with the `~` key, from [the Unity Wiki](http://wiki.unity3d.com/index.php?title=DebugConsole). Classes found in the `Debug` folder.

## Persistent Hash Generation

A way to keep track of objects across loads and even after the user closes and re-opens the game. Classes found in the `Hash` folder.

## Various Helper Classes

An uncategorized array of helpful classes. Among them are:

* `GlobalsBase`, a partial class that serves as the "hub" for many of the other functions found in this package.
* `Bounds2D`, a class attempting to recreate the 3D bounds class for 2D objects.
* `MonoHelper`, a replacement for MonoBehaviour that handles things like JSON serialization and hash generation.
* *Partially implemented:* `RoomSettings`, a way to generate scene-specific settings.

All helper classes can be found in the `Helpers` folder.

## Input Management

*Partially implemented:* A way to deal with cross-platform input. Touchscreen touches easily get converted to mouse movement/clicks and vice versa. Keys can easily be rebound to perform various operations. Classes can be found in the `Input` folder.

## Instancing

An easy way to Instantiate objects. No more do you have to call Resources.Load before you create a new object -- with `InstanceManager`, simply call `InstanceManager.Load(path)` and it will automatically load and instantiate an object for you. InstanceManager can also convert objects to and from JSON and preserve them across scene loads. Classes can be found in the `Instancing` folder.

## JSON Serialization

An easy way to read and write JSON objects, [based off of SimpleJSON from the Unity Wiki](http://wiki.unity3d.com/index.php/SimpleJSON). Classes can be found in the `JSON` folder.

## Localization

A simple way to localize strings! By providing an ID and the English version of a string, a JSON file is created holding all the English versions of strings. If the language is not set to English, the system will attempt to look up the localization file for that language and return the proper string based off of the provided ID.

For "classic" string behavior, nothing will be written to file if the provided ID is empty (""). If the ID is empty, any provided "hard-coded" text will be displayed. Files can be found in the `Localization` folder.

## Lua

Easy Unity Lua integration, based on [KopiLua](https://github.com/gfoot/kopilua). Any method in the `LuaBinding` class or things that inherit from it can be accessed via Lua. Additionally, `LuaBinding` extends the `Observer` class, allowing for the Observer Pattern to call functions in Lua classes. Classes can be found in the `Lua` folder.

## Mods

An easy way to allow your users to create mods for your game. Works with the JSON system to load arbitrary files and execute the contents within. Classes can be found in the `Mods` folder.

## Observer Pattern

This is an implementation of the [Observer Pattern](http://en.wikipedia.org/wiki/Observer_pattern) that allows for events to propagate down to other systems. The base classes can be found in the `Observers` folder.

## Saving

*Partially implemented:* A way to easily save/load the state of your game using JSON. Can also perform "autosaves" in addition to save filenames specified by the user. Can be found in the `Saving` folder.

## Text

A way to display text to the user. Said text exists in 3D space and can be manipulated by the developer. Uses [TextMeshController](http://forum.unity3d.com/threads/32227-3D-Text-Wrap) from the Unity Forum. Can be found in the `Text` folder.

## Various Visual Helper Classes

* `CameraFade`: From the [Unity Wiki](http://wiki.unity3d.com/index.php?title=FadeInOut), an easy way to fade the camera in and out.
* `MessageBox`: A way to display dialog boxes with arbitrary text to the user.

Classes found in the `Visual` folder.

# Forthcoming:

* Pause Menu: A way to quickly pause and unpause the game.
* Settings Menu: An easy way for the user to change game settings.
* Sprite Generator: Easily converts .png files into sprites at runtime.
* Photoshop Helper: A way to make a "mockup" in Photoshop that can be easily imported into Unity.
* 2D Animation Helper: A way to generate 2D animations on-the-fly via code, allowing users to specify a list of .png files to load and have the output be an animated sprite.
