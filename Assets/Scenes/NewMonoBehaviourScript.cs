using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class TicTacToeGame : MonoBehaviour
{
    [Header("Game Elements")]
    public Button[] cells;
    public Button restartButton;
    public Text resultText;
    public Text turnIndicator;

    [Header("Sprites")]
    public Sprite crossSprite;
    public Sprite noughtSprite;
    public Sprite emptySprite;

    private int[] board = new int[9];
    private bool gameActive;
    private bool playerTurn; // true - ��� ������, false - ��� ����������
    private bool gameStarted;
    private bool playerIsCross; // true - ����� �������, false - ����� �����

    void Start()
    {
        restartButton.onClick.AddListener(RestartGame);
        InitializeCells();
        StartNewGame();
    }

    void InitializeCells()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            int index = i;
            cells[i].onClick.RemoveAllListeners();
            cells[i].onClick.AddListener(() => OnCellClick(index));
            cells[i].image.sprite = emptySprite;
            cells[i].interactable = true;

            var colors = cells[i].colors;
            colors.normalColor = Color.white;
            colors.disabledColor = Color.white;
            cells[i].colors = colors;
        }
    }

    void StartNewGame()
    {
        for (int i = 0; i < 9; i++)
        {
            board[i] = 0;
            cells[i].image.sprite = emptySprite;
            cells[i].interactable = true;
        }

        // �������� ����������, ��� ����� ��������� (����� ������)
        playerIsCross = Random.Range(0, 2) == 0;

        // ������ ��� �� ���������
        playerTurn = playerIsCross; // ���� ����� �������, �� ����� ������

        gameActive = true;
        gameStarted = true;
        resultText.gameObject.SetActive(false);

        UpdateTurnIndicator();

        // ���� ������ ����� ��������� (��� �������)
        if (!playerTurn && gameActive)
        {
            ComputerMove();
        }
    }

    void OnCellClick(int index)
    {
        if (!gameActive || !playerTurn || board[index] != 0 || !gameStarted)
            return;

        // ����� ������ ��� ����� ������� (������� ��� �����)
        MakeMove(index, playerIsCross ? 1 : 2);
        playerTurn = false;

        if (CheckWin(playerIsCross ? 1 : 2))
        {
            ShowResult("�� ��������!");
            EndGame();
            return;
        }

        if (IsDraw())
        {
            ShowResult("�����!");
            EndGame();
            return;
        }

        ComputerMove();
    }

    void ComputerMove()
    {
        if (!gameActive || playerTurn || !gameStarted)
            return;

        UpdateTurnIndicator(); // ����������, ��� ��� ����������
        Invoke("ExecuteComputerMove", 0.5f); // ��� 1 �������
    }

    IEnumerator ComputerThinkingRoutine()
    {
        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            // �������� ������ (�����������)
            turnIndicator.text = playerIsCross ?
                "��������� ������ (������)" :
                "��������� ������ (��������)";

            yield return new WaitForSeconds(0.3f);
            turnIndicator.text = "";
            yield return new WaitForSeconds(0.3f);

            elapsedTime += 0.6f;
        }
        UpdateTurnIndicator(); // ���������� ������� �����
    }

    void ExecuteComputerMove()
    {
        if (!gameActive || playerTurn || !gameStarted)
            return;

        int move = FindBestMove();

        if (move != -1)
        {
            // ��������� ������ ��� ��������������� �������
            MakeMove(move, playerIsCross ? 2 : 1);
            playerTurn = true;

            if (CheckWin(playerIsCross ? 2 : 1))
            {
                ShowResult("��������� �������!");
                EndGame();
                return;
            }

            if (IsDraw())
            {
                ShowResult("�����!");
                EndGame();
                return;
            }
        }

        UpdateTurnIndicator();
    }

    int FindBestMove()
    {
        int computerSymbol = playerIsCross ? 2 : 1;
        int playerSymbol = playerIsCross ? 1 : 2;

        // ������� ���������, ����� �� ��������� ��������
        int move = FindWinningMove(computerSymbol);
        if (move != -1) return move;

        // ����� ���������, ����� �� ����� ��������, ����� �����������
        move = FindWinningMove(playerSymbol);
        if (move != -1) return move;

        // �����, ���� ��������
        if (board[4] == 0) return 4;

        // ��������� ���
        List<int> emptyCells = new List<int>();
        for (int i = 0; i < 9; i++)
        {
            if (board[i] == 0) emptyCells.Add(i);
        }

        return emptyCells.Count > 0 ? emptyCells[Random.Range(0, emptyCells.Count)] : -1;
    }

    int FindWinningMove(int player)
    {
        for (int i = 0; i < 9; i++)
        {
            if (board[i] == 0)
            {
                board[i] = player;
                bool win = CheckWin(player);
                board[i] = 0;
                if (win) return i;
            }
        }
        return -1;
    }

    void MakeMove(int index, int player)
    {
        board[index] = player;
        cells[index].image.sprite = player == 1 ? crossSprite : noughtSprite;
        cells[index].interactable = false;

        var colors = cells[index].colors;
        colors.disabledColor = Color.white;
        cells[index].colors = colors;

        UpdateTurnIndicator();
    }

    bool CheckWin(int player)
    {
        int[][] wins = {
            new[] {0, 1, 2}, new[] {3, 4, 5}, new[] {6, 7, 8},
            new[] {0, 3, 6}, new[] {1, 4, 7}, new[] {2, 5, 8},
            new[] {0, 4, 8}, new[] {2, 4, 6}
        };

        foreach (var combo in wins)
        {
            if (board[combo[0]] == player &&
                board[combo[1]] == player &&
                board[combo[2]] == player)
            {
                return true;
            }
        }
        return false;
    }

    bool IsDraw()
    {
        foreach (int cell in board)
        {
            if (cell == 0) return false;
        }
        return true;
    }

    void EndGame()
    {
        gameActive = false;
        playerTurn = false;
        foreach (var cell in cells)
        {
            cell.interactable = false;
        }
        turnIndicator.gameObject.SetActive(false);
    }

    void ShowResult(string message)
    {
        resultText.gameObject.SetActive(true);
        resultText.text = message;
    }

    void UpdateTurnIndicator()
    {
        if (turnIndicator == null) return;

        if (!gameActive)
        {
            turnIndicator.text = "���� ���������";
            return;
        }

        if (playerTurn)
        {
            turnIndicator.text = playerIsCross ?
                "��� ��� (��������)" :
                "��� ��� (������)";
        }
        else
        {
            turnIndicator.text = playerIsCross ?
                "��� ���������� (������)" :
                "��� ���������� (��������)";
        }
    }

    public void RestartGame()
    {
        gameStarted = false;
        InitializeCells(); 
        StartNewGame();    
        turnIndicator.gameObject.SetActive(true); 
        UpdateTurnIndicator(); 
    }
}