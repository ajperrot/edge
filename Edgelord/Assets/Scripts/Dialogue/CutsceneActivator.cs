using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneActivator : MonoBehaviour
{
    public GameObject Cutscene; //the cutscene to activate
    public SerializableIntDictionary ScenePerDay = new SerializableIntDictionary(); //which scene to play each day

    private TextLog Log; //the log which writes the scene 

    // Start is called before the first frame update
    void Start()
    {
        Log = Cutscene.GetComponent<TextLog>();
    }

    // Update is called once per frame
    void Update()
    {
        //activate the scene for today if one exists
        if(ScenePerDay.ContainsKey(Setting.currentDay))
        {
            Log.sceneNumber = ScenePerDay[Setting.currentDay];
            Cutscene.SetActive(true);
        }
        //disable self no matter what
        this.enabled = false;
    }
}
