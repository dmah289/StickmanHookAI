using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class InitCommunication : GenericSingleton<InitCommunication>
{
    public bool isLoggedIn;

    void Start()
    {
        Login();
    }

    private async void Login()
    {
        await UnityServices.InitializeAsync();

        if(UnityServices.State == ServicesInitializationState.Initialized)
        {
            AuthenticationService.Instance.SignedIn += OnSignedIn;

            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            if (AuthenticationService.Instance.IsSignedIn)
            {
                isLoggedIn = true;

                string username = PlayerPrefs.GetString(KeySave.username);

                if(string.IsNullOrEmpty(username))
                {
                    username = "Player" + AuthenticationService.Instance.PlayerId.Substring(0, 5);
                    PlayerPrefs.SetString(KeySave.username, username);
                }
                // Đặt username
                CanvasManager.instance.username.text = username;
            }
        }
    }

    private void OnSignedIn()
    {
        // print($"Token: {AuthenticationService.Instance.AccessToken}");
    }
}
