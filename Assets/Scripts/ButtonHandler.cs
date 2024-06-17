using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonHandler : MonoBehaviour
{
    public static bool predictionPointShow;
    public static bool radiusShow;
    public SpriteRenderer predictionPoint;
    public SpriteRenderer radius;

    [Header("Audio Manager")]
    public AudioManager audioManager;
    public void buttonEasy()
    {
        predictionPoint.enabled = true;
        radius.enabled = true;
        audioManager.PlaySFX(audioManager.click);
    }
    public void buttonNormal()
    {
        predictionPoint.enabled = true;
        radius.enabled = false;
        audioManager.PlaySFX(audioManager.click);
    }
    public void buttonHard()
    {
        predictionPoint.enabled = false;
        radius.enabled = false;
        audioManager.PlaySFX(audioManager.click);
    }
    public void buttonPlay()
    {
        predictionPointShow = predictionPoint.enabled;
        radiusShow = radius.enabled;
        audioManager.PlaySFX(audioManager.click);
        SceneManager.LoadSceneAsync(1);
    }
}
