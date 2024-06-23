using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace ItemIconCreator
{
    public class IconCreatorCanvas : MonoBehaviour
    {
        public Text textLabel;
        public GameObject borders;

        public static IconCreatorCanvas instance;

        private void Awake()
        {
            instance = this;
        }

        public void SetInfo(int totalItens, int currentItem, string itemName, bool isRecording, KeyCode key)
        {
            borders.gameObject.SetActive(isRecording);

            if (isRecording == false)
            {
                textLabel.text = "Go to your icon builder in hierarchy and press 'Build icons'";

                return;
            }

            textLabel.text = currentItem + " / " + totalItens + " - " + itemName +  "   |   Press <b>" + key + "</b> to continue" ;

        }

        public void SetTakingPicture()
        {
            textLabel.text = "Generating icon...";
        }
    }
}
