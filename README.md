# ComboSplitter
A simple Beat Saber mod that replaces the current combo panel with a modified one that counts your combo for **each** hand instead of both.

## Dependencies:
- BSIPA: v4.2.0+
- SiraUtil: v2.5.7+
- BeatSaberMarkupLanguage: v1.5.4+

## Current Featureset:
- Counts combo for each hand instead of both.
- Follows the current saber color scheme (can be disabled in settings).
- Resets combo back to 0 on either a bad cut or bomb hit based on which saber hit said note.
- Sticking your head inside of a wall resets **both** hands back to 0.

## Roadpath:
- Split the combo breaking animation to only affect the numbers instead of the entire panel.
- Make the object names actually correspond to which side they're on (this one's more internal and has no effect on user).
- Patch the end results screen to show max combo for each hand and/or both hands.
- Add support for multiplayer.
