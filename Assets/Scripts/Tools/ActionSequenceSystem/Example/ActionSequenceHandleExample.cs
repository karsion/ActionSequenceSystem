using UnityEngine;
using UnrealM;

public class ActionSequenceHandleExample : MonoBehaviour
{
    private readonly ActionSequenceHandle infiniteSequenceHandle = new ActionSequenceHandle();

    private void Start()
    {
        //Notes：An instance must be preserved to manually stop an infinite loop sequence.
        ActionSequenceSystem.Looper(infiniteSequenceHandle, 0.2f, -1, false, () => Debug.Log("No id infinite looper"));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            infiniteSequenceHandle.StopSequence();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            transform.Looper(infiniteSequenceHandle, 1, 5, false, i =>
            {
                if (i == 2)
                {
                    infiniteSequenceHandle.StopSequence();
                }
            });
        }
    }
}