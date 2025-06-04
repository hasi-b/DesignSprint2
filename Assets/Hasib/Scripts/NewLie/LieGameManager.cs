// GameManager.cs - Enhanced version with UILineRenderer for pattern drawing
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;
using System.Collections;

public class LieGameManager : MonoBehaviour
{
    [Header("HoodlumReaction")]
    [SerializeField]Animator hoodlumAnimator;
    [Header("Game Settings")]
    public float gameTime = 30f;
    public int currentLevel = 1;
    bool isGameRunning;

    [Header("Level System")]
    public LevelData[] levels;
    public bool useRandomLevels = false;

    [Header("UI References")]
    public Canvas gameCanvas;
    public Slider timerSlider;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI qustionText;
    public Button restartButton;
    public Button nextLevelButton;

    [Header("Node Settings")]
    public GameObject nodePrefab;
    public Transform nodeParent;
    public float circleRadius = 250f;
    public Vector2 centerOffset = Vector2.zero;

    [Header("Line Drawing Settings")]
    public UILineRenderer uiLineRenderer; // Assign your UILineRenderer component
    public Color lineColor = Color.blue;
    public float lineWidth = 5f;
    public Color completedLineColor = Color.green;
    public Color errorLineColor = Color.red;
    public float lineFadeTime = 1f; // Time to fade out lines after pattern check

    private List<WordNode> nodes = new List<WordNode>();
    private List<WordNode> currentPattern = new List<WordNode>();
    private float currentTime;
    private int totalScore = 0;
    private bool isGameActive = false;
    private bool isDragging = false;
    private Camera gameCamera;
    private LevelData currentLevelData;
    private int patternsCompleted = 0;

    // Line drawing variables
    private List<Vector2> currentLinePoints = new List<Vector2>();
    private Vector2 currentMousePosition;

    [System.Serializable]
    public class LevelData
    {
        public string levelName;
        public string[] allWords;
        public SentencePattern[] correctSentences;
        public int targetScore;

        [System.Serializable]
        public class SentencePattern
        {
            public string sentenceName;
            public string[] words;
            public int scoreValue;
        }
    }

    [System.Serializable]
    public class PatternData
    {
        public string patternName;
        public string[] wordSequence;
        public int scoreValue;
    }

    void OnEnable()
    {
        gameCamera = Camera.main;
        InitializeLineRenderer();
        SetupGame();
    }

    void InitializeLineRenderer()
    {
        if (uiLineRenderer == null)
        {
            Debug.LogError("UILineRenderer not assigned! Please assign it in the inspector.");
            return;
        }

        // Set initial line renderer properties
        uiLineRenderer.color = lineColor;
        uiLineRenderer.LineThickness = lineWidth;
        uiLineRenderer.Points = new Vector2[0]; // Start with no points
    }

    void Update()
    {
        if (!isGameActive) return;

        UpdateTimer();
        HandleInput();
        UpdateLineRenderer();
    }

    void UpdateLineRenderer()
    {
        if (uiLineRenderer == null) return;

        if (isDragging && currentPattern.Count > 0)
        {
            // Update line points based on current pattern and mouse position
            UpdateCurrentLinePoints();
            uiLineRenderer.Points = currentLinePoints.ToArray();
        }
    }

    void UpdateCurrentLinePoints()
    {
        currentLinePoints.Clear();

        // Add all connected nodes
        for (int i = 0; i < currentPattern.Count; i++)
        {
            Vector2 nodePosition = GetNodeUIPosition(currentPattern[i]);
            currentLinePoints.Add(nodePosition);
        }

        // Add current mouse position if dragging
        if (isDragging && currentPattern.Count > 0)
        {
            Vector2 mouseUIPos = GetMouseUIPosition();
            currentLinePoints.Add(mouseUIPos);
        }
    }

    Vector2 GetNodeUIPosition(WordNode node)
    {
        if (node == null) return Vector2.zero;

        RectTransform nodeRect = node.GetComponent<RectTransform>();
        if (nodeRect == null) return Vector2.zero;

        // Convert the node's anchored position to the line renderer's coordinate system
        // Since UILineRenderer uses the same coordinate system as UI elements, we can use anchored position directly
        return nodeRect.anchoredPosition;
    }

    Vector2 GetMouseUIPosition()
    {
        // Convert mouse screen position to UI position
        Vector2 mousePos = Input.mousePosition;

        // Convert screen position to local position relative to the line renderer's parent
        RectTransform canvasRect = gameCanvas.GetComponent<RectTransform>();
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, mousePos, gameCanvas.worldCamera, out localPos);

        return localPos;
    }

    void SetupGame()
    {
        ClearNodes();
        ClearLines();
        LoadCurrentLevel();
        CreateNodesInCircularPattern();
        ResetGame();
        isGameRunning = true;
    }

    void ClearLines()
    {
        if (uiLineRenderer != null)
        {
            uiLineRenderer.Points = new Vector2[0];
        }
        currentLinePoints.Clear();
    }

    void LoadCurrentLevel()
    {
        if (useRandomLevels)
        {
            currentLevelData = levels[Random.Range(0, levels.Length)];
        }
        else
        {
            int levelIndex = Mathf.Clamp(currentLevel - 1, 0, levels.Length - 1);
            currentLevelData = levels[levelIndex];
        }
        qustionText.text = currentLevelData.levelName;
        Debug.Log($"Loaded: {currentLevelData.levelName}");
    }

    void CreateNodesInCircularPattern()
    {
        if (currentLevelData == null || currentLevelData.allWords == null)
        {
            Debug.LogError("No level data available!");
            return;
        }

        string[] shuffledWords = ShuffleArray(currentLevelData.allWords);
        int wordCount = shuffledWords.Length;

        // Set consistent size
        float nodeWidth = 120f;
        float nodeHeight = 50f;

        // Compute a safe radius to avoid overlap
        float angleStep = 360f / wordCount;
        float minDistanceBetweenNodes = nodeWidth + 10f; // Add a bit of spacing
        float safeRadius = minDistanceBetweenNodes / (2f * Mathf.Sin(Mathf.Deg2Rad * angleStep / 2f));
        float radius = Mathf.Max(circleRadius, safeRadius); // Ensure it's not smaller than your default

        for (int i = 0; i < wordCount; i++)
        {
            GameObject nodeObj = Instantiate(nodePrefab, nodeParent);
            WordNode node = nodeObj.GetComponent<WordNode>() ?? nodeObj.AddComponent<WordNode>();

            RectTransform rectTransform = nodeObj.GetComponent<RectTransform>() ?? nodeObj.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(nodeWidth, nodeHeight);

            float angleDeg = i * angleStep;
            float angleRad = angleDeg * Mathf.Deg2Rad;

            float ellipseWidth = radius * 1.5f;  // Wider horizontally
            float ellipseHeight = radius * 0.8f; // Shorter vertically

            Vector2 position = new Vector2(
                Mathf.Cos(angleRad) * ellipseWidth,
                Mathf.Sin(angleRad) * ellipseHeight
            ) + centerOffset;

            rectTransform.anchoredPosition = position;
            node.Initialize(i, shuffledWords[i], this);
            nodes.Add(node);
        }
    }


    List<Vector2> GenerateCircularPositions(int count)
    {
        List<Vector2> positions = new List<Vector2>();
        float startAngle = Random.Range(0f, 360f);
        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + (i * angleStep);
            angle += Random.Range(-10f, 10f);
            float angleRad = angle * Mathf.Deg2Rad;

            Vector2 position = new Vector2(
                Mathf.Cos(angleRad) * circleRadius*1f,
                Mathf.Sin(angleRad) * circleRadius*1f
            ) + centerOffset;

            positions.Add(position);
        }

        return positions;
    }

    private string[] ShuffleArray(string[] array)
    {
        string[] shuffled = new string[array.Length];
        System.Array.Copy(array, shuffled, array.Length);

        for (int i = 0; i < shuffled.Length; i++)
        {
            string temp = shuffled[i];
            int randomIndex = Random.Range(i, shuffled.Length);
            shuffled[i] = shuffled[randomIndex];
            shuffled[randomIndex] = temp;
        }

        return shuffled;
    }

    void ClearNodes()
    {
        foreach (WordNode node in nodes)
        {
            if (node != null && node.gameObject != null)
                DestroyImmediate(node.gameObject);
        }
        nodes.Clear();
    }

    void ResetGame()
    {
        currentTime = gameTime;
        totalScore = 0;
        patternsCompleted = 0;
        isGameActive = true;
        currentPattern.Clear();
        isDragging = false;
        ClearLines();

        UpdateUI();
        ShowFeedback($"Level {currentLevel}: Form sentences by connecting words!", Color.white);

        if (nextLevelButton != null)
            nextLevelButton.gameObject.SetActive(false);
    }

    void UpdateTimer()
    {
        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            currentTime = 0;
            EndGame();
        }

        UpdateUI();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) && isGameRunning)
        {
            StartDragging();
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            ContinueDragging();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndDragging();
        }

        // Update current mouse position for line drawing
        if (isDragging)
        {
            currentMousePosition = GetMouseUIPosition();
        }
    }

    void StartDragging()
    {
        Vector2 mousePos = Input.mousePosition;
        WordNode hitNode = GetNodeAtPosition(mousePos);

        if (hitNode != null)
        {
            isDragging = true;
            currentPattern.Clear();
            currentPattern.Add(hitNode);
            hitNode.SetHighlighted(true);

            // Set line color to default
            if (uiLineRenderer != null)
            {
                uiLineRenderer.color = lineColor;
            }

            Debug.Log($"Started dragging from: {hitNode.GetWord()}");
        }
    }

    void ContinueDragging()
    {
        Vector2 mousePos = Input.mousePosition;
        WordNode hitNode = GetNodeAtPosition(mousePos);

        if (hitNode != null && !currentPattern.Contains(hitNode))
        {
            currentPattern.Add(hitNode);
            hitNode.SetHighlighted(true);
            Debug.Log($"Added to pattern: {hitNode.GetWord()}");
        }
    }

    void EndDragging()
    {
        if (!isDragging) return;

        isDragging = false;

        // Clear highlights
        foreach (WordNode node in nodes)
        {
            node.SetHighlighted(false);
        }

        // Check if pattern is valid
        CheckPattern();
    }

    WordNode GetNodeAtPosition(Vector2 screenPosition)
    {
        GraphicRaycaster raycaster = gameCanvas.GetComponent<GraphicRaycaster>();
        if (raycaster == null)
        {
            Debug.LogError("No GraphicRaycaster found on game canvas!");
            return null;
        }

        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = screenPosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            WordNode node = result.gameObject.GetComponent<WordNode>();
            if (node == null)
                node = result.gameObject.GetComponentInParent<WordNode>();

            if (node != null)
            {
                return node;
            }
        }

        return null;
    }

    public void OnNodeHovered(WordNode node)
    {
        if (!isDragging) return;

        if (!currentPattern.Contains(node))
        {
            currentPattern.Add(node);
            node.SetHighlighted(true);
        }
    }

    //Animator
    #region 

    public void SmoothSetBlendParameter(Animator animator, string parameterName, float targetValue, float duration)
    {
        StartCoroutine(LerpBlend(animator, parameterName, targetValue, duration));
    }

    private IEnumerator LerpBlend(Animator animator, string parameterName, float targetValue, float duration)
    {
        float timeElapsed = 0f;
        float startValue = animator.GetFloat(parameterName);

        while (timeElapsed < duration)
        {
            float newValue = Mathf.Lerp(startValue, targetValue, timeElapsed / duration);
            animator.SetFloat(parameterName, newValue);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        animator.SetFloat(parameterName, targetValue); // Ensure it ends exactly at target
    }

    #endregion

















    void CheckPattern()
    {
        if (currentPattern.Count < 2)
        {
            ShowError("Pattern too short! Connect at least 2 words.");
            ShowPatternResult(false);
            return;
        }

        string[] currentWords = currentPattern.Select(n => n.GetWord()).ToArray();

        foreach (var sentence in currentLevelData.correctSentences)
        {
            if (IsPatternMatch(currentWords, sentence.words))
            {

                // SmoothSetBlendParameter(hoodlumAnimator,"x",sentence.scoreValue,2);
                //hoodlumAnimator.SetFloat("x",sentence.scoreValue);

                string stateName = sentence.scoreValue.ToString(); // e.g., "Reaction3"
                hoodlumAnimator.CrossFade(stateName, 0.4f);

                totalScore += sentence.scoreValue;
                patternsCompleted++;

                ShowSuccess($"Correct! '{string.Join(" ", currentWords)}' (+{sentence.scoreValue} points)");
                ShowPatternResult(true);

                AdvanceToNextLevel();
                return;
            }
        }

        ShowError($"Try again: '{string.Join(" ", currentWords)}'");
        ShowPatternResult(false);
    }

    void ShowPatternResult(bool isCorrect)
    {
        if (uiLineRenderer == null) return;

        // Change line color based on result
        Color resultColor = isCorrect ? completedLineColor : errorLineColor;
        uiLineRenderer.color = resultColor;

        // Remove the mouse position from line points (keep only node connections)
        if (currentPattern.Count > 1)
        {
            List<Vector2> finalPoints = new List<Vector2>();
            foreach (WordNode node in currentPattern)
            {
                finalPoints.Add(GetNodeUIPosition(node));
            }
            uiLineRenderer.Points = finalPoints.ToArray();
        }

        // Clear the pattern after showing result
        currentPattern.Clear();

        // Fade out the line after a delay
        StartCoroutine(FadeOutLine());
    }

    System.Collections.IEnumerator FadeOutLine()
    {
        yield return new WaitForSeconds(lineFadeTime);
        ClearLines();
    }

    void AdvanceToNextLevel()
    {
       isGameRunning = false;
        Invoke(nameof(GoToNextLevel), 1.5f);
    }

    void GoToNextLevel()
    {
        isGameRunning = true;
        currentLevel++;
        if (currentLevel > levels.Length)
        {
            ShowFeedback("All levels complete! Starting over...", Color.yellow);
        }
        else
        {
            ShowFeedback($"Moving to Level {currentLevel}...", Color.cyan);
            SetupGame();
        }
    }

    bool IsPatternMatch(string[] current, string[] target)
    {
        if (current.Length != target.Length) return false;

        for (int i = 0; i < current.Length; i++)
        {
            if (!current[i].Equals(target[i], System.StringComparison.OrdinalIgnoreCase))
                return false;
        }

        return true;
    }

    void ShowSuccess(string message)
    {
        ShowFeedback(message, Color.green);
    }

    void ShowError(string message)
    {
        ShowFeedback(message, Color.red);
    }

    void ShowFeedback(string message, Color color)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.color = color;
        }
    }

    void UpdateUI()
    {
        if (timerSlider != null && isGameRunning)
        {
            timerSlider.value = currentTime / gameTime;
        }

        if (scoreText != null)
        {
            scoreText.text = $"Score: {totalScore}";
        }

        if (levelText != null)
        {
            string levelInfo = currentLevelData != null ? currentLevelData.levelName : $"Level {currentLevel}";
            levelText.text = $"{levelInfo} | Sentences: {patternsCompleted}/{(currentLevelData?.correctSentences?.Length ?? 0)}";
        }
    }

    void EndGame()
    {
        isGameActive = false;
        isDragging = false;
        ClearLines();
        ShowFeedback($"Time's Up! Final Score: {totalScore}", Color.yellow);

        if (nextLevelButton != null)
            nextLevelButton.gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        SetupGame();
    }

    public void NextLevel()
    {
        currentLevel++;
        if (currentLevel > levels.Length)
        {
            currentLevel = 1;
            ShowFeedback("All levels complete! Starting over...", Color.yellow);
        }

        SetupGame();
    }

    public void SetLevel(int levelNumber)
    {
        currentLevel = Mathf.Clamp(levelNumber, 1, levels.Length);
        SetupGame();
    }

    public void ToggleRandomLevels(bool random)
    {
        useRandomLevels = random;
        SetupGame();
    }
}