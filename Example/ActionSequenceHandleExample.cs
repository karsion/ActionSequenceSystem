using UnityEngine;
using UnrealM;

public class ActionSequenceHandleExample : MonoBehaviour
{
    private readonly ActionSequenceHandle infiniteSequenceHandle = new ActionSequenceHandle();

    private void Start()
    {
        //Notes：An instance must be preserved to manually stop an infinite loop sequence.
        //this.LooperUnscaled(infiniteSequenceHandle, 1f, () => Debug.Log("No id infinite looper"));
        infiniteSequenceHandle.Looper(2f, () => transform.Rotate(15, 0, 0));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            infiniteSequenceHandle.StopSequence();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            transform.LooperUnscaled(infiniteSequenceHandle, 1, 5, false, i =>
            {
                Debug.Log("transform looper " + i);
                if (i == 2)
                {
                    infiniteSequenceHandle.StopSequence();
                }
            });
        }
    }

    //private void OnDestroy()
    //{
        //ActionSequenceSystem.StopAll();
    //}
}