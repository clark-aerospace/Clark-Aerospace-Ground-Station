using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconFollow : MonoBehaviour {
    public Transform objectToOverlay;

	public Image imageComponent;

	RectTransform rectTransform;
	// Use this for initialization
	void Start () {
		rectTransform = GetComponent<RectTransform>();
		imageComponent = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (objectToOverlay == null) {return;}
		
		Vector2 viewportPoint = Camera.main.WorldToViewportPoint(objectToOverlay.position);
		RectTransform canvasRect = rectTransform.parent.parent.gameObject.GetComponent<RectTransform>();

		Vector2 finalViewpointPort = new Vector2(
			(viewportPoint.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f),
			(viewportPoint.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)
		);

		rectTransform.anchoredPosition = finalViewpointPort;
	}
}