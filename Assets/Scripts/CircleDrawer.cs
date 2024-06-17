using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleDrawer : MonoBehaviour
{
    public LineRenderer circleRenderer;
    // Start is called before the first frame update
    void Start()
    {
        DrawCircle(100, 10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void DrawCircle(int steps, float radius)
    {
        circleRenderer.positionCount = steps;
        for(int currentStep = 0; currentStep < steps; currentStep++)
        {
            float circumferenceProgress = (float)(currentStep+1) / steps;
            float currentRadian = circumferenceProgress * 2 * Mathf.PI;
            float xScaled = Mathf.Cos(currentRadian);
            float yScaled = Mathf.Cos(currentRadian);
            float x = xScaled * radius;
            float y = yScaled * radius;
            Vector3 currentPosition = new Vector3(x, y, 0);
            circleRenderer.SetPosition(currentStep, currentPosition);
        }
    }
}
