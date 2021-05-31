using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Linea : MonoBehaviour {
	LineRenderer linea;
	List<Vector2> puntos;
	Vector2 ultimoPunto;

	private void Awake(){
		linea = GetComponent<LineRenderer> ();

	}

	public void DibujarLinea(Vector2 ratonPos)
	{
		if (puntos == null) {
			puntos = new List<Vector2> ();
			DibujarPunto (ratonPos);
			return;
		}

		if(Vector2.Distance(ultimoPunto,ratonPos) >= .05f)
		{
			DibujarPunto(ratonPos);
		}


	}

	void DibujarPunto(Vector2 punto)
	{
		puntos.Add (punto);
		linea.positionCount = puntos.Count;
		linea.SetPosition (puntos.Count - 1, punto);
	}
}
