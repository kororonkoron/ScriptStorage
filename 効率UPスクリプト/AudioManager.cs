using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Sound
{
    public enum Type
    {
        BGM,
        SE,
        CLICK_TAP
    };

    public Type type;
    [Tooltip("�T�E���h�̖��O")]
    public string name;
    [Tooltip("�T�E���h�̉���")]
    public AudioClip audio;
    [Tooltip("�T�E���h�̏����{�����[��")]
    [SerializeField, Range(0, 1)]
    public float voluem;
    [Tooltip("ture=���[�v������")]
    public bool isLoop = false;

    [HideInInspector]
    public AudioSource audioSource;
}
public class AudioManager : MonoBehaviour
{
    [SerializeField] bool isSelect = true;
    [SerializeField] private Sound[] sounds;

    public static AudioManager instance;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound sound in sounds)
        {
            sound.audioSource = gameObject.AddComponent<AudioSource>();
            sound.audioSource.clip = sound.audio;
            sound.audioSource.volume = sound.voluem;
        }
    }

	private void Update()
	{
        clickAndTap(isSelect);
    }

	/// <summary>
	/// �N���b�N�E�^�b�v��
	/// </summary>
	void clickAndTap(bool isTake)
    {
        if(!isTake)
		{
            return;
		}
        if (Input.GetMouseButtonDown(0))
        {
            Sound sound = Array.Find(sounds, sound => sound.type == Sound.Type.CLICK_TAP);
            if (sound == null)
            {
                return;
            }
            sound.audioSource.PlayOneShot(sound.audio);
        }
    }

    /// <summary>
    /// �Đ�
    /// </summary>
    /// <param name="name"></param>
    public void play(string name)
    {
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        if (sound == null)
        {
            return;
        }
        sound.audioSource.loop = sound.isLoop;

        switch (sound.type)
        {
            case Sound.Type.BGM:
                sound.audioSource.Play();
                break;

            case Sound.Type.SE:
				sound.audioSource.PlayOneShot(sound.audio);
				break;
        }
    }

    /// <summary>
    /// ��~
    /// </summary>
    /// <param name="name">name=null�������炷�ׂẴT�E���h���~����</param>
    public void stop(string name=null)
    {
        if(name!=null)
		{
            Sound sound = Array.Find(sounds, sound => sound.name == name);
            if (sound == null)
            {
                return;
            }
            sound.audioSource.Stop();
		}
        else
		{
            foreach (Sound sound in sounds)
            {
                sound.audioSource.Stop();
            }
        }
    }

    /// <summary>
    /// �|�[�Y
    /// </summary>
    /// <param name="name"></param>
    public void pause(string name)
	{
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        if(sound==null)
		{
            return;
		}
        sound.audioSource.Pause();
	}

    /// <summary>
    /// �|�[�Y����
    /// </summary>
    /// <param name="name"></param>
    public void  unPausse(string name)
	{
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        if (sound == null)
        {
            return;
        }
        sound.audioSource.UnPause();
    }

    /// <summary>
    /// ���ʒ���
    /// </summary>
    /// <param name="type"></param>
    /// <param name="volume"></param>
    public void changeVoluem(Sound.Type type ,float volume)
	{
        foreach(Sound sound in sounds)
		{
            if(sound!=null)
			{
                switch(sound.type)
				{
                    case Sound.Type.BGM:
                        if(sound.type==type)
						{
                            sound.audioSource.volume = volume;
                            Debug.Log("BGM");
						}
                        break;

                    case Sound.Type.SE:
                        if(sound.type==type)
						{
                            sound.audioSource.volume = volume;
                            Debug.Log("SE");
						}
                        break;

                    case Sound.Type.CLICK_TAP:
                        if (sound.type == type)
                        {
                            sound.audioSource.volume = volume;
                            Debug.Log("SE");
                        }
                        break;
                }
			}
		}
	}
}