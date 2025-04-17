using Newtonsoft.Json;
using OpenAI;
using Samples.Whisper;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class OpenaiSpeechToTextManager : Singleton<OpenaiSpeechToTextManager> {

    private AudioClip clip;
    public string message;
    private readonly string fileName = "output.wav";

    public bool isRecording = false;
    public float time = 0;

    public int duration = 5;
    private int FREQUENCY = 44100;

    public string microphone_name = string.Empty;

    private OpenAIApi openai = new OpenAIApi("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");

    public void StartRecording() {
        if (isRecording)
            return;

        if (microphone_name == string.Empty) {
            microphone_name = Microphone.devices[0];
        }
            
        clip = Microphone.Start(microphone_name, false, duration, FREQUENCY);
        isRecording = true;
        Debug.Log("start recording...");
    }

    private async void EndRecording() {
        message = "Transcripting...";
        Debug.Log(message);

        byte[] data = SaveWav.Save(fileName, clip);

        var req = new CreateAudioTranscriptionsRequest {
            FileData = new FileData() { Data = data, Name = "audio.wav" },
            //File = Application.persistentDataPath + "/" + fileName,
            Model = "whisper-1",
            //Language = "fr"
        };

        var response = await openai.CreateAudioTranscription(req);
        message = response.Text;
        Debug.Log(message);
        isRecording = false;
        time = 0;
    }

    private void Update() {
        if (isRecording) {
            time += Time.deltaTime;

            if (time >= duration) {
                time = -100;
                EndRecording();
            }
        }
    }
}
