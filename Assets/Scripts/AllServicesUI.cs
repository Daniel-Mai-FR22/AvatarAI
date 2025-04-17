using OpenAI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class AllServicesUI : MonoBehaviour {

    [SerializeField]
    private Dropdown dropdown;
    [SerializeField]
    private Image progressBar;
    [SerializeField]
    private Button recordButton;

    [SerializeField]
    private ScrollRect scroll;

    [SerializeField]
    private RectTransform sent;
    [SerializeField]
    private RectTransform received;

    private float height;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private Toggle toggle;
    [SerializeField]
    private GameObject messageArea;

    private void Start() {
        foreach (var device in Microphone.devices) {
            dropdown.options.Add(new Dropdown.OptionData(device));
        }
        recordButton.onClick.AddListener(CallSpeechToText);
        dropdown.onValueChanged.AddListener(ChangeMicrophone);

        var index = PlayerPrefs.GetInt("user-mic-device-index");
        dropdown.SetValueWithoutNotify(index);

        toggle.onValueChanged.AddListener(ShowChat);
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
                recordButton.enabled = true;
                CallChatCompletion(stt.message);
            }
        }
    }

    private void AppendMessage(ChatMessage message) {
        scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

        var item = Instantiate(message.Role == "user" ? sent : received, scroll.content);
        item.GetChild(0).GetChild(0).GetComponent<Text>().text = message.Content;
        item.anchoredPosition = new Vector2(0, -height);
        LayoutRebuilder.ForceRebuildLayoutImmediate(item);
        height += item.sizeDelta.y;
        scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        scroll.verticalNormalizedPosition = 0;
    }

    private void CallChatCompletion(string message) {
        var newMessage = new ChatMessage() {
            Role = "user",
            Content = message
        };

        AppendMessage(newMessage);

        OpenaiChatCompletionManager.Instance.SendRequest(newMessage, async (response) => {
            AppendMessage(response);
            AudioClip clip = await OpenaiTextToSpeechManager.Instance.CreateTextToSpeech(response.Content);
            audioSource.clip = clip;
            audioSource.Play();
        });
    }

    private void ShowChat(bool value) {
        messageArea.SetActive(value);
    }
}
