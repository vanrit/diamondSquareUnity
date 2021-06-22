using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Text valueText1;
    public Text valueText2;
    public Slider slider1;
    public Slider slider2;
    public InputField inputField;

    public static float valueRoughness = 20;
    public static int valueSize = 254;
    public static int valueSeed;
    public static bool setSeed = false;

    void Start()
    {
        //Обновляем значения при откротии меню
        valueText1.text = valueRoughness.ToString();
        valueText2.text = valueSize.ToString();
        slider1.value = valueRoughness;
        slider2.value = valueSize;
        if (setSeed)
            inputField.text = valueSeed.ToString();

    }

    /// <summary>
    /// Запускаем генерию
    /// </summary>
    public void StartGeneration()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    /// <summary>
    /// Выходим из программы
    /// </summary>
    public void ExitProgram()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    /// <summary>
    /// Устанавливаем значение через слайдер
    /// </summary>
    /// <param name="value">значение полученное со слайдера</param>
    public void SetRoughness(float value)
    {
        valueText1.text = value.ToString();
        valueRoughness = value;
    }

    /// <summary>
    /// Устанавливаем значение через слайдер
    /// </summary>
    /// <param name="value">значение полученное со слайдера</param>
    public void SetSize(float value)
    {
        valueText2.text = value.ToString();
        valueSize = (int)value;
    }

    /// <summary>
    /// Выбираем фулл скрин
    /// </summary>
    /// <param name="isFullScreen">поставлена ли галочка</param>
    public void SetFullScreen(bool isFullScreen)
    {
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        Screen.fullScreen = isFullScreen;
    }

    /// <summary>
    /// Устанавливаем значение через слайдер
    /// </summary>
    /// <param name="value">значение полученное со слайдера</param>
    public void SetSeed(string value)
    {
        if (inputField.text.Length > 0)
        {
            if (int.TryParse(value, out valueSeed))
                setSeed = true;
            else
                setSeed = false;
        }
        else
            setSeed = false;
    }

}
