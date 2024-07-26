using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    public AudioClip audioClip;                         // 재생할 오디오 클립
    public List<Note> notes = new List<Note>();         // 모든 노트의 정보를 담는 리스트
    public float bpm = 120f;                            // 곡의 BPM
    public float speed = 1f;                            // 재생 속도
    public GameObject notePrefabs;                      // 노트 프리팹

    public float audioLatency = 0.1f;                   // 오디오 지연 시간
    public float hitPosition = -8.0f;                   // 노트 판정 위치
    public float noteSpeed = 10;                        // 노트 이동 속도

    private AudioSource audioSource;                    // 오디오 소스 컴포넌트
    private float startTime;                            // 게임 시작 시간
    private List<Note> activeNotes = new List<Note>();  // 아직 생성되지 않은 노트 리스트
    private float spawnOffset;                          // 노드 생성 시간 오프셋

    public bool debugMode = false;                      // 디버그 모드 플래그
    public GameObject hitPositionMarker;                // 판정 위치 마커 오브젝트

    public float initialDelay = 3f;                     // 초기 지연시간


    // 게임 초기화
    public void Initialize()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        startTime = Time.time + initialDelay;               // 시작 시간을 지연 시간만큼 미룸
        activeNotes.Clear();                                // List 사용시 초기와 Clear 해주는 것이 좋음
        activeNotes.AddRange(notes);
        spawnOffset = (10 - hitPosition) / noteSpeed;       // 노트 생성 시간 오프셋 계산

        if (debugMode)
        {
            CreateHitPositionMarker();
        }


        StartCoroutine(StartAudioWithDelay());              // 지연 후 오디오 재생 코루틴 시작
    }



    // 지연 후 오디오 재생
    private IEnumerator StartAudioWithDelay()
    {
        yield return new WaitForSeconds(initialDelay);
        audioSource.Play();
    }

    void Update()
    {
        float currentTime = Time.time - startTime;              // 현재 게임 시간을 계산

        // 활성화된 노트를 처리
        for (int i = activeNotes.Count - 1; i >= 0; i--)        // 뒤에부터 하는 이유는 앞에서 Destroy되면 오류가 나기 때문
        {
            Note note = activeNotes[i];
            
            if (currentTime >= note.startTime - spawnOffset && currentTime < note.startTime + note.duration)
            {
                SpawnNoteObject(note);
                activeNotes.RemoveAt(i);
            }
            else if (currentTime >= note.startTime + note.duration)
            {
                activeNotes.RemoveAt(i);
            }
        }
    }

    // 새로운 노트 추가
    public void AddNote(Note note)
    {
        notes.Add(note);
    }

    // 재생 속도 설정
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    // 노트 오브젝트 생성
    private void SpawnNoteObject(Note note)
    {
        GameObject noteObject = Instantiate(notePrefabs, new Vector3(10, note.trackIndex * 2, 0), Quaternion.identity);
        noteObject.GetComponent<NoteObject>().Initialize(note, noteSpeed, hitPosition, startTime);  // 생성 하면서 데이터 초기화
    }

    // 오디오 지연 시간 설정
    public void AdjustAudioLatency(float latency)
    {
        audioLatency = latency;
    }

    // 디버그 용 판정 위치 마커 생성
    private void CreateHitPositionMarker()
    {
        hitPositionMarker = GameObject.CreatePrimitive(PrimitiveType.Cube);
        hitPositionMarker.transform.position = new Vector3(hitPosition, 0, 0);      // hit 위치로 이동 시킨다.
        hitPositionMarker.transform.localScale = new Vector3(0.1f, 10.0f, 1.0f);    // 스케일 값을 조절하여 선을 만든다
    }
}
