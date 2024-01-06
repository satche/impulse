using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class MenusController : MonoBehaviour
{
    [Tooltip("All menus in the game")]
    public List<Canvas> menus;

    private XRController xrController;

    private void Awake()
    {
        xrController = GetComponent<XRController>();
    }

    void Start()
    {
        InitSlidersValues();
        SwitchMenu("Main");
    }

    private void Update()
    {
    }

    /// <summary>
    /// Switches to the menu at the given index.
    /// </summary>
    /// <param name="name">The name of the menu to switch to</param>
    public void SwitchMenu(string name)
    {
        UpdateSliders();
        foreach (Canvas menu in menus)
        {
            menu.enabled = false;
            if (menu.name == name) { menu.enabled = true; }
        }
    }

    /// <summary>
    /// Open the pause menu
    /// </summary>
    public void OpenPauseMenu(bool isPaused)
    {
        Canvas menu = menus.Find(m => m.name == "Pause Menu");
        menu.enabled = isPaused;

        // Update the sliders
        foreach (Slider slider in menu.GetComponentsInChildren<Slider>())
        {
            UpdateSlider(slider);
        }
    }

    /// <summary>
    /// Loads the scene with the given name.
    /// </summary>
    /// <param name="sceneName">Name of the scene</param>
    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Quits the game.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    private void InitSlidersValues()
    {
        foreach (Canvas menu in menus)
        {
            foreach (Slider slider in menu.GetComponentsInChildren<Slider>())
            {
                slider.value = PlayerPrefs.GetFloat(slider.name, slider.value);
            }
        }
    }

    public void OnSliderChange(Slider slider)
    {
        slider.value = Mathf.Floor(slider.value * 10) / 10;
        UpdateSlider(slider);
    }

    public void UpdateSliders()
    {
        foreach (Canvas menu in menus)
        {
            foreach (Slider slider in menu.GetComponentsInChildren<Slider>())
            {
                UpdateSlider(slider);
            }
        }
    }

    private void UpdateSlider(Slider slider)
    {
        float value = slider.value;

        // Update the text
        TextMeshProUGUI textValue = slider.transform.Find("Handle Slide Area/Handle/Slider Value").GetComponent<TextMeshProUGUI>();
        textValue.text = value.ToString();

        PlayerPrefs.SetFloat(slider.name, value);
    }
}
