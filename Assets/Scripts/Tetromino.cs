using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tetromino : MonoBehaviour
{
    float fall = 0f;
    public float fallSpeed = 1f;
    public bool allowRotation = true;

    public Button LeftArrowButton;
    public Button RightArrowButton;
    public Button DownArrowButton;
    public Button RotateButton;

    

    


    // Start is called before the first frame update
    void Start()
    {
        LeftArrowButton = GameObject.Find("LeftArrowButton").GetComponent<Button>();
        RightArrowButton = GameObject.Find("RightArrowButton").GetComponent<Button>();
        DownArrowButton = GameObject.Find("DownArrowButton").GetComponent<Button>();
        RotateButton = GameObject.Find("RotateButton").GetComponent<Button>();

        LeftArrowButton.onClick.AddListener(delegate{MoveLeft();});
        RightArrowButton.onClick.AddListener(delegate{MoveRight();});
        DownArrowButton.onClick.AddListener(delegate{MoveDown();});
        DownArrowButton.onClick.AddListener(delegate{PlaySoundClip(0);});
        RotateButton.onClick.AddListener(delegate{Rotate();});
    }

    // Update is called once per frame
    void Update()
    {
        CheckConditions();
    }

    void CheckConditions()
    {
        if(GameManager.GameStopped || GameManager.GamePaused)
        {
            return;
        }

        
        if(Time.time - fall >= fallSpeed)
        {   
            MoveDown();
        }
    }
    


    bool CheckIsValidPosition()
    {
        foreach(Transform mino in transform)
        {
            Vector2 pos = GameManager.instance.Round(mino.position);

            if(GameManager.instance.CheckIsInsideGrid(pos) == false)
            {
                return false;
            }

            if(GameManager.instance.GetTransformAtGridPosition(pos) != null && GameManager.instance.GetTransformAtGridPosition(pos).parent != transform)
            {
                return false;
            }
        }

        return true;
    }


    void MoveDown()
    {
        transform.position += Vector3.down; 
           
        if(CheckIsValidPosition())
        {
            GameManager.instance.UpdateGrid(this);     
        }else
        {
            transform.position += Vector3.up;

            GameManager.instance.DeleteRow();

            if(GameManager.instance.CheckIsAboveGrid(this))
            {
                GameManager.instance.GameOver();
            }
            
            PlaySoundClip(2);
            GameManager.instance.SpawnNextTetromino();

            LeftArrowButton.onClick.RemoveAllListeners();
            LeftArrowButton = null;
            RightArrowButton.onClick.RemoveAllListeners();
            RightArrowButton = null;
            DownArrowButton.onClick.RemoveAllListeners();
            DownArrowButton = null;
            RotateButton.onClick.RemoveAllListeners();
            RotateButton = null;

            enabled = false;

        }
        fall = Time.time;
    }


    void MoveRight()
    {
        if(GameManager.GameStopped)
        {
            return;
        }


        transform.position += Vector3.right;

        if(CheckIsValidPosition())
        {
            GameManager.instance.UpdateGrid(this);
            PlaySoundClip(0);
        }else
        {
            transform.position += Vector3.left;
        }
    }


    void MoveLeft()
    {
        if(GameManager.GameStopped)
        {
            return;
        }


        transform.position += Vector3.left;

        if(CheckIsValidPosition())
        {
            GameManager.instance.UpdateGrid(this);
            PlaySoundClip(0);
        }else
        {
            transform.position += Vector3.right;
        }
    }


     void Rotate()
     {
         if(allowRotation)
            {
                transform.Rotate(0,0,-90);

                if(CheckIsValidPosition())
                {
                    GameManager.instance.UpdateGrid(this);
                    PlaySoundClip(1);
                }
                else
                {
                   transform.Rotate(0,0,90);
                }       
            }
     }


     void PlaySoundClip(int clip)
     {
         SoundManager.instance.PlaySound(clip);
     }
}
    


