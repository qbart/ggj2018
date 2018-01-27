using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockText : MonoBehaviour
{
    TextMesh mesh;

    public float textWidth
    {
        get
        {
            float width = 0;
            foreach (char symbol in mesh.text)
            {
                CharacterInfo info;
                if (mesh.font.GetCharacterInfo(symbol, out info, mesh.fontSize, mesh.fontStyle))
                {
                    width += info.advance;
                }
            }
            return width * mesh.characterSize * 0.1f;
        }
    }

    public string text
    {
        get
        {
            return mesh.text;
        }

        set
        {
            mesh.text = value;
        }
    }

    public void changeStyle(bool invalid)
    {
        if (invalid)
        {
            mesh.color = Color.red;
        }
        else
        {
            mesh.color = Color.white;
        }
    }

    void Awake()
    {
        mesh = GetComponent<TextMesh>();
    }
	

}
