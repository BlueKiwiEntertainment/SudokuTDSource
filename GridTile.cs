using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile : MonoBehaviour
{

    public Transform InstantPoint;
    public GameObject DummyObj;
    public bool HasBuilding = false;
    GameObject tooltip;
    public GameObject BuildEffect;



    private void OnMouseEnter()
    {
        BuildingUIScript.instance.SelectedTile = this.gameObject;
        if(!HasBuilding)
        {
            if (BuildingUIScript.instance.SelectedObj != null)
            {
                tooltip = Instantiate(BuildingUIScript.instance.SelectedObj, InstantPoint.position, InstantPoint.rotation);
                tooltip.tag = "tooltep";

                tooltip.GetComponentInChildren<SkinnedMeshRenderer>().material.SetInt("_UseEmission", 1);
                tooltip.GetComponentInChildren<SkinnedMeshRenderer>().material.SetColor("_EmissionColor", Color.green);
                tooltip.GetComponentInChildren<Collider>().enabled = false;
            }
            
        }
        
        
    }

    private void OnMouseExit()
    {
        Destroy(tooltip);
    }

    private void OnMouseDown()
    {
        BuildingUIScript.instance.PleaseBuild();
        if(Input.GetAxis("Fire3") == 0)
        {
            BuildingUIScript.instance.ClearSelection();
        }
        else
        {
            BuildingUIScript.instance.SelectAgain();
        }
    }

    public void Build(GameObject obj)
    {
        if (!HasBuilding)
        {
            

            Instantiate(obj, InstantPoint.transform.position, InstantPoint.transform.rotation);
            HasBuilding = true;
            GameObject effect = Instantiate(BuildEffect, InstantPoint.position, InstantPoint.rotation);
            Destroy(effect, 3);
        }

    }
}
