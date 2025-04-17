using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class OpenaiTextToSpeechManager : Singleton<OpenaiTextToSpeechManager> {

    private string OPENAI_KEY = "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";

    public async Task<AudioClip> CreateTextToSpeech(string text) {
        var path = $"https://api.openai.com/v1/audio/speech";

        var data = new {
            model = "tts-1",
            input = text,
            voice = "alloy"
        };

        string jsonData = JsonConvert.SerializeObject(data);

        using (UnityWebRequest req = new UnityWebRequest(path, "POST")) {

            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();

            req.SetRequestHeader("Authorization", $"Bearer {OPENAI_KEY}");
            req.SetRequestHeader("Content-Type", "application/json");

            var operation = req.SendWebRequest();
            while (!operation.isDone) {
                await Task.Yield();
            }

            if (req.result != UnityWebRequest.Result.Success) {
                Debug.LogError($"Error: {req.error}\nResponse: {req.downloadHandler.text}");
                return null;
            }

            var audioPath = Path.Combine(Application.persistentDataPath, "speech.mp3");
            File.WriteAllBytes(audioPath, req.downloadHandler.data);

            Debug.Log($"Audio saved to: {audioPath}");

            using (UnityWebRequest audioRequest = UnityWebRequestMultimedia.GetAudioClip($"file://{audioPath}", AudioType.MPEG)) {
                var audioOperation = audioRequest.SendWebRequest();
                while (!audioOperation.isDone) {
                    await Task.Yield();
                }

                if (audioRequest.result != UnityWebRequest.Result.Success) {
                    Debug.LogError($"Failed to load audio: {audioRequest.error}");
                    return null;
                }

                AudioClip clip = DownloadHandlerAudioClip.GetContent(audioRequest);

                return clip;
            }
        }
    }
}
