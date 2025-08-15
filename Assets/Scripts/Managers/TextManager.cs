using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Febucci.UI;
    
public class TextManager : MonoBehaviour
{
    public static TextManager Instance { get; private set; }
    
    public TextMeshProUGUI text;
    public float typingSpeed = 0.05f;   // Delay between each character typing
    public float sizeDuration = 0.2f;   // Duration for AnimateCharacterSize
    public float maxSizeMultiplier = 1.5f; // Maximum size multiplier for AnimateCharacterSize
    
    // Queue to hold lines to render
    private Queue<string> lineQueue = new Queue<string>();
    private bool isRendering = false;
    private int visibleCharIndex = 0;
    
    void Awake()
    {
        // Ensure only one instance exists (singleton pattern)
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("More than one TextManager in scene.");
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        // Persist across scenes
        DontDestroyOnLoad(gameObject);

        Debug.Log("TextManager is created");
    }
    
    void Start()
    {
        // Clear the text box
        text.text = "";
        
        AddLine("ArmeriaOS");
    }

    public void AddLine(string line)
    {
         // Add the line to the queue
         lineQueue.Enqueue(line);

         // If not already rendering, start processing the queue
         if (!isRendering)
         {
             StartCoroutine(ProcessQueue());
         }
    }

    private IEnumerator ProcessQueue()
    {
        // Set rendering flag
        isRendering = true;

        // Render lines one by one
        while (lineQueue.Count > 0)
        {
            string line = lineQueue.Dequeue();
            yield return TypeText(line + "\n");
        }

        // Reset rendering flag
        isRendering = false;
    }
    
    private IEnumerator TypeText(string fullText)
    {
        int index = 0;
        string appendText = "";
        
        while (index < fullText.Length)
        {
            // Check if the current character is the start of a tag
            if (fullText[index] == '<')
            {
                // Find the end of the tag
                int closingIndex = fullText.IndexOf('>', index);
                if (closingIndex != -1)
                {
                    // Add the entire tag at once
                    appendText = fullText.Substring(index, closingIndex - index + 1);
                    index = closingIndex + 1; // Move past the tag
                    
                    // Update the text instantly (no delay for tags)
                    text.text += appendText;
                    text.ForceMeshUpdate();
                }
                else
                {
                    // Handle case where tag is incomplete (unlikely in properly formatted text)
                    Debug.LogWarning("Incomplete tag detected in text.");
                    break;
                }
            }
            else
            {
                // Add the next character
                appendText = fullText[index].ToString();
                text.text += appendText;
                text.ForceMeshUpdate();
                
                // Animate the size of the new character
                StartCoroutine(AnimateCharacterSize(visibleCharIndex));
                
                visibleCharIndex++;
                index++;
                
                yield return new WaitForSeconds(typingSpeed);
            }
        }
    }

    private IEnumerator AnimateCharacterSize(int charIndex)
    {
        // Ensure the text mesh is up-to-date
        text.ForceMeshUpdate();

        TMP_TextInfo textInfo = text.textInfo;

        // Text info is no longer updating, so player likely switched windows.
        if (charIndex > textInfo.characterInfo.Length - 1)
        {
            yield break;
        }
        
        // Skip if the character is not visible
        if (!textInfo.characterInfo[charIndex].isVisible)
        {
            yield break;
        }

        float elapsedTime = 0f;

        // Get the material index and vertex index for the character
        int materialIndex = textInfo.characterInfo[charIndex].materialReferenceIndex;
        int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;

        // Get the vertices of the character
        Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

        // Calculate the character's center for scaling
        Vector3 charCenter = (vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2;

        // Store the original vertex positions
        Vector3[] originalVertices = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            originalVertices[i] = vertices[vertexIndex + i];
        }

        // Animate the size increase
        while (elapsedTime < sizeDuration)
        {
            elapsedTime += Time.deltaTime;

            // Calculate the current scale factor
            float progress = elapsedTime / sizeDuration;
            float scale = Mathf.Lerp(maxSizeMultiplier, 1f, progress);

            // Scale the vertices around the character's center
            for (int i = 0; i < 4; i++)
            {
                vertices[vertexIndex + i] = charCenter + (originalVertices[i] - charCenter) * scale;
            }

            // Update the mesh
            text.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);

            yield return null;
        }

        // Restore the original vertex positions
        for (int i = 0; i < 4; i++)
        {
            vertices[vertexIndex + i] = originalVertices[i];
        }

        // Update the mesh to finalize the animation
        text.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
        text.ForceMeshUpdate();
    }
}
