using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteAlways]
public class TextWobble : MonoBehaviour
{
	public AnimationCurve upwardMotion, lateralMotion, alphaOverTime;
	public float offsetTime;
	public float offsetTimePerCharacter;

	TextContainer container;
	TMP_Text text;
	
	void Awake()
	{
		text = GetComponent<TMP_Text>();
		container = GetComponent<TextContainer>();        
		text.ForceMeshUpdate();
    }
	void Start()
    {

    }

    private void Update()
    {
		if (!text) return;
		text.ForceMeshUpdate();
		var mesh = text.mesh;
		var vertices = mesh.vertices;

        for (int i = 0; i < text.textInfo.characterCount; i++)
        {
			TMP_CharacterInfo info = text.textInfo.characterInfo[i];

			if (info.character == ' ') continue;

			var index = info.vertexIndex;
			int meshIndex = info.materialReferenceIndex;


			float evaluationTime = Mathf.Max(0, offsetTime - offsetTimePerCharacter * i);

			vertices[index] += new Vector3(lateralMotion.Evaluate(evaluationTime), upwardMotion.Evaluate(evaluationTime));
			vertices[index + 1] += new Vector3(lateralMotion.Evaluate(evaluationTime), upwardMotion.Evaluate(evaluationTime));
			vertices[index + 2] += new Vector3(lateralMotion.Evaluate(evaluationTime), upwardMotion.Evaluate(evaluationTime));
			vertices[index + 3] += new Vector3(lateralMotion.Evaluate(evaluationTime), upwardMotion.Evaluate(evaluationTime));

			var color = new Color(text.color.r, text.color.g, text.color.b, alphaOverTime.Evaluate(evaluationTime));

			Color32[] vertexColors = text.textInfo.meshInfo[meshIndex].colors32;
			vertexColors[index + 0] = color;
			vertexColors[index + 1] = color;
			vertexColors[index + 2] = color;
			vertexColors[index + 3] = color;
		}

		mesh.vertices = vertices;
		text.canvasRenderer.SetMesh(mesh);

		for (int i = 0; i < text.textInfo.characterCount; i++)
		{
			TMP_CharacterInfo info = text.textInfo.characterInfo[i];

			if (info.character == ' ') continue;

			var index = info.vertexIndex;
			int meshIndex = info.materialReferenceIndex;


			float evaluationTime = Mathf.Max(0, offsetTime - offsetTimePerCharacter * i);

			var color = new Color(text.color.r, text.color.g, text.color.b, alphaOverTime.Evaluate(evaluationTime));

			Color32[] vertexColors = text.textInfo.meshInfo[meshIndex].colors32;
			vertexColors[index + 0] = color;
			vertexColors[index + 1] = color;
			vertexColors[index + 2] = color;
			vertexColors[index + 3] = color;
		}

		text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
	}
}
