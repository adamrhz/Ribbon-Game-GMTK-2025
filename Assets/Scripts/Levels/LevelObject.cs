using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Level Object", menuName = "Ribbon/ScriptableObjects/Level Object", order = 51)]
public class LevelObject : ScriptableObject
{
    public string LevelName = "New Level";
    public int LoopCounts = 3;
    public string SceneName = "Level1";//ToUseLater
    public AudioClip MusicTrack;
    public AudioClip FinalLapMusicTrack;
    public int ID; // used for saving since ints are easier to keep track of


    public bool SecretStartPicked = false;

}
