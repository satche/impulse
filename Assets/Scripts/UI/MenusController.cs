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

    [Tooltip("GameObject responsible for XR UI interactions")]
    public GameObject XRInteractionManager;

    [Tooltip("GameObject responsible for desktop UI interactions")]
    public GameObject desktopInteractionManager;

    void Start()
    {
        ChoseInteractionManager();
        InitSlidersValues();
        SwitchMenu("Main");
    }

    /// <summary>
    /// Choses the interaction manager to use depending on the device (XR or desktop)
    /// </summary>
    private void ChoseInteractionManager()
    {
        if (XRSettings.isDeviceActive)
        {
            desktopInteractionManager.SetActive(false);
            XRInteractionManager.SetActive(true);
        }
        else
        {
            desktopInteractionManager.SetActive(true);
            XRInteractionManager.SetActive(false);
        }
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

    /// <summary>
    /// Initializes the sliders values with the values stored in the PlayerPrefs.
    /// </summary>
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

    /// <summary>
    /// Called when a slider value changes.
    /// </summary>
    /// <param name="slider">The slider that changed</param>
    public void OnSliderChange(Slider slider)
    {
        slider.value = Mathf.Floor(slider.value * 10) / 10;
        UpdateSlider(slider);
    }

    /// <summary>
    /// Updates the value of all the sliders found in the canvas in the PlayerPrefs.
    /// </summary>
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

    /// <summary>
    /// Updates the value of the given slider in the PlayerPrefs.
    /// </summary>
    /// <param name="slider">The slider to update</param>
    private void UpdateSlider(Slider slider)
    {
        float value = slider.value;

        // Update the text
        TextMeshProUGUI textValue = slider.transform.Find("Handle Slide Area/Handle/Slider Value").GetComponent<TextMeshProUGUI>();
        textValue.text = value.ToString();

        PlayerPrefs.SetFloat(slider.name, value);
    }
}
