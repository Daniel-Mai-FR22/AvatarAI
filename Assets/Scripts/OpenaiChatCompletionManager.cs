using OpenAI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class OpenaiChatCompletionManager : Singleton<OpenaiChatCompletionManager> {

    private OpenAIApi openai = new OpenAIApi("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");

    private List<ChatMessage> listMessages = new List<ChatMessage>();
    private string prompt = "Jouez le r�le d'un inconnu dans un salon de discussion et r�pondez aux questions. " +
        "Ne changez pas de personnage. Ne mentionnez jamais que vous �tes un mod�le d'IA. " +
        "D�terminer la langue de l'utilisateur et r�pondre dans cette langue.";

    public async void SendRequest(ChatMessage message, Action<ChatMessage> callback) {

        if (listMessages.Count == 0) {
            var firstMessage = new ChatMessage() {
                Role = "user",
                Content = prompt
            };
            listMessages.Add(firstMessage);

            //message.Content = prompt + "\n" + message.Content;
        }

        listMessages.Add(message);

        // Complete the instruction
        var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest() {
            Model = "gpt-4o-mini",
            Messages = listMessages
        });

        var messageResponse = new ChatMessage();

        if (completionResponse.Choices != null && completionResponse.Choices.Count > 0) {
            messageResponse = completionResponse.Choices[0].Message;
            messageResponse.Content = messageResponse.Content.Trim();

            listMessages.Add(messageResponse);
        } else {
            Debug.LogWarning("No text was generated from this prompt.");
        }

        callback.Invoke(messageResponse);
    }
}
