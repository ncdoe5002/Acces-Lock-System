using UnityEngine;
using TMPro;
using System.Collections;
using System;
using UnityEngine.EventSystems;

public class LockDevice : MonoBehaviour
{
    [SerializeField] private TMP_Text displayText;    // Reference to the on-screen display text (PIN output or status)
    [SerializeField] private int pinLength;           // Length of the PIN code (e.g., 4-digit or 6-digit)
    [SerializeField] private string pincode;             // The actual PIN code to be matched
    [SerializeField] private float resetTime;         // Time before the lock resets after entry
    [SerializeField] private char emptyDisplayChar;
    [SerializeField] private DialKey[] dialKeys;      // Reference to all dial pad keys in the scene

    private char[] inputPIN;             // Stores the current user input
    private int digitPointer;
    private bool shouldTakeInput;   // Prevents input after entry or during reset

    //Addition Functionality
    public Transform greenLight;
    public Transform redLight;
    private Transform currentLight;

    //Events that allows specific action on a specific outcome
    public event EventHandler onUnlockSystem;
    public event EventHandler onErrorSystem;

    private void Start()
    {
        Reset();
    }
    private void Update()
    {
        // Check and handle dial interaction every frame
        HandleDialingSystem();
    }

    // Main logic for handling input from dial pad or keypad
    private void HandleDialingSystem()
    {
        if (!shouldTakeInput) return;     // Do nothing if lock is currently disabled

        Ray fireRay = Camera.main.ScreenPointToRay(Input.mousePosition);   // Ray from camera to mouse
        RaycastHit hitInfo;

        // Detect mouse click and perform raycast to identify hit object
        if (Input.GetMouseButtonDown(0) && Physics.Raycast(fireRay, out hitInfo))
        {
            DialKey selectedKey = hitInfo.collider.gameObject.GetComponent<DialKey>();   // Try to get DialKey component
            if (selectedKey == null) return;     // Not a valid key

            if (selectedKey.getKeyVal() == 'e')
            {
                if (new string(inputPIN) == pincode)
                {
                    onUnlockSystem?.Invoke(this, EventArgs.Empty);
                    currentLight = greenLight;

                    displayText.text = "SUCCESS";
                }
                else
                {
                    onErrorSystem?.Invoke(this, EventArgs.Empty);
                    currentLight = redLight;
                    displayText.text = "ERROR";
                }

                StartCoroutine(ResetLockCoroutine());
                return;
            }
            else if (selectedKey.getKeyVal() == 'x')
            {
                digitPointer -= digitPointer == 0 ? 0 : 1;
                inputPIN[digitPointer] = emptyDisplayChar == '\0' ? '0' : emptyDisplayChar;
                displayText.text = new string(inputPIN);
                return;
            }

            if (digitPointer == pinLength) return;
            inputPIN[digitPointer] = selectedKey.getKeyVal();
            displayText.text = new string(inputPIN);
            digitPointer++;

        }
    }
    private void Reset()
    {
        digitPointer = 0;     // Set starting digit (e.g., 1000 for 4-digit PIN)
        displayText.text = emptyDisplayChar == '\0' ?
            new string('0', pinLength) : new string(emptyDisplayChar, pinLength);

        inputPIN = new char[pinLength];

        for (int i = 0; i < pinLength; i++)
            inputPIN[i] = emptyDisplayChar == '\0' ? '0' : emptyDisplayChar;

        shouldTakeInput = true;
    }
    // Coroutine to reset the lock system after a delay
    private IEnumerator ResetLockCoroutine()
    {
        currentLight.gameObject.SetActive(true);
        shouldTakeInput = false;

        yield return new WaitForSeconds(resetTime);

        currentLight.gameObject.SetActive(false);
        Reset();
    }
}
