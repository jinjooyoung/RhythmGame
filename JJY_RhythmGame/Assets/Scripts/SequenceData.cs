using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;              // Json 라이브러리를 가져온다.
using UnityEditor;

[CreateAssetMenu(fileName = "NewSequence", menuName = "Sequencer/Sequence")]        // 생성 파일 메뉴에 추가 시켜준다.
public class SequenceData : ScriptableObject
{
    public int bpm;                                                 // 음악의 BPM
    public int numberOfTracks;                                      // 노트 트랙 수
    public AudioClip audioClip;                                     // 오디오 클립
    public List<List<int>> trackNotes = new List<List<int>>();      // 노트 데이터 정보
    public TextAsset trackJsonFile;                                 // .json 파일 텍스트 에셋

    public void SaveToJson()
    {
        if (trackJsonFile == null)
        {
            Debug.LogError("Track JSON 파일이 없습니다.");
            return;
        }

        var data = JsonConvert.SerializeObject(new
        {
            bpm,
            numberOfTracks,
            audioClipPath = AssetDatabase.GetAssetPath(audioClip),
            trackNotes
        }, Formatting.Indented);            // JSON 변환 및 파일 포맷 설정

        System.IO.File.WriteAllText(AssetDatabase.GetAssetPath(trackJsonFile), data);       // 파일에 JSON을 쓴다
        AssetDatabase.Refresh();                                                            // 완료후 리프레시
    }

    
    public void LoadFromJson()
    {
        if (trackJsonFile == null)
        {
            Debug.LogError("Track JSON 파일이 없습니다.");
            return;
        }

        var data = JsonConvert.DeserializeAnonymousType(trackJsonFile.text, new
        {
            bpm = 0,
            numberOfTracks = 0,
            AudioClipPath = "",
            trackNotes = new List<List<int>>()
        });                                     // JSON은 복호화하여 받아온다. data에 할당해준다.

        bpm = data.bpm;
        numberOfTracks = data.numberOfTracks;
        audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(data.AudioClipPath);
        trackNotes = data.trackNotes;
    }
}
