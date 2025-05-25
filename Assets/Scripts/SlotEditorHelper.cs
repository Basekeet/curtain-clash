using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class SlotEditorHelper : MonoBehaviour
{
    [Header("Visualization")]
    public float radius = 0.5f;
    public Color gizmoColor = Color.yellow;

    [Header("Child Positioning")]
    public float yOffset = 0.5f;

    [Header("Rendering")]
    public string sortingLayer = "Default";
    public int orderInLayer = 0;
    public float zCoord = 0;
    private void OnValidate()
    {
        UpdateChildren();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            UpdateChildren();
#endif
    }

    public void UpdateChildren(bool savePos = false)
    {
        foreach (Transform child in transform)
        {
            if (!savePos){
                // Apply positional offset
                Vector3 childPos = child.localPosition;
                childPos.x = 0;
                childPos.y = -yOffset;
                childPos.z = zCoord;
                child.localPosition = childPos;
            }
            else
            {
                Vector3 childPos = child.localPosition;
                childPos.z = zCoord;
                child.localPosition = childPos;
            }
        }

        UpdateChildren(transform);
    }

    public void UpdateChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            // Apply sorting layer (if SpriteRenderer exists)
            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingLayerName = sortingLayer;
                sr.sortingOrder = orderInLayer;
            }
            UpdateChildren(child);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}