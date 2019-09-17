﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

[System.Serializable]
public class InformationItem2020 : MonoBehaviour
{
    [SerializeField]
    private string _title;

    [SerializeField]
    private string _value;

    private GameObject instanceGameObject;

    private TextMeshProUGUI _titleLabel;
    private TextMeshProUGUI _valueLabel;


    public string Value {
        set {
            _valueLabel.text = value;
            _value = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        instanceGameObject = Instantiate(UIManager2020.instance.InformationItemPrefab);
        instanceGameObject.name = "Information Item - " + (_title != null ? "No Title" : _title);
        _titleLabel = instanceGameObject.transform.Find("InformationItem_Title").GetComponent<TextMeshProUGUI>();
        _valueLabel = instanceGameObject.transform.Find("InformationItem_Value").GetComponent<TextMeshProUGUI>();

        instanceGameObject.transform.SetParent(transform, false);



    }

    // Update is called once per frame
    void Update()
    {
        _titleLabel.text = _title;
        _valueLabel.text = _value; 
    }
}


[CustomEditor(typeof(InformationItem2020))]
public class InformationItem2020Editor : Editor 
{
	SerializedProperty titleProperty;
	SerializedProperty valueProperty;

    void OnEnable() {
        titleProperty = serializedObject.FindProperty("_title");
        valueProperty = serializedObject.FindProperty("_value");
    }

	public override void OnInspectorGUI()
	{
        serializedObject.Update();
        EditorGUILayout.LabelField("Visible Properties", EditorStyles.boldLabel);
		EditorGUILayout.PropertyField(titleProperty);
        EditorGUILayout.PropertyField(valueProperty);

        serializedObject.ApplyModifiedProperties ();
    }
}