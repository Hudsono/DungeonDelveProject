using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class RPGStat
{
    [Tooltip("The full name of the statistic.")]
    public string statisticName;

    [Tooltip("What the statistic is about.")]
    public string statisticDescription;

    [Tooltip("The short-hand statistic name.")]
    public string statisticNickname;

    [Tooltip("The current XP gained for this statistic.")]
    public int xpProgress = 0;

    [Tooltip("How much XP is needed to level up per level. Time (X) = level; value (Y) = XP required to gain that level.")]
    public AnimationCurve xpMaxCurve;

    [Tooltip("The current level of this statistic.")]
    public int statisticLevel = 1;

    [SerializeField]
    [Tooltip("UI Slider representing XP progress.")]
    private Slider m_slider;

    [Tooltip("Should the player be unable to advance beyond the maximum level defined in the Max Curve?")]
    public bool capLevel = false;

    public RPGStat()
    {
        // Set the slider to match the starting level and XP requirement.
        if (m_slider != null)
        {
            m_slider.minValue = 0;
            m_slider.maxValue = xpMaxCurve.Evaluate(statisticLevel);
            m_slider.value = xpProgress;
        }
    }

    /// <summary>
    /// Add XP to this statistic.
    /// </summary>
    /// <param name="amount">The amount of XP to add.</param>
    public void AddXP(int amount)
    {
        // If we're at the maximum level and the statistic is capped at the maximum, ignore positive XP.
        // We determine the maximum level from the Max Curve: the last keyframe's time (as a key's time = level; a key's value = XP required for that level).
        if (amount > 0 && capLevel && statisticLevel == xpMaxCurve.keys[xpMaxCurve.length - 1].time)
            return;
        if (amount < 0 && statisticLevel == 0)
            return;

        // Add the given experience.
        xpProgress += amount;

        // Check if this statistic is eligible to advance one level, based on the XP required for this level defined in the curve.
        if (xpProgress >= (int)xpMaxCurve.Evaluate(statisticLevel))
            LevelUp();
        else if (xpProgress < 0)
            LevelDown();

        // Update UI Slider, if specified.
        if (m_slider != null)
            m_slider.value = xpProgress;

        // Sanitise the XP amount if at/over the maximum level (if capped), or 
        if ((capLevel && statisticLevel >= xpMaxCurve.keys[xpMaxCurve.length - 1].time) || (statisticLevel <= 0))
        {
            xpProgress = 0;
            m_slider.value = xpProgress;
        }
    }

    /// <summary>
    /// Level up this statistic.
    /// </summary>
    public void LevelUp()
    {
        if (statisticLevel >= xpMaxCurve.keys[xpMaxCurve.length - 1].time && capLevel)
            return;

        // Remove the XP used for this level.
        xpProgress -= (int)xpMaxCurve.Evaluate(statisticLevel);
        
        // Advance this statistic's level by one.
        statisticLevel++;

        // Keep recursively checking if the statistic is eligible to level up again.
        if (xpProgress >= (int)xpMaxCurve.Evaluate(statisticLevel))
            LevelUp();

        // Update slider maximum to match the new XP requirement for this level.
        m_slider.maxValue = xpMaxCurve.Evaluate(statisticLevel);

        // Feedback.
        Debug.Log(statisticName + " level up! (" + (statisticLevel - 1) + " -> " + statisticLevel + "). Next level's XP: " + xpMaxCurve.Evaluate(statisticLevel) + "XP.");
    }

    public void LevelDown()
    {
        if (statisticLevel <= 0)
            return;

        // De-level this statistic by one.
        statisticLevel--;

        // Add the negative XP to the maximum XP of this level.
        xpProgress = (int)xpMaxCurve.Evaluate(statisticLevel) + xpProgress;

        // Keep recursively checking if the statistic is eligible to level down again.
        if (xpProgress < 0)
            LevelDown();

        // Update slider maximum to match new XP requirement for this level.
        m_slider.maxValue = xpMaxCurve.Evaluate(statisticLevel);

        // Feedback.
        Debug.Log(statisticName + " level down! (" + statisticLevel + " <- " + (statisticLevel + 1) + "). Next level's XP: " + xpMaxCurve.Evaluate(statisticLevel) + "XP.");
    }
}
