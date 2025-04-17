using OpenAI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class ChatCompletionUI : MonoBehaviour {
    [SerializeField]
    private InputField inputField;
    [SerializeField]
    private Button button;
    [SerializeField]
    private ScrollRect scroll;

    [SerializeField]
    private RectTransform sent;
    [SerializeField]
    private RectTransform received;

    private float height;

    private void Start() {
        button.onClick.AddListener(Request);
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

    private void Request() {
        var newMessage = new ChatMessage() {
            Role = "user",
            Content = inputField.text
        };

        AppendMessage(newMessage);

        button.enabled = false;
        inputField.text = "";
        inputField.enabled = false;

        OpenaiChatCompletionManager.Instance.SendRequest(newMessage, (response) => {
            AppendMessage(response);
            button.enabled = true;
            inputField.enabled = true;
        });
    }
}
