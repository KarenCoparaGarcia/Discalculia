using System.Collections;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WritingHandler : MonoBehaviour
{
	public GameObject[] numbers; //lista de numeros
	public Sprite[] nSprites; // sprites de numeros
	public Animator windDialog; //animacion de pantalla final
	public SpriteRenderer WinDiaNumSprite;// Sprite del dialogo del numero final
	public static int currnumindex;//index del numero actual
	private bool clickAreanumero;//click comienza a mover
	private int previousTracingPointIndex;// index del numero previo
	private ArrayList currtracingpoints; // index de los puntos del tracing
	private Vector3 prevpos,currpos = Vector3.zero; //click posicion previa
	public GameObject linearenprefab;// prefabs de la linea
	public GameObject circlePointPrefab;//circle point prefab
	private GameObject currentlineren = null;
	public Material dibujarmat;
	private bool numberdone = false;
	private bool setRandomColor =true;
	private bool clickinicia; //Dibujar Linea con el mouse
	public Transform hand;
	public bool showCursor;
	public AudioClip sonidoanimado;
	public AudioClip sonidopositivo;
	public AudioClip sonidoerror;
	//private int contadornivelcompleto =0;

	IEnumerator Start ()
	{   

		Cursor.visible = showCursor;//show curosr or hide
		currtracingpoints = new ArrayList ();//initiate the current tracing points
		LoadNumber();
		yield return 0;
	}


	//Executes Every Single Frame
	void Update ()
	{

		if (numberdone) {//if the letter is done then skip the next
			return;
		}

		if (Input.GetKeyDown (KeyCode.Escape)) {//on escape pressed
			BackToMenu ();//back to menu 
		}

		RaycastHit2D hit2d = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero);//raycast hid c

		if (hit2d.collider != null) {
			if (Input.GetMouseButtonDown (0)) {
				TouchNumberHandle (hit2d.collider.gameObject, true, Camera.main.ScreenToWorldPoint (Input.mousePosition));//touch for letter move(drawing);
				clickinicia = true;
			} else if (clickinicia) {
				TouchNumberHandle (hit2d.collider.gameObject, false, Camera.main.ScreenToWorldPoint (Input.mousePosition));//touch for letter move(drawing);
			}  
		}
		if (Input.GetMouseButtonUp (0)) {

			if (clickinicia) {
				EndTouchNumberHandle ();
				clickinicia = false;
				clickAreanumero = false;
			}
		}
	
		if (hand != null) {
			
			//	Debug.Log ("clickPosition");
			//drag the hand on screen
			Vector3 clickPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			clickPosition.z = -6;
			hand.position = clickPosition;
		
		}
	}
	//Number touch hanlder
	private void TouchNumberHandle (GameObject ob, bool isTouchBegain, Vector3 touchPos)
	{
		string obTag = ob.tag;// name of button that ray hit it
		bool flag1 = (obTag == "Number" || obTag == "TracingPoint" || obTag == "Background") && currentlineren != null;
		bool flag2 = (obTag == "TracingPoint");

		if (flag1 && !isTouchBegain) {//Touch Moved
			if (obTag == "TracingPoint") {
				TracingPoint tracingPoint = ob.GetComponent<TracingPoint> ();//get the current tracing point
				int currentindex = tracingPoint.index;//get the tracing point index
				if (tracingPoint.single_touch) {//skip if the touch is single
					return;
				}

				if (currentindex != previousTracingPointIndex) {
					currtracingpoints.Add (currentindex);
					;//add the current tracing point to the list
					previousTracingPointIndex = currentindex;//set the previous tracing point
				}
			} else if (obTag == "Background") {
				clickAreanumero = true;
				EndTouchNumberHandle ();
				clickinicia = false;
				return;
			}

			currpos = touchPos;
			currpos.z = -5.0f;
			float distance = Mathf.Abs (Vector3.Distance (currpos, new Vector3 (prevpos.x, prevpos.y, currpos.z)));//the distance between the current touch and the previous touch
			if (distance <= 0.1f) {//0.1 is distance offset
				return;	
			}

			prevpos = currpos;//set the previous position

			InstaitaeCirclePoint (currpos, currentlineren.transform);//create circle point

			//add the current point to the current line
			LineRenderer ln = currentlineren.GetComponent<LineRenderer> ();
			LineRendererAttributes line_attributes = currentlineren.GetComponent<LineRendererAttributes> ();
			int numberOfPoints = line_attributes.NumberOfPoints;
			numberOfPoints++;
			line_attributes.Points.Add (currpos);
			line_attributes.NumberOfPoints = numberOfPoints;
			ln.SetVertexCount (numberOfPoints);
			ln.SetPosition (numberOfPoints - 1, currpos);
		} else if (flag2 && isTouchBegain) {//Touch Began
			TracingPoint tracingPoint = ob.GetComponent<TracingPoint> ();//get the tracing point
			int currentindex = tracingPoint.index;//get the tracing point index
			if (currentindex != previousTracingPointIndex) {
				currtracingpoints.Add (currentindex);//add the current tracing point to the list
				previousTracingPointIndex = currentindex;//set the previous tracing point

				if (currentlineren == null) {
					currentlineren = (GameObject)Instantiate (linearenprefab);//instaiate new line
					if (setRandomColor) {
						currentlineren.GetComponent<LineRendererAttributes> ().SetRandomColor ();//set a random color for the line
						setRandomColor = false;
					}
				}

				Vector3 currpos = touchPos;//ge the current touch position
				currpos.z = -5.0f;
				prevpos = currpos;//set the previous position

				if (tracingPoint.single_touch) {
					InstaitaeCirclePoint (currpos, currentlineren.transform);//create circle point
				} else {
					InstaitaeCirclePoint (currpos, currentlineren.transform);//create circle point

					//add the current point to the current line
					LineRenderer ln = currentlineren.GetComponent<LineRenderer> ();
					LineRendererAttributes line_attributes = currentlineren.GetComponent<LineRendererAttributes> ();
					int numberOfPoints = line_attributes.NumberOfPoints;

					numberOfPoints++;
					if (line_attributes.Points == null) {
						line_attributes.Points = new List<Vector3> ();
					}

					line_attributes.Points.Add (currpos);
					line_attributes.NumberOfPoints = numberOfPoints;
					ln.SetVertexCount (numberOfPoints);
					ln.SetPosition (numberOfPoints - 1, currpos);
				}
			}
		} 
	}

	//On tocuh released
	private void EndTouchNumberHandle ()
	{

		if (currentlineren == null || currtracingpoints.Count == 0) {
			return;//skip the next
		}


		TracingPart [] tracingParts = numbers [currnumindex].GetComponents<TracingPart> ();//get the tracing parts of the current letter
		bool equivfound = false;//whether a matching or equivalent tracing part found
		if (!clickAreanumero) {

			foreach (TracingPart part in tracingParts) {//check tracing parts
				if (currtracingpoints.Count == part.order.Length && !part.succeded) {
					if (PreviousColliderCorrecto (part, tracingParts)) {//check whether the previous tracing parts are succeeded
						equivfound = true;//assume true
						for (int i =0; i < currtracingpoints.Count; i++) {
							int index = (int)currtracingpoints [i];
							if (index != part.order [i]) {
								equivfound = false;
								break;
							}
						}
					}
				}
				if (equivfound) {//if equivalent found 

					part.succeded = true;//then the tracing part is succeed (written as wanted)
					break;
				}
			}
		}

		if (equivfound) {//if equivalent found 

			if (currtracingpoints.Count != 1) {
				StartCoroutine ("SmoothCurrentLine");//make the current line smoother
			} else {
				currentlineren = null;
			}
			PlaySonidoPositivo ();//play positive sound effect
		} else {
			PlaySonidoError ();//play sonido en caso de existir error
			Destroy (currentlineren);//destroy the current line
			currentlineren = null;//release the current line
		}

		prevpos = Vector2.zero;//resetea posicion previa
		currtracingpoints.Clear ();//limpia record
		previousTracingPointIndex = 0;//resetea index previo seleccionado
		CheckNumberDone ();//check if the entier letter is written successfully or done
		if (numberdone) {//if the current letter done or wirrten successfully
			if (sonidoanimado != null)
				AudioSource.PlayClipAtPoint (sonidoanimado, Vector3.zero, 0.8f);//play the cheering sound effect
			hand.GetComponent<SpriteRenderer> ().enabled = false;//hide the hand

		}
	}

	//Check number done or not
	private void CheckNumberDone ()
	{
		bool success = true;//number  success or done flag
		TracingPart [] tracingParts = numbers [currnumindex].GetComponents<TracingPart> ();//get the tracing parts of the current letter
		foreach (TracingPart part in tracingParts) {
			if (!part.succeded) {
				success = false;
				break;
			}
		}

		if (success) {
			
    
			numberdone = true;//number done flag
			/*menu.SetActive(false);
			GameObject [] linesRenderes = GameObject.FindGameObjectsWithTag("LineRenderer");
			foreach (GameObject line in linesRenderes){
				line.GetComponent<LineRenderer> ().enabled = false;
			}
			GameObject [] circlePoint = GameObject.FindGameObjectsWithTag("CirclePoint");
			foreach (GameObject cp in circlePoint){
				cp.GetComponent<MeshRenderer> ().enabled = false;
			}*/
			Debug.Log ("Esta haciendo el numero " + numbers [currnumindex].name);
		}
	}

	//Back To Menu
	private void BackToMenu ()
	{
		Application.LoadLevel ("MenuNivel");
	}

	//Refresh the lines and reset the tracing parts
	public void RefreshProcess ()
	{
		RefreshLines ();
		TracingPart [] tracingParts = numbers [currnumindex].GetComponents<TracingPart> ();
		foreach (TracingPart part in tracingParts) {
			part.succeded = false;
		}
		if (hand != null)
			hand.GetComponent<SpriteRenderer> ().enabled = true;
		numberdone = false;
	}

	//Refreesh the lines
	private void RefreshLines ()
	{
		StopCoroutine ("SmoothCurrentLine");
		GameObject [] gameobjs = HierrachyManager.FindActiveGameObjectsWithTag ("LineRenderer");
		if (gameobjs == null) {
			return;
		}
		foreach (GameObject gob in gameobjs) {
			Destroy (gob);	
		}
	}

	//Make the current lime more smoother
	private IEnumerator SmoothCurrentLine ()
	{
		LineRendererAttributes line_attributes = currentlineren.GetComponent<LineRendererAttributes> ();
		LineRenderer ln = currentlineren.GetComponent<LineRenderer> ();
		Vector3[] vectors = SmoothCurve.MakeSmoothCurve (line_attributes.Points.ToArray (), 10);

		int childscount = currentlineren.transform.childCount;
		for (int i = 0; i < childscount; i++) {
			Destroy (currentlineren.transform.GetChild (i).gameObject);
		}

		line_attributes.Points.Clear ();
		for (int i = 0; i <vectors.Length; i++) {
			if (i == 0 || i == vectors.Length - 1)
				InstaitaeCirclePoint (vectors [i], currentlineren.transform);
			line_attributes.NumberOfPoints = i + 1;
			line_attributes.Points.Add (vectors [i]);
			ln.SetVertexCount (i + 1);
			ln.SetPosition (i, vectors [i]);
		}
		currentlineren = null;
		yield return new WaitForSeconds (0);
	}

	//Check If User Passed The Previous Parts Before The Give Letter Part
	public static bool PreviousColliderCorrecto (TracingPart currentpart, TracingPart[] lparts)
	{
		int p = currentpart.priority;

		if (p == 1) {
			return true;
		}

		bool prevsucceded = true;
		foreach (TracingPart part in lparts) {
			if (part.priority < p) {
				if (!part.succeded && part.order.Length != 1) {//make single point TracingParts have no priority
					prevsucceded = false;
					break;
				}
			}
		}

		return prevsucceded;
	}

	//Play a positive or correct sound effect
	private void PlaySonidoPositivo ()
	{
		if (sonidopositivo != null)
			Debug.Log ("Chequear sonido 2");
		AudioSource.PlayClipAtPoint (sonidopositivo, Vector3.zero, 0.8f);//play the cheering sound effect
	}

	//Play wrong or opps sound effect
	private void PlaySonidoError ()
	{
		if (sonidoerror != null)
			Debug.Log ("Chequear sonido 3");
		AudioSource.PlayClipAtPoint (sonidoerror, Vector3.zero, 0.8f);//play the cheering sound effect
	}

	//Load the next letter
	public void LoadNextNumber ()
	{

		if (currnumindex == numbers.Length - 1) {
			currnumindex = 0;
			Application.LoadLevel ("MenuNivel");
		} else if (currnumindex >= 0 && currnumindex < numbers.Length - 1) {
			currnumindex++;
			LoadNumber ();
		}
	}

	//Load the previous letter
	public void LoadPreviousNumber ()
	{

		if (currnumindex > 0 && currnumindex < numbers.Length) {
			currnumindex--;
			LoadNumber();
		}

	}

	//Load the current letter
	private void LoadNumber()
	{
		if (numbers == null) {
			return;
		}

		if (!(currnumindex >= 0 && currnumindex < numbers.Length)) {
			return;
		}

		if (numbers [currnumindex] == null) {
			return;
		}
		numberdone = false;
		RefreshProcess ();
		OcultarNumeros ();

		numbers [currnumindex].SetActive (true);

		setRandomColor = true;
	}

	//Esconder los numeros
	private void OcultarNumeros ()
	{
		if (numbers == null) {
			return;
		}

		foreach (GameObject Number in numbers) {
			if (Number != null)
				Number.SetActive (false);
		}
	}

	//Create Cicle at given Point
	private void InstaitaeCirclePoint (Vector3 position, Transform parent)
	{
		
		GameObject currentcicrle = (GameObject)Instantiate (circlePointPrefab);//instaiate object
		//currentcicrle.transform.SetParent (circlePointPrefab.gameObject.transform);
		currentcicrle.GetComponent<Renderer>().material = currentlineren.GetComponent<LineRendererAttributes> ().material;
		currentcicrle.transform.position = position;
	}
}
