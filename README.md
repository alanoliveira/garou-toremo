# Garou Training Mode Hack


## IMPORTANT

**This is a hack that changes Garou process memory.**  
**I don't know what are the policies of Steam / SNK / Code Mystics about it.**  
**USE IT BY YOUR OWN RISK.**  

## About

Video demonstration https://www.youtube.com/watch?v=1CelZkTmVqc  
This is a tool that enable some training features in Garou Mark of the Wolves Steam edition.  
The main features are:

  - Display input history
  - Record/Playback inputs
  - Reset position

## Download

### Self Contained
- X86: https://github.com/alanoliveira/garou-toremo/releases/download/latest/GarouToremo-self-contained-x86.exe (31.83 MB)
- X64: https://github.com/alanoliveira/garou-toremo/releases/download/latest/GarouToremo-self-contained-x64.exe (36.46 MB)

### .Net Framework Dependent
- X86: https://github.com/alanoliveira/garou-toremo/releases/download/latest/GarouToremo-x86.exe (2.37 MB)
- X64: https://github.com/alanoliveira/garou-toremo/releases/download/latest/GarouToremo-x64.exe (2.69 MB)
=======

## How to use

Open Garou, choose **Normal Mode** > **Versus** > **Choose the characters** > **Choose the handicap**.  
After the match starts right click on **garou-toremo.exe** then **Run as administrator**.  
In the **garou-toremo** window press `2 <enter>`, select the device you will use to activate the cheats, then set the hotkeys.  
That's it.  
\* The joystick support is very poor, so it might not work with your.


## Features

### Command history display

To show the command the program constantly check the state of current pressed button. As the **garou-toremo** process is not synchronized with **Garou** process the results might be unprecise sometimes.

### Reset Position

After you set the **hotkey** for **reset position** and **set custom position**, you can do the following:

- Press **save custom position hotkey** to save your current position
- Press **reset position hotkey** to reset position to the previously saved position
- Press **reset position hotkey + down** to reset the position to the center
- Press **reset position hotkey + left** to reset the position to the left of the screen
- Press **reset position hotkey + right** to reset the position to the right of the screen

### Input Record and Playback

In the **garou-toremo** window you can press 3 to select the slot from 0-9. Each slot can record 1 minute of inputs.  

The workflow of the record and playback is the following:  
  - Press **record input hotkey** first time to invert P1 and P2 controller
  - Press **record input hotkey** second time to start record the inputs
  - Press **record input hotkey** third time to stop record the inputs
After you recorded:  
  - Press **playback input hotkey** first time to start the playback
  - Press **playback input hotkey** second time to stop the playback

## Backlog

  - I will do
    - Add cheat to toggle the TOP
  - Maybe
    - Improve joystick support
    - Display hitboxes
    - Save / Load config
    - Improve command history visual
  - If I win in the lottery
    - Wakeup macros
    
## Powered by

- **Memory.dll**: https://github.com/erfg12/memory.dll
- **GameOverlay.Net**: https://github.com/michel-pi/GameOverlay.Net
- **SharpDX.DirectInput**: https://github.com/sharpdx/SharpDX

