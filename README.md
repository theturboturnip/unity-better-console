# BetterConsole
This is a Unity project demoing my BetterConsole asset, which aims to make the Unity console less of a debug storage facility and more of an actual command console.

It currently supports arbitrary commands that can be registered at runtime, and intercepts Debug.Log/LogWarning/LogError calls so it can be smoothly integrated into existing code without a lot of find/replace. It currently features command and argument autocomplete, with scene tree navigation; builtin get_var and set_var commands; fully commented parsing/autocomplete code.

In the future I plan to add a few more default commands with reflection, optional arguments, and a toggle for the Console GUI ingame.
