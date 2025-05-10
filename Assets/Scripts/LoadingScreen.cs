using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI loadingText;
    public float loadingTime = 5f;
    public float animateDotTime = 0.25f;
    public string sceneToLoad = "Tutorial";
    private string baseText = "Loading";

    private void Start()
    {
        StartCoroutine(AnimateLoading());
        StartCoroutine(AnimateDots());
    }

    private IEnumerator AnimateLoading()
    {
        float elapsedTime = 0f;

        while (elapsedTime < loadingTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene(sceneToLoad);
    }

    private IEnumerator AnimateDots()
    {
        int dotCount = 0;

        while (true)
        {
            loadingText.text = baseText + new string('.', dotCount);

            dotCount = (dotCount + 1) % 4;

            yield return new WaitForSeconds(animateDotTime);
        }
    }
}
