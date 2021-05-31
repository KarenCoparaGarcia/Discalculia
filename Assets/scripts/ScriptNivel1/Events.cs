using UnityEngine;
using System.Collections;

/// Implement your game events in this script
public class Events : MonoBehaviour
{
	private WritingHandler writingHandler;
	public Animator winDialog;
	public GameObject menu;

		void Start ()
		{
				//Setting up the writingHandler reference
				GameObject numbers = HierrachyManager.FindActiveGameObjectWithName ("Numbers");
				if (numbers != null)
					writingHandler = numbers.GetComponent<WritingHandler> ();
		}

		//Cargar la siguiente letra
		public void LoadTheNextNumero (object ob)
		{
				if (writingHandler == null) {
						return;
				}
				writingHandler.LoadNextNumber ();
		}

		//Load the previous Numero
		public void LoadThePreviousNumber (object ob)
		{
				if (writingHandler == null) {
						return;
				}
				writingHandler.LoadPreviousNumber ();

		}

		//Load the current Number
		public void LoadNumber (Object ob)
		{
				if (ob == null) {
						return;
				}
				WritingHandler.currnumindex = int.Parse (ob.name.Split ('-') [1]);
				Application.LoadLevel ("NivelPrueba");
		}
	
		//Erase the current Number
		public void EraseNumber (Object ob)
		{
				if (writingHandler == null) {
						return;
				}
				writingHandler.RefreshProcess ();

		}

	//Load number menu
	public void LoadNumberMenu(Object ob){
		Application.LoadLevel ("MenuNivel");
	}
}