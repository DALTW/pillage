using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PencilFragmentHud : MonoBehaviour
{
    private static PencilFragmentHud instance;
    private static Sprite iconSprite;

    private const float IconStartX = 18f;
    private const float IconY = -18f;
    private const float IconSpacing = 78f;

    private readonly List<GameObject> iconObjects = new List<GameObject>();

    public static void ShowCollected()
    {
        PencilFragmentHud hud = GetOrCreate();
        hud.gameObject.SetActive(true);
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

        int count = Mathf.Max(1, GameProgress.CollectedPencilFragmentCount);

        while (iconObjects.Count < count)
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

            bool shouldShow = i < count;
            iconObject.SetActive(shouldShow);

            if (!shouldShow)
            {
                continue;
            }

            RectTransform rectTransform = iconObject.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = new Vector2(IconStartX + IconSpacing * i, IconY);
            }
        }
    }

    private GameObject CreateIconObject(int index)
    {
        GameObject iconObject = new GameObject($"GeneratedPencilFragmentHudIcon_{index:00}");
        iconObject.transform.SetParent(transform, false);

        RectTransform rectTransform = iconObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(0f, 1f);
        rectTransform.pivot = new Vector2(0f, 1f);
        rectTransform.anchoredPosition = new Vector2(IconStartX + IconSpacing * index, IconY);
        rectTransform.sizeDelta = new Vector2(74f, 34f);

        Image image = iconObject.AddComponent<Image>();
        image.sprite = GetIconSprite();
        image.preserveAspect = true;
        image.raycastTarget = false;

        return iconObject;
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
