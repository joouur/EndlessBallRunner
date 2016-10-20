using UnityEngine;
using System.Collections;

public class StatusCore : MonoBehaviour {

    public float Distance;
    public Vector3 startPosition;
    public float HighScore;
    public int deaths;
    void Start()
    {
        Distance = 0;
        startPosition = transform.position;
        HighScore = 0;
    }
	
	// Update is called once per frame
    void Update()
    {
        Distance = gameObject.transform.position.z;
        if (transform.position.y <= -20)
        {
            transform.position = startPosition;
            Change();
            deaths++;
        }
        if (Distance > HighScore)
            HighScore = Distance;
        
    }

    void OnGUI()
    {
        GUI.Box(new Rect(50, 10, 200, 30), "HighScore = " + HighScore.ToString("f0"));
        GUI.Box(new Rect(50, 40, 200, 30), "Score = " + Distance.ToString("f0"));
        GUI.Box(new Rect(50, 70, 200, 30), "Deaths = " + deaths.ToString("f0"));
    }

    public void Change()
    {
        Color newColor = new Color(Random.value, Random.value, Random.value, 1.0f);

        // apply it on current object's material
        renderer.material.color = newColor;
    }
}
