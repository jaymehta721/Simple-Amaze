using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject ball;
    [SerializeField] private Transform maze, floor;

    public Text levelText, movesLimitText;
    public Button nextLevelButton;
    public GameObject pausePanel, gameModePanel, gameOverPanel;
    private BallController ballController;
    private MapModel mapModel;
    private int currentLevel ;
    private int uiLevel;
    private int levelCompletedCount;
    private int movesLimit;
    private bool isLimitedMode;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        mapModel = GetComponent<MapModel>();
        ballController = ball.GetComponent<BallController>();
        CreateLevel(mapModel.maze, currentLevel);
        uiLevel = currentLevel + 1;
        levelText.text = "Level " + uiLevel;
    }

    private void CreateLevel(int[,,] mapArray, int level)
    {
        if (level > 2)
            level = 0;
        levelCompletedCount = 0;

        ball.transform.localPosition = new Vector3(mapModel.ballPosition[level, 0], mapModel.ballPosition[level, 1],
            mapModel.ballPosition[level, 2]);
        int childCount = 0;
        for (int i = 0; i < 13; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                if (mapArray[level, i, j] == 0)
                {
                    maze.GetChild(childCount).gameObject.SetActive(false);
                    floor.GetChild(childCount).gameObject.SetActive(true);
                    levelCompletedCount++;
                }
                else
                {
                    maze.GetChild(childCount).gameObject.SetActive(true);
                    floor.GetChild(childCount).gameObject.SetActive(false);
                }

                floor.GetChild(childCount).gameObject.GetComponent<Renderer>().material.color = Color.black;
                childCount++;
            }
        }

        ballController.ColoredWallCounter = 0;
    }

    #region UI Control Functions

    public void OpenGameModePanel()
    {
        gameModePanel.SetActive(true);
    }

    public void CloseGameModePanel()
    {
        gameModePanel.SetActive(false);
    }

    private void OpenGameOverPanel()
    {
        gameOverPanel.SetActive(true);
        ballController.MoveCounter = 0;
    }

    public void TryAgain()
    {
        gameOverPanel.SetActive(false);
        CreateLevel(mapModel.maze, currentLevel);
    }

    public void SelectGameMode(int mode)
    {
        if (mode == 1)
        {
            currentLevel = 2;
            movesLimit = mapModel.movesLimit[currentLevel];
            isLimitedMode = true;
            ballController.MoveCounter = 0;
            uiLevel = currentLevel + 1;
            levelText.text = "Level " + 1;
            movesLimitText.gameObject.SetActive(true);
        }
        else
        {
            currentLevel = 0;
            isLimitedMode = false;
            movesLimitText.gameObject.SetActive(false);
        }

        levelCompletedCount = 0;
        CreateLevel(mapModel.maze, currentLevel);
        gameModePanel.SetActive(false);
    }

    public void OpenPausePanel()
    {
        pausePanel.SetActive(true);
    }

    public void ClosePausePanel()
    {
        pausePanel.SetActive(false);
    }

    public void CompleteLevel()
    {
        if (currentLevel > 1)
        {
            currentLevel = -1;
            isLimitedMode = false;
            movesLimitText.gameObject.SetActive(false);
        }

        currentLevel++;
        nextLevelButton.gameObject.SetActive(false);
        uiLevel = currentLevel + 1;
        levelText.text = "Level " + uiLevel;
        CreateLevel(mapModel.maze, currentLevel);
    }

    #endregion

    private void FinishLevel()
    {
        levelText.text = (currentLevel + 1) + ". LEVEL COMPLETED";
        nextLevelButton.gameObject.SetActive(true);
    }

    private void Update()
    {
        movesLimitText.text = (movesLimit - ballController.MoveCounter).ToString();
        if (ballController.ColoredWallCounter == levelCompletedCount)
        {
            FinishLevel();
        }

        if (ballController.MoveCounter == movesLimit && isLimitedMode)
            OpenGameOverPanel();
    }
}