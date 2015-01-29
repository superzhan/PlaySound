# PlaySound
unity3d 声音播放控制插件

Quick Start
  1.PlaySound开发目的在于使2D 游戏的声音控制模块更加容易使用。
  2.把所有要使用的的音频文件放到Resources/Audio 文件夹（若没有，就新建一个）。音频文件设置为2D 音频。
  3.代码运行会自动加载该文件夹下的音频，然后调用相应的接口就可以实现音频的控制。
  
  example:
  播放背景音乐
  PlaySound.Instance.StarMusic("luffy",0.3f);
