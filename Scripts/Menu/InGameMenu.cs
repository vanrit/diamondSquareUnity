using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
    //Стоит ли программа на паузе
    public static bool GameIsPaused=false;

    public GameObject pauseMenuUI;

    void Start()
    {
        pauseMenuUI.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        //Проверка нажата ли клавиша вызова меню
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    /// <summary>
    /// Работа программы возобновляется
    /// </summary>
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        GameIsPaused = false;
    }

    /// <summary>
    /// Программа на паузе
    /// </summary>
    void Pause()
    {
        pauseMenuUI.SetActive(true);
        GameIsPaused=true;
    }

    /// <summary>
    /// Загружаем главное меню
    /// </summary>
    public void LoadMenu()
    {
        GameIsPaused = false;
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Выход из программы
    /// </summary>
    public void Quit()
    {
        GameIsPaused = false;
        Debug.Log("Quit");
        Application.Quit();
    }
}
