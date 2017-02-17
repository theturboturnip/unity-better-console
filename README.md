# BetterConsole
This is a Unity project demoing my BetterConsole asset, which aims to make the Unity console less of a debug storage facility and more of an actual command console.

It currently supports arbitrary commands that can be registered at runtime, and intercepts Debug.Log/LogWarning/LogError calls so it can be smoothly integrated into existing code without a lot of find/replace. It features command and argument autocomplete, with scene tree navigation; builtin commands to get variables, set variables and call functions; optional and params arguments; fully commented parsing/autocomplete code.

It's pretty much done now, and the rest of the work I'm going to do on it now is adding more comments. New features might be added, but don't count on it.
