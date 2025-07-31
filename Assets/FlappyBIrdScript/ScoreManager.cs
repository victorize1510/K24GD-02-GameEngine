using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    // Update is called once per frame
    public Sprite[] numberSprites; // Array chứa các sprite số từ 0 đến 9
    public Image[] digitImages;    // Image đại diện cho từng chữ số trên UI

    public void UpdateScore(int score)
    {
       string scoreStr = score.ToString();

    // Ẩn hết các digit trước
    for (int i = 0; i < digitImages.Length; i++)
    {
        digitImages[i].enabled = false;
    }

    // Hiển thị đúng số lượng chữ số
    int len = scoreStr.Length;
    for (int i = 0; i < len; i++)
    {
        int digit = int.Parse(scoreStr[len - 1 - i].ToString());
        int index = digitImages.Length - 1 - i;
        digitImages[index].sprite = numberSprites[digit];
        digitImages[index].enabled = true;
    }

    // ⚠️ BẮT BUỘC: ép HorizontalLayoutGroup cập nhật lại
    LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
}
