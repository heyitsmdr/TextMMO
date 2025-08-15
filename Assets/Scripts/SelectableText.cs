using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using System.Text;
using System.Collections.Generic;

public class SelectableText : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Color highlightColor = new Color(1, 1, 1, 0.8f);
    
    public TMP_InputField focusAfterField;
    
    private TextMeshProUGUI _textMeshPro;
    private Camera _camera;
    private bool _isDragging = false;
    private int _selectionStart = -1;
    private int _selectionEnd = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _textMeshPro = GetComponent<TextMeshProUGUI>();
        _camera = _textMeshPro.canvas.worldCamera;
        Debug.Log("SelectableText initialized for " + _textMeshPro.gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDragging)
        {
            HighlightSelectedText();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector3 mousePosition = Input.mousePosition;
        
        // Get the character index at the mouse position
        int charIndex = TMP_TextUtilities.FindNearestCharacter(_textMeshPro, mousePosition, _camera, true);
        if (charIndex != -1)
        {
            Debug.Log($"PointerDown: Mouse near character '{_textMeshPro.textInfo.characterInfo[charIndex].character}' at index {charIndex}");
            _selectionStart = charIndex;
            _selectionEnd = charIndex;
            _isDragging = true;
        }
        else
        {
            Debug.Log("PointerDown: Mouse not over any character.");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mousePosition = Input.mousePosition;

        // Update the selection end based on the current mouse position
        int charIndex = TMP_TextUtilities.FindNearestCharacter(_textMeshPro, mousePosition, _camera, true);
        if (charIndex != -1)
        {
            Debug.Log("Extending selection" + ", selection start: " + _selectionStart + ", selection end: " + _selectionEnd);
            _selectionEnd = charIndex;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_isDragging)
        {
            _isDragging = false;
            
            // Copy the selected text to the clipboard
            string selectedText = GetSelectedText();
            if (!string.IsNullOrEmpty(selectedText))
            {
                GUIUtility.systemCopyBuffer = selectedText;
                Debug.Log("Copied text to clipboard: " + selectedText);
            }

            ClearSelection();
            
            // Focus the focus field, if present
            if (focusAfterField != null)
            {
                focusAfterField.Select();
            }
        }
    }

    private string GetSelectedText()
    {
        if (_selectionStart == -1 || _selectionEnd == -1)
        {
            return string.Empty;
        }

        int startIndex = Mathf.Min(_selectionStart, _selectionEnd);
        int endIndex = Mathf.Max(_selectionStart, _selectionEnd);

        StringBuilder selectedText = new StringBuilder();
        for (int i = startIndex; i <= endIndex; i++)
        {
            selectedText.Append(_textMeshPro.textInfo.characterInfo[i].character);
        }

        return selectedText.ToString();
    }

    private void HighlightSelectedText()
    {
        if (_selectionStart == -1 || _selectionEnd == -1) return;

        // Ensure selectionStart is less than selectionEnd
        int start = Mathf.Min(_selectionStart, _selectionEnd);
        int end = Mathf.Max(_selectionStart, _selectionEnd);

        TMP_TextInfo textInfo = _textMeshPro.textInfo;

        // Get the highlight color
        Color32 highlightColor32 = highlightColor;

        // Loop through the selected characters
        for (int i = start; i <= end; i++)
        {
            if (i < 0 || i >= textInfo.characterCount) continue;

            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible) continue; // Skip invisible characters

            int meshIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            // Get the original vertex positions
            Vector3[] vertices = textInfo.meshInfo[meshIndex].vertices;

            // Copy the original vertex positions to a temporary array
            Vector3 bottomLeft = charInfo.bottomLeft;
            Vector3 topLeft = charInfo.topLeft;
            Vector3 topRight = charInfo.topRight;
            Vector3 bottomRight = charInfo.bottomRight;

            // Expand the box slightly for better visibility
            float padding = 2f; // Adjust padding as needed
            bottomLeft.x -= padding;
            bottomRight.x += padding;
            topLeft.x -= padding;
            topRight.x += padding;
            bottomLeft.y -= padding;
            bottomRight.y -= padding;
            topLeft.y += padding;
            topRight.y += padding;

            // Apply the adjusted positions to the mesh vertices
            vertices[vertexIndex + 0] = bottomLeft;
            vertices[vertexIndex + 1] = topLeft;
            vertices[vertexIndex + 2] = topRight;
            vertices[vertexIndex + 3] = bottomRight;

            // Get the vertex colors
            Color32[] vertexColors = textInfo.meshInfo[meshIndex].colors32;

            // Set the highlight color for the background box
            vertexColors[vertexIndex + 0] = highlightColor32;
            vertexColors[vertexIndex + 1] = highlightColor32;
            vertexColors[vertexIndex + 2] = highlightColor32;
            vertexColors[vertexIndex + 3] = highlightColor32;
        }

        // Update the mesh to apply changes
        _textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices | TMP_VertexDataUpdateFlags.Colors32);
    }

    private void ClearSelection()
    {
        // Reset the vertex colors
        _textMeshPro.ForceMeshUpdate();

        _selectionStart = -1;
        _selectionEnd = -1;
    }
}
