using UnityEngine;

public class GroundCollider : MonoBehaviour
{
    [SerializeField] private BoxCollider2D hitBox;
    [SerializeField] private SpriteRenderer mySprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hitBox.enabled = true;
        hitBox.size = new Vector2(3, mySprite.size.y);
    }
}
