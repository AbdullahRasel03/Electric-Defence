using System.Collections;
using UnityEngine;

public class CamController : MonoBehaviour
{
    [Header("View Settings")]
    [SerializeField] private float fightViewZ = 3f;
    [SerializeField] private float shopViewZ = -3f;
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector3 fightViewPosition;
    private Vector3 shopViewPosition;
    private Coroutine transitionCoroutine;
    private float initialZ;

    private void Awake()
    {
        initialZ = transform.position.z;
        fightViewPosition = new Vector3(transform.position.x, transform.position.y, fightViewZ);
        shopViewPosition = new Vector3(transform.position.x, transform.position.y, shopViewZ);
    }

    public void SetFightView()
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        transitionCoroutine = StartCoroutine(TransitionToPosition(fightViewPosition));
    }

    public void SetShopView()
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        transitionCoroutine = StartCoroutine(TransitionToPosition(shopViewPosition));
    }

    private IEnumerator TransitionToPosition(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = transitionCurve.Evaluate(elapsedTime / transitionDuration);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        transform.position = targetPosition;
        transitionCoroutine = null;
    }

    // Optional: Add camera shake or other effects
    public void ShakeCamera(float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.position = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
    }
}