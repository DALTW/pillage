using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PencilFragmentHud : MonoBehaviour
{
    private static PencilFragmentHud instance;
    private static Sprite iconSprite;
    private static Sprite usedIconSprite;
    private static Sprite greenCrayonIconSprite;

    private const float IconStartX = 18f;
    private const float IconY = -18f;
    private const float IconSpacing = 78f;

    private readonly List<GameObject> iconObjects = new List<GameObject>();
    private GameObject greenCrayonIconObject;

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

        bool hasGreenCrayonIcon = RefreshGreenCrayonIcon(collectedCount);

        if (collectedCount <= 0 && !hasGreenCrayonIcon)
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

    private const float IconStartX = 18f;
    private const float IconY = -70f;
    private const float FragmentIconSpacing = 42f;

    private readonly List<GameObject> iconObjects = new List<GameObject>();

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
        HideChildrenWithPrefix("GeneratedLetterQuestHudIcon_");

        if (collectedCount <= 0)
        {
            for (int i = 0; i < iconObjects.Count; i++)
            {
                if (iconObjects[i] != null)
                {
                    iconObjects[i].SetActive(false);
                }
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
