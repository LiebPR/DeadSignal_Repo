using UnityEngine;

public class IntroCameraMove : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float speed = 1.5f;

    bool moving = false;

    void Start()
    {
        if (MenuDirector.Instance != null)
        {
            MenuDirector.Instance.OnStateChanged += HandleState;
        }
    }

    void OnDestroy()
    {
        if (MenuDirector.Instance != null)
        {
            MenuDirector.Instance.OnStateChanged -= HandleState;
        }
    }

    void HandleState(MenuState state)
    {
        if (state == MenuState.Intro)
            moving = true;
    }

    void Update()
    {
        if (!moving) return;

        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * speed);
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * speed);

        if (Vector3.Distance(transform.position, target.position) < 0.05f)
        {
            moving = false;

            MenuDirector.Instance.ChangeState(MenuState.Login);
        }
    }
}