using UnityEngine;
using UnityEngine.Tilemaps;

public class hightlight : MonoBehaviour
{
    public Tilemap hightlightTilemap; // Gắn Tilemap cần làm tối trong Inspector
    [Range(0f, 1f)]
    public float darkenFactor = 0f; // 0 = đen hoàn toàn, 1 = giữ nguyên

    void Start()
    {
        foreach (var pos in hightlightTilemap.cellBounds.allPositionsWithin)
        {
            if (hightlightTilemap.HasTile(pos))
            {
                Color originalColor = hightlightTilemap.GetColor(pos);
                Color darkerColor = new Color(
                    originalColor.r * darkenFactor,
                    originalColor.g * darkenFactor,
                    originalColor.b * darkenFactor,
                    originalColor.a
                );

                hightlightTilemap.SetColor(pos, darkerColor);
            }
        }
    }
}
