using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class WordNode : MonoBehaviour, IPointerEnterHandler
{
    [Header("Node Settings")]
    public TextMeshProUGUI wordText;
    public Image background;
    public Color normalColor = Color.white;
    public Color highlightColor = Color.yellow;

    private int nodeId;
    private string word;
    private GameManager gameManager;
    private bool isHighlighted = false;

    void Awake()
    {
        // Get components if not assigned
        if (wordText == null)
            wordText = GetComponentInChildren<TextMeshProUGUI>();

        if (background == null)
            background = GetComponent<Image>();

        // Ensure we have required components
        if (background == null)
        {
            background = gameObject.AddComponent<Image>();
            background.color = normalColor;
        }

        // Ensure we have a RectTransform
        if (GetComponent<RectTransform>() == null)
        {
            gameObject.AddComponent<RectTransform>();
        }

        if (wordText == null)
        {
            GameObject textObj = new GameObject("WordText");
            textObj.transform.SetParent(transform);
            wordText = textObj.AddComponent<TextMeshProUGUI>();
            wordText.fontSize = 16;
            wordText.text = "Sample";
            wordText.color = Color.black;
            wordText.alignment = TextAlignmentOptions.Center;

            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
        }

        // Ensure the image can be raycast
        if (background != null)
        {
            background.raycastTarget = true;
        }
    }

    public void Initialize(int id, string nodeWord, GameManager manager)
    {
        nodeId = id;
        word = nodeWord;
        gameManager = manager;

        if (wordText != null)
        {
            wordText.text = word;
        }

        SetHighlighted(false);
    }

    public void SetHighlighted(bool highlighted)
    {
        isHighlighted = highlighted;

        if (background != null)
        {
            background.color = highlighted ? highlightColor : normalColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (gameManager != null)
        {
            gameManager.OnNodeHovered(this);
        }
    }

    public string GetWord()
    {
        return word;
    }

    public int GetNodeId()
    {
        return nodeId;
    }
}
