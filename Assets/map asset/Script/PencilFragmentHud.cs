using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PencilFragmentHud : MonoBehaviour
{
    private static PencilFragmentHud instance;
    private static Sprite iconSprite;
    private static Sprite usedIconSprite;
    private static Sprite greenCrayonIconSprite;
    private static Sprite brownCrayonIconSprite;
    private static Sprite blueCrayonIconSprite;

    private const float IconStartX = 18f;
    private const float IconY = -18f;
    private const float IconSpacing = 78f;

    private readonly List<GameObject> iconObjects = new List<GameObject>();
    private GameObject greenCrayonIconObject;
    private GameObject brownCrayonIconObject;
    private GameObject blueCrayonIconObject;

    public static void ShowCollected()
    {
        PencilFragmentHud hud = GetOrCreate();
        hud.gameObject.SetActive(true);
        hud.RefreshIcons();
    }

    public static void RefreshCollected()
    {
        PencilFragmentHud hud = instance != null ? instance : GetOrCreate();
        hud.RefreshIcons();
    }

    private static PencilFragmentHud GetOrCreate()
    {
        if (instance != null)
        {
            return instance;
        }

        GameObject hudObject = new GameObject("GeneratedPencilFragmentHud");
        Canvas canvas = hudObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 500;

        CanvasScaler canvasScaler = hudObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
        hudObject.AddComponent<GraphicRaycaster>();

        instance = hudObject.AddComponent<PencilFragmentHud>();
        instance.RefreshIcons();
        return instance;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void RefreshIcons()
    {
        Transform oldCountObject = transform.Find("GeneratedPencilFragmentHudCount");
        if (oldCountObject != null)
        {
            Destroy(oldCountObject.gameObject);
        }

        HideChildrenWithPrefix("GeneratedPencilFragmentHudIcon_");

        int collectedCount = GameProgress.CollectedPencilFragmentCount;
        int availableCount = GameProgress.AvailablePencilFragmentCount;

        while (iconObjects.Count < collectedCount)
        {
            iconObjects.Add(CreateIconObject(iconObjects.Count));
        }

        for (int i = 0; i < iconObjects.Count; i++)
        {
            GameObject iconObject = iconObjects[i];
            if (iconObject == null)
            {
                continue;
            }

            bool shouldShow = i < collectedCount;
            iconObject.SetActive(shouldShow);

            if (!shouldShow)
            {
                continue;
            }

            Image image = iconObject.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = i < availableCount ? GetIconSprite() : GetUsedIconSprite();
                image.color = Color.white;
            }

            RectTransform rectTransform = iconObject.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = new Vector2(IconStartX + IconSpacing * i, IconY);
            }
        }

        int crayonIconIndex = collectedCount;
        bool hasGreenCrayonIcon = RefreshGreenCrayonIcon(crayonIconIndex);
        if (hasGreenCrayonIcon)
        {
            crayonIconIndex++;
        }

        bool hasBrownCrayonIcon = RefreshBrownCrayonIcon(crayonIconIndex);
        if (hasBrownCrayonIcon)
        {
            crayonIconIndex++;
        }

        bool hasBlueCrayonIcon = RefreshBlueCrayonIcon(crayonIconIndex);

        if (collectedCount <= 0 && !hasGreenCrayonIcon && !hasBrownCrayonIcon && !hasBlueCrayonIcon)
        {
            gameObject.SetActive(false);
        }
    }

    private GameObject CreateIconObject(int index)
    {
        string objectName = $"GeneratedPencilFragmentHudIcon_{index:00}";
        Transform existing = transform.Find(objectName);
        GameObject iconObject = existing != null ? existing.gameObject : new GameObject(objectName);
        iconObject.transform.SetParent(transform, false);

        RectTransform rectTransform = iconObject.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = iconObject.AddComponent<RectTransform>();
        }

        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(0f, 1f);
        rectTransform.pivot = new Vector2(0f, 1f);
        rectTransform.anchoredPosition = new Vector2(IconStartX + IconSpacing * index, IconY);
        rectTransform.sizeDelta = new Vector2(74f, 34f);

        Image image = iconObject.GetComponent<Image>();
        if (image == null)
        {
            image = iconObject.AddComponent<Image>();
        }

        image.sprite = GetIconSprite();
        image.preserveAspect = true;
        image.raycastTarget = false;

        return iconObject;
    }

    private bool RefreshGreenCrayonIcon(int pencilIconCount)
    {
        bool shouldShow = GameProgress.HasCollectedGreenCrayon || GameProgress.HasUsedGreenCrayon;
        if (!shouldShow)
        {
            if (greenCrayonIconObject != null)
            {
                greenCrayonIconObject.SetActive(false);
            }

            return false;
        }

        if (greenCrayonIconObject == null)
        {
            greenCrayonIconObject = CreateGreenCrayonIconObject();
        }

        greenCrayonIconObject.SetActive(true);

        Image image = greenCrayonIconObject.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = GetGreenCrayonIconSprite();
            image.color = Color.white;
        }

        RectTransform rectTransform = greenCrayonIconObject.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = new Vector2(IconStartX + IconSpacing * Mathf.Max(0, pencilIconCount), IconY);
        }

        return true;
    }

    private bool RefreshBrownCrayonIcon(int iconIndex)
    {
        bool shouldShow = GameProgress.HasCollectedBrownCrayon || GameProgress.HasUsedBrownCrayon;
        if (!shouldShow)
        {
            if (brownCrayonIconObject != null)
            {
                brownCrayonIconObject.SetActive(false);
            }

            return false;
        }

        if (brownCrayonIconObject == null)
        {
            brownCrayonIconObject = CreateBrownCrayonIconObject();
        }

        brownCrayonIconObject.SetActive(true);

        Image image = brownCrayonIconObject.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = GetBrownCrayonIconSprite();
            image.color = Color.white;
        }

        RectTransform rectTransform = brownCrayonIconObject.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = new Vector2(IconStartX + IconSpacing * Mathf.Max(0, iconIndex), IconY);
        }

        return true;
    }

    private bool RefreshBlueCrayonIcon(int iconIndex)
    {
        bool shouldShow = GameProgress.HasCollectedBlueCrayon || GameProgress.HasUsedBlueCrayon;
        if (!shouldShow)
        {
            if (blueCrayonIconObject != null)
            {
                blueCrayonIconObject.SetActive(false);
            }

            return false;
        }

        if (blueCrayonIconObject == null)
        {
            blueCrayonIconObject = CreateBlueCrayonIconObject();
        }

        blueCrayonIconObject.SetActive(true);

        Image image = blueCrayonIconObject.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = GetBlueCrayonIconSprite();
            image.color = Color.white;
        }

        RectTransform rectTransform = blueCrayonIconObject.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = new Vector2(IconStartX + IconSpacing * Mathf.Max(0, iconIndex), IconY);
        }

        return true;
    }

    private GameObject CreateGreenCrayonIconObject()
    {
        GameObject iconObject = new GameObject("GeneratedGreenCrayonHudIcon");
        iconObject.transform.SetParent(transform, false);

        RectTransform rectTransform = iconObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(0f, 1f);
        rectTransform.pivot = new Vector2(0f, 1f);
        rectTransform.anchoredPosition = new Vector2(IconStartX, IconY);
        rectTransform.sizeDelta = new Vector2(74f, 34f);

        Image image = iconObject.AddComponent<Image>();
        image.sprite = GetGreenCrayonIconSprite();
        image.preserveAspect = true;
        image.raycastTarget = false;

        return iconObject;
    }

    private GameObject CreateBrownCrayonIconObject()
    {
        GameObject iconObject = new GameObject("GeneratedBrownCrayonHudIcon");
        iconObject.transform.SetParent(transform, false);

        RectTransform rectTransform = iconObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(0f, 1f);
        rectTransform.pivot = new Vector2(0f, 1f);
        rectTransform.anchoredPosition = new Vector2(IconStartX, IconY);
        rectTransform.sizeDelta = new Vector2(74f, 34f);

        Image image = iconObject.AddComponent<Image>();
        image.sprite = GetBrownCrayonIconSprite();
        image.preserveAspect = true;
        image.raycastTarget = false;

        return iconObject;
    }

    private GameObject CreateBlueCrayonIconObject()
    {
        GameObject iconObject = new GameObject("GeneratedBlueCrayonHudIcon");
        iconObject.transform.SetParent(transform, false);

        RectTransform rectTransform = iconObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(0f, 1f);
        rectTransform.pivot = new Vector2(0f, 1f);
        rectTransform.anchoredPosition = new Vector2(IconStartX, IconY);
        rectTransform.sizeDelta = new Vector2(74f, 34f);

        Image image = iconObject.AddComponent<Image>();
        image.sprite = GetBlueCrayonIconSprite();
        image.preserveAspect = true;
        image.raycastTarget = false;

        return iconObject;
    }

    private void HideChildrenWithPrefix(string prefix)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child != null && child.name.StartsWith(prefix))
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    private static Sprite GetIconSprite()
    {
        if (iconSprite != null)
        {
            return iconSprite;
        }

        const int width = 96;
        const int height = 44;
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;

        Color clear = new Color(0f, 0f, 0f, 0f);
        Color outline = new Color32(38, 34, 28, 255);
        Color yellow = new Color32(236, 181, 51, 255);
        Color wood = new Color32(203, 143, 77, 255);
        Color lead = new Color32(44, 40, 36, 255);
        Color red = new Color32(190, 66, 72, 255);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                texture.SetPixel(x, y, clear);
            }
        }

        FillRect(texture, 8, 14, 58, 15, outline);
        FillRect(texture, 10, 16, 54, 11, yellow);
        FillRect(texture, 64, 14, 15, 15, outline);
        FillRect(texture, 64, 16, 12, 11, wood);
        FillRect(texture, 76, 17, 9, 9, outline);
        FillRect(texture, 76, 19, 6, 5, lead);
        FillRect(texture, 5, 13, 9, 17, outline);
        FillRect(texture, 7, 15, 5, 13, red);

        texture.Apply();
        iconSprite = Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
        iconSprite.name = "GeneratedPencilFragmentHudSprite";
        return iconSprite;
    }

    private static Sprite GetUsedIconSprite()
    {
        if (usedIconSprite != null)
        {
            return usedIconSprite;
        }

        const int width = 96;
        const int height = 44;
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;

        Color clear = new Color(0f, 0f, 0f, 0f);
        Color outline = new Color32(60, 58, 55, 255);
        Color body = new Color32(166, 166, 158, 255);
        Color wood = new Color32(145, 140, 132, 255);
        Color lead = new Color32(64, 62, 58, 255);
        Color edge = new Color32(116, 112, 105, 255);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                texture.SetPixel(x, y, clear);
            }
        }

        FillRect(texture, 8, 14, 58, 15, outline);
        FillRect(texture, 10, 16, 54, 11, body);
        FillRect(texture, 64, 14, 15, 15, outline);
        FillRect(texture, 64, 16, 12, 11, wood);
        FillRect(texture, 76, 17, 9, 9, outline);
        FillRect(texture, 76, 19, 6, 5, lead);
        FillRect(texture, 5, 13, 9, 17, outline);
        FillRect(texture, 7, 15, 5, 13, edge);

        texture.Apply();
        usedIconSprite = Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
        usedIconSprite.name = "GeneratedUsedPencilFragmentHudSprite";
        return usedIconSprite;
    }

    private static Sprite GetGreenCrayonIconSprite()
    {
        if (greenCrayonIconSprite != null)
        {
            return greenCrayonIconSprite;
        }

        const int width = 96;
        const int height = 44;
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;

        Color clear = new Color(0f, 0f, 0f, 0f);
        Color outline = new Color32(30, 58, 34, 255);
        Color green = new Color32(72, 174, 80, 255);
        Color darkGreen = new Color32(36, 116, 55, 255);
        Color wrapper = new Color32(205, 236, 158, 255);
        Color edge = new Color32(44, 132, 64, 255);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                texture.SetPixel(x, y, clear);
            }
        }

        FillRect(texture, 8, 14, 62, 15, outline);
        FillRect(texture, 10, 16, 58, 11, green);
        FillRect(texture, 66, 15, 14, 13, outline);
        FillRect(texture, 66, 18, 10, 7, darkGreen);
        FillRect(texture, 25, 13, 15, 17, outline);
        FillRect(texture, 27, 15, 11, 13, wrapper);
        FillRect(texture, 5, 13, 9, 17, outline);
        FillRect(texture, 7, 15, 5, 13, edge);

        texture.Apply();
        greenCrayonIconSprite = Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
        greenCrayonIconSprite.name = "GeneratedGreenCrayonHudSprite";
        return greenCrayonIconSprite;
    }

    private static Sprite GetBrownCrayonIconSprite()
    {
        if (brownCrayonIconSprite != null)
        {
            return brownCrayonIconSprite;
        }

        const int width = 96;
        const int height = 44;
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;

        Color clear = new Color(0f, 0f, 0f, 0f);
        Color outline = new Color32(66, 42, 25, 255);
        Color brown = new Color32(151, 87, 42, 255);
        Color darkBrown = new Color32(93, 54, 31, 255);
        Color wrapper = new Color32(229, 185, 121, 255);
        Color edge = new Color32(111, 64, 33, 255);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                texture.SetPixel(x, y, clear);
            }
        }

        FillRect(texture, 8, 14, 62, 15, outline);
        FillRect(texture, 10, 16, 58, 11, brown);
        FillRect(texture, 66, 15, 14, 13, outline);
        FillRect(texture, 66, 18, 10, 7, darkBrown);
        FillRect(texture, 25, 13, 15, 17, outline);
        FillRect(texture, 27, 15, 11, 13, wrapper);
        FillRect(texture, 5, 13, 9, 17, outline);
        FillRect(texture, 7, 15, 5, 13, edge);

        texture.Apply();
        brownCrayonIconSprite = Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
        brownCrayonIconSprite.name = "GeneratedBrownCrayonHudSprite";
        return brownCrayonIconSprite;
    }

    private static Sprite GetBlueCrayonIconSprite()
    {
        if (blueCrayonIconSprite != null)
        {
            return blueCrayonIconSprite;
        }

        const int width = 96;
        const int height = 44;
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;

        Color clear = new Color(0f, 0f, 0f, 0f);
        Color outline = new Color32(31, 62, 91, 255);
        Color blue = new Color32(55, 145, 218, 255);
        Color darkBlue = new Color32(30, 83, 153, 255);
        Color wrapper = new Color32(180, 223, 248, 255);
        Color edge = new Color32(35, 102, 176, 255);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                texture.SetPixel(x, y, clear);
            }
        }

        FillRect(texture, 8, 14, 62, 15, outline);
        FillRect(texture, 10, 16, 58, 11, blue);
        FillRect(texture, 66, 15, 14, 13, outline);
        FillRect(texture, 66, 18, 10, 7, darkBlue);
        FillRect(texture, 25, 13, 15, 17, outline);
        FillRect(texture, 27, 15, 11, 13, wrapper);
        FillRect(texture, 5, 13, 9, 17, outline);
        FillRect(texture, 7, 15, 5, 13, edge);

        texture.Apply();
        blueCrayonIconSprite = Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
        blueCrayonIconSprite.name = "GeneratedBlueCrayonHudSprite";
        return blueCrayonIconSprite;
    }

    private static void FillRect(Texture2D texture, int startX, int startY, int width, int height, Color color)
    {
        for (int y = startY; y < startY + height; y++)
        {
            for (int x = startX; x < startX + width; x++)
            {
                if (x >= 0 && x < texture.width && y >= 0 && y < texture.height)
                {
                    texture.SetPixel(x, y, color);
                }
            }
        }
    }
}

public class LetterQuestHud : MonoBehaviour
{
    private static LetterQuestHud instance;
    private static Sprite fragmentSprite;
    private static Sprite completedLetterSprite;
    private static Sprite waterBottleSprite;
    private static Sprite acornSprite;
    private static Sprite mushroomSprite;
    private static Sprite forestBranchSprite;
    private static Sprite glowingLeafSprite;

    private const float IconStartX = 18f;
    private const float IconY = -70f;
    private const float FragmentIconSpacing = 42f;
    private const float CompletedLetterWaterBottleOffset = 82f;
    private const float CollectibleIconSpacing = 42f;

    private readonly List<GameObject> iconObjects = new List<GameObject>();
    private GameObject waterBottleIconObject;
    private GameObject acornIconObject;
    private GameObject mushroomIconObject;
    private GameObject forestBranchIconObject;
    private GameObject glowingLeafIconObject;

    public static void ShowProgress()
    {
        LetterQuestHud hud = GetOrCreate();
        hud.gameObject.SetActive(true);
        hud.RefreshIcons();
    }

    private static LetterQuestHud GetOrCreate()
    {
        if (instance != null)
        {
            return instance;
        }

        GameObject hudObject = new GameObject("GeneratedLetterQuestHud");
        Canvas canvas = hudObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 501;

        CanvasScaler canvasScaler = hudObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
        hudObject.AddComponent<GraphicRaycaster>();

        instance = hudObject.AddComponent<LetterQuestHud>();
        instance.RefreshIcons();
        return instance;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void RefreshIcons()
    {
        int collectedCount = GameProgress.AvailableLetterFragmentCount;
        bool hasWaterBottleIcon = GameProgress.HasCollectedWellWaterBottle;
        bool hasAcornIcon = GameProgress.HasCollectedAcorn;
        bool hasMushroomIcon = GameProgress.HasCollectedMushroom;
        bool hasForestBranchIcon = GameProgress.HasCollectedForestBranch;
        bool hasGlowingLeafIcon = GameProgress.HasCollectedGlowingLeaf;
        bool shouldReserveDeliveredLetterSlot = GameProgress.HasDeliveredCompletedLetter;
        HideChildrenWithPrefix("GeneratedLetterQuestHudIcon_");

        if (collectedCount <= 0
            && !hasWaterBottleIcon
            && !hasAcornIcon
            && !hasMushroomIcon
            && !hasForestBranchIcon
            && !hasGlowingLeafIcon)
        {
            for (int i = 0; i < iconObjects.Count; i++)
            {
                if (iconObjects[i] != null)
                {
                    iconObjects[i].SetActive(false);
                }
            }

            if (waterBottleIconObject != null)
            {
                waterBottleIconObject.SetActive(false);
            }

            if (acornIconObject != null)
            {
                acornIconObject.SetActive(false);
            }

            if (mushroomIconObject != null)
            {
                mushroomIconObject.SetActive(false);
            }

            if (forestBranchIconObject != null)
            {
                forestBranchIconObject.SetActive(false);
            }

            if (glowingLeafIconObject != null)
            {
                glowingLeafIconObject.SetActive(false);
            }

            gameObject.SetActive(false);
            return;
        }

        bool hasCompletedLetter = GameProgress.HasCompletedLetter;
        int visibleCount = hasCompletedLetter ? 1 : collectedCount;

        while (iconObjects.Count < visibleCount)
        {
            iconObjects.Add(CreateIconObject(iconObjects.Count));
        }

        for (int i = 0; i < iconObjects.Count; i++)
        {
            GameObject iconObject = iconObjects[i];
            if (iconObject == null)
            {
                continue;
            }

            bool shouldShow = i < visibleCount;
            iconObject.SetActive(shouldShow);

            if (!shouldShow)
            {
                continue;
            }

            RectTransform rectTransform = iconObject.GetComponent<RectTransform>();
            Image image = iconObject.GetComponent<Image>();

            if (hasCompletedLetter)
            {
                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = new Vector2(IconStartX, IconY);
                    rectTransform.sizeDelta = new Vector2(74f, 48f);
                }

                if (image != null)
                {
                    image.sprite = GetCompletedLetterSprite();
                }

                continue;
            }

            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = new Vector2(IconStartX + FragmentIconSpacing * i, IconY);
                rectTransform.sizeDelta = new Vector2(34f, 34f);
            }

            if (image != null)
            {
                image.sprite = GetFragmentSprite();
            }
        }

        float nextCollectibleX = GetCollectibleIconX(hasCompletedLetter, shouldReserveDeliveredLetterSlot, visibleCount, 0);
        RefreshWaterBottleIcon(hasWaterBottleIcon, nextCollectibleX);
        if (hasWaterBottleIcon)
        {
            nextCollectibleX += CollectibleIconSpacing;
        }

        RefreshAcornIcon(hasAcornIcon, nextCollectibleX);
        if (hasAcornIcon)
        {
            nextCollectibleX += CollectibleIconSpacing;
        }

        RefreshMushroomIcon(hasMushroomIcon, nextCollectibleX);
        if (hasMushroomIcon)
        {
            nextCollectibleX += CollectibleIconSpacing;
        }

        RefreshForestBranchIcon(hasForestBranchIcon, nextCollectibleX);
        if (hasForestBranchIcon)
        {
            nextCollectibleX += CollectibleIconSpacing;
        }

        RefreshGlowingLeafIcon(hasGlowingLeafIcon, nextCollectibleX);
    }

    private GameObject CreateIconObject(int index)
    {
        string objectName = $"GeneratedLetterQuestHudIcon_{index:00}";
        Transform existing = transform.Find(objectName);
        GameObject iconObject = existing != null ? existing.gameObject : new GameObject(objectName);
        iconObject.transform.SetParent(transform, false);

        RectTransform rectTransform = iconObject.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = iconObject.AddComponent<RectTransform>();
        }

        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(0f, 1f);
        rectTransform.pivot = new Vector2(0f, 1f);
        rectTransform.anchoredPosition = new Vector2(IconStartX + FragmentIconSpacing * index, IconY);
        rectTransform.sizeDelta = new Vector2(34f, 34f);

        Image image = iconObject.GetComponent<Image>();
        if (image == null)
        {
            image = iconObject.AddComponent<Image>();
        }

        image.sprite = GetFragmentSprite();
        image.preserveAspect = true;
        image.raycastTarget = false;

        return iconObject;
    }

    private void RefreshWaterBottleIcon(bool shouldShow, float x)
    {
        if (!shouldShow)
        {
            if (waterBottleIconObject != null)
            {
                waterBottleIconObject.SetActive(false);
            }

            return;
        }

        if (waterBottleIconObject == null)
        {
            waterBottleIconObject = CreateWaterBottleIconObject();
        }

        waterBottleIconObject.SetActive(true);

        Image image = waterBottleIconObject.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = GetWaterBottleSprite();
            image.color = Color.white;
        }

        RectTransform rectTransform = waterBottleIconObject.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = new Vector2(x, IconY + 2f);
            rectTransform.sizeDelta = new Vector2(34f, 46f);
        }
    }

    private GameObject CreateWaterBottleIconObject()
    {
        GameObject iconObject = new GameObject("GeneratedLetterQuestHudWaterBottleIcon");
        iconObject.transform.SetParent(transform, false);

        RectTransform rectTransform = iconObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(0f, 1f);
        rectTransform.pivot = new Vector2(0f, 1f);
        rectTransform.anchoredPosition = new Vector2(IconStartX, IconY + 2f);
        rectTransform.sizeDelta = new Vector2(34f, 46f);

        Image image = iconObject.AddComponent<Image>();
        image.sprite = GetWaterBottleSprite();
        image.preserveAspect = true;
        image.raycastTarget = false;

        return iconObject;
    }

    private void RefreshAcornIcon(bool shouldShow, float x)
    {
        if (!shouldShow)
        {
            if (acornIconObject != null)
            {
                acornIconObject.SetActive(false);
            }

            return;
        }

        if (acornIconObject == null)
        {
            acornIconObject = CreateAcornIconObject();
        }

        acornIconObject.SetActive(true);

        Image image = acornIconObject.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = GetAcornSprite();
            image.color = Color.white;
        }

        RectTransform rectTransform = acornIconObject.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = new Vector2(x, IconY + 8f);
            rectTransform.sizeDelta = new Vector2(34f, 34f);
        }
    }

    private GameObject CreateAcornIconObject()
    {
        GameObject iconObject = new GameObject("GeneratedLetterQuestHudAcornIcon");
        iconObject.transform.SetParent(transform, false);

        RectTransform rectTransform = iconObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(0f, 1f);
        rectTransform.pivot = new Vector2(0f, 1f);
        rectTransform.anchoredPosition = new Vector2(IconStartX + CollectibleIconSpacing, IconY + 8f);
        rectTransform.sizeDelta = new Vector2(34f, 34f);

        Image image = iconObject.AddComponent<Image>();
        image.sprite = GetAcornSprite();
        image.preserveAspect = true;
        image.raycastTarget = false;

        return iconObject;
    }

    private void RefreshMushroomIcon(bool shouldShow, float x)
    {
        if (!shouldShow)
        {
            if (mushroomIconObject != null)
            {
                mushroomIconObject.SetActive(false);
            }

            return;
        }

        if (mushroomIconObject == null)
        {
            mushroomIconObject = CreateMushroomIconObject();
        }

        mushroomIconObject.SetActive(true);

        Image image = mushroomIconObject.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = GetMushroomSprite();
            image.color = Color.white;
        }

        RectTransform rectTransform = mushroomIconObject.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = new Vector2(x, IconY + 7f);
            rectTransform.sizeDelta = new Vector2(36f, 36f);
        }
    }

    private GameObject CreateMushroomIconObject()
    {
        GameObject iconObject = new GameObject("GeneratedLetterQuestHudMushroomIcon");
        iconObject.transform.SetParent(transform, false);

        RectTransform rectTransform = iconObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(0f, 1f);
        rectTransform.pivot = new Vector2(0f, 1f);
        rectTransform.anchoredPosition = new Vector2(IconStartX + CollectibleIconSpacing * 2f, IconY + 7f);
        rectTransform.sizeDelta = new Vector2(36f, 36f);

        Image image = iconObject.AddComponent<Image>();
        image.sprite = GetMushroomSprite();
        image.preserveAspect = true;
        image.raycastTarget = false;

        return iconObject;
    }

    private void RefreshForestBranchIcon(bool shouldShow, float x)
    {
        if (!shouldShow)
        {
            if (forestBranchIconObject != null)
            {
                forestBranchIconObject.SetActive(false);
            }

            return;
        }

        if (forestBranchIconObject == null)
        {
            forestBranchIconObject = CreateForestBranchIconObject();
        }

        forestBranchIconObject.SetActive(true);

        Image image = forestBranchIconObject.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = GetForestBranchSprite();
            image.color = Color.white;
        }

        RectTransform rectTransform = forestBranchIconObject.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = new Vector2(x, IconY + 8f);
            rectTransform.sizeDelta = new Vector2(38f, 34f);
        }
    }

    private GameObject CreateForestBranchIconObject()
    {
        GameObject iconObject = new GameObject("GeneratedLetterQuestHudForestBranchIcon");
        iconObject.transform.SetParent(transform, false);

        RectTransform rectTransform = iconObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(0f, 1f);
        rectTransform.pivot = new Vector2(0f, 1f);
        rectTransform.anchoredPosition = new Vector2(IconStartX + CollectibleIconSpacing * 3f, IconY + 8f);
        rectTransform.sizeDelta = new Vector2(38f, 34f);

        Image image = iconObject.AddComponent<Image>();
        image.sprite = GetForestBranchSprite();
        image.preserveAspect = true;
        image.raycastTarget = false;

        return iconObject;
    }

    private void RefreshGlowingLeafIcon(bool shouldShow, float x)
    {
        if (!shouldShow)
        {
            if (glowingLeafIconObject != null)
            {
                glowingLeafIconObject.SetActive(false);
            }

            return;
        }

        if (glowingLeafIconObject == null)
        {
            glowingLeafIconObject = CreateGlowingLeafIconObject();
        }

        glowingLeafIconObject.SetActive(true);

        Image image = glowingLeafIconObject.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = GetGlowingLeafSprite();
            image.color = Color.white;
        }

        RectTransform rectTransform = glowingLeafIconObject.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = new Vector2(x, IconY + 7f);
            rectTransform.sizeDelta = new Vector2(34f, 36f);
        }
    }

    private GameObject CreateGlowingLeafIconObject()
    {
        GameObject iconObject = new GameObject("GeneratedLetterQuestHudGlowingLeafIcon");
        iconObject.transform.SetParent(transform, false);

        RectTransform rectTransform = iconObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(0f, 1f);
        rectTransform.pivot = new Vector2(0f, 1f);
        rectTransform.anchoredPosition = new Vector2(IconStartX + CollectibleIconSpacing * 4f, IconY + 7f);
        rectTransform.sizeDelta = new Vector2(34f, 36f);

        Image image = iconObject.AddComponent<Image>();
        image.sprite = GetGlowingLeafSprite();
        image.preserveAspect = true;
        image.raycastTarget = false;

        return iconObject;
    }

    private static float GetCollectibleIconX(bool hasCompletedLetter, bool reserveCompletedLetterSlot, int visibleLetterIconCount, int collectibleIndex)
    {
        float x = IconStartX;
        if (visibleLetterIconCount > 0)
        {
            x += hasCompletedLetter
                ? CompletedLetterWaterBottleOffset
                : FragmentIconSpacing * visibleLetterIconCount;
        }
        else if (reserveCompletedLetterSlot)
        {
            x += CompletedLetterWaterBottleOffset;
        }

        return x + CollectibleIconSpacing * collectibleIndex;
    }

    private static float GetAcornIconX(
        bool hasWaterBottleIcon,
        float waterBottleX,
        bool hasCompletedLetter,
        bool reserveCompletedLetterSlot,
        int visibleLetterIconCount)
    {
        if (hasWaterBottleIcon)
        {
            return waterBottleX + CollectibleIconSpacing;
        }

        return GetCollectibleIconX(hasCompletedLetter, reserveCompletedLetterSlot, visibleLetterIconCount, 0);
    }

    private static float GetMushroomIconX(
        bool hasAcornIcon,
        float acornX,
        bool hasWaterBottleIcon,
        float waterBottleX,
        bool hasCompletedLetter,
        bool reserveCompletedLetterSlot,
        int visibleLetterIconCount)
    {
        if (hasAcornIcon)
        {
            return acornX + CollectibleIconSpacing;
        }

        if (hasWaterBottleIcon)
        {
            return waterBottleX + CollectibleIconSpacing;
        }

        return GetCollectibleIconX(hasCompletedLetter, reserveCompletedLetterSlot, visibleLetterIconCount, 0);
    }

    private void HideChildrenWithPrefix(string prefix)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child != null && child.name.StartsWith(prefix))
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    private static Sprite GetFragmentSprite()
    {
        if (fragmentSprite != null)
        {
            return fragmentSprite;
        }

        const int width = 42;
        const int height = 42;
        Texture2D texture = CreateClearTexture(width, height);
        Color outline = new Color32(45, 39, 32, 255);
        Color paper = new Color32(250, 241, 206, 255);
        Color crease = new Color32(176, 143, 92, 255);

        FillRect(texture, 9, 9, 24, 25, outline);
        FillRect(texture, 11, 11, 20, 21, paper);
        FillRect(texture, 25, 9, 8, 8, outline);
        FillRect(texture, 25, 11, 5, 5, paper);
        FillRect(texture, 13, 18, 16, 2, crease);
        FillRect(texture, 13, 25, 10, 2, crease);

        texture.Apply();
        fragmentSprite = Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
        fragmentSprite.name = "GeneratedLetterFragmentHudSprite";
        return fragmentSprite;
    }

    private static Sprite GetCompletedLetterSprite()
    {
        if (completedLetterSprite != null)
        {
            return completedLetterSprite;
        }

        const int width = 92;
        const int height = 60;
        Texture2D texture = CreateClearTexture(width, height);
        Color outline = new Color32(45, 39, 32, 255);
        Color paper = new Color32(250, 241, 206, 255);
        Color fold = new Color32(205, 167, 103, 255);
        Color seal = new Color32(178, 66, 70, 255);

        FillRect(texture, 10, 12, 72, 36, outline);
        FillRect(texture, 13, 15, 66, 30, paper);
        FillRect(texture, 16, 17, 30, 3, fold);
        FillRect(texture, 46, 17, 30, 3, fold);
        FillRect(texture, 18, 22, 25, 3, fold);
        FillRect(texture, 49, 22, 25, 3, fold);
        FillRect(texture, 43, 28, 8, 8, seal);
        FillRect(texture, 39, 25, 4, 3, outline);
        FillRect(texture, 51, 25, 4, 3, outline);
        FillRect(texture, 13, 15, 5, 3, outline);
        FillRect(texture, 74, 15, 5, 3, outline);

        texture.Apply();
        completedLetterSprite = Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
        completedLetterSprite.name = "GeneratedCompletedLetterHudSprite";
        return completedLetterSprite;
    }

    private static Sprite GetWaterBottleSprite()
    {
        if (waterBottleSprite != null)
        {
            return waterBottleSprite;
        }

        const int width = 44;
        const int height = 60;
        Texture2D texture = CreateClearTexture(width, height);
        Color outline = new Color32(36, 78, 88, 255);
        Color glass = new Color32(178, 224, 230, 205);
        Color water = new Color32(72, 151, 190, 225);
        Color cork = new Color32(134, 86, 42, 255);
        Color shine = new Color32(246, 255, 255, 225);

        FillRect(texture, 17, 5, 10, 7, outline);
        FillRect(texture, 19, 7, 6, 5, cork);
        FillRect(texture, 14, 12, 16, 5, outline);
        FillRect(texture, 16, 14, 12, 3, glass);
        FillRect(texture, 10, 17, 24, 35, outline);
        FillRect(texture, 13, 20, 18, 29, glass);
        FillRect(texture, 13, 34, 18, 15, water);
        FillRect(texture, 16, 23, 3, 14, shine);
        FillRect(texture, 28, 24, 2, 20, new Color32(106, 175, 200, 185));

        texture.Apply();
        waterBottleSprite = Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
        waterBottleSprite.name = "GeneratedWaterBottleHudSprite";
        return waterBottleSprite;
    }

    private static Sprite GetAcornSprite()
    {
        if (acornSprite != null)
        {
            return acornSprite;
        }

        const int width = 44;
        const int height = 44;
        Texture2D texture = CreateClearTexture(width, height);
        Color outline = new Color32(63, 42, 24, 255);
        Color cap = new Color32(113, 72, 35, 255);
        Color capLight = new Color32(153, 101, 50, 255);
        Color body = new Color32(172, 103, 43, 255);
        Color bodyLight = new Color32(207, 136, 62, 255);
        Color shine = new Color32(232, 174, 92, 210);

        FillRect(texture, 19, 5, 6, 6, outline);
        FillRect(texture, 21, 3, 3, 6, outline);
        FillRect(texture, 10, 11, 24, 10, outline);
        FillRect(texture, 12, 13, 20, 7, cap);
        FillRect(texture, 14, 13, 5, 2, capLight);
        FillRect(texture, 23, 14, 6, 2, capLight);
        FillRect(texture, 8, 18, 28, 8, outline);
        FillRect(texture, 11, 20, 22, 5, cap);
        FillRect(texture, 11, 24, 22, 13, outline);
        FillRect(texture, 14, 25, 16, 10, body);
        FillRect(texture, 16, 26, 5, 5, bodyLight);
        FillRect(texture, 22, 28, 3, 5, shine);
        FillRect(texture, 18, 35, 8, 4, outline);
        FillRect(texture, 20, 35, 4, 2, body);

        texture.Apply();
        acornSprite = Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
        acornSprite.name = "GeneratedAcornHudSprite";
        return acornSprite;
    }

    private static Sprite GetMushroomSprite()
    {
        if (mushroomSprite != null)
        {
            return mushroomSprite;
        }

        const int width = 44;
        const int height = 44;
        Texture2D texture = CreateClearTexture(width, height);
        Color outline = new Color32(70, 34, 34, 255);
        Color cap = new Color32(191, 62, 55, 255);
        Color capDark = new Color32(139, 43, 45, 255);
        Color spot = new Color32(255, 230, 190, 255);
        Color stem = new Color32(235, 205, 158, 255);
        Color stemShade = new Color32(184, 141, 92, 255);

        FillRect(texture, 10, 13, 24, 5, outline);
        FillRect(texture, 7, 17, 30, 7, outline);
        FillRect(texture, 10, 15, 24, 7, cap);
        FillRect(texture, 12, 21, 20, 5, capDark);
        FillRect(texture, 15, 15, 4, 3, spot);
        FillRect(texture, 25, 16, 4, 3, spot);
        FillRect(texture, 20, 21, 3, 3, spot);
        FillRect(texture, 16, 24, 13, 14, outline);
        FillRect(texture, 18, 25, 9, 11, stem);
        FillRect(texture, 25, 27, 2, 8, stemShade);
        FillRect(texture, 14, 35, 17, 4, outline);
        FillRect(texture, 17, 35, 11, 2, stem);

        texture.Apply();
        mushroomSprite = Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
        mushroomSprite.name = "GeneratedMushroomHudSprite";
        return mushroomSprite;
    }

    private static Sprite GetForestBranchSprite()
    {
        if (forestBranchSprite != null)
        {
            return forestBranchSprite;
        }

        const int width = 48;
        const int height = 44;
        Texture2D texture = CreateClearTexture(width, height);
        Color outline = new Color32(58, 37, 22, 255);
        Color bark = new Color32(112, 72, 38, 255);
        Color barkLight = new Color32(158, 101, 51, 255);
        Color leaf = new Color32(79, 151, 68, 255);
        Color leafLight = new Color32(126, 188, 76, 255);

        FillRect(texture, 7, 29, 30, 6, outline);
        FillRect(texture, 10, 30, 26, 3, bark);
        FillRect(texture, 14, 28, 14, 2, barkLight);
        FillRect(texture, 27, 23, 6, 12, outline);
        FillRect(texture, 29, 24, 3, 9, bark);
        FillRect(texture, 12, 20, 6, 12, outline);
        FillRect(texture, 14, 22, 3, 9, bark);
        FillRect(texture, 8, 17, 11, 8, outline);
        FillRect(texture, 10, 18, 8, 5, leaf);
        FillRect(texture, 11, 18, 4, 2, leafLight);
        FillRect(texture, 29, 13, 12, 8, outline);
        FillRect(texture, 31, 14, 8, 5, leaf);
        FillRect(texture, 32, 14, 4, 2, leafLight);
        FillRect(texture, 34, 31, 7, 4, outline);
        FillRect(texture, 35, 31, 5, 2, bark);

        texture.Apply();
        forestBranchSprite = Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
        forestBranchSprite.name = "GeneratedForestBranchHudSprite";
        return forestBranchSprite;
    }

    private static Sprite GetGlowingLeafSprite()
    {
        if (glowingLeafSprite != null)
        {
            return glowingLeafSprite;
        }

        const int width = 44;
        const int height = 44;
        Texture2D texture = CreateClearTexture(width, height);
        Color glow = new Color32(255, 236, 95, 105);
        Color glowStrong = new Color32(255, 240, 98, 150);
        Color outline = new Color32(105, 89, 20, 255);
        Color leaf = new Color32(228, 204, 49, 255);
        Color leafLight = new Color32(255, 237, 102, 255);
        Color vein = new Color32(145, 122, 25, 255);

        FillRect(texture, 12, 9, 20, 25, glow);
        FillRect(texture, 9, 15, 26, 13, glowStrong);
        FillRect(texture, 19, 7, 7, 5, outline);
        FillRect(texture, 15, 10, 15, 4, outline);
        FillRect(texture, 12, 14, 21, 7, outline);
        FillRect(texture, 14, 21, 17, 7, outline);
        FillRect(texture, 17, 28, 10, 5, outline);
        FillRect(texture, 18, 10, 8, 3, leafLight);
        FillRect(texture, 15, 14, 15, 6, leaf);
        FillRect(texture, 16, 20, 13, 7, leaf);
        FillRect(texture, 19, 27, 6, 4, leaf);
        FillRect(texture, 21, 11, 3, 20, vein);
        FillRect(texture, 18, 18, 3, 2, vein);
        FillRect(texture, 24, 22, 3, 2, vein);
        FillRect(texture, 18, 32, 5, 4, vein);

        texture.Apply();
        glowingLeafSprite = Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
        glowingLeafSprite.name = "GeneratedGlowingLeafHudSprite";
        return glowingLeafSprite;
    }

    private static Texture2D CreateClearTexture(int width, int height)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;

        Color clear = new Color(0f, 0f, 0f, 0f);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                texture.SetPixel(x, y, clear);
            }
        }

        return texture;
    }

    private static void FillRect(Texture2D texture, int startX, int startY, int width, int height, Color color)
    {
        for (int y = startY; y < startY + height; y++)
        {
            for (int x = startX; x < startX + width; x++)
            {
                if (x >= 0 && x < texture.width && y >= 0 && y < texture.height)
                {
                    texture.SetPixel(x, y, color);
                }
            }
        }
    }
}
