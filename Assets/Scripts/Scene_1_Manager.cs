using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using kgh.Signals;

public class Scene_1_Manager : MonoBehaviour
{
    DataIO dataIO;
    NoteData noteData;
    public AudioSource audioSource;
    Switch audioLoaded, playAudio;
    float delay = 3.0f;
    bool once = true;

    void Awake(){
        noteData = new NoteData();
        dataIO = new DataIO(noteData);
    }
    void Start(){
        string path = Application.streamingAssetsPath + "/Resources/Audio/" + GameManager.instance.selectedSong + "/" + GameManager.instance.selectedSong;
        dataIO.Load(path + "_data.txt");
        
        audioSource.clip = NAudioPlayer.FromMp3Data(File.ReadAllBytes(path + ".mp3"));
        audioLoaded = GameManager.instance.sigs.Register("audio_loaded", typeof(Action<NoteData>));
        playAudio = GameManager.instance.sigs.Register("play_audio", typeof(Action));
        StartCoroutine(StartDelay());
    }
    void Update(){
        if(once){
            audioLoaded.Invoke(noteData);
            once = false;
        }
    }
    IEnumerator StartDelay(){
        yield return new WaitForSeconds(delay);
        audioSource.Play();
    }
}
