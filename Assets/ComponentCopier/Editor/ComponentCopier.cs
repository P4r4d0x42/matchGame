using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;

class ComponentCopier : EditorWindow {
    
	Vector2 scrollPos;
	static bool[] foldout;
	static bool[] selectAll;
	static Component[] components;
	static Type[] t;
	static bool[] enabled;
	static FieldInfo[][] fields;
	static PropertyInfo[][] properties;
	static object[][] fieldVals;
	static object[][] propertyVals;

	static bool[] selectedComponents;
	static bool[][] selectedFields;
	static bool[][] selectedProperties;
    
    [MenuItem ("Component/Copy Components")]
	[MenuItem ("CONTEXT/Transform/Copy Components")]
    static void Copy () {
    	if (!Selection.activeGameObject)
		{
			EditorUtility.DisplayDialog("Nothing selected", "Select a gameobject first.", "oops");
    		return;
		}
		
		components = Selection.activeGameObject.GetComponents<Component>();
		
		selectedComponents = new bool[components.Length];
		t = new Type[components.Length];
		enabled = new bool[components.Length];
		fields = new FieldInfo[components.Length][];
		properties = new PropertyInfo[components.Length][];
		fieldVals = new object[components.Length][];
		propertyVals = new object[components.Length][];
		
		selectedFields = new bool[components.Length][];
		selectedProperties = new bool[components.Length][];
		foldout = new bool[components.Length];
		selectAll = new bool[components.Length];
		
		for(int i = 0; i < components.Length; i++)
		{
			t[i] = components[i].GetType();
			
			if(t[i].IsSubclassOf(typeof(Behaviour)))
				enabled[i] = (components[i] as Behaviour).enabled;
			else if(t[i].IsSubclassOf(typeof(Component)) && t[i].GetProperty("enabled") != null)
				enabled[i] = (bool) t[i].GetProperty("enabled").GetValue(components[i], null);
			else
				enabled[i] = true;
			
			fields[i] = t[i].GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			properties[i] = t[i].GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			
			fieldVals[i] = new object[fields[i].Length];
			propertyVals[i] = new object[properties[i].Length];
			
			selectedFields[i] = new bool[fields[i].Length];
			selectedProperties[i] = new bool[properties[i].Length];
			
			foldout[i] = false;
		}
		
		EditorWindow.GetWindow(typeof(ComponentCopier), true, "Which components would you like to copy?", true);
    }
	
	void OnGUI()
	{
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
			
			for (int i = 0; i < components.Length; i++)
			{

				selectedComponents[i] = EditorGUILayout.BeginToggleGroup(t[i].Name, selectedComponents[i]);
					
				if(fields[i].Length > 0 || properties[i].Length > 0)
				{
					foldout[i] = EditorGUILayout.Foldout(foldout[i], t[i].Name + " fields and properties");
					if(foldout[i])
					{
						if((fields[i].Length + properties[i].Length) > 1)
						{
							EditorGUI.BeginChangeCheck();
							selectAll[i] = EditorGUILayout.Toggle("Select All", selectAll[i]);
							if(EditorGUI.EndChangeCheck())
								SelectDeselectAll(i);
						}
						
						if(fields[i].Length > 0)
						{
							EditorGUILayout.LabelField("Fields:", "");
							for(int j = 0; j < fields[i].Length; j++)
								selectedFields[i][j] = EditorGUILayout.Toggle(fields[i][j].Name, selectedFields[i][j]);
						}
			
						if(properties[i].Length > 0)
						{
							EditorGUILayout.LabelField("Properties:", "");
				         	for(int j = 0; j < properties[i].Length; j++)
								selectedProperties[i][j] = EditorGUILayout.Toggle(properties[i][j].Name, selectedProperties[i][j]);
						}
					}
				}
				EditorGUILayout.EndToggleGroup();
			}
		

		EditorGUILayout.EndScrollView();
		
		if(GUILayout.Button("Copy", GUILayout.Height(30)))
		{
			CopyData();
			this.Close();
		}
		
	}
	
	static void SelectDeselectAll(int componentIndex)
	{
		if(fields[componentIndex].Length > 0)
		{
			for(int j = 0; j < fields[componentIndex].Length; j++)
				selectedFields[componentIndex][j] = selectAll[componentIndex];
		}

		if(properties[componentIndex].Length > 0)
		{
         	for(int j = 0; j < properties[componentIndex].Length; j++)
				selectedProperties[componentIndex][j] = selectAll[componentIndex];
		}
	}
	
	void CopyData()
	{
		for(int i = 0; i < selectedComponents.Length; i++)
		{
			if(selectedComponents[i])
			{	
				for(int j = 0; j < selectedFields[i].Length; j++)
				{
					if(selectedFields[i][j])
						fieldVals[i][j] = fields[i][j].GetValue(components[i]);
				}
				
				for(int j = 0; j < selectedProperties[i].Length; j++)
				{
					if(selectedProperties[i][j])
					{
						if(properties[i][j].CanRead && properties[i][j].GetIndexParameters().Length == 0)
							propertyVals[i][j] = properties[i][j].GetValue(components[i], null); 
						else
							Debug.LogWarning(properties[i][j].Name + " could not be copied.");
					}
				}
			}
		}
	}

    [MenuItem ("Component/Paste Components")]
	[MenuItem ("CONTEXT/Transform/Paste Components")]
    static void Paste () {
		
		if (Selection.gameObjects.Length == 0)
    		return;
		
		Undo.RegisterSceneUndo("Paste Components");
    	
    	foreach (GameObject obj in Selection.gameObjects) {
    		
	    	for(int i = 0; i < selectedComponents.Length; i++)
			{
				if(selectedComponents[i])
				{
					Component c = obj.GetComponent(t[i]);
					
					if(c == null)
						c = obj.AddComponent(t[i]);
					
					if(t[i].IsSubclassOf(typeof(Behaviour)))
						(c as Behaviour).enabled = enabled[i];
					else if(t[i].IsSubclassOf(typeof(Component)) && t[i].GetProperty("enabled") != null)
						t[i].GetProperty("enabled").SetValue(c, enabled[i], null);
					
					for(int j = 0; j < selectedFields[i].Length; j++)
					{
						if(selectedFields[i][j])
							fields[i][j].SetValue(c, fieldVals[i][j]);
					}
					
					for(int j = 0; j < selectedProperties[i].Length; j++)
					{
						if(selectedProperties[i][j])
						{
							if(properties[i][j].CanWrite)
								properties[i][j].SetValue(c, propertyVals[i][j], null);
						}
					}
				}
			}
    	}
    	
    }
	
}
