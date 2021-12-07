using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleManager : MonoBehaviour
{
    private static TimeScaleManager timeScaleManager;

    float unscaledTimeSinceChangedTimescale = 0;
    float targetTimeScale = 1f;
    float scaleDuration = 1f;
    public float defaultTimeScale = 1f;
    void Awake()
    {
        timeScaleManager = this;
    }
    public static TimeScaleManager GetInstance()
    {
        return timeScaleManager;
    }

    public void SetTimeScale(float newTimeScale, float duration)
    {
        targetTimeScale = newTimeScale;
        scaleDuration = duration;
        unscaledTimeSinceChangedTimescale = 0;
    }
	private void Update()
	{
        unscaledTimeSinceChangedTimescale += Time.unscaledDeltaTime;
        float f = Mathf.Clamp01(unscaledTimeSinceChangedTimescale / scaleDuration);
        Time.timeScale = Mathf.Lerp(targetTimeScale, defaultTimeScale, unscaledTimeSinceChangedTimescale);
	}
}
