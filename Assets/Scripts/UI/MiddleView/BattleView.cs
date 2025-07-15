using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Coffee.UIEffects;
using DG.Tweening;

public class BattleView : MiddleViews
{
    [Header("World Info")]
    [SerializeField] private TMP_Text worldText;
    [SerializeField] private TMP_Text areaText;
    [SerializeField] private Button playButton;
    [SerializeField] private Button leftArrowButton;
    [SerializeField] private Button rightArrowButton;
    [SerializeField] private UIShiny playButtonShiny;

    // [Header("Background Management")]
    // [SerializeField] private List<WorldImageHolder> bgImageHolders; // Holds Image GameObjects composing the full background

    // [SerializeField] private List<WorldImage> worldImages;
    public int selectedWorldId;
    private int currentWorldId;
    private int currentAreaId;
    private int lastDirection = 0;


    protected override void OnStart()
    {
        lastDirection = 0;
        base.OnStart();

        selectedWorldId = GameManager.GetInstance().GetPlayerData().currentWorldId;
        currentWorldId = selectedWorldId;
        currentAreaId = GameManager.GetInstance().GetPlayerData().currentAreaId;

        if (areaText != null)
            areaText.text = $"Chapter {currentAreaId}/{Statics.worldWiseAreaCount[selectedWorldId - 1]}";

        leftArrowButton.onClick.AddListener(OnLeftArrowClick);
        rightArrowButton.onClick.AddListener(OnRightArrowClick);

        SetupWorldNavigation();
        // UpdateBackgroundImages(true);
    }

    public override void OnViewSelected()
    {
        base.OnViewSelected();
    }

    public override void OnViewUnSelected()
    {
        base.OnViewUnSelected();
    }


    public void OnLeftArrowClick()
    {
        AudioManager.CallPlaySFX(Sound.ButtonClick);
        lastDirection = -1;
        selectedWorldId--;
        if (selectedWorldId < 1) selectedWorldId = Statics.maxWorldId;
        SetupWorldNavigation();
    }

    public void OnRightArrowClick()
    {
        AudioManager.CallPlaySFX(Sound.ButtonClick);
        lastDirection = 1;
        selectedWorldId++;
        if (selectedWorldId > Statics.maxWorldId) selectedWorldId = Statics.minWorldId;
        SetupWorldNavigation();
    }

    private void SetupWorldNavigation()
    {
        if (worldText != null)
            worldText.text = $"World: {selectedWorldId}";

        currentAreaId = GameManager.GetInstance().GetPlayerData().currentAreaId;
        if (currentWorldId > selectedWorldId) currentAreaId = Statics.worldWiseAreaCount[selectedWorldId - 1];
        else if (currentWorldId < selectedWorldId) currentAreaId = 1;


        if (areaText != null)
            areaText.text = $"Chapter {currentAreaId}/{Statics.worldWiseAreaCount[selectedWorldId - 1]}";


        bool isUnlocked = selectedWorldId == GameManager.GetInstance().GetPlayerData().currentWorldId;
        playButton.interactable = isUnlocked;

        if (isUnlocked)
        {
            playButton.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 1, 0.5f);
            playButtonShiny.Play();
        }
        else
        {
            playButtonShiny.Stop();
        }

        currentWorldId = selectedWorldId;
        lastDirection = 0;



        // WorldImage newWorldData = GetWorldData(selectedWorldId);

        // UpdateBackgroundImages();



    }

    // private void UpdateBackgroundImages(bool isInit = false)
    // {
    //     leftArrowButton.interactable = false;
    //     rightArrowButton.interactable = false;

    //     int activeIndex = -1;
    //     switch (selectedWorldId)
    //     {
    //         case 1:
    //             activeIndex = 0;
    //             break;
    //         case 2:
    //             activeIndex = 1;
    //             break;
    //         case 3:
    //             activeIndex = 2;
    //             break;
    //         case 4:
    //             activeIndex = 3;
    //             break;
    //         case 5:
    //             activeIndex = 4;
    //             break;
    //         default:
    //             Debug.LogWarning($"Unexpected worldId: {selectedWorldId}");
    //             break;
    //     }

    //     for (int i = 0; i < bgImageHolders.Count; i++)
    //     {
    //         WorldImageHolder holder = bgImageHolders[i];
    //         if (holder.bgImageHolder == null) continue;

    //         // Ensure CanvasGroup exists
    //         CanvasGroup canvasGroup = holder.bgImageHolder;
    //         // SpriteGroup spriteGroup = holder.bgSpritesHolder;

    //         // Fade in the selected background, fade out the rest
    //         if (i == activeIndex)
    //         {
    //             if (isInit)
    //             {
    //                 canvasGroup.alpha = 1f;
    //                 canvasGroup.interactable = true;
    //                 canvasGroup.blocksRaycasts = true;
    //                 // spriteGroup.Alpha = 1f; // Ensure alpha is set to 1 at the start
    //                 holder.bgImageHolder.gameObject.SetActive(true); // Ensure it's visible at the start
    //             }

    //             else
    //             {
    //                 // DOTween.To(() => spriteGroup.Alpha, x => spriteGroup.Alpha = x, 1f, 0.25f)
    //                 // .OnComplete(() => spriteGroup.Alpha = 1f); // Ensure alpha is set to 1 after fade in

    //                 holder.bgImageHolder.gameObject.SetActive(true); // Ensure it's visible before fade in

    //                 canvasGroup.DOFade(1f, 0.25f).OnComplete(() =>
    //                 {
    //                     canvasGroup.interactable = true;
    //                     canvasGroup.blocksRaycasts = true;
    //                 });
    //             }

    //         }
    //         else
    //         {

    //             if (isInit)
    //             {
    //                 canvasGroup.alpha = 0f;
    //                 canvasGroup.interactable = false;
    //                 canvasGroup.blocksRaycasts = false;
    //                 // spriteGroup.Alpha = 0f; // Ensure alpha is set to 0 at the start
    //                 holder.bgImageHolder.gameObject.SetActive(false); // Hide it at the start
    //                 leftArrowButton.interactable = true;
    //                 rightArrowButton.interactable = true;
    //             }

    //             else
    //             {
    //                 // DOTween.To(() => spriteGroup.Alpha, x => spriteGroup.Alpha = x, 0f, 0.25f)
    //                 // .OnComplete(() => spriteGroup.Alpha = 0f); // Ensure alpha is set to 0 after fade out

    //                 canvasGroup.DOFade(0f, 0.25f).OnComplete(() =>
    //                 {
    //                     canvasGroup.interactable = false;
    //                     canvasGroup.blocksRaycasts = false;
    //                     holder.bgImageHolder.gameObject.SetActive(false); // Hide it after fade out completes
    //                     leftArrowButton.interactable = true;
    //                     rightArrowButton.interactable = true;
    //                 });
    //             }


    //         }
    //     }

    //     if (currentWorldId < selectedWorldId)
    //     {
    //         currentAreaId = 1;
    //     }

    //     // if (areaText != null)
    //     //     areaText.text = $"Chapter {currentAreaId}/{Statics.worldWiseAreaCount[selectedWorldId - 1]}";

    //     // bool isUnlocked = selectedWorldId == GameManager.GetInstance().playerData.currentWorldId;
    //     // playButton.interactable = isUnlocked;

    //     // if (isUnlocked)
    //     // {
    //     //     playButton.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 1, 0.5f);
    //     //     playButtonShiny.Play();
    //     // }
    //     // else
    //     // {
    //     //     playButtonShiny.Stop();
    //     // }

    //     currentWorldId = selectedWorldId;
    //     lastDirection = 0;
    // }



    public void OnPlayButtonPressed()
    {
        // GameManager.GetInstance().UpdateCurrentWorldId(selectedWorldId);

        AudioManager.CallPlaySFX(Sound.ButtonClick);
        TransitionManager.GetInstance().ChangeLevel(SCENE_NUM.GAME_SCENE);
    }

    // private WorldImage GetWorldData(int worldId)
    // {
    //     return worldImages.Find(x => x.worldId == worldId);
    // }
}

// [System.Serializable]
// public struct WorldImage
// {
//     public int worldId;
//     public string worldName;
// }

// [System.Serializable]
// public struct WorldImageHolder
// {
//     public CanvasGroup bgImageHolder;
//     // public SpriteGroup bgSpritesHolder;
// }
