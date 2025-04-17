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
    private string prompt = "Jouez le rôle d'un inconnu dans un salon de discussion et répondez aux questions. " +
        "Ne changez pas de personnage. Ne mentionnez jamais que vous êtes un modèle d'IA. " +
        "Déterminer la langue de l'utilisateur et répondre dans cette langue.";

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
