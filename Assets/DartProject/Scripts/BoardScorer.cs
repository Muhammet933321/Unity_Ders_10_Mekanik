using UnityEngine;

public class BoardScorer : MonoBehaviour
{
	[Header("References")]
	public Transform boardCenter;
	public Transform[] ringEdgePoints;

	[Header("Scores")]
	public int[] ringScores;
	public int missScore = 0;
	public bool logScore = false;

	private float[] ringRadii;

	private void Awake()
	{
		BuildRadii();
	}

	private void OnValidate()
	{
		BuildRadii();
	}

	private void BuildRadii()
	{
		if (boardCenter == null || ringEdgePoints == null)
			return;

		if (ringRadii == null || ringRadii.Length != ringEdgePoints.Length)
			ringRadii = new float[ringEdgePoints.Length];

		for (int i = 0; i < ringEdgePoints.Length; i++)
		{
			Transform point = ringEdgePoints[i];
			if (point == null)
				ringRadii[i] = 0f;
			else
				ringRadii[i] = Vector3.Distance(boardCenter.position, point.position);
		}
	}

	public int ScoreTip(Transform dartTip)
	{
		if (dartTip == null)
			return missScore;

		return ScorePoint(dartTip.position);
	}

	public int ScorePoint(Vector3 hitPosition)
	{
		if (boardCenter == null)
			return missScore;

		if (ringRadii == null || ringRadii.Length == 0)
			BuildRadii();

		float distance = Vector3.Distance(boardCenter.position, hitPosition);

		for (int i = 0; i < ringRadii.Length; i++)
		{
			float radius = ringRadii[i];
			if (radius <= 0f)
				continue;

			if (distance <= radius)
			{
				int score = GetScoreForRing(i);
				LogScore(score, distance);
				return score;
			}
		}

		LogScore(missScore, distance);
		return missScore;
	}

	private int GetScoreForRing(int index)
	{
		if (ringScores == null || index >= ringScores.Length)
			return missScore;

		return ringScores[index];
	}

	private void LogScore(int score, float distance)
	{
		if (!logScore)
			return;

		Debug.Log("Score: " + score + " Distance: " + distance.ToString());
	}
}
