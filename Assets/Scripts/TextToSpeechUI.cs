using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextToSpeechUI : MonoBehaviour {
    [SerializeField]
    private InputField inputField;
    [SerializeField]
    private Button button;
    [SerializeField]
    private AudioSource audioSource;

    private void Start() {
        button.onClick.AddListener(CallTextToSpeech);
    }

    private async void CallTextToSpeech() {
        var text = inputField.text;
        inputField.text = "";
        inputField.enabled = false;
        button.enabled = false;

        AudioClip clip = await OpenaiTextToSpeechManager.Instance.CreateTextToSpeech(text);
        audioSource.clip = clip;
        audioSource.Play();

        inputField.enabled = true;
        button.enabled = true;
    }
}
