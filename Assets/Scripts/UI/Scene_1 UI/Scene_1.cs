using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using kgh.Signals;

public class Scene_1 : MonoBehaviour
{
    public static Scene_1 instance;

    public ParticleSystem noteParticles;
    UIDocument uiDocument;
    VisualElement rootVisualElement;
    VisualElement noteDisplay, strings, mainDisplay, other, noteRoute;
    // bool geometryChanged = false;
    public NoteData _noteData{
        private set;
        get;
    }
    public AudioSource audioSource;
    bool start = false;
    List<VisualElement> noteIndicators;
    float visibleAreaSize = 3f;
    public List<Sprite> noteIndicatorSprites;
    float time;
    public float noteDelay = 0f;

    List<(int, float)> notes;

    void OnGeometryChanged(GeometryChangedEvent e){
        noteParticles.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(
            e.newRect.xMin,
            Screen.height - e.newRect.center.y,     //Screen pixel & VisualElement y=0 position is inverted
            0
        ));
    }
    void Awake(){
        instance = this;

        uiDocument = GetComponent<UIDocument>();
        rootVisualElement = uiDocument.rootVisualElement;
        noteDisplay = rootVisualElement.Query<VisualElement>("note_display");
        noteRoute = noteDisplay.Query<VisualElement>("notes");
        noteIndicators = new List<VisualElement>();

        noteDisplay.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        noteDisplay.style.opacity = 0.75f;

        notes = new List<(int, float)>();
    }
    void Start(){
        GameManager.instance.sigs.Subscribe("audio_loaded", this, "AudioLoaded");
        time = -visibleAreaSize;
    }
    async void Update(){
        int visibleIndStart = -1;
        int visibleIndFin = notes.Count - 1;
        bool playParticle = false;
        for(int i = 0; i < notes.Count; i++){
            if(notes[i].Item2 >= time && visibleIndStart == -1){
                visibleIndStart = i;
                if(Mathf.Abs(notes[i].Item2 - time) < 0.01f){
                    playParticle = true;
                }
            }
            if(notes[i].Item2 > time + visibleAreaSize){
                visibleIndFin = i - 1;
                break;
            }
        }


        int cnt = 0;
        if(visibleIndStart == -1){
            Debug.Log("NoMoreNotes!");
            
            if(time >= audioSource.clip.length){
                Debug.Log("GameOver!");
            }
            time += Time.deltaTime;

            while(cnt < noteIndicators.Count){
                noteIndicators[cnt].style.left = -500;
                cnt++;
            }
            return;
        }

        cnt = 0;
        for(int i = visibleIndStart ; i < visibleIndFin + 1; i++){
            VisualElement noteIndicator, noteNumIndicator;
            noteNumIndicator = new VisualElement();
            if(cnt < noteIndicators.Count){
                noteIndicator = noteIndicators[cnt];
                foreach(var item in noteIndicator.Children()){
                    if(item.ClassListContains("note-num-indicator")){
                        noteNumIndicator = (VisualElement)item;
                    }
                }
            }
            else{
                noteIndicator = new VisualElement();
                noteIndicator.AddToClassList("note-indicator");
                noteNumIndicator = new VisualElement();
                noteNumIndicator.AddToClassList("note-num-indicator");
                
                noteIndicator.Add(noteNumIndicator);
                noteRoute.Add(noteIndicator);
                noteIndicators.Add(noteIndicator);
            }
            if(notes[i].Item1 > 5){
                noteIndicator.style.backgroundImage = new StyleBackground(noteIndicatorSprites[12]);
                noteNumIndicator.style.backgroundImage = new StyleBackground(noteIndicatorSprites[notes[i].Item1 + 6]);
            }
            else{
                noteIndicator.style.backgroundImage = new StyleBackground(noteIndicatorSprites[12]);
                noteNumIndicator.style.backgroundImage = new StyleBackground(noteIndicatorSprites[notes[i].Item1]);
            }
            noteIndicator.style.left = noteDisplay.worldBound.width * ((notes[i].Item2 + noteDelay - time) / visibleAreaSize); //노트 생성위치?
            cnt++;
        }
        while(cnt < noteIndicators.Count){
            noteIndicators[cnt].style.left = -500;
            cnt++;
        }
        if(playParticle){
            noteParticles.Clear();
            noteParticles.Play();
            playParticle = false;
        }
        time += Time.deltaTime;
    }
    void AudioLoaded(NoteData noteData){
        _noteData = noteData;
        bool cont = true;
        while(cont){
            int minInd = -1;
            float minVal = float.MaxValue;
            for(int i = 0; i < _noteData.notes.Count; i++){
                if(_noteData.notes[i].Count == 0){
                    continue;
                }            
                if(minVal > _noteData.notes[i][0]){
                    minVal = _noteData.notes[i][0];
                    minInd = i;
                }
            }
            if(minInd == -1){
                break;
            }
            notes.Add((minInd, minVal));
            _noteData.notes[minInd].RemoveAt(0);
        }
    }

    public void Print(string message){
        Label db = rootVisualElement.Query<Label>("Debug");
        db.text = message;
    }
}