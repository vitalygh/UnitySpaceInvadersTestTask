//USING_ZENJECT
using UnityEngine;
using TMPro;
#if USING_ZENJECT
using Zenject;
#endif

public class MainUI : MonoBehaviour
{
    public GameObject GameLogic = null;
    ILogController logController = null;
    IGameController gameController = null;
    public RectTransform Header = null;
    public TMP_Text ScoreText = null;
    public TMP_Text LevelText = null;
    public TMP_Text HPText = null;
    private int screenWidth = -1;

#if USING_ZENJECT
    [Inject]
#endif
    private void Construct(ILogController logController, IGameController gameController)
    {
        this.logController = logController;
        this.gameController = gameController;
    }

    // Start is called before the first frame update
    void Start()
    {
#if !USING_ZENJECT
        Construct(GameLogic.GetComponent<ILogController>(), GameLogic.GetComponent<IGameController>());
#endif
        if (gameController == null)
        {
            logController.Error(nameof(gameController) + " is null");
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
