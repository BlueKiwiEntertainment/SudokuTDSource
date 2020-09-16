using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDummy1 : MonoBehaviour
{
    public GameObject AssociatedGameObject;
    
    


    private void Awake()
    {
        
    }
    private void OnMouseDown()
    {
        if(GameManager.instance.SelectionAllowed)
        {
            Action();
        }   
        
    }

    public void Action()
    {
        GameManager.instance.GameObjectsToReactivate.Add(this.gameObject);   
        Instantiate(AssociatedGameObject, this.gameObject.transform.position, this.gameObject.transform.rotation);
        this.gameObject.SetActive(false);
        

    }

}
