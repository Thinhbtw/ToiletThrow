using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassShattered : MonoBehaviour
{
    [SerializeField] List<GameObject> glass_Pieces;
    [SerializeField] List<GlassPiecesPhysics> glassPiecesPhysics;
    [Tooltip("0 - left | 1 - right | 2 - down")]
    [SerializeField] int direction;
    [SerializeField] Vector2 forceDir;

    private void Start()
    {
        foreach(GlassPiecesPhysics i in glassPiecesPhysics)
        {
            i.direction = direction;
            i.forceDir = forceDir;
        }
    }

    public void EnablePieces()
    {
        this.gameObject.SetActive(false);
        foreach(GameObject i in glass_Pieces)
        {
            i.SetActive(true);
        }
    }
}
