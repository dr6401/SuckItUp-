using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cutscene", menuName = "Cutscenes Creation/Cutscene")]
public class CutsceneSO : ScriptableObject
{
    public enum CutsceneID
    {
        Intro,
        //ChickenToPig,
        //PigToSheep,
        //SheepToCow,
        //CowToChick,
        //ChickToEndless,
    }

    public CutsceneID id;
    public List<string> text;
    public string nextScene;
    public AudioClip music;

}
