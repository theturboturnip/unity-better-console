# BetterConsole
This is a Unity project demoing my BetterConsole asset, which aims to make the Unity console less of a debug storage facility and more of an actual command console.

It currently supports arbitrary commands that can be registered at runtime, and intercepts Debug.Log/LogWarning/LogError calls so it can be smoothly integrated into existing code without a lot of find/replace. 

In the future I plan to add autocomplete; Transform, Vector2/3/4 and GameObject argument support (you can set these types as arguments currently but bad things happen when you do) and a better GUIs for Editor/Playmode.
