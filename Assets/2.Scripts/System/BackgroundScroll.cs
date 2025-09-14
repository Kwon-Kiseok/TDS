using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    [SerializeField] float _speed = 3f;
    float _repeatDistance;

    Vector2 _startPos;
    float _newPos;

    void Start()
    {
        _startPos = transform.position;
        _repeatDistance = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        _newPos = Mathf.Repeat(Time.time * _speed, _repeatDistance);
        transform.position = _startPos + Vector2.left * _newPos;
    }
}
