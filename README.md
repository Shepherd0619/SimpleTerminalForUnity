# SimpleTerminal
This is a template project helps gamedev who wish to implement retro DOS console feature in their game project.
The project also enable developer to create custom command and accept args as parameters.

# How to run
Head to `Scenes/TerminalScene.unity` and Play.

# How to create custom command
Head to `_Projects/Scripts/BaseCommand.cs` and `_Projects/Scripts/Interfaces/ICommand` for more information.

# Limitation
- Currently the system is using `Input.inputString`. This works for Desktop and I haven't tested that on devices does not have a physical keyboard.
You may consider modifying `HandleInput` function in `SimpleTerminal.cs` to suit your needs. A possible idea is to use a InputField.
- The cursor may not be 100% identical to the one in the real console. Normally, when you use arrow key to move cursor, the cursor symbol (which is `_`) should be under the character.
Right now, it's `c_lear`.

> [!NOTE]
> This project uses [MorePerfectDOS VGA](https://laemeur.sdf.org/fonts/) font for achieving retro visual. A huge thanks to the creator.
