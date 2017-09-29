/**************************************************************************************** 
 * Wakaka Studio 2017
 * Author: iLYuSha Dawa-mumu Wakaka Kocmocovich Kocmocki KocmocA
 * Project: 0escape Medieval - Music Loder
 * Version: Tool Package v1.001a
 * Tools: Unity 5/C# + Arduino/C++
 * Last Updated: 2017/07/29
 ****************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicLoader : MonoBehaviour
{
    public static AudioSource bgAudio;
    public static AudioClip[] clips;
    private static int orderPlay;

    void Awake()
    {
        bgAudio = GetComponent<AudioSource>();
    }

    void Start()
    {
        StartCoroutine(MusicLoading());
    }

    IEnumerator MusicLoading()
    {
        // 資源路徑
        string dPath = Application.streamingAssetsPath + "/Music";
        // 取得該路徑下資源數，篩選wav檔，因為Editor會含有meta的檔案
        string[] files = Directory.GetFileSystemEntries(dPath, "*.wav");
        int count = (int)(files.Length);
        clips = new AudioClip[count];

        for (int i = 0; i < count; i++)
        {
            string sPath = "file://" + files[i];
            WWW www = new WWW(sPath);
            yield return www;
            clips[i] = WWWAudioExtensions.GetAudioClip(www, true, true);
            clips[i].name = files[i].Replace(dPath + "\\", "");
        }
        orderPlay = Random.Range(0, clips.Length);
        bgAudio.clip = clips[orderPlay];
    }

    public static string PlayTrack()
    {
        if (!bgAudio.isPlaying)
        {
            orderPlay++;
            if (orderPlay == clips.Length)
                orderPlay = 0;
            bgAudio.clip = clips[orderPlay];
            bgAudio.Play();
        }
        return clips[orderPlay].name;
    }

    public static string NextTrack()
    {
        orderPlay++;
        if (orderPlay == clips.Length)
            orderPlay = 0;
        bgAudio.clip = clips[orderPlay];
        bgAudio.Play();
        return clips[orderPlay].name;
    }
}
