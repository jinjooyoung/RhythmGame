using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SimpleSequenceEditor : EditorWindow
{
    private SequenceData sequenceData;              // ������ ������ ���� ��ü
    private Vector2 scrollPos;                      // ��ũ�� �� ��ġ
    private float beatHedight = 20;                 // �� ��Ʈ�� ����
    private float trackWidth = 50;                  // �� Ʈ���� �ʺ�
    private int totalBeats;                         // �� ��Ʈ ��
    private bool isPlaying = false;                 // ���� ��� ����
    private int currentBeatTime = 0;                // ���� ��� ���� ��Ʈ �ð�
    private int playFromBeat = 0;                   // ����� ������ ��Ʈ
    private float startTime = 0;                    // ��� ���� �ð�
    private AudioSource audioSource;                // ����� ����� ���� AudioSource

    [MenuItem("Tool/Simple Sequence Editor")]
    private static void ShowWindow()
    {
        var window = GetWindow<SimpleSequenceEditor>();
        window.titleContent = new GUIContent("Simple Sequencer");
        window.Show();
    }

    private void OnEnable()                         // ������ ������ â�� ������ ��
    {
        EditorApplication.update += Update;       // ������Ʈ �Լ��� �̺�Ʈ�� ���
        CreateAudioSource();                        // ������� ����� ���̱� ������ ����� ����� ���ش�
    }

    private void OnDisable()                        // ������ ������ â�� �ݾ��� ��
    {
        EditorApplication.update -= Update;       // ������Ʈ �Լ��� �̺�Ʈ ����
        if (audioSource != null)                    // ������ ������ â�� �ݾ��� �� ����� �ҽ��� ������� ���
        {
            DestroyImmediate(audioSource.gameObject);       // ��ϵ� ����� �ҽ� ������Ʈ�� �ı��Ѵ�.
            audioSource = null;
        }
    }


    private void OnGUI()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(200));                        // GUI ���̾ƿ� ���� ����
        EditorGUILayout.LabelField("������ ������ ���� ", EditorStyles.boldLabel);  // ��Ʈ

        sequenceData = (SequenceData)EditorGUILayout.ObjectField("������ ������", sequenceData, typeof(SequenceData), false);

        if (sequenceData == null)
        {
            EditorGUILayout.EndVertical();
            return;
        }

        InitializeTracks();

        EditorGUILayout.LabelField("BPM", sequenceData.bpm.ToString());
        EditorGUILayout.LabelField("����� Ŭ��", sequenceData.audioClip != null ? sequenceData.audioClip.name : "None");

        EditorGUILayout.LabelField("Ʈ�� �� ����", EditorStyles.boldLabel);
        int newNumberOfTracks = EditorGUILayout.IntField("Ʈ�� ��", sequenceData.numberOfTracks);
        if (newNumberOfTracks != sequenceData.numberOfTracks)       // Ʈ�� ���� �ٸ� ���
        {
            sequenceData.numberOfTracks = newNumberOfTracks;        // Ʈ�� ���� �����ͼ� ����
            InitializeTracks();
        }

        if (sequenceData.numberOfTracks < 1) sequenceData.numberOfTracks = 1;       // Ʈ���� 1 ���Ϸ� �������� �ּ� Ʈ���� 1
    }

    private void CreateAudioSource()                // ����Ƽ Scene���� ó�� ����� �ҽ��� ����ϱ� ���ؼ� ������ �ʰ� ���
    {
        var audioSourceGameobject = new GameObject("EditorAudioSource");
        audioSourceGameobject.hideFlags = HideFlags.HideAndDontSave;
        audioSource = audioSourceGameobject.AddComponent<AudioSource>();
    }

    private void InitializeTracks()                 // ������ �����Ͱ� ������ �� �����͸� �������ִ� �Լ�
    {
        if (sequenceData == null) return;               // �����Ͱ� ���� ��� �׳� ����

        if (sequenceData.trackNotes == null)            // �����Ͱ� �ִµ� Ʈ�� ��Ʈ�� ���� ��� �ڷ����� ���� �����ش�.
        {
            sequenceData.trackNotes = new List<List<int>>();
        }

        while (sequenceData.trackNotes.Count < sequenceData.numberOfTracks)     // Ʈ���� ��ŭ List<int>()�� �־��ش�.
        {
            sequenceData.trackNotes.Add(new List<int>());
        }

        foreach (var track in sequenceData.trackNotes)      // �� Ʈ�� ��Ʈ�� ��Ʈ �� ��ŭ 0 ������ �����͸� �־��ش�.
        {
            while (track.Count < totalBeats)
            {
                track.Add(0);
            }
        }
        if (audioSource != null)                            // ������ ������ �������� ����� Ŭ���� ���� ��� ����� �����͸� �Ҵ��Ѵ�.
        {
            audioSource.clip = sequenceData.audioClip;
        }
    }

    private void Update()
    {
        if (this == null)           // �� ��ũ��Ʈ�� �ı��Ǿ����� Ȯ��
        {
            EditorApplication.update -= Update;       // �̺�Ʈ �Ҵ��� �������ش�.
            return;
        }

        if (isPlaying && audioSource != null && audioSource.isPlaying)
        {
            float elapseTime = audioSource.time;                                        // ����� �ð��� �����´�.
            currentBeatTime = Mathf.FloorToInt(elapseTime * sequenceData.bpm / 60f);    // BPM���� 60�� �帥 �ð��� ���ϸ� ���� ��Ʈ�̴�.

            if (currentBeatTime >= totalBeats)
            {
                //StopPlayBack();
            }
            Repaint();                              // ������Ʈ�� ���� �Ŀ� ������Ʈ (ȭ�� ������ ������ �ʴ� ������Ʈ�̱� ������)
        }
    }

    private void StartPlayBack(int fromBeat)        // ������� Ư�� ��Ʈ���� �÷��� ���ִ� �Լ�
    {
        if (sequenceData == null || sequenceData.audioClip == null || audioSource == null) return;      // 3���� �� �ϳ��� ������ ����

        isPlaying = true;
        currentBeatTime = fromBeat;
        playFromBeat = fromBeat;

        if (audioSource.clip != sequenceData.audioClip)         // ���� ������Ʈ Ŭ���� �޾ƿ� ������ ������ ����� Ŭ���� �ٸ��ٸ�
        {
            audioSource.clip = sequenceData.audioClip;          // ���� ������ ������ Ŭ������ ���� �����ش�.
        }

        float startTime = fromBeat * 60f / sequenceData.bpm;    // ���۽ð��� ��Ʈ�� BPM���κ��� ����Ͽ� �����Ѵ�.
        audioSource.time = startTime;                           // ����� �ð��� ������ �ð����� ����
        audioSource.Play();

        this.startTime = (float)EditorApplication.timeSinceStartup - startTime;     // �����Ϳ� �ð��� �ݿ��Ѵ�.
        EditorApplication.update += Update;                                         // ������Ʈ �̺�Ʈ�� ���
    }

    private void PausePlayback()
    {
        isPlaying = false;
        if (audioSource != null) audioSource.Pause();
    }
    private void StopPlayback()
    {
        isPlaying = false;
        currentBeatTime = 0;
        playFromBeat = 0;

        if (audioSource != null) audioSource.Stop();
        EditorApplication.update -= Update;                         // ������Ʈ �̺�Ʈ ����
    }


    private void DrawBeat(int trackIndex, int beatIndex)
    {
        if (sequenceData == null || sequenceData.trackNotes == null || trackIndex >= sequenceData.trackNotes.Count) return;

        Rect rect = GUILayoutUtility.GetRect(trackWidth, beatHedight);
        bool isCurrentBeat = currentBeatTime == beatIndex;  // currentBeatTime�� beatIndex�� �����ϸ� true �ƴϸ� false
        // ���� Ʈ���� ���� ��Ʈ �ε����� �ش��ϴ� ��Ʈ ���� �������鼭 �˻��ؼ� ��ȿ�� �ε����� �ƴ� ��� 0���� �����ȴ�.
        int noteValue = (sequenceData.trackNotes[trackIndex].Count > beatIndex) ? sequenceData.trackNotes[trackIndex][beatIndex] : 0;

        Color color = Color.gray;
        if (isCurrentBeat) color = Color.cyan;
        else
        {
            switch(noteValue)
            {
                case 1: color = Color.green; break;
                case 2: color = Color.yellow; break;
                case 3: color = Color.red; break;
                case 4: color = Color.blue; break;
            }
        }
        EditorGUI.DrawRect(rect, color);

        // ������ �����쿡���� ���콺 Ŭ�� �̺�Ʈ�� �Ʒ��� ���� ���·� �޾ƿ´�.
        if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
        {
            if (Event.current.button == 0)      // ���콺 ���� Ŭ��
            {
                noteValue = (noteValue + 1) % 5;
                while (sequenceData.trackNotes[trackIndex].Count <= beatIndex)
                {
                    sequenceData.trackNotes[trackIndex].Add(0);
                }
                sequenceData.trackNotes[trackIndex][beatIndex] = noteValue;
            }
            else if (Event.current.button == 1)     // ���콺 ������ Ŭ��
            {
                if (sequenceData.trackNotes[trackIndex].Count > beatIndex)
                {
                    sequenceData.trackNotes[trackIndex][beatIndex] = 0;
                }
            }
            Event.current.Use();
        }
    }
}
