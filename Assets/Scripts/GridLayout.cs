using UnityEngine;

[ExecuteInEditMode]
public class GridLayout : MonoBehaviour
{
    public int rows;
    public int columns;
    public Vector2 cellSize = new Vector2(1, 1);
    public Vector2 spacing = new Vector2(0.1f, 0.1f);
    public float rowOffset;
    public float columnOffset;

    private void Update()
    {
        if (!Application.isPlaying)
        {
            RearrangeChildren();
        }
    }

    private void RearrangeChildren()
    {
        int totalChildren = transform.childCount;

        for (int i = 0; i < totalChildren; i++)
        {
            Transform child = transform.GetChild(i);
            int row = i / columns;
            int column = i % columns;

            Vector3 newPosition = new Vector3(
                column * (cellSize.x + spacing.x) + row * columnOffset,
                0,
                -row * (cellSize.y + spacing.y) + column * rowOffset
            );

            child.localPosition = newPosition;
        }
    }
}
