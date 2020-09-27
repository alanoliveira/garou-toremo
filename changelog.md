# Changelog

## 0.0.2

- **Auto stops playback after it finishes**  
It was really annoying stopping playback manually all the time, mainly for short playbacks.  

- **Support side change in playback**  
The playback will play the same command regardless you changed side after recording playback.  
In other words, *Power Wave* will not turn into a *Burning Knuckle* anymore.  

- **Improve input history precision**  
To reduce the consume of CPU the program was making a 50ms break in each interaction. It was causing a huge imprecision on displaying input history. This break was reduce to 10ms.  

- Set hp/time/super reset time to 1 second  
Maybe is not necessary reset everything every time, so the reset hp/time/super will run 1 time per second.  

- **Improve error feedback**  
If some error occurs, instead closing the program, now it is showing the error stack trace. It should make troubleshoot easier.  

- **Add transition from `control inverted` to `playback`**  
Now if you accidentally press **record input hotkey** after record some input, you can just press the playback button to play it back and cancel the record command.  

- **Enlarge input history font**
Now it is easier to see  
