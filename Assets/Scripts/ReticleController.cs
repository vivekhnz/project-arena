using UnityEngine;
using System.Collections;

public class ReticleController : MonoBehaviour
{
    public Texture2D ReticleTexture;

	void Start ()
    {
        Cursor.SetCursor(ReticleTexture, new Vector2(0.5f, 0.5f), CursorMode.Auto);
	}

	void FixedUpdate()
    {
	}

    void OnDestroy()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
