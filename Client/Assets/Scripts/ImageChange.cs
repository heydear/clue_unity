using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageChange : MonoBehaviour
{
    public Image myImage;



    public void GetImage(Image targetImage)
    {
        myImage.sprite = targetImage.sprite;
    }
}
