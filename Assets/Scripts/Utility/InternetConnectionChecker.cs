using System.Collections;
using UnityEngine;

public class InternetConnectionChecker : MonoBehaviour
{
    private float checkInterval = 5f;
    private bool runInternetCheckLoop;
    private bool isInternetAvailable;

    private static InternetConnectionChecker instance;
    private IEnumerator CheckConnection;
    private IEnumerator CheckInternet;

    private float lastUpdateTime;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        runInternetCheckLoop = true;
        lastUpdateTime = Time.time;

        CheckConnection = CheckInternetConnectionRepeatedly();
        StartCoroutine(CheckConnection);
        StartCoroutine(MonitorUpdateTimeout());
    }

    IEnumerator CheckInternetConnectionRepeatedly()
    {
        while (runInternetCheckLoop)
        {
            // CLog.Print("Checking Connection");
            if(CheckInternet!=null)
                StopCoroutine(CheckInternet);
            CheckInternet = CheckInternetConnection();
            yield return StartCoroutine(CheckInternet);

            lastUpdateTime = Time.time;

            yield return new WaitForSeconds(checkInterval);
        }
    }

    IEnumerator CheckInternetConnection()
    {
        float time = Time.time;
        using (WWW www = new WWW("https://www.google.com"))
        {
            yield return www;

            if (string.IsNullOrEmpty(www.error))
            {
                // CLog.Print("Internet connection is available.");
                isInternetAvailable = true;
            }
            else
            {
                // CLog.Print("No internet connection.");
                isInternetAvailable = false;
            }
        }
        // CLog.Error("Ping Time: " + (Time.time - time) + " Seconds");
    }

    IEnumerator MonitorUpdateTimeout()
    {
        while (true)
        {
            float elapsed = Time.time - lastUpdateTime;

            if (elapsed >= 10f)
            {
                isInternetAvailable = false;
                // CLog.Print("Connection check timed out");
                StopCoroutine(CheckConnection);
                CheckConnection = CheckInternetConnectionRepeatedly();
                StartCoroutine(CheckConnection);
                lastUpdateTime = Time.time;
            }

            yield return null;
        }
    }

    public bool IsInternetAvailable()
    {
        return isInternetAvailable;
    }

    public void SetRunInternetCheckLoop(bool flag)
    {
        runInternetCheckLoop = flag;
    }

    public static InternetConnectionChecker GetInstance()
    {
        return instance;
    }
}