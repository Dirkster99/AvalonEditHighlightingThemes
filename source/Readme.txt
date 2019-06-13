
Overview
--------

This project shows how syntax highlighting can be integrated into WPF Theming to make both
theme coloring functions look like a seemless application. The project demos WPF themes:

- Dark
- Light
- True Blue (Light)
- True Blue (Dark)

which implement MLib based theming with a black or white background. Each of these WPF themes
is associated with a dark/light highlighting theme defined via xshd resources in the
themeable highlighting manager:

HL.Manager.ThemedHighlightingManager

This means we cannot only use dark/light WPF themes but also concert their application in
conjunction with a matching highlighting definition for a given text file.

- Dark  -> Dark  C# highlighting, Dark  Java Highlighting ...
- Light -> Light C# highlighting, Light Java Highlighting ...

Original Source
---------------

This project originates from this source:
https://github.com/Dirkster99/MLib/tree/master/source/00_DemoTemplates

This solution is intended as a template for developing (demo) applications
that use MLib or related components.

The solution is empty but does already support:
- Dark and Light Theming
- Is ready to Save/Load Application settings (requires activation in code)

based on MVVM pattern.
