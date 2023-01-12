using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class DialogueController : MonoBehaviour
{
    public static event Action onDialogueStart;
    public static event Action onDialogueEnd;
    public static event Action onVideoPlay;

    // Объекты для хранения информации о диалоге
    [SerializeField] private GameObject dialoguePanel = null;
    [SerializeField] private TMP_Text tmp_speakerName = null;
    [SerializeField] private TMP_Text tmp_dialogueLine = null;
    [SerializeField] private TMP_Text tmp_dialogueHint = null; // Подсказка для продолжения диалога
    [SerializeField] private Image portrait = null;
    [SerializeField] private Dialogue currentDialogue = null;
    [SerializeField] private float timeOfLetterShowing = 0.01f;
    [SerializeField] private KeyCode keyToContinueDialogue = KeyCode.F;

    private int _currentDialogueLineIndex = -1;
    private bool isDialogueActive = false;
    private bool isPanelInitialized = false;
    private AudioManager audioManager = null;

    public int CurrentDialogueLineIndex { get { return _currentDialogueLineIndex; } }

    private void Start()
    {   
        InitializeDialoguePanel();
        tmp_dialogueHint.text = "Нажмите " + keyToContinueDialogue.ToString() + ", чтобы продолжить";
        audioManager = FindObjectOfType<AudioManager>();
    }

    void OnEnable()
    {
        // подписка на событие для начала диалога
        DialogueArea.OnEnteringDialogue += StartDialogue;
    }

    void OnDisable()
    {
        DialogueArea.OnEnteringDialogue -= StartDialogue;
    }

    private void Update() 
    {
        if (isDialogueActive)
        {
            // при нажатии клавиши - следующая строчка диалога
            if (Input.GetKeyUp(keyToContinueDialogue))
            {
                NextDialogueLine();
            }
        }
    }

    private void InitializeDialoguePanel()
    {   
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(Dialogue dialogue)
    {
        ActivateDialoguePanel();
        currentDialogue = dialogue;
        // -1 - чтобы диалог начинался с первой строчки
        _currentDialogueLineIndex = -1;
        onDialogueStart?.Invoke();
        NextDialogueLine();
    }

    public void ActivateDialoguePanel()
    {
        dialoguePanel.SetActive(true);
        isDialogueActive = true;
    }

    public void DeactivateDialoguePanel()
    {
        dialoguePanel.SetActive(false);
        isDialogueActive = false;
    }

    // Загрузка следующей строчки диалога
    private void NextDialogueLine()
    {
        _currentDialogueLineIndex++;
        DialogueLine dialogueLine = currentDialogue.GetDialogueLine
            (
            _currentDialogueLineIndex
            );

        if (dialogueLine == null)
        {
            onDialogueEnd?.Invoke();
            DeactivateDialoguePanel();
            return;
        }

        StopAllCoroutines();
        tmp_speakerName.text = dialogueLine.speakerData.speakerName;
        if (dialogueLine.voiceText != null)
        {
            audioManager.audioSource.clip = dialogueLine.voiceText;
            audioManager.audioSource.Play();
        }
        StartCoroutine(TextVisualisation(dialogueLine.text));
        portrait.sprite = dialogueLine.speakerData.portrait;
        portrait.color = dialogueLine.speakerData.color;
    }

    private IEnumerator TextVisualisation(string text)
    {
        tmp_dialogueLine.text = "";
        foreach (var letter in text)
        {
            tmp_dialogueLine.text += letter;
            yield return new WaitForSeconds(timeOfLetterShowing);
        }
    }
}
