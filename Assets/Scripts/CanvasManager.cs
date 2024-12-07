using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class CanvasManager : GenericSingleton<CanvasManager>
{
    [Header("Title---------------------------------")]
    [SerializeField] Text title;
    [SerializeField] Vector3 targetTitlePos;
    [SerializeField] Vector3 originalTitlePos;


    [Header("Looading Bar--------------------------------")]
    [SerializeField] Image barBG;
    [SerializeField] Image fillImg;
    [SerializeField] float fakeDuration;

    [Header("Match Making---------------------------------")]
    [SerializeField] RectTransform matchMaking;
    [SerializeField] public Text username;

    [Header("Notification---------------------------------")] 
    [SerializeField] private RectTransform notificationTransform;
    [SerializeField] private Text notificationText;
    
    [Header("Lobby ---------------------------")]
    [SerializeField] PlayerDataLobby playerDataLobbyManagerPrefab;
    
    private void Start()
    {
        AnimateIntro();
    }

    private async Task FakeLoading()
    {
        title.GetComponent<RectTransform>().DOAnchorPos(Vector3.zero, 1.5f).SetDelay(0.3f).SetEase(Ease.OutBack);

        fillImg.fillAmount = 0;

        float timer = 0f;

        while (timer < fakeDuration)
        {
            timer += Time.deltaTime;
            fillImg.fillAmount = timer / fakeDuration;
            await Task.Yield();
        }

        fillImg.fillAmount = 1;

        barBG.transform.parent.transform.DOScale(Vector3.zero, 0.5f).SetDelay(0.5f).SetEase(Ease.InBack);

        await Task.Delay(1500);
        barBG.transform.parent.gameObject.SetActive(false);
    }

    private async void AnimateIntro()
    {
        DataTransition.instance.gameState = GameState.LOADING;
        await FakeLoading();

        if (InitCommunication.instance.isLoggedIn)
        {
            DataTransition.instance.gameState = GameState.MATCHMAKING;
            matchMaking.gameObject.SetActive(true);
            matchMaking.transform.DOScale(Vector3.one, 1f).SetDelay(0.1f).SetEase(Ease.OutBack);
        }
        else await Task.Yield();

        Reset();
    }

    private void Reset()
    {
        title.transform.position = originalTitlePos;
        fillImg.fillAmount = 0;
        barBG.transform.parent.localScale = Vector3.one;
    }

    public async void EnterGameplay()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(async () =>
        {
            barBG.transform.parent.gameObject.SetActive(true);

            AsyncOperation scene = SceneManager.LoadSceneAsync(1);
            scene.allowSceneActivation = false;

            await FakeLoading();

            DataTransition.instance.gameState = GameState.GAMEPLAY;
            scene.allowSceneActivation = true;
        });
    }

    public async void AnimateNotification(string message)
    {
        notificationText.text = message;
        
        notificationTransform.gameObject.SetActive(true);
        notificationTransform.DOScale(Vector3.zero, 1f).SetEase(Ease.Linear).SetDelay(3f);

        await Task.Delay(4000);
        
        notificationTransform.gameObject.SetActive(false);
        notificationTransform.localScale = Vector3.one;
    }
}
