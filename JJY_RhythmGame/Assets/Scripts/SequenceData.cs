using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;              // Json ���̺귯���� �����´�.
using UnityEditor;

[CreateAssetMenu(fileName = "NewSequence", menuName = "Sequencer/Sequence")]        // ���� ���� �޴��� �߰� �����ش�.
public class SequenceData : ScriptableObject
{
    public int bpm;                                                 // ������ BPM
    public int numberOfTracks;                                      // ��Ʈ Ʈ�� ��
    public AudioClip audioClip;                                     // ����� Ŭ��
    public List<List<int>> trackNotes = new List<List<int>>();      // ��Ʈ ������ ����
    public TextAsset trackJsonFile;                                 // .json ���� �ؽ�Ʈ ����

    public void SaveToJson()
    {
        if (trackJsonFile == null)
        {
            Debug.LogError("Track JSON ������ �����ϴ�.");
            return;
        }

        var data = JsonConvert.SerializeObject(new
        {
            bpm,
            numberOfTracks,
            audioClipPath = AssetDatabase.GetAssetPath(audioClip),
            trackNotes
        }, Formatting.Indented);            // JSON ��ȯ �� ���� ���� ����

        System.IO.File.WriteAllText(AssetDatabase.GetAssetPath(trackJsonFile), data);       // ���Ͽ� JSON�� ����
        AssetDatabase.Refresh();                                                            // �Ϸ��� ��������
    }

    
    public void LoadFromJson()
    {
        if (trackJsonFile == null)
        {
            Debug.LogError("Track JSON ������ �����ϴ�.");
            return;
        }

        var data = JsonConvert.DeserializeAnonymousType(trackJsonFile.text, new
        {
            bpm = 0,
            numberOfTracks = 0,
            AudioClipPath = "",
            trackNotes = new List<List<int>>()
        });                                     // JSON�� ��ȣȭ�Ͽ� �޾ƿ´�. data�� �Ҵ����ش�.

        bpm = data.bpm;
        numberOfTracks = data.numberOfTracks;
        audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(data.AudioClipPath);
        trackNotes = data.trackNotes;
    }
}
