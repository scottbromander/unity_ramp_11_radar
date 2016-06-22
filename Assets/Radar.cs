using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Radar : MonoBehaviour {
	public float insideRadarDistance = 20;
	public float blipSizePercent = 5;

	public GameObject rawImageBlipCube;
	public GameObject rawImageBlipSphere;

	private RawImage rawImageRadarBackground;
	private Transform playerTransform;
	private float radarWidth;
	private float radarHeight;
	private float blipHeight;
	private float blipWidth;

	// Use this for initialization
	void Start () {
		playerTransform = GameObject.FindGameObjectWithTag ("Player").transform;
		rawImageRadarBackground = GetComponent<RawImage> ();

		radarWidth = rawImageRadarBackground.rectTransform.rect.width;
		radarHeight = rawImageRadarBackground.rectTransform.rect.height;

		blipHeight = radarHeight * blipSizePercent / 100;
		blipWidth = radarWidth * blipSizePercent / 100;
	}
	
	// Update is called once per frame
	void Update () {
		RemoveAllBlips ();
		FindAndDisplayBlipsForTag ("Cube", rawImageBlipCube);
		FindAndDisplayBlipsForTag ("Sphere", rawImageBlipSphere);
	}
		
	private void FindAndDisplayBlipsForTag(string tag, GameObject prefabBlip){
		//Get the player position
		Vector3 playerPos = playerTransform.position;
		//Find all the things with a specific tag, them store them into an array called 'targets'
		GameObject[] targets = GameObject.FindGameObjectsWithTag (tag);

		//Take that array, and we are going to do something to each one (each 'thing' is called 'target')
		foreach (GameObject target in targets) {
			Vector3 targetPos = target.transform.position;
			float distanceToTarget = Vector3.Distance (targetPos, playerPos);
			if ((distanceToTarget <= insideRadarDistance)) {
				Vector3 normalisedTargetPosition = NormalisedPosition (playerPos, targetPos);
				Vector2 blipPosition = CalculateBlipPosition (normalisedTargetPosition);
				DrawBlip (blipPosition, prefabBlip);
			}
		}
	}

	private void RemoveAllBlips() {
		GameObject[] blips = GameObject.FindGameObjectsWithTag ("Blip");
		foreach (GameObject blip in blips) {
			Destroy (blip);
		}
	}


	private Vector3 NormalisedPosition(Vector3 playerPos, Vector3 targetPos){
		float normalisedyTargetX = (targetPos.x - playerPos.x) / insideRadarDistance;
		float normalisedyTargetZ = (targetPos.z - playerPos.z) / insideRadarDistance;
		return new Vector3 (normalisedyTargetX, 0, normalisedyTargetZ);
	}

	private Vector2 CalculateBlipPosition(Vector3 targetPos){
		float angleToTarget = Mathf.Atan2 (targetPos.x, targetPos.z) * Mathf.Rad2Deg;
		float anglePlayer = playerTransform.eulerAngles.y;

		float angleRadarDegrees = angleToTarget - anglePlayer - 90;

		float normalisedDistanceToTarget = targetPos.magnitude;
		float angleRadians = angleRadarDegrees * Mathf.Deg2Rad;
		float blipX = normalisedDistanceToTarget * Mathf.Cos (angleRadians);
		float blipY = normalisedDistanceToTarget * Mathf.Sin (angleRadians);

		blipX *= radarWidth / 2;
		blipY *= radarHeight / 2;

		blipX += radarWidth / 2;
		blipY += radarHeight / 2;

		return new Vector2 (blipX, blipY);
	}

	private void DrawBlip (Vector2 pos, GameObject prefabBlip){
		GameObject blipGO = (GameObject)Instantiate (prefabBlip);
		blipGO.transform.SetParent (transform.parent);
		print (transform.parent);
		RectTransform rt = blipGO.GetComponent<RectTransform> ();
		rt.SetInsetAndSizeFromParentEdge (RectTransform.Edge.Left, pos.x, blipWidth);
		rt.SetInsetAndSizeFromParentEdge (RectTransform.Edge.Top, pos.y, blipHeight);
	}
}
