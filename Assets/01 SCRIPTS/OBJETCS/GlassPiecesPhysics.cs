using System.Collections;
using UnityEngine;
using DG.Tweening;

public class GlassPiecesPhysics : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb2d;

    [SerializeField] float torque;
    float ranTorque, ranForceX, ranForceY;
    public Vector2 forceDir;
    public int direction;
    [SerializeField] SpriteRenderer spriteRenderer;

    private void OnEnable()
    {
        Physics2D.IgnoreLayerCollision(7, 8);
        Physics2D.IgnoreLayerCollision(7, 7);
        ranTorque = Random.Range(-20f, 20f);
        switch(direction)
        {
            case 0:
                ranForceX = Random.Range(-forceDir.x -50, -forceDir.x);
                ranForceY = Random.Range(forceDir.y, forceDir.y + 50);
                break;
            case 1:
                ranForceX = Random.Range(forceDir.x, forceDir.x + 50);
                ranForceY = Random.Range(forceDir.y, forceDir.y + 50);
                break;
            case 2:
                ranForceY = Random.Range(forceDir.y, forceDir.y + 50);
                break;

        }   
        forceDir.x = ranForceX;
        forceDir.y = ranForceY;

        rb2d.AddForce(forceDir);
        rb2d.AddTorque(ranTorque);

        spriteRenderer.DOFade(0f, 2f);
        StartCoroutine(TurnOffGameObject());
    }
    IEnumerator TurnOffGameObject()
    {
        yield return new WaitForSeconds(2f);
        this.gameObject.SetActive(false);
    }
}
