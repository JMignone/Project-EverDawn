using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClanListItem : MonoBehaviour
{
    [SerializeField]
    private Text myText;

    public void setText(string textString){
        myText.textString = textString;
    }

    public void onClick(){
        
    }

}
