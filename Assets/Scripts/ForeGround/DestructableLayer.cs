using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DestructableLayer : MonoBehaviour {
    [Tooltip("破坏层的偏移量")]
    public Vector2 offset = new Vector2(0.2f, 0.2f);

    private Tilemap destructableTilemap;
    private Vector3[] position = new Vector3[8];

    void Start() {
        destructableTilemap = GetComponent<Tilemap>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Bullet") {
            Vector3 hitPosition = collision.bounds.ClosestPoint(collision.transform.position);
            position[0] = new Vector3(hitPosition.x, hitPosition.y + offset.y, 0f);
            position[1] = new Vector3(hitPosition.x, hitPosition.y - offset.y, 0f);
            position[2] = new Vector3(hitPosition.x + offset.x, hitPosition.y , 0f);
            position[3] = new Vector3(hitPosition.x + offset.x, hitPosition.y + offset.y, 0f);
            position[4] = new Vector3(hitPosition.x + offset.x, hitPosition.y - offset.y, 0f);
            position[5] = new Vector3(hitPosition.x - offset.x, hitPosition.y, 0f);
            position[6] = new Vector3(hitPosition.x - offset.x, hitPosition.y + offset.y, 0f);
            position[7] = new Vector3(hitPosition.x - offset.x, hitPosition.y - offset.y, 0f);
        
            for(int i = 0; i < 8; i++) {
                Vector3Int pos = destructableTilemap.WorldToCell(position[i]);
                destructableTilemap.SetTile(pos, null);
            }

            Destroy(collision.gameObject);
        }
    }
}
