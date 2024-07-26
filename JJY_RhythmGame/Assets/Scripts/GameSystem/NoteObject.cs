using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    public Note note;           // ��Ʈ ����
    public float speed;         // ��Ʈ �̵� �ӵ�
    public float hitPosition;   // ���� ��ġ
    public float startTime;     // ���� ���� �ð�

    // ��Ʈ ������Ʈ �ʱ�ȭ
    public void Initialize(Note note, float speed, float hitPosition, float startTime)
    {
        this.note = note;
        this.speed = speed;
        this.hitPosition = hitPosition;
        this.startTime = startTime;

        // ��Ʈ�� �ʱ� ��ġ ����
        float initialDistance = speed * (note.startTime - (Time.time - startTime));
        transform.position = new Vector3(hitPosition + initialDistance, note.trackIndex * 2, 0);
    }

    void Update()
    {
        // ��Ʈ �̵�
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        // ���� ��ġ�� ������ �ı�
        if (transform.position.x < hitPosition - 1)
        {
            Destroy(gameObject);
        }
    }
}
