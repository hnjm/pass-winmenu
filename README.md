# pass-winmenu
dmenu-inspired Windows interface for pass

# Usage

Press Ctrl-Alt-P to open the password menu.

## Configuration

On first run the application will generate a pass-winmenu.yaml file 
(containing all its settings initialised to their default value) in its current directory. 
Edit the file and restart the application to apply the changes.

## Known issues

- The "always on top" feature does not yet work. 
  If the window loses focus, you'll have to click it in order for it to react to input again.
- Opening the menu seems to occasionally crash it after it has been running for a while.