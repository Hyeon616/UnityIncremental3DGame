using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace DemoImage_WeaponPack3
{
    public class SetDemoImage : MonoBehaviour
    {
        public Image[] DemoImage = new Image[101];

        // Start is called before the first frame update
        void Start()
        {
            DemoImage = GetComponentsInChildren<Image>();
            for (int i = 1; i < DemoImage.Length; i++) // DemoImage[0] is background
            {
                DemoImage[i].sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Weapon Icons Pack3/Icons/Weapon Icons Pack3_" + i + ".png");
            }
        }
    }
}