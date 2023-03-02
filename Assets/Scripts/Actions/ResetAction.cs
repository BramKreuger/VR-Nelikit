using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ResetAction : MonoBehaviour
{
    public InputActionReference resetReference = null;

    private void Awake()
    {
        resetReference.action.started += ResetActionStarted;
    }

    private void OnDestroy()
    {
        resetReference.action.started -= ResetActionStarted;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void ResetActionStarted(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene(0);
    }
}
