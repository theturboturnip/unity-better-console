# BetterConsole
This is a Unity project demoing my BetterConsole asset, which aims to make the Unity console less of a debug storage facility and more of an actual command console.

It currently supports arbitrary commands that can be registered at runtime, and intercepts Debug.Log/LogWarning/LogError calls so it can be smoothly integrated into existing code without a lot of find/replace. It currently features command and argument autocomplete, with scene tree navigation. 

In the future I plan to add automatic string enclosure for arguments with spaces, similar to path autocomplete in a real console; make the GUI use monospaced font; and make the GUI wrap horizontal overflow.
