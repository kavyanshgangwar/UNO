using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using Kavyansh.Core.Singletons;
using UnityEngine.SceneManagement;

public class AdManager : Singleton<AdManager>
{
    private BannerView bannerAd;
    private BannerView bannerAd2;
    private InterstitialAd interstitialAd;
    // Start is called before the first frame update
    void Start()
    {
        MobileAds.Initialize(InitializationStatus => { });
        this.RequestBanner();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void RequestBanner()
    {
        string adUnitId = "ca-app-pub-6226852136664310/4712935316";
        string adUnitId2 = "ca-app-pub-6226852136664310/8652976480";
        this.bannerAd = new BannerView(adUnitId,AdSize.MediumRectangle,AdPosition.BottomRight);
        this.bannerAd2 = new BannerView(adUnitId2,AdSize.Banner,AdPosition.BottomLeft);
        

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        AdRequest request1 = new AdRequest.Builder().Build();
        // Load the banner with the request.
        this.bannerAd.LoadAd(request);
        this.bannerAd2.LoadAd(request1);

    }

    public void RequestInterstitial()
    {
        string adUnitId = "ca-app-pub-6226852136664310/9710111128";
        if(this.interstitialAd != null)
        {
            this.interstitialAd.Destroy();
        }
        this.interstitialAd = new InterstitialAd(adUnitId);

        this.interstitialAd.OnAdClosed += OnInterstitialClosedHandler;
        this.interstitialAd.LoadAd(new AdRequest.Builder().Build());
    }

    private void OnInterstitialClosedHandler(object sender, EventArgs args)
    {
        SceneManager.LoadScene("GameOver");
    }
    public void ShowInterstitial()
    {
        if (this.interstitialAd.IsLoaded())
        {
            this.interstitialAd.Show();
        }
        else
        {
            SceneManager.LoadScene("GameOver");
        }
    }
}
