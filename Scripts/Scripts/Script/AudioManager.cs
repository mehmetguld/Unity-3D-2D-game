using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager: MonoBehaviour
{
    public Sound[] sounds;

    private HeroMove thePlayer;

    private void Awake()
    {
        foreach (var s in sounds)
        {
            thePlayer = FindAnyObjectByType<HeroMove>();
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Stop();
    }

    public void Volume(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.volume -= Time.deltaTime / 5;
        s.source.pitch -= Time.deltaTime / 7;
    }
}
