using OpenAI;
using Samples.Whisper;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class SpeechToTextUI : MonoBehaviour {
    [SerializeField]
    private Button recordButton;
    [SerializeField]
    private Image progressBar;
    [SerializeField]
    private Text message;
    [SerializeField]
    private Dropdown dropdown;

    private void Start() {
        foreach (var device in Microphone.devices) {
            dropdown.options.Add(new Dropdown.OptionData(device));
        }
        recordButton.onClick.AddListener(CallSpeechToText);
        dropdown.onValueChanged.AddListener(ChangeMicrophone);

        var index = PlayerPrefs.GetInt("user-mic-device-index");
        dropdown.SetValueWithoutNotify(index);
    }

    private void ChangeMicrophone(int index) {
        PlayerPrefs.SetInt("user-mic-device-index", index);
        OpenaiSpeechToTextManager.Instance.microphone_name = dropdown.options[index].text;
    }

    private void CallSpeechToText() {
        recordButton.enabled = false;
        OpenaiSpeechToTextManager.Instance.StartRecording();
    }

    private void Update() {
        var stt = OpenaiSpeechToTextManager.Instance;
        if (stt.isRecording) {
            var ratio = stt.time / stt.duration;
            ratio = Mathf.Clamp(ratio, 0, 1);
            progressBar.fillAmount = ratio;
        } else {
            if (!recordButton.enabled) {
                progressBar.fillAmount = 0;
                message.text = stt.message;
                recordButton.enabled = true;
            }
        }
    }
}
