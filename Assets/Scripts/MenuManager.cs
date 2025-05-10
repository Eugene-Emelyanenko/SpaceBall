using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Levels")]
    [SerializeField] private Image selectedLevelPreviewImage;
    [SerializeField] private TextMeshProUGUI selectedLevelNameText;

    [Header("Balls")]
    [SerializeField] private Image previousBallImage;
    [SerializeField] private Image selectedBallImage;
    [SerializeField] private Image nextBallImage;
    [SerializeField] private TextMeshProUGUI selectedBallNameText;

    private LevelData[] levelDataList;
    private int selectedLevelIndex;

    private BallData[] ballDataList;
    private int selectedBallIndex;

    private void Awake()
    {
        levelDataList = Resources.LoadAll<LevelData>("LevelDatas");
        selectedLevelIndex = PlayerPrefs.GetInt("SelectedLevel", 0);

        ballDataList = Resources.LoadAll<BallData>("BallDatas");
        selectedBallIndex = PlayerPrefs.GetInt("SelectedBall", 0);
    }

    private void Start()
    {
        UpdateLevel();

        UpdateBall();
    }

    public void ChooseLevel(int value)
    {
        selectedLevelIndex += value;

        if (selectedLevelIndex < 0)
            selectedLevelIndex = levelDataList.Length - 1;
        else if (selectedLevelIndex > levelDataList.Length - 1)
            selectedLevelIndex = 0;

        UpdateLevel();

        PlayerPrefs.SetInt("SelectedLevel", selectedLevelIndex);
        PlayerPrefs.Save();
    }

    private void UpdateLevel()
    {
        selectedLevelPreviewImage.sprite = levelDataList[selectedLevelIndex].previewSprite;
        selectedLevelNameText.text = levelDataList[selectedLevelIndex].GetFullName();
    }

    public void ChooseBall(int value)
    {
        selectedBallIndex += value;

        if (selectedBallIndex < 0)
            selectedBallIndex = ballDataList.Length - 1;
        else if (selectedBallIndex > ballDataList.Length - 1)
            selectedBallIndex = 0;

        UpdateBall();

        PlayerPrefs.SetInt("SelectedBall", selectedBallIndex);
        PlayerPrefs.Save();
    }

    private void UpdateBall()
    {
        int previousBallIndex = selectedBallIndex - 1;
        if(previousBallIndex < 0)
            previousBallIndex = ballDataList.Length - 1;

        int nextBallIndex = selectedBallIndex + 1;
        if (nextBallIndex > ballDataList.Length - 1)
            nextBallIndex = 0;

        previousBallImage.sprite = ballDataList[previousBallIndex].ballSprite;

        selectedBallImage.sprite = ballDataList[selectedBallIndex].ballSprite;

        nextBallImage.sprite = ballDataList[nextBallIndex].ballSprite;

        selectedBallNameText.text = ballDataList[selectedBallIndex].ballName;
    }
}
