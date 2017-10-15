﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	public Text displayText;
	public InputAction[] inputActions;
	public Animator StartGameAnimator;
	public Animator LoadLevelAnimator;
	public GameObject StartTextScreen;
	public GameObject DialogTextScreen;
	public InputField inputField;
	public Dictionary<string, string> inventoryDictionary = new Dictionary<string, string> ();

	[SerializeField]
	private GameObject CreditsTextScreen;
	[SerializeField]
	private Text RedButtonStartScreenText;


	[HideInInspector] public RoomNavigation roomNavigation;
	[HideInInspector] public List<string> interactionDescriptionsInRoom = new List<string> ();
	[HideInInspector] public InteractableItems interactableItems;
	// Use this for initialization

	List<string> actionLog = new List<string> ();
	void Awake () {
		interactableItems = GetComponent<InteractableItems> ();
		roomNavigation = GetComponent<RoomNavigation> ();
	}

		

	public void showCredits() {

		if (StartTextScreen.activeSelf){
			StartTextScreen.SetActive (false);
			DialogTextScreen.SetActive (false);
			CreditsTextScreen.SetActive (true);
			RedButtonStartScreenText.text = "Go Back";
		} else {
			StartTextScreen.SetActive (true);
			DialogTextScreen.SetActive (true);
			CreditsTextScreen.SetActive (false);
			RedButtonStartScreenText.text = "Credits";
		}
	}

	public void LoadLevel(string levelName) {

		SceneManager.LoadScene (levelName);

	}

	public void RestartGame () {

		SceneManager.LoadScene ("Main");

	}

	public void QuitGame () {

		Application.Quit ();

	}

	public void StartGame() {

		StartGameAnimator.SetBool ("IsActive", false);
		inputField.ActivateInputField();

	}
		

	void Start(){
		DisplayRoomText ();
		DisplayLoggedText ();
	}

	public void DisplayLoggedText() {
		string logAsText = string.Join ("\n", actionLog.ToArray ());

		displayText.text = logAsText;
	}

	public void DisplayRoomText()
	{
		ClearCollectionForNewRoom ();

		UnpackRoom ();

		string joinedInteractionDescriptions = string.Join ("\n", interactionDescriptionsInRoom.ToArray ());

		string combinedText = roomNavigation.currentRoom.description + "\n" + joinedInteractionDescriptions;

		if (!combinedText.Equals ("")) {
			LogStringWithReturn (combinedText);
		}
	}

	void UnpackRoom() {
		roomNavigation.UnpackExitsInRoom ();
		PrepareObjectsToTakeOrExamine (roomNavigation.currentRoom);
	}

	void PrepareObjectsToTakeOrExamine(Room currentRoom) {
	
		for (int i = 0; i < currentRoom.interactableObjectsInRoom.Length; i++) {

			string descriptionNotInInventory = interactableItems.GetObjectsNotInInventory (currentRoom, i);
			if (descriptionNotInInventory != null) {

				interactionDescriptionsInRoom.Add (descriptionNotInInventory);
			}
				

			InteractableObject interactableInRoom = currentRoom.interactableObjectsInRoom [i];

			for (int j = 0; j < interactableInRoom.interactions.Length; j++) {
				Interaction interaction = interactableInRoom.interactions [j];
				if (interaction.inputAction.keyWord == "examine" && !(interactableItems.nounsInInventory.Contains(interactableInRoom.noun))) {
					interactableItems.examineDictionary.Add(interactableInRoom.noun, interaction.textResponse);
				}

				if (interaction.inputAction.keyWord == "take") {
					interactableItems.takeDictionary.Add(interactableInRoom.noun, interaction.textResponse);
	
				}


			}
		}
	}

	public string TestVerbDictionarywithNoun(Dictionary<string, string> verbDictionary, string TestVerbDictionarywithNoun, string noun) {

		if (verbDictionary.ContainsKey (noun)) {

			if (TestVerbDictionarywithNoun.Equals ("take")) {
				//Examine items in inventory
				string itemDescription = verbDictionary[noun];
				int lengthDescription = itemDescription.Length;
				itemDescription = itemDescription.Substring(itemDescription.IndexOf("the") + 4);
				string itemName = noun;
				itemDescription = itemName + " (" + itemDescription;
				inventoryDictionary.Add(itemName, itemDescription);
			}

			return verbDictionary [noun];
		}



		return "You can't " + TestVerbDictionarywithNoun + " \"" + noun +"\"";
	}

	void ClearCollectionForNewRoom() {
		interactableItems.ClearCollections ();
		interactionDescriptionsInRoom.Clear ();
		roomNavigation.ClearExits ();
	}

	public void LogStringWithReturn(string stringToAdd) {
	
		actionLog.Add (stringToAdd + "\n");
	}

	// Update is called once per frame
	void Update () {
		
	}
}
