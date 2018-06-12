using HoloToolkit.Unity;
using UnityEngine;

public class AudioStartup : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        TextToSpeechManager textToSpeech = base.GetComponent<TextToSpeechManager>();
        textToSpeech.Voice = TextToSpeechVoice.Mark;
        textToSpeech.SpeakText("Drop it like its hot");
    }

    // Update is called once per frame
    void Update()
    {

    }
}

