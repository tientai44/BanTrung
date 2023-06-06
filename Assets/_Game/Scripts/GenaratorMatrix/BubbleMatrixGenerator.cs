using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleMatrixGenerator : MonoBehaviour
{
    public int rows = 5; // Số hàng của ma trận
    public int cols = 12; // Số cột của ma trận

    public int maxBubbleType = 3; // Số loại bong bóng tối đa
    public int difficultyIncrement = 1; // Mức độ khó tăng dần sau mỗi level

    private int[,] bubbleMatrix; // Ma trận bong bóng

    private void Start()
    {
        GenerateBubbleMatrix();
    }

    private void GenerateBubbleMatrix()
    {
        bubbleMatrix = new int[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                // Sinh ngẫu nhiên giá trị bong bóng từ 0 đến maxBubbleType
                int bubbleType = Random.Range(0, maxBubbleType + 1);

                // Áp dụng quy tắc tăng độ khó sau mỗi level
                if (i > 0 && bubbleMatrix[i - 1, j] == bubbleMatrix[i, j])
                {
                    bubbleType = (bubbleType + difficultyIncrement) % (maxBubbleType + 1);
                }

                bubbleMatrix[i, j] = bubbleType;
            }
        }

        // In ra ma trận để kiểm tra
        PrintBubbleMatrix();
    }

    private void PrintBubbleMatrix()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Debug.Log(bubbleMatrix[i, j] + " ");
            }
            Debug.Log("\n");
        }
    }
}
