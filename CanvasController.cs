using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    [SerializeField] GameObject CanvasBackground;
    [SerializeField] public MainMenu CanvasMainMenu;
    [SerializeField] public Roster CanvasRoster;
    [SerializeField] public Header CanvasHeader;
    [SerializeField] public GameStart CanvasGameStart;
    [SerializeField] Button TitleButton;

    Coroutine m_coTitleFade = null;

    void Awake()
    {
        EventManager.EOnPanel += OnPanel;
        EventManager.EOffPanel += OffPanel;
    }

    void OnDestroy()
    {
        EventManager.EOnPanel -= OnPanel;
        EventManager.EOffPanel -= OffPanel;
    }

    void Start()
    {
        TitleButton.onClick.AddListener(() =>
        {
            if (m_coTitleFade != null)
            {
                StopCoroutine(m_coTitleFade);
                m_coTitleFade = null;
            }
            TitleButton.gameObject.SetActive(false);
        });

        m_coTitleFade = StartCoroutine(TitleFadeOut());
        SoundManager.Instance.SetSound(AudioChannel.BGM, "bgm_lobby");
    }

    IEnumerator TitleFadeOut()
    {
        yield return new WaitForSeconds(2f);

        TitleButton.gameObject.SetActive(false);
    }

    void OffPanel(EnUIPanel panel)
    {
        switch (panel)
        {
            case EnUIPanel.MainMenu:
                CanvasMainMenu.gameObject.SetActive(false);
                CanvasBackground.SetActive(false);
                break;
            case EnUIPanel.Roster:
                CanvasRoster.gameObject.SetActive(false);
                break;
            case EnUIPanel.Header:
                CanvasHeader.gameObject.SetActive(false);
                break;
            case EnUIPanel.Option:
                option.gameObject.SetActive(false);
                break;
            case EnUIPanel.GameStart:
                CanvasGameStart.gameObject.SetActive(false);
                break;
        }
    }

    void OnPanel(EnUIPanel panel)
    {
        switch (panel)
        {
            case EnUIPanel.MainMenu:
                CanvasMainMenu.gameObject.SetActive(true);
                CanvasBackground.SetActive(true);

                OffPanel(EnUIPanel.Header);
                OffPanel(EnUIPanel.Instance);
                OffPanel(EnUIPanel.GameClear);
                OffPanel(EnUIPanel.GameOver);
                OffPanel(EnUIPanel.GameStart);
                OffPanel(EnUIPanel.Roster);
                OffPanel(EnUIPanel.StageSelect);
                OffPanel(EnUIPanel.End);
                break;
            case EnUIPanel.Roster:
                CanvasRoster.gameObject.SetActive(true);

                OffPanel(EnUIPanel.Header);
                OffPanel(EnUIPanel.Instance);
                OffPanel(EnUIPanel.GameClear);
                OffPanel(EnUIPanel.GameOver);
                OffPanel(EnUIPanel.Option);
                OffPanel(EnUIPanel.GameStart);
                OffPanel(EnUIPanel.StageSelect);
                OffPanel(EnUIPanel.End);
                break;
            case EnUIPanel.Header:
                CanvasHeader.gameObject.SetActive(true);
                break;
            case EnUIPanel.Option:
                //option.gameObject.SetActive(true);
                break;
            case EnUIPanel.GameStart:
                SoundManager.Instance.SetSound(AudioChannel.BGM, "bgm_lobby");
                CanvasGameStart.gameObject.SetActive(true);
                CanvasMainMenu.gameObject.SetActive(true);
                CanvasBackground.SetActive(true);

                OffPanel(EnUIPanel.Header);
                OffPanel(EnUIPanel.Instance);
                OffPanel(EnUIPanel.GameClear);
                OffPanel(EnUIPanel.GameOver);
                OffPanel(EnUIPanel.Option);
                OffPanel(EnUIPanel.Roster);
                OffPanel(EnUIPanel.StageSelect);
                OffPanel(EnUIPanel.End);
                break;
        }
    }
}
