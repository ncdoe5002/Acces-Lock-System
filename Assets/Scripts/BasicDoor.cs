using System;
using UnityEngine;

public enum DoorState { LOCKED, OPEN };
public class BasicDoor : MonoBehaviour
{
    private DoorState doorState;
    [SerializeField] private LockDevice lockDevice;

    void Start()
    {
        doorState = DoorState.LOCKED;
        lockDevice.onUnlockSystem += Unlock;
        lockDevice.onErrorSystem += Lock;
    }
    private void Unlock(object sender, EventArgs e)
    {
        Debug.Log("Unlocked the door");
    }
    private void Lock(object sender, EventArgs e)
    {
        Debug.Log("Locked the door");
    }
}
