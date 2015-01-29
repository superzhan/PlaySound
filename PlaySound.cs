using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Play sound. 
/// 声音播放控制 组件
/// </summary>
public class PlaySound : MonoBehaviour {
	
	
	public bool is_mute=false;
	
	private int channelCount;
	private Object[] clips;
	GameObject SoundPlayer;
	private List<AudioSource> _channels; //普通声道组
	List<bool> channels_pause;
	
	//频繁使用的音效 使用这个声道组
	private int frequence_count=10;
	List<AudioSource> frequence_chan_list;
	List<bool> frequence_chan_pause;
	
	//背景音乐
	AudioSource bg_audio;
    public  	bool is_bg_playing=false;
	
	private     bool is_fightui_bg=false;
	
	
	static Hashtable ClipTable;
	
	private static PlaySound _instance=null;
	public static PlaySound Instance {
		get {
			if (!_instance) {
				_instance = (PlaySound)GameObject.FindObjectOfType(typeof(PlaySound));
				if (!_instance) {
					GameObject container = new GameObject();
					DontDestroyOnLoad(container);
					container.name = "SoundPlayer";
					_instance = container.AddComponent(typeof(PlaySound)) as PlaySound;
					_instance.Init();
				}
			}
			return _instance;
		}
	}
	
	void Init(){
		
		//加载音效 并建立哈希表
		ClipTable=new Hashtable();
		clips = Resources.LoadAll("Audio");
		for(int k=0;k<clips.Length;++k) 
		{
			ClipTable.Add(clips[k].name,clips[k]);
		}
		
		
		_channels = new List<AudioSource>();
		channels_pause=new List<bool>();
		channelCount = 10;
		for (int i = 0; i < channelCount; i++)
        {
            _channels.Add(this.gameObject.AddComponent<AudioSource>());
			channels_pause.Add(false);
        }
		
		frequence_chan_list=new List<AudioSource>();
		frequence_chan_pause=new List<bool>();
		for(int j=0;j<frequence_count;++j)
		{
			frequence_chan_list.Add(this.gameObject.AddComponent<AudioSource>());
			frequence_chan_pause.Add(false);
		}
		
		//background audio
		
		bg_audio=this.gameObject.AddComponent<AudioSource>();
		
	}
	
	public void SetMusic(string ClipName)
	{
		
		if(null!=ClipTable[ClipName]){
		     bg_audio.clip=(AudioClip)ClipTable[ClipName];
			 
		}
	}
	
	/// <summary>
	/// Stars the music. 播放背景音乐
	/// </summary>
	public void StarMusic(string ClipName,float vol)
	{
		if(null!=ClipTable[ClipName]){
			
			bg_audio.clip=(AudioClip)ClipTable[ClipName];
			bg_audio.loop=true;
			bg_audio.volume=0.3f;
			bg_audio.playOnAwake=false;
			bg_audio.Play();
			
		    is_bg_playing=true;
		}
	}
	
	public void ResumeMusic()
	{
		is_bg_playing=true;
		bg_audio.volume=0.3f;
		bg_audio.Play();
	}
	
	/// <summary>
	/// Stops the music.停止背景音乐
	/// </summary>
	public void StopMusic()
	{
		is_bg_playing=false;
		bg_audio.Stop();
	}
	
	/// <summary>
	/// Pauses the music. 暂停音乐
	/// </summary>
	public void PauseMusic()
	{
		is_bg_playing=false;
		bg_audio.Pause();
	}
	
	/// <summary>
	/// Shows the U. 控制UI 显示时 背景音乐 的播放
	/// </summary>
	/// <param name='is_ui'>
	/// Is_ui.
	/// </param>
	public void ShowUI(bool is_ui)
	{
		if(is_ui)
		{
			if(is_bg_playing)
			{
				is_fightui_bg=true;
				PauseMusic();
			}
		}else
		{
			if(is_fightui_bg)
			{
				is_fightui_bg=false;
				ResumeMusic();
			}
		}
	}
		
	
	
	/// <summary>
	/// Starts the sound. 播放音效 可以有音效重叠
	/// </summary>
	/// <param name='ClipName'>
	/// Clip name.
	/// </param>
	/// <param name='loop'>
	/// Loop.
	/// </param>
	/// <param name='vol'>
	/// Vol.
	/// </param>
	public void StartSound(string ClipName,bool loop,float vol){
		
		if(is_mute)
		{
			return;
		}
		
		int c = FindChannel();
		if (c == -1)
		{
			return;
		}
		if(null!=ClipTable[ClipName]){
			_channels[c].clip = (AudioClip)ClipTable[ClipName];
	        _channels[c].loop = loop;
	        _channels[c].volume = vol;
			_channels[c].playOnAwake = false;
			_channels[c].Play();
		}
	}
	
	
	public void StopAllSound()
	{
		for (int i =0; i<channelCount;i++)
		{
            if (_channels[i] != null)
            {
                _channels[i].Stop();
            }
		}
	}
	
	public void StopSound(string ClipName)
	{
		int no=-1;
		//find clip
		try{
			 for (int i =0; i<channelCount;i++)
			{
	            if (_channels[i] != null)
	            {
	                if (_channels[i].clip.name.CompareTo(ClipName)==0) 
					{
						no=i;
						break;
					}
	            }
			}
		
		}catch{
			
			Debug.LogError("no clip");
			return;
		}	
			
		if(no<0 || no>=channelCount)
		{
			return;
		}else
		{
			_channels[no].Stop();
		}
		
		//stop clip
	}
	
	/// <summary>
	/// Starts the sound. 播放音效 设置音效是否重叠
	/// </summary>
	/// <param name='ClipName'>
	/// Clip name.
	/// </param>
	/// <param name='loop'>
	/// Loop.
	/// </param>
	/// <param name='vol'>
	/// Vol.
	/// </param>
	/// <param name='multi'>
	/// Multi.
	/// </param>
	public void StartSound(string ClipName,bool loop,float vol,bool multi){
		
		if(is_mute)
		{
			return ;
		}
		
		bool s = true;
		if(!multi){
			for (int i =0; i<channelCount;i++)
			{
	            if (_channels[i] != null)
	            {
	                if (_channels[i].clip == (AudioClip)ClipTable[ClipName]){
						s = false;
					}
	            }
			}
		}
		if(s){
			int c = FindChannel();
			if (c == -1)
			{
				return;
			}
			if(null!=ClipTable[ClipName]){
				_channels[c].clip = (AudioClip)ClipTable[ClipName];
		        _channels[c].loop = loop;
		        _channels[c].volume =vol;
				_channels[c].playOnAwake = false;
				_channels[c].Play();
			}
		}
	}
	
	
	
	/// <summary>
	/// Starts the sound fre. 
	/// 播放使用频繁的音效
	/// </summary>
	/// <param name='ClipName'>
	/// Clip name.
	/// </param>
	/// <param name='vol'>
	/// Vol.
	/// </param>
	public void StartSoundFre(string ClipName,float vol)
	{
		if(is_mute)
		{
			return;
		}
		
		//find enpty channel
		int empty_no=0;
		for(int i=0;i<frequence_count;++i)
		{
			if( frequence_chan_list[i]!=null)
			{
				if(!frequence_chan_list[i].isPlaying)
				{
					empty_no=i;
					break;
				}
			}
		}
		
			//Find Clip
			AudioClip clip=(AudioClip)ClipTable[ClipName];
			//play
			if(null!=clip){
				frequence_chan_list[empty_no].clip = clip;
	        	frequence_chan_list[empty_no].loop = false;
	        	frequence_chan_list[empty_no].volume =vol;
				frequence_chan_list[empty_no].playOnAwake = false;
				frequence_chan_list[empty_no].Play();
			}
		
	}
	
	/// <summary>
	/// Turns the on sound. 开启音效
	/// </summary>
	public void TurnOnSound()
	{
		is_mute=false;
		
		for(int i=0;i<_channels.Count;++i)
		{
			if(null!=_channels[i])
			{
				_channels[i].mute=false;
			}
		}
		
		for(int j=0;j<frequence_chan_list.Count;++j)
		{
			if(null!=frequence_chan_list[j])
			{
				frequence_chan_list[j].mute=false;
			}
		}
	}
	
	/// <summary>
	/// Turns the off sound. 静音
	/// </summary>
	public void TurnOffSound()
	{
		is_mute=true;
		
		for(int i=0;i<_channels.Count;++i)
		{
			if(null!=_channels[i])
			{
				_channels[i].mute=true;
			}
		}
		
		for(int j=0;j<frequence_chan_list.Count;++j)
		{
			if(null!=frequence_chan_list[j])
			{
				frequence_chan_list[j].mute=true;
			}
		}
	}
	
	
	//暂停
	public void Pause()
	{
		for(int i=0;i<_channels.Count;++i)
		{
			if(null!=_channels[i] && _channels[i].isPlaying==true)
			{
				_channels[i].Pause();
				channels_pause[i]=true;
			}
		}
		
		for(int j=0;j<frequence_chan_list.Count;++j)
		{
			if(null!=frequence_chan_list[j] && _channels[j].isPlaying==true)
			{
				frequence_chan_list[j].Pause();
				frequence_chan_pause[j]=true;
			}
		}
		
		bg_audio.Pause();
	}
	
	//恢复播放
	public void Resume()
	{
		for(int i=0;i<_channels.Count;++i)
		{
			if(null!=_channels[i] && channels_pause[i])
			{
				_channels[i].Play();
				channels_pause[i]=false;
			}
		}
		
		
		if(bg_audio.isPlaying==false)
		{
			if(is_bg_playing)
			{
				bg_audio.Play();
			}
		}
		
		
//		for(int j=0;j<frequence_chan_list.Count;++j)
//		{
//			if(null!=frequence_chan_list[j] && frequence_chan_pause[j])
//			{
//				frequence_chan_list[j].Play();
//				frequence_chan_pause[j]=false;
//			}
//		}
	}
	
	
	
	/// <summary>
	/// Finds the channel.
	/// </summary>
	/// <returns>
	/// The channel.
	/// </returns>
	int FindChannel ()
	{
	    for (int i =0; i<channelCount;i++)
		{
            if (_channels[i] != null)
            {
                if (!_channels[i].isPlaying) return i;
            }
		}
		return -1;
	}
	
}
