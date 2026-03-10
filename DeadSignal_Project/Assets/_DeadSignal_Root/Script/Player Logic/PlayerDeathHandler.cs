using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerDeathHandler : MonoBehaviour
{
    HealthSystem health;
    [SerializeField] string scene = "";

    private void Awake()
    {
        health = GetComponent<HealthSystem>();
    }

    private void OnEnable()
    {
        health.OnDeath += DeathHandler;
    }
    private void OnDisable()
    {
        health.OnDeath -= DeathHandler;
    }

    void DeathHandler()
    {
        SceneManager.LoadScene(scene);
    }
}
