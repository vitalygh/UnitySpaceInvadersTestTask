using UnityEngine;
using TMPro;

public class MainUI : MonoBehaviour
{
    public GameObject GameLogic = null;
    public RectTransform Header = null;
    public TMP_Text ScoreText = null;
    public TMP_Text LevelText = null;
    public TMP_Text HPText = null;
    private int screenWidth = -1;

    // Start is called before the first frame update
    void Start()
    {
        var logController = GameLogic.GetComponent<ILogController>();
        var gameController = GameLogic.GetComponent<IGameController>();
        if (gameController == null)
        {
            logController.Error(nameof(gameController) + " is null");
            return;
        }
        var spaceshipController = GameLogic.GetComponent<ISpaceshipController>();
        if (spaceshipController == null)
        {
            logController.Error(nameof(spaceshipController) + " is null");
            return;
        }
        if (ScoreText != null)
        {
            ScoreText.text = gameController.Score.ToString();
            gameController.OnScoreChanged += (score) =>
            {
                ScoreText.text = score.ToString();
                ScoreText.color = score > gameController.Score ? Color.green : Color.red;
            };
        }
        if (LevelText != null)
        {
            LevelText.text = (gameController.Level + 1).ToString();
            gameController.OnLevelChanged += (level) =>
            {
                LevelText.text = (level + 1).ToString();
                LevelText.color = level > gameController.Level ? Color.green : Color.red;
            };
        }
        if (HPText != null)
        {
            HPText.text = gameController.HP.ToString();
            gameController.OnHPChanged += (hp) =>
            {
                HPText.text = hp.ToString();
                HPText.color = hp > gameController.HP ? Color.green : Color.red;
            };
        }
    }

    void UpdateSafeArea()
    {
        if (Screen.width == screenWidth)
            return;
        screenWidth = Screen.width;
        var delta = Screen.safeArea.height - Screen.height;
        if (Header != null)
        {
            var position = Header.localPosition;
            position.y = delta;
            Header.localPosition = position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSafeArea();
    }
}
