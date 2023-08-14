using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    int currentHealth;
    public int maxHealth;
    public GameObject blood;

    public float timer = 1.0f;
    public bool timerStarted = false;

    void Awake()
    {
        currentHealth = maxHealth;
        blood.SetActive(false);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        blood.SetActive(true);
        StartTimer();

        if (currentHealth <= 0)
        { Death(); }
    }

    void Death()
    {
        // Death function
        // TEMPORARY: Destroy Object
        Destroy(gameObject);
    }

    private void Update()
    {
        if (timerStarted)
        {
            timer -= Time.deltaTime;

            if (timer <= 0.0f)
            {
                timer = 0.0f;
                blood.SetActive(false);
                timerStarted = false; // Stop the timer
                timer = 1f;
            }
        }
    }

    public void StartTimer()
    {
        timerStarted = true;
    }
}
