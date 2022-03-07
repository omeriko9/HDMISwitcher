# Monitor Switcher
Code to switch a Monitor's Input Selection (HDMI, DP) via C#

# Background
I have two laptops and 2 monitors.
I used to have a setup where a KVM switched the Keyboard, the Mouse and the first monitor between the two laptops, while the 2nd monitor was connected to both laptops and I switched between them manually using the physical buttons on the monitor for input selection (HDMI1, HDMI2, DP etc'). 

I've found out that HDMI has a software interface exposed by Windows API that allows selecting input, so I obviously looked for software that does that.
The closest thing I've came to find was NirSoft's [ControlMyMonitor](https://www.nirsoft.net/articles/set_monitor_input_source_command_line.html) utility, which allows to do that from the command line. 

But looking for a more elegant and integrated solution, I decided to write the code myself, using C#.

I combined the Monitor Switching code with some MQTT messages to a home automation framework I use, then connected an ESP8266 to a relay and the relay to the KVM switch so the code allows me on double-clicking Scroll Lock to switch both monitors and the keyboard and mouse.

This is of course not necessary for anyone thinking of using the code here, just be aware there's extra code.
