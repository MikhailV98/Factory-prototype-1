using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] RectTransform saveSlotsPanel;      //  Панель кнопок выбора профиля
    [SerializeField] GameObject profileButtonPrefab;    //  Префаб кнопки
    [SerializeField] Button removeProfileButton;        //  Кнопка удаления профиля
    UIButtonSelection[] buttonsSelectionsArr;
    private void Start()
    {
        SaveSystem.LoadSettings();
        buttonsSelectionsArr = new UIButtonSelection[SaveSystem.player_save_paths.Length];
        //  Выбираем актуальный слот
        GenerateSaveSlotButtons();
    }
    //  Save slots management
    void GenerateSaveSlotButtons()
    {
        for (int i = 0; i < SaveSystem.player_save_paths.Count(); i++)
        {
            int slotIndex = i;
            GameObject buttonObj = Instantiate(profileButtonPrefab, saveSlotsPanel);
            buttonsSelectionsArr[i] = buttonObj.GetComponent<UIButtonSelection>();
            if (File.Exists(SaveSystem.GetActualPathOfProfileWithNumber(i)))
            {
                //  save slot exists
                buttonsSelectionsArr[i].SetText("Save " + (i + 1));
                buttonObj.GetComponent<Button>().onClick.AddListener(() => SelectSaveSlotWithNumber(slotIndex));
            }
            else
            {
                buttonsSelectionsArr[i].SetText("Empty Slot " + (i + 1));
                buttonObj.GetComponent<Button>().onClick.AddListener(() => SelectSaveSlotWithNumber(slotIndex));
            }
        }
        SelectActualSaveSlot();
    }
    void SelectActualSaveSlot()
    {
        if (File.Exists(SaveSystem.GetActualPathOfCurrentProfile()))
            SelectSaveSlotWithNumber(SaveSystem.CurrentPlayerProfileNumber);
    }
    void RemoveSaveSlotButtons()
    {
        foreach (RectTransform child in saveSlotsPanel)
            Destroy(child.gameObject);
    }
    void UpdateSaveSlotButtons()
    {
        RemoveSaveSlotButtons();
        GenerateSaveSlotButtons();
    }
    void SelectSaveSlotWithNumber(int saveSlotNumber)
    {
        //  Indicate slot
        ClearSaveSlotIndication();
        buttonsSelectionsArr[saveSlotNumber].IndicationON();
        //  Tell SaveSystem current slot number
        SaveSystem.CurrentPlayerProfileNumber = saveSlotNumber;
        removeProfileButton.interactable = true;
    }
    void ClearSaveSlotIndication()
    {
        foreach (UIButtonSelection button in buttonsSelectionsArr)
            button.IndicationOFF();
        removeProfileButton.interactable = false;
    }

    //  Menu Buttons
    public void OpenGamePlayScene()
    {
        SaveSystem.SaveSettings();
        SceneManager.LoadScene(1);
    }
    public void RemoveCurrentProfile()
    {
        string filePath = SaveSystem.GetActualPathOfCurrentProfile();
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            //  Update buttons
            UpdateSaveSlotButtons();
        }
        else
            Debug.LogError("You are trying to delete unexisting profile");
    }
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
