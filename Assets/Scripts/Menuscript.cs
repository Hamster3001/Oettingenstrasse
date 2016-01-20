﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class Menuscript : MonoBehaviour {

	public static string[] roomlist = {
		"001",
		"002",
		"003",
		"004",
		"005",
		"006",
		"008",
		"010",
		"012",
		"016",
		"022",
		"024",
		"026",
		"027",
		"027",
		"028",
		"030",
		"032",
		"033",
		"034",
		"036",
		"038",
		"039",
		"040",
		"040A",
		"041",
		"042",
		"042A",
		"043",
		"044",
		"044A",
		"045",
		"046",
		"049",
		"050",
		"052",
		"053",
		"054",
		"055",
		"056",
		"057",
		"057",
		"058",
		"060",
		"061",
		"061",
		"062",
		"064",
		"065",
		"066",
		"067",
		"067",
		"068",
		"070",
		"071",
		"072",
		"073",
		"074",
		"075",
		"076",
		"077",
		"078",
		"079",
		"666N",
		"666P",
		"A001",
		"A002",
		"A003",
		"A004",
		"A005",
		"A007",
		"A008",
		"A008",
		"A009",
		"A011",
		"A012",
		"A013",
		"A014",
		"A015",
		"A016",
		"A070",
		"A090",
		"A090",
		"B001",
		"B001",
		"C001",
		"C002",
		"C003",
		"C004",
		"C006",
		"C007",
		"C007",
		"C008",
		"C010",
		"C011",
		"C012",
		"C070",
		"C090",
		"D001",
		"D002",
		"D003",
		"D004",
		"D005",
		"D006",
		"D007",
		"D008",
		"D009",
		"D010",
		"D011",
		"D012",
		"D090A",
		"D091",
		"E001",
		"E002",
		"E003",
		"E004",
		"E005",
		"E006",
		"E007",
		"E008",
		"E009",
		"E010",
		"E011",
		"E012",
		"E070",
		"E071",
		"F001",
		"F002",
		"F003",
		"F004",
		"F005",
		"F006",
		"F007",
		"F008",
		"F009",
		"F010",
		"F011",
		"F012",
		"F070",
		"F071",
		"G001",
		"G002",
		"G003",
		"G004",
		"G005",
		"G006",
		"G007",
		"G008",
		"G009",
		"G009",
		"G010",
		"G010",
		"G070",
		"G071",
		"H001",
		"H002",
		"H003",
		"H004",
		"H005",
		"H006",
		"H007",
		"H008",
		"H009",
		"H010",
		"H012",
		"H070",
		"H071",
		"L001",
		"L001",
		"L003",
		"L004",
		"L004",
		"L005",
		"L006",
		"L007",
		"L009",
		"L011",
		"L012",
		"L012A",
		"L013",
		"L013",
		"L014",
		"L014A",
		"L016",
		"L016A",
		"L018",
		"L018A",
		"L020",
		"L020A",
		"L022",
		"L022A",
		"L024",
		"L024A",
		"L025",
		"L026",
		"L026A",
		"L027",
		"L028",
		"L028",
		"L030",
		"L041",
		"L041",
		"L042",
		"L043",
		"L044",
		"L045",
		"L046",
		"L049",
		"L055",
		"L070",
		"L071001",
		"002",
		"003",
		"004",
		"005",
		"006",
		"008",
		"010",
		"012",
		"016",
		"022",
		"024",
		"026",
		"027",
		"027",
		"028",
		"030",
		"032",
		"033",
		"034",
		"036",
		"038",
		"039",
		"040",
		"040A",
		"041",
		"042",
		"042A",
		"043",
		"044",
		"044A",
		"045",
		"046",
		"049",
		"050",
		"052",
		"053",
		"054",
		"055",
		"056",
		"057",
		"057",
		"058",
		"060",
		"061",
		"061",
		"062",
		"064",
		"065",
		"066",
		"067",
		"067",
		"068",
		"070",
		"071",
		"072",
		"073",
		"074",
		"075",
		"076",
		"077",
		"078",
		"079",
		"666N",
		"666P",
		"A001",
		"A002",
		"A003",
		"A004",
		"A005",
		"A007",
		"A008",
		"A008",
		"A009",
		"A011",
		"A012",
		"A013",
		"A014",
		"A015",
		"A016",
		"A070",
		"A090",
		"A090",
		"B001",
		"B001",
		"C001",
		"C002",
		"C003",
		"C004",
		"C006",
		"C007",
		"C007",
		"C008",
		"C010",
		"C011",
		"C012",
		"C070",
		"C090",
		"D001",
		"D002",
		"D003",
		"D004",
		"D005",
		"D006",
		"D007",
		"D008",
		"D009",
		"D010",
		"D011",
		"D012",
		"D090A",
		"D091",
		"E001",
		"E002",
		"E003",
		"E004",
		"E005",
		"E006",
		"E007",
		"E008",
		"E009",
		"E010",
		"E011",
		"E012",
		"E070",
		"E071",
		"F001",
		"F002",
		"F003",
		"F004",
		"F005",
		"F006",
		"F007",
		"F008",
		"F009",
		"F010",
		"F011",
		"F012",
		"F070",
		"F071",
		"G001",
		"G002",
		"G003",
		"G004",
		"G005",
		"G006",
		"G007",
		"G008",
		"G009",
		"G009",
		"G010",
		"G010",
		"G070",
		"G071",
		"H001",
		"H002",
		"H003",
		"H004",
		"H005",
		"H006",
		"H007",
		"H008",
		"H009",
		"H010",
		"H012",
		"H070",
		"H071",
		"L001",
		"L001",
		"L003",
		"L004",
		"L004",
		"L005",
		"L006",
		"L007",
		"L009",
		"L011",
		"L012",
		"L012A",
		"L013",
		"L013",
		"L014",
		"L014A",
		"L016",
		"L016A",
		"L018",
		"L018A",
		"L020",
		"L020A",
		"L022",
		"L022A",
		"L024",
		"L024A",
		"L025",
		"L026",
		"L026A",
		"L027",
		"L028",
		"L028",
		"L030",
		"L041",
		"L041",
		"L042",
		"L043",
		"L044",
		"L045",
		"L046",
		"L049",
		"L055",
		"L070",
		"L071"
	};

	public static string searchstring;
	public static string locationstring;
	public static bool vrEnabled = true;
	public static bool locationEnabled = true;
	public static bool movementEnabled = true;
	public static bool triggerEnabled = false;

	public GameObject navDialog;
	public GameObject locDialog;
	public GameObject loadingDialog;
	public InputField navInputField;
	public InputField locInputField;
	public Toggle toggleVR;
	public Toggle toggleLocation;
	public Toggle toggleMovement;
	public Toggle toggleTrigger;

	// Use this for initialization
	void Start () {
		toggleVR.isOn = vrEnabled;
		toggleLocation.isOn = locationEnabled;
		toggleMovement.isOn = movementEnabled;
		toggleTrigger.isOn = triggerEnabled;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void StartNavigation() {
		string input = navInputField.text;

		if (roomlist.Contains(input)) {
			searchstring = input;

			if (locationEnabled) {
				navDialog.SetActive(false);
				loadingDialog.SetActive(true);
				Application.LoadLevel(1);
			}
			else {
				navDialog.SetActive(false);
				locDialog.SetActive(true);
			}
		}
		else {
			navInputField.text = "Room not found";
		}
	}

	public void StartLocation() {
		string input = locInputField.text;
		
		if (roomlist.Contains(input)) {
			locationstring = input;

			locDialog.SetActive(false);
			loadingDialog.SetActive(true);
			Application.LoadLevel(1);
		}
		else {
			locInputField.text = "Room not found";
		}
	}

	public void ResetNav() {
		navInputField.text = "Enter a roomnumber";
		locInputField.text = "Enter a roomnumber";
	}

	public void EnableVR() {
		vrEnabled = toggleVR.isOn;
		if (vrEnabled) {
			toggleTrigger.gameObject.SetActive(true);
		}
		else {
			toggleTrigger.gameObject.SetActive(false);
		}
	}

	public void EnableTrigger() {
		triggerEnabled = toggleTrigger.isOn;
	}

	public void EnableLocation() {
		locationEnabled = toggleLocation.isOn;
	}

	public void EnableMovement() {
		movementEnabled = toggleMovement.isOn;
	}

	public void closeApp(){

		Application.Quit();

	}
}
