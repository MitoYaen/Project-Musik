using UnityEngine;

public class FpsLimitter : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = 60;
    }
}
