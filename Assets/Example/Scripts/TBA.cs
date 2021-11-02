using UnityEngine;

[ExecuteInEditMode]
public class TBA : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log(Platform + " Awake");
        gameObject.AddComponent<NPC>();
        Debug.Log("Fuck 2");
    }

    private void Update()
    {
        
    }

    private void OnDestroy()
    {
        Debug.Log(Platform + " OnDestroy");
    }

    private string Platform
    {
        get { return Application.isPlaying ? "Playing" : "Editor"; }
    }
}