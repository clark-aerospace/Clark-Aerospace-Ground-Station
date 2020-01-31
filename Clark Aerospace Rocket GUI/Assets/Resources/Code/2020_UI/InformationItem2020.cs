using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

[System.Serializable]
public class InformationItem2020 : MonoBehaviour
{
    [SerializeField]
    private string _title = "Title";

    [SerializeField]
    private string _value = "--";

    [SerializeField]
    private string dataSource = "null";

    [SerializeField]
    private string _suffix = "";

    private GameObject instanceGameObject;
    private RectTransform instanceGameObjectRectTransform;

    private TextMeshProUGUI _titleLabel;
    private TextMeshProUGUI _valueLabel;


    public string Value {
        set {
            //_valueLabel.text = value;
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

        instanceGameObjectRectTransform = instanceGameObject.GetComponent<RectTransform>();
        instanceGameObjectRectTransform.anchorMax = new Vector2(0,0);
        instanceGameObjectRectTransform.anchorMin = new Vector2(0,0);
        instanceGameObjectRectTransform.anchoredPosition = new Vector2(0,0);



    }

    // Update is called once per frame
    void Update()
    {
        _titleLabel.text = _title;


        if (dataSource != "inherit") {
            RocketData d = ArduinoReceiver2020.instance.latestData;
            if (d != null)
                _value = d.GetFloatValue(dataSource).ToString();
            else
                _value = "NULL";
        }
        _valueLabel.text = _value + _suffix; 
    }
}


[CustomEditor(typeof(InformationItem2020))]
public class InformationItem2020Editor : Editor 
{
	SerializedProperty titleProperty;
	SerializedProperty valueProperty;
    SerializedProperty dataSourceProperty;
    SerializedProperty suffixProperty;


    void OnEnable() {
        titleProperty = serializedObject.FindProperty("_title");
        valueProperty = serializedObject.FindProperty("_value");
        dataSourceProperty = serializedObject.FindProperty("dataSource");
        suffixProperty = serializedObject.FindProperty("_suffix");
    }

	public override void OnInspectorGUI()
	{
        serializedObject.Update();
        EditorGUILayout.LabelField("Visible Properties", EditorStyles.boldLabel);
		EditorGUILayout.PropertyField(titleProperty);
        EditorGUILayout.PropertyField(valueProperty);
        EditorGUILayout.PropertyField(suffixProperty);
        EditorGUILayout.PropertyField(dataSourceProperty);


        serializedObject.ApplyModifiedProperties ();
    }
}