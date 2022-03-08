using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pikachu : MonoBehaviour
{
    public GameObject Background;
    public GameObject Icon;
    public Node nodePikachu;

    private SpriteRenderer icon;
    private SpriteRenderer backGround;

    void Start()
    {
        
        
    }

    public void SetIconPikachu(Sprite pikachuImage)
    {
        icon = Icon.GetComponent<SpriteRenderer>();
        icon.sprite = pikachuImage;
    }

    public void SetBackgroundIsPick()
    {
        backGround = Background.GetComponent<SpriteRenderer>();
        backGround.color = new Color(backGround.color.r, backGround.color.g, backGround.color.b, 1);
    }
    public void resetBackground()
    {
        backGround = Background.GetComponent<SpriteRenderer>();
        backGround.color = new Color(backGround.color.r, backGround.color.g, backGround.color.b, 0.5f);
    }
}
