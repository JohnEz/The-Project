using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBarController : MonoBehaviour {

	public Image hpBar;
	[SerializeField]
	public GameObject hpMarkerPrefab;

	private List<GameObject> hpMarkers = new List<GameObject>();

	private const int markerInterval = 10;

	private float currentMax = 0;

	// Use this for initialization
	public void Initialize (float maxHp) {
		hpBar = transform.FindChild("hpBar").GetComponent<Image>();
		currentMax = maxHp;
		createMarkers (maxHp);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetHP(float currentHp, float maxHp) {
		hpBar.fillAmount = currentHp / maxHp;

		if (maxHp != currentMax) {
			currentMax = maxHp;
			destroyMarkers ();
			createMarkers (maxHp);
		}
	}

	public void SetHPColor(Color color) {
		hpBar.color = color;
	}

	public void createMarkers(float maxHp) {
		int numberOfMarkers = (int)(maxHp / markerInterval);
		float increment = hpBar.rectTransform.rect.width / (numberOfMarkers);

		for (int i = 0; i < numberOfMarkers-1; i++) {
			GameObject newMarker = createMarker (i, increment);
			hpMarkers.Add (newMarker);
		}
	}

	public GameObject createMarker(int index, float increment) {
		GameObject newMarker = Instantiate (hpMarkerPrefab);
		Vector3 newPosition = newMarker.transform.position;
		newMarker.transform.SetParent(hpBar.transform, false);
		newPosition.x = Mathf.RoundToInt((index+1)*increment);
		newMarker.GetComponent<RectTransform> ().anchoredPosition = newPosition;
		return newMarker;
	}

	public void destroyMarkers() {
		hpMarkers.ForEach ((hpMarker) => {
			Destroy (hpMarker);
		});
		hpMarkers.Clear ();
	}
}
