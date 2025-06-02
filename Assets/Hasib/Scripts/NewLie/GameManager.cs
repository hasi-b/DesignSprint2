// GameManager.cs - Main game controller with circular layout and fixes
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public float gameTime = 30f;
    public int currentLevel = 1;
    
    [Header("Level System")]
    public LevelData[] levels;
    public bool useRandomLevels = false;
    
    [Header("UI References")]
    public Canvas gameCanvas;
    public Slider timerSlider; // Changed from TextMeshProUGUI to Slider
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI levelText;
    public Button restartButton;
    public Button nextLevelButton;
    public LineRenderer lineRenderer;
    
    [Header("Node Settings")]
    public GameObject nodePrefab;
    public Transform nodeParent;
    public float circleRadius = 250f; // Radius for circular arrangement
    public Vector2 centerOffset = Vector2.zero; // Offset from center of screen
    
    [Header("Line Renderer Settings")]
    public Material lineMaterial; // Assign a material for the line
    public float lineWidth = 5f;
    public Color lineColor = Color.blue;
    
    private List<WordNode> nodes = new List<WordNode>();
    private List<WordNode> currentPattern = new List<WordNode>();
    private float currentTime;
    private int totalScore = 0;
    private bool isGameActive = false;
    private bool isDragging = false;
    private Camera gameCamera;
    private LevelData currentLevelData;
    private int patternsCompleted = 0;
    
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
    
    void Start()
    {
        gameCamera = Camera.main;
        SetupLineRenderer();
       
        SetupGame();
    }
    
    void SetupLineRenderer()
    {
        if (lineRenderer != null)
        {
            lineRenderer.useWorldSpace = true; // Use world space
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.positionCount = 0;
            lineRenderer.sortingOrder = 10; // Make sure it renders on top
            
            // If no material is assigned, create a simple one
            if (lineMaterial != null)
            {
                lineRenderer.material = lineMaterial;
            }
            else
            {
                // Create a simple material if none provided
                Material defaultMat = new Material(Shader.Find("Sprites/Default"));
                defaultMat.color = lineColor;
                lineRenderer.material = defaultMat;
            }
            
            // Set line color using gradient
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(lineColor, 0.0f), new GradientColorKey(lineColor, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
            );
            lineRenderer.colorGradient = gradient;
        }
    }
    
    
    void Update()
    {
        if (!isGameActive) return;
        
        UpdateTimer();
        HandleInput();
        UpdateLineRenderer();
    }
    
    void SetupGame()
    {
        ClearNodes();
        LoadCurrentLevel();
        CreateNodesInCircularPattern();
        ResetGame();
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
        
        Debug.Log($"Loaded: {currentLevelData.levelName}");
    }
    
    void CreateNodesInCircularPattern()
    {
        if (currentLevelData == null || currentLevelData.allWords == null)
        {
            Debug.LogError("No level data available!");
            return;
        }
        
        // Randomize the word order
        string[] shuffledWords = ShuffleArray(currentLevelData.allWords);
        
        // Generate circular positions with random starting angle
        List<Vector2> positions = GenerateCircularPositions(shuffledWords.Length);
        
        for (int i = 0; i < shuffledWords.Length; i++)
        {
            GameObject nodeObj = Instantiate(nodePrefab, nodeParent);
            WordNode node = nodeObj.GetComponent<WordNode>();
            
            if (node == null)
            {
                node = nodeObj.AddComponent<WordNode>();
            }
            
            // Setup RectTransform
            RectTransform rectTransform = nodeObj.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                rectTransform = nodeObj.AddComponent<RectTransform>();
            }
            
            // Set size based on word length (dynamic sizing)
            float wordWidth = Mathf.Max(80f, shuffledWords[i].Length * 15f + 20f);
            rectTransform.sizeDelta = new Vector2(wordWidth, 50);
            
            // Set circular position
            rectTransform.anchoredPosition = positions[i];
            
            // Initialize node
            node.Initialize(i, shuffledWords[i], this);
            nodes.Add(node);
        }
    }
    
    List<Vector2> GenerateCircularPositions(int count)
    {
        List<Vector2> positions = new List<Vector2>();
        
        // Random starting angle to randomize the circle
        float startAngle = Random.Range(0f, 360f);
        
        // Calculate angle step between nodes
        float angleStep = 360f / count;
        
        for (int i = 0; i < count; i++)
        {
            // Calculate angle for this node
            float angle = startAngle + (i * angleStep);
            
            // Add some random variation to make it less perfect
            angle += Random.Range(-10f, 10f);
            
            // Convert to radians
            float angleRad = angle * Mathf.Deg2Rad;
            
            // Calculate position on circle
            Vector2 position = new Vector2(
                Mathf.Cos(angleRad) * circleRadius,
                Mathf.Sin(angleRad) * circleRadius
            ) + centerOffset;
            
            positions.Add(position);
        }
        
        return positions;
    }
    
    // Helper method to shuffle array
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
        totalScore = 0; // Reset total score for new level
        patternsCompleted = 0;
        isGameActive = true;
        currentPattern.Clear();
        isDragging = false;
        
        UpdateUI();
        ShowFeedback($"Level {currentLevel}: Form sentences by connecting words!", Color.white);
        
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
        }
        
        // Hide next level button
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
        if (Input.GetMouseButtonDown(0))
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
        
        // Clear current pattern
        currentPattern.Clear();
        
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
        }
    }
    
    WordNode GetNodeAtPosition(Vector2 screenPosition)
    {
        // Use GraphicRaycaster for UI elements
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
    
    void UpdateLineRenderer()
    {
        if (lineRenderer == null || !isDragging || currentPattern.Count < 1) 
        {
            if (lineRenderer != null)
                lineRenderer.positionCount = 0;
            return;
        }
        
        lineRenderer.positionCount = currentPattern.Count;
        
        for (int i = 0; i < currentPattern.Count; i++)
        {
            // Get the RectTransform of the node
            RectTransform nodeRect = currentPattern[i].GetComponent<RectTransform>();
            
            // Convert UI position to world position
            Vector3 worldPos;
            
            // Check canvas render mode
            if (gameCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                // For overlay canvas, convert screen position to world position
                Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null, nodeRect.position);
                worldPos = gameCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, gameCamera.nearClipPlane + 2f));
            }
            else if (gameCanvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                // For camera canvas, use the canvas camera
                Canvas canvas = gameCanvas;
                Camera canvasCamera = canvas.worldCamera ?? gameCamera;
                Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(canvasCamera, nodeRect.position);
                worldPos = canvasCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, canvasCamera.nearClipPlane + 2f));
            }
            else // WorldSpace
            {
                worldPos = nodeRect.position;
                worldPos.z = gameCamera.transform.position.z + 1f; // Ensure it's in front of camera
            }
            
            lineRenderer.SetPosition(i, worldPos);
        }
    }

    void CheckPattern()
    {
        if (currentPattern.Count < 2)
        {
            ShowError("Pattern too short! Connect at least 2 words.");
            return;
        }

        string[] currentWords = currentPattern.Select(n => n.GetWord()).ToArray();

        // Check against current level's correct sentences
        foreach (var sentence in currentLevelData.correctSentences)
        {
            if (IsPatternMatch(currentWords, sentence.words))
            {
                // Correct sentence found!
                totalScore += sentence.scoreValue;
                patternsCompleted++;

                ShowSuccess($"Correct! '{string.Join(" ", currentWords)}' (+{sentence.scoreValue} points)");

                // Immediately advance to next level after any correct sequence
                AdvanceToNextLevel();
                return;
            }
        }

        // No match found
        ShowError($"Try again: '{string.Join(" ", currentWords)}'");
    }

    // New method to handle level advancement
    void AdvanceToNextLevel()
    {
        // Small delay to show the success message
        Invoke(nameof(GoToNextLevel), 1.5f);
    }

    void GoToNextLevel()
    {
        currentLevel++;
        if (currentLevel > levels.Length)
        {
            //currentLevel = 1; // Loop back to first level
            ShowFeedback("All levels complete! Starting over...", Color.yellow);
        }
        else
        {
            ShowFeedback($"Moving to Level {currentLevel}...", Color.cyan);
            // Start the next level
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
        // Update timer slider instead of text
        if (timerSlider != null)
        {
            timerSlider.value = currentTime / gameTime; // Normalized value between 0 and 1
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
        ShowFeedback($"Time's Up! Final Score: {totalScore}", Color.yellow);
        
        // Show next level button even if time runs out
        if (nextLevelButton != null)
            nextLevelButton.gameObject.SetActive(true);
    }
    
    public void RestartGame()
    {
        SetupGame(); // This will reset everything including score
    }
    
    public void NextLevel()
    {
        currentLevel++;
        if (currentLevel > levels.Length)
        {
            currentLevel = 1; // Loop back to first level
            ShowFeedback("All levels complete! Starting over...", Color.yellow);
        }
        
        SetupGame(); // This will automatically hide the next level button
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