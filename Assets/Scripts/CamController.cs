using System.Collections;
using UnityEngine;

public class CamController : MonoBehaviour
{
    [Header("View Settings")]
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("View Parents")]
    [SerializeField] private Transform fightViewParent;
    [SerializeField] private Transform shopViewParent;

    private Coroutine transitionCoroutine;

    [SerializeField] private GameObject[] shopViewObjects;
    public void SetFightView()
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        foreach (GameObject go in shopViewObjects) go.SetActive(false);
        transform.SetParent(fightViewParent);
        transitionCoroutine = StartCoroutine(TransitionToLocalZero());
    }

    public void SetShopView()
    {
        print("Shop");
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        foreach (GameObject go in shopViewObjects) go.SetActive(true);
        transform.SetParent(shopViewParent);
        transitionCoroutine = StartCoroutine(TransitionToLocalZero());
    }

    private IEnumerator TransitionToLocalZero()
    {
        Vector3 startPos = transform.localPosition;
        Quaternion startRot = transform.localRotation;

        Vector3 endPos = Vector3.zero;
        Quaternion endRot = Quaternion.identity;

        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = transitionCurve.Evaluate(elapsed / transitionDuration);

            transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            transform.localRotation = Quaternion.Slerp(startRot, endRot, t);

            yield return null;
        }

        transform.localPosition = endPos;
        transform.localRotation = endRot;
        transitionCoroutine = null;
    }

    public void ShakeCamera(float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
