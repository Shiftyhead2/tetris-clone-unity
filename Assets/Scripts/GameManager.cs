using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static bool GameStopped = false;
    public static bool GamePaused = false;
    public static bool AudioMuted = false;

    public GameObject[] TetroMinos;

    public static Transform[,] grid = new Transform[HelperScript.gridWidth,HelperScript.gridHeight];

    public GameObject GameOverCanvas;
    public GameObject PausedCanvas;
    public TextMeshProUGUI ScoreText;
    public GameObject PauseButton;
    public GameObject MuteButton;
    public Sprite[] SoundSprites;


    int score = 0;
    int numberOfRowsThisTurn = 0;
    GameObject nextTetromino;
    GameObject previewTetromino;

    private bool gameStarted = false;

    Vector2 previewTetrominoPosition = new Vector2(10f,22.5f);

    AudioSource[] AudioSources;
    
    



    // Start is called before the first frame update
    void Start()
    {
        GameStopped = false;
        GamePaused = false;
        AudioMuted = false;

        AudioSources = FindObjectsOfType<AudioSource>();
        MuteAudioSources();



        if(!instance)
        {
            instance = this;
        }

        SpawnNextTetromino();
        UpdateUI();

    }

    void Update()
    {
        UpdateScore();
    }


    public void UpdateScore()
    {
        if(numberOfRowsThisTurn > 0)
        {
            if(numberOfRowsThisTurn == 1)
            {
                score += HelperScript.firstRowScore;
            }
            else if(numberOfRowsThisTurn == 2)
            {
                score += HelperScript.secondRowScore;
            }
            else if(numberOfRowsThisTurn == 3)
            {
                score += HelperScript.thirdRowScore;
            }
            else if(numberOfRowsThisTurn == 4)
            {
                score += HelperScript.fourthRowScore;
            }

            numberOfRowsThisTurn = 0;
            UpdateUI();
            SoundManager.instance.PlaySound(3);
        }

    }

    public bool CheckIsAboveGrid(Tetromino tetromino)
    {
        for (int x = 0; x < HelperScript.gridWidth; x++)
        {
            foreach(Transform mino in tetromino.transform)
            {
                Vector2 pos = Round(mino.position);

                if(pos.y >= HelperScript.gridHeight - 1)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool IsRowFullAt(int y)
    {
        for (int x = 0; x < HelperScript.gridWidth; x++)
        {
            if(grid[x,y] == null)
            {
                return false;
            }
        }

        //Since we found a row , we increment the full row
        numberOfRowsThisTurn++;

        return true;
    }

    public void DeleteMinoAt(int y)
    {
        for (int x = 0; x < HelperScript.gridWidth; x++)
        {
            Destroy(grid[x,y].gameObject);

            grid[x,y] = null;
        }

    }

    public void MoveRowDown(int y)
    {
        for (int x = 0; x < HelperScript.gridWidth; x++)
        {
            if(grid[x,y] != null)
            {
                grid[x,y-1] = grid[x,y];
                grid[x,y] = null;

                grid[x,y-1].position += Vector3.down;
            }
        }
    }


    public void MoveAllRowsDown(int y)
    {
        for (int i = y; i < HelperScript.gridHeight; i++)
        {
            MoveRowDown(i);
        }
    }

    public void DeleteRow()
    {
        for (int y = 0; y < HelperScript.gridHeight; y++)
        {
            if(IsRowFullAt(y))
            {
                DeleteMinoAt(y);
                MoveAllRowsDown(y+1);

                y--;
            }
        }
    }

    public void UpdateGrid(Tetromino tetromino)
    {
        for (int y = 0; y < HelperScript.gridHeight; y++)
        {
            for (int x = 0; x < HelperScript.gridWidth; x++)
            {
                if(grid[x,y] != null)
                {
                    if(grid[x,y].parent == tetromino.transform)
                    {
                        grid[x,y] = null;
                    }
                }
            }
        }

        foreach(Transform mino in tetromino.transform)
        {
            Vector2 pos = Round(mino.position);

            if(pos.y < HelperScript.gridHeight)
            {
                grid[(int)pos.x,(int)pos.y] = mino;
            }
        }
    }

    public Transform GetTransformAtGridPosition(Vector2 pos)
    {
        if(pos.y > HelperScript.gridHeight - 1)
        {
            return null;
        }
        else
        {
            return grid[(int)pos.x,(int)pos.y];
        }
    }

    public void SpawnNextTetromino()
    {
        if(!gameStarted)
        {
            gameStarted = true;
            nextTetromino = (GameObject)Instantiate(GetRandomTetromino(), new Vector2(4.0f,19.0f),Quaternion.identity);
            previewTetromino = (GameObject)Instantiate(GetRandomTetromino(),previewTetrominoPosition,Quaternion.identity);
            previewTetromino.GetComponent<Tetromino>().enabled = false;
        }
        else
        {
            previewTetromino.transform.localPosition = new Vector2(4.0f,20.0f);
            nextTetromino = previewTetromino;
            nextTetromino.GetComponent<Tetromino>().enabled = true;

            previewTetromino = (GameObject)Instantiate(GetRandomTetromino(),previewTetrominoPosition,Quaternion.identity);
            previewTetromino.GetComponent<Tetromino>().enabled = false;

        }
        
    }

    public bool CheckIsInsideGrid(Vector2 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < HelperScript.gridWidth && (int)pos.y >= 0);
    }

    public Vector2 Round(Vector2 pos)
    {
      return new Vector2(Mathf.Round(pos.x),Mathf.Round(pos.y));
    }

    GameObject GetRandomTetromino()
    {
        int randomValue = Random.Range(0,TetroMinos.Length);
        GameObject RandomTetromino = TetroMinos[randomValue];
        return RandomTetromino;
    }


    public void GameOver()
    {
        GameStopped = true;
        AudioMuted = true;
        MuteAudioSources();
        GameOverCanvas.SetActive(true); 
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void UpdateUI()
    {
        ScoreText.text = score.ToString();
    }

    public void PauseGame()
    {
        GamePaused = !GamePaused;

        PausedCanvas.SetActive(GamePaused);
        PauseButton.SetActive(!GamePaused);
    }

    public void ChangeMuteBool()
    {
        AudioMuted = !AudioMuted;
        MuteAudioSources();
        ChangeMuteButtonSprite();
    }

    void ChangeMuteButtonSprite()
    {
        if(AudioMuted)
        {
            MuteButton.GetComponent<Image>().sprite = SoundSprites[1];
        }else
        {
            MuteButton.GetComponent<Image>().sprite = SoundSprites[0];
        }
    }

    void MuteAudioSources()
    {
        foreach(AudioSource audiosource in AudioSources)
        {
            audiosource.mute = AudioMuted;
        }
    }
}
