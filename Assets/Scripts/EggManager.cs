using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class EggManager : MonoBehaviour {

    public Transform prefab;

    GestureRecognizer recognizer;

    private void Start()
    {
        recognizer = new GestureRecognizer();
        recognizer.SetRecognizableGestures(GestureSettings.Tap);
        recognizer.TappedEvent += Recognizer_TappedEvent;
        recognizer.StartCapturingGestures();
    }

    private void OnDestroy()
    {
        recognizer.TappedEvent -= Recognizer_TappedEvent;
    }

    private void Recognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        var instance = Instantiate(this.prefab);

        instance.gameObject.transform.position =
          GazeManager.Instance.GazeOrigin +
          GazeManager.Instance.GazeNormal * 1.5f;
    }
}
