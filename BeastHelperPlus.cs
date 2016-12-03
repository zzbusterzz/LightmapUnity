using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class BeastHelperPlus : EditorWindow
{
	int ArraySize = 1;
	Vector2 scrollpos1;
	static GameObject[] staticobjects; // Static Gameobjects Array
	static object[,,] arrayobjects = new object[255,150,3];  // array relative to baketex array, contains gamobject(s) and tiling info for each lightmap

	static object[,,] arrayobjectsRealtime = new object[255,150,3];  // array relative to realtimetex array, contains gamobject(s) and tiling info for each lightmap

	Texture2D[] baketex = new Texture2D[255]; //array of lightmaps
	Texture2D[] lighttex = new Texture2D[255];//arry of light intensity on lightmaps

	//---------------------------------------------

	// Add menu named "My Window" to the Window menu
	[MenuItem("Window/Beast Helper Plus")]
	static void Init()
	{
		// Get existing open window or if none, make a new one:
		BeastHelperPlus window = (BeastHelperPlus)EditorWindow.GetWindow(typeof(BeastHelperPlus));
		window.autoRepaintOnSceneChange = true;
		window.title = "Beast Helper Plus";
		window.Show();

		//-------------- Get Lightmapped geometry... ----------------

		GameObject[] gameobjects = (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject));

		int i1 = 0;
		foreach (GameObject g in gameobjects)
		{
			if (g.isStatic)
			{
				i1++;
			}
		}

		staticobjects = new GameObject[i1];

		int i2 = 0;
		foreach (GameObject g in gameobjects)
		{
			if (g.isStatic)
			{
				staticobjects.SetValue((GameObject)g, i2);
				i2++;
			}
		}

		Debug.Log("Static GameObjects count = " + i1);

		for (int i = 0; i < arrayobjects.GetLength(0); i++)
		{
			for (int i3 = 0; i3 < arrayobjects.GetLength(1); i3++)
			{
				arrayobjects[i, i3, 0] = null;
				arrayobjects[i, i3, 1] = (Vector4)new Vector4();
				arrayobjects[i, i3, 2] = (bool)false;
			}
		}

		for (int i = 0; i < arrayobjectsRealtime.GetLength(0); i++)
		{
			for (int i3 = 0; i3 < arrayobjectsRealtime.GetLength(1); i3++)
			{
				arrayobjectsRealtime[i, i3, 0] = null;
				arrayobjectsRealtime[i, i3, 1] = (Vector4)new Vector4();
				arrayobjectsRealtime[i, i3, 2] = (bool)false;
			}
		}

		//----------------------------------------------------------

	}

	void OnGUI()
	{

		GUILayout.Label("Stored Lightmaps", EditorStyles.boldLabel);

		ArraySize = EditorGUILayout.IntField("Array Size(" + ArraySize.ToString() + ")", ArraySize);

		GUILayoutOption[] layout1 = new GUILayoutOption[2];

		layout1[0] = (GUILayoutOption)GUILayout.Height(75);
		layout1[1] = (GUILayoutOption)GUILayout.Width(75);


		scrollpos1 = EditorGUILayout.BeginScrollView(scrollpos1);
		for (int i = 0; i < ArraySize; i++)
		{

			// Debug.Log("Loop1 i = " + i);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(i.ToString(),"", GUILayout.Width(20));
			baketex[i] = (Texture2D)EditorGUILayout.ObjectField((Texture2D)baketex.GetValue(i), typeof(Texture2D), false, layout1);
			lighttex [i] = (Texture2D)EditorGUILayout.ObjectField ((Texture2D)lighttex.GetValue (i), typeof(Texture2D), false, layout1);

			//----------- Each GameObject whit [i] lightmap index go here -----------

			for (int i2 = 0; i2 < arrayobjects.GetLength(1); i2++)
			{

				GUILayout.BeginVertical(GUILayout.Width(300));
				EditorGUILayout.LabelField("GameObject: " + i2.ToString(), "", GUILayout.Width(300));
				arrayobjects[i, i2, 0] = (GameObject)EditorGUILayout.ObjectField((GameObject)arrayobjects[i, i2, 0], typeof(GameObject), false, GUILayout.Width(300));
				arrayobjects[i, i2, 1] = (Vector4)EditorGUILayout.Vector4Field("Tiling (X-Y) / Offset (X-Y)", (Vector4)arrayobjects[i, i2, 1], GUILayout.Width(300));
				arrayobjects[i, i2, 2] = (bool)EditorGUILayout.Toggle("Use: ",(bool)arrayobjects[i, i2, 2], GUILayout.Width(300));
				GUILayout.EndVertical();

			}

			for (int i2 = 0; i2 < arrayobjectsRealtime.GetLength(1); i2++)
			{

				GUILayout.BeginVertical(GUILayout.Width(300));
				EditorGUILayout.LabelField("GameObject: " + i2.ToString(), "", GUILayout.Width(300));
				arrayobjectsRealtime[i, i2, 0] = (GameObject)EditorGUILayout.ObjectField((GameObject)arrayobjectsRealtime[i, i2, 0], typeof(GameObject), false, GUILayout.Width(300));
				arrayobjectsRealtime[i, i2, 1] = (Vector4)EditorGUILayout.Vector4Field("Tiling (X-Y) / Offset (X-Y)", (Vector4)arrayobjectsRealtime[i, i2, 1], GUILayout.Width(300));
				arrayobjectsRealtime[i, i2, 2] = (bool)EditorGUILayout.Toggle("Use: ",(bool)arrayobjectsRealtime[i, i2, 2], GUILayout.Width(300));
				GUILayout.EndVertical();

			}


			//-------------------------------------------------------------------------


			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndScrollView();

		Repaint();

		EditorGUILayout.BeginHorizontal();
		//-------------- Buttons -------------------

		if(GUILayout.Button("Read current maps"))
		{
			if (LightmapSettings.lightmaps.Length > ArraySize)
				ArraySize = LightmapSettings.lightmaps.Length;

			Repaint();

			for (int i = 0; i < LightmapSettings.lightmaps.Length; i++)
			{
				// Debug.Log("Loop2 i = " + i);
				baketex.SetValue(LightmapSettings.lightmaps[i].lightmapFar,i);
				lighttex.SetValue (LightmapSettings.lightmaps [i].lightmapNear, i);

				//------------- Get objects with same lightmap index ---------
				if (baketex.GetValue(i) != null)
				{
					var i2 = 0;
					foreach (GameObject r in staticobjects)
					{
						if (r.GetComponent<Renderer>())
						{
							if (r.GetComponent<Renderer>().lightmapIndex == i)
							{
								arrayobjects[i, i2, 0] = (GameObject)r; // The real GameObject
								arrayobjects[i, i2, 1] = (Vector4)r.GetComponent<Renderer>().lightmapScaleOffset; // Tiling Lightmap properties
								arrayobjects[i, i2, 2] = (bool)true; // use it??
								i2++;
							}

							if (r.GetComponent<Renderer>().realtimeLightmapIndex == i)
							{
								arrayobjectsRealtime[i, i2, 0] = (GameObject)r; // The real GameObject
								arrayobjectsRealtime[i, i2, 1] = (Vector4)r.GetComponent<Renderer>().realtimeLightmapScaleOffset; // Tiling Lightmap properties
								arrayobjectsRealtime[i, i2, 2] = (bool)true; // use it??
								i2++;
							}
						}

					}
				}
				//------------------------------------------------------------
			}

			Repaint();
		}

		if (GUILayout.Button("Load Maps info from Disk"))
		{
			string path = EditorUtility.OpenFilePanel("Select beast maps info file", Application.dataPath, "ini");
			if (path.Length > 0)
			{
				//-------------- Loading .ini file -------------
				cIni inifile = new cIni(path);

				string mapscountstr = inifile.ReadValue("MAPSCOUNT", "COUNT");

				int mapscount = 0;
				if (mapscountstr.Length > 0)
				{
					mapscount = int.Parse(mapscountstr);
				}

				if (ArraySize < mapscount)
				{
					ArraySize = mapscount;
					Repaint();
				}

				int arrayofsset = 0;

				bool cancel = false;

				switch (EditorUtility.DisplayDialogComplex("Merge with current maps?", "If you choose merge, the loaded maps will be added after the current ones", "Merge", "Replace", "Cancel"))
				{
				case 0:
					{
						for (int i = 0; i < ArraySize; i++)
						{
							if (baketex[i] != null)
								arrayofsset++;// = i;
						}
						/*
                            if (arrayofsset > 0)
                                arrayofsset++;
*/
						if ((arrayofsset + mapscount) > ArraySize)
						{
							ArraySize = arrayofsset + mapscount;
						}
					}
					break;
				case 1:
					{
						for (int i = 0; i < ArraySize; i++)
						{
							// Debug.Log("Loop2 i = " + i);
							baketex.SetValue(null, i);
							lighttex.SetValue (null, i);
							for (int i2 = 0; i2 < arrayobjects.GetLength(1); i2++)
							{
								arrayobjects[i, i2, 0] = null;
								arrayobjects[i, i2, 1] = (Vector4)new Vector4();
								arrayobjects[i, i2, 2] = (bool)false;
							}

							for (int i2 = 0; i2 < arrayobjectsRealtime.GetLength(1); i2++)
							{
								arrayobjectsRealtime[i, i2, 0] = null;
								arrayobjectsRealtime[i, i2, 1] = (Vector4)new Vector4();
								arrayobjectsRealtime[i, i2, 2] = (bool)false;
							}

						}
						Repaint(); 
					}
					break;
				case 2:
					{
						cancel = true;
					}
					break;
				}

				if (cancel == false)
				{
					int counter1 = 0;
					for (int i = arrayofsset; i < ArraySize; i++)
					{
						string texpath = inifile.ReadValue("MAPS", counter1.ToString());

						if (texpath.Length > 0)
						{
							baketex.SetValue((Texture2D)AssetDatabase.LoadAssetAtPath(texpath, typeof(Texture2D)), i);
						}

						texpath = inifile.ReadValue("MAPSDIRCOMP", counter1.ToString());
						if (texpath.Length > 0)
						{
							lighttex.SetValue((Texture2D)AssetDatabase.LoadAssetAtPath(texpath, typeof(Texture2D)), i);
						}
						counter1++;
					}

					for (int i2 = arrayofsset; i2 < ArraySize; i2++)
					{
						int counter2 = 0;
						for (int i3 = 0; i3 < arrayobjects.GetLength(1); i3++)
						{
							string gpath = inifile.ReadValue("MAP" + counter2.ToString(), "GAMEOBJECT" + i3.ToString());
							string gtiling = inifile.ReadValue("MAP" + counter2.ToString(), "TILING" + i3.ToString());
							string guse = inifile.ReadValue("MAP" + counter2.ToString(), "USE" + i3.ToString());
							Debug.Log(inifile.ReadValue("MAP" + counter2.ToString(), "GAMEOBJECT" + i3.ToString()));
							if (gpath.Length > 0)
							{
								arrayobjects[i2, i3, 0] = (GameObject)GameObject.Find(gpath);
								arrayobjects[i2, i3, 1] = (Vector4)vector4fromstring(gtiling);
								arrayobjects[i2, i3, 2] = (bool)Convert.ToBoolean(guse);
							}
						}

						for (int i3 = 0; i3 < arrayobjectsRealtime.GetLength(1); i3++)
						{
							string gpath = inifile.ReadValue("MAP" + counter2.ToString(), "GAMEOBJECT" + i3.ToString());
							string gtiling = inifile.ReadValue("MAP" + counter2.ToString(), "TILING" + i3.ToString());
							string guse = inifile.ReadValue("MAP" + counter2.ToString(), "USE" + i3.ToString());
							Debug.Log(inifile.ReadValue("MAP" + counter2.ToString(), "GAMEOBJECT" + i3.ToString()));
							if (gpath.Length > 0)
							{
								arrayobjectsRealtime[i2, i3, 0] = (GameObject)GameObject.Find(gpath);
								arrayobjectsRealtime[i2, i3, 1] = (Vector4)vector4fromstring(gtiling);
								arrayobjectsRealtime[i2, i3, 2] = (bool)Convert.ToBoolean(guse);
							}
						}

						counter2++;
					}
				}
				//-----------------------------------------------
			}
		}

		if(GUILayout.Button("Store Maps info to Disk"))
		{
			EditorUtility.DisplayDialog("Important!", "If you want to start a new bake operation whitout lose current maps, move them to another folder in the project view! Beast will remove old maps otherwise!", "Ok");

			string path = EditorUtility.SaveFilePanel("Save beast maps info to file", Application.dataPath, "beast_info", "ini");
			if (path.Length > 0)
			{
				//-------------- Creating .ini file -------------
				if (File.Exists(path))
					File.Delete(path); // Delete old ini file, otherwise files content will be merged.

				cIni inifile = new cIni(path);

				inifile.WriteValue("MAPSCOUNT", "COUNT", ArraySize.ToString());

				for (int i = 0; i < ArraySize; i++)
				{
					inifile.WriteValue("MAPS",i.ToString(), AssetDatabase.GetAssetPath(baketex[i]));
					inifile.WriteValue("MAPSDIRCOMP", i.ToString(), AssetDatabase.GetAssetPath(lighttex[i]));
				}

				for (int i2 = 0; i2 < ArraySize; i2++)
				{
					for (int i3 = 0; i3 < arrayobjects.GetLength(1); i3++)
					{
						inifile.WriteValue("MAP" + i2.ToString(), "GAMEOBJECT" + i3.ToString(), GetHierarchy((GameObject)arrayobjects[i2,i3,0]));
						Vector4 tiling = (Vector4)arrayobjects[i2, i3, 1];
						inifile.WriteValue("MAP" + i2.ToString(), "TILING" + i3.ToString(), "(" + tiling.x.ToString() + "," + tiling.y.ToString() + "," + tiling.w.ToString() + "," + tiling.z.ToString() + ")");
						inifile.WriteValue("MAP" + i2.ToString(), "USE" + i3.ToString(), arrayobjects[i2, i3, 2].ToString().ToLower());
					}

					for (int i3 = 0; i3 < arrayobjectsRealtime.GetLength(1); i3++)
					{
						inifile.WriteValue("MAP" + i2.ToString(), "GAMEOBJECT" + i3.ToString(), GetHierarchy((GameObject)arrayobjectsRealtime[i2,i3,0]));
						Vector4 tiling = (Vector4)arrayobjectsRealtime[i2, i3, 1];
						inifile.WriteValue("MAP" + i2.ToString(), "TILING" + i3.ToString(), "(" + tiling.x.ToString() + "," + tiling.y.ToString() + "," + tiling.w.ToString() + "," + tiling.z.ToString() + ")");
						inifile.WriteValue("MAP" + i2.ToString(), "USE" + i3.ToString(), arrayobjectsRealtime[i2, i3, 2].ToString().ToLower());
					}
				}

				//-----------------------------------------------
			}



		}

		if(GUILayout.Button("Set actual Maps in Beast"))
		{
			bool cancel = false;



			if(!cancel)
			{
				if (EditorUtility.DisplayDialog("Do you want to apply these maps??", "If you have unsaved maps in beast, you will lose them", "Apply", "Cancel"))
				{

					LightmapData[] maps = LightmapSettings.lightmaps;
					Array.Resize(ref maps, ArraySize);


					for (int c2 = 0; c2 < maps.Length; c2++)
					{
						maps.SetValue(new LightmapData(), c2);
					}

					for (int c1 = 0; c1 < ArraySize; c1++)
					{
						maps[c1].lightmapFar = baketex[c1];
						maps [c1].lightmapNear = lighttex [c1];
					}

					LightmapSettings.lightmaps = maps;


					for (int i2 = 0; i2 < ArraySize; i2++)
					{
						for (int i3 = 0; i3 < arrayobjects.GetLength(1); i3++)
						{
							bool use = (bool)arrayobjects[i2, i3, 2];

							if (use == true)
							{
								GameObject g = (GameObject)arrayobjects[i2, i3, 0];
								g.GetComponent<Renderer>().lightmapIndex = i2;
								//g.GetComponent<Renderer> ().realtimeLightmapIndex = i2;
								Debug.Log(g.GetComponent<Renderer>().lightmapIndex);
								g.GetComponent<Renderer>().lightmapScaleOffset = (Vector4)arrayobjects[i2, i3, 1];
							}
						}

						for (int i3 = 0; i3 < arrayobjectsRealtime.GetLength(1); i3++)
						{
							bool use = (bool)arrayobjectsRealtime[i2, i3, 2];

							if (use == true)
							{
								GameObject g = (GameObject)arrayobjectsRealtime[i2, i3, 0];
								g.GetComponent<Renderer>().lightmapIndex = i2;
								//g.GetComponent<Renderer> ().realtimeLightmapIndex = i2;
								Debug.Log(g.GetComponent<Renderer>().lightmapIndex);
								g.GetComponent<Renderer>().lightmapScaleOffset = (Vector4)arrayobjectsRealtime[i2, i3, 1];
							}
						}
					}

				}
			}
		}

		//------------------------------------------
		EditorGUILayout.EndHorizontal();

		//--------------- Down Buttons -------------

		EditorGUILayout.BeginHorizontal();

		if (GUILayout.Button("Clear All"))
		{
			for (int i = 0; i < ArraySize; i++)
			{
				// Debug.Log("Loop2 i = " + i);
				baketex.SetValue(null, i);
				lighttex.SetValue (null, i);
				for (int i2 = 0; i2 < arrayobjects.GetLength(1); i2++)
				{
					arrayobjects[i, i2, 0] = null;
					arrayobjects[i, i2, 1] = (Vector4)new Vector4();
					arrayobjects[i, i2, 2] = (bool)false;
				}

				for (int i2 = 0; i2 < arrayobjectsRealtime.GetLength(1); i2++)
				{
					arrayobjectsRealtime[i, i2, 0] = null;
					arrayobjectsRealtime[i, i2, 1] = (Vector4)new Vector4();
					arrayobjectsRealtime[i, i2, 2] = (bool)false;
				}

			}
			Repaint();
		}

		//if (GUILayout.Button("Reload Gameobject List"))
		//{

		//}

		if (GUILayout.Button("Close"))
		{
			Close();
		}

		EditorGUILayout.EndHorizontal();

		if (GUI.changed)
			Repaint();

	}

	// Class for handling .ini files...

	public class cIni
	{

		[DllImport("kernel32", SetLastError = true)]
		private static extern int WritePrivateProfileString(string pSection, string pKey, string pValue, string pFile);
		[DllImport("kernel32", SetLastError = true)]
		private static extern int WritePrivateProfileStruct(string pSection, string pKey, string pValue, int pValueLen, string pFile);
		[DllImport("kernel32", SetLastError = true)]
		private static extern int GetPrivateProfileString(string pSection, string pKey, string pDefault, byte[] prReturn, int pBufferLen, string pFile);
		[DllImport("kernel32", SetLastError = true)]
		private static extern int GetPrivateProfileStruct(string pSection, string pKey, byte[] prReturn, int pBufferLen, string pFile);

		private string ls_IniFilename;
		private int li_BufferLen = 256;

		/// <summary>
		/// cINI Constructor
		/// </summary>
		public cIni(string pIniFilename)
		{
			ls_IniFilename = pIniFilename;
		}

		/// <summary>
		/// INI filename (If no path is specifyed the function will look with-in the windows directory for the file)
		/// </summary>
		public string IniFile
		{
			get { return (ls_IniFilename); }
			set { ls_IniFilename = value; }
		}

		/// <summary>
		/// Max return length when reading data (Max: 32767)
		/// </summary>
		public int BufferLen
		{
			get { return li_BufferLen; }
			set
			{
				if (value > 32767) { li_BufferLen = 32767; }
				else if (value < 1) { li_BufferLen = 1; }
				else { li_BufferLen = value; }
			}
		}

		/// <summary>
		/// Read value from INI File
		/// </summary>
		public string ReadValue(string pSection, string pKey, string pDefault)
		{

			return (z_GetString(pSection, pKey, pDefault));

		}

		/// <summary>
		/// Read value from INI File, default = ""
		/// </summary>
		public string ReadValue(string pSection, string pKey)
		{

			return (z_GetString(pSection, pKey, ""));

		}

		/// <summary>
		/// Write value to INI File
		/// </summary>
		public void WriteValue(string pSection, string pKey, string pValue)
		{

			WritePrivateProfileString(pSection, pKey, pValue, this.ls_IniFilename);

		}

		/// <summary>
		/// Remove value from INI File
		/// </summary>
		public void RemoveValue(string pSection, string pKey)
		{

			WritePrivateProfileString(pSection, pKey, null, this.ls_IniFilename);

		}

		/// <summary>
		/// Read values in a section from INI File
		/// </summary>
		public void ReadValues(string pSection, ref Array pValues)
		{

			pValues = z_GetString(pSection, null, null).Split((char)0);

		}

		/// <summary>
		/// Read sections from INI File
		/// </summary>
		public void ReadSections(ref Array pSections)
		{

			pSections = z_GetString(null, null, null).Split((char)0);

		}

		/// <summary>
		/// Remove section from INI File
		/// </summary>
		public void RemoveSection(string pSection)
		{

			WritePrivateProfileString(pSection, null, null, this.ls_IniFilename);

		}

		/// <summary>
		/// Call GetPrivateProfileString / GetPrivateProfileStruct API
		/// </summary>
		private string z_GetString(string pSection, string pKey, string pDefault)
		{

			string sRet = pDefault; byte[] bRet = new byte[li_BufferLen];
			int i = GetPrivateProfileString(pSection, pKey, pDefault, bRet, li_BufferLen, ls_IniFilename);
			sRet = System.Text.Encoding.GetEncoding(1252).GetString(bRet, 0, i).TrimEnd((char)0);
			return (sRet);

		}

	}

	//Return the full hierarchy string of a gameObject, ready for use in GameObject.Find method later...
	string GetHierarchy(GameObject g)
	{
		if (!g || g == null)
		{
			return "";
		}

		string hierarchy = null;
		string gname = g.name;

		Transform parent = g.transform.parent;

		while (parent)
		{
			hierarchy += "/" + parent.gameObject.name;
			parent = parent.parent;
		}
		string inverse = gname + hierarchy;

		string[] arrayh = (string[])inverse.Split(new string[] { "/" }, StringSplitOptions.None);
		Array.Reverse(arrayh);

		return "/" + string.Join("/", arrayh);

	}

	Vector4 vector4fromstring(string vectorstring)
	{
		if (vectorstring == null || vectorstring.Length == 0)
			return new Vector4();

		vectorstring = vectorstring.Substring(1, vectorstring.Length - 2);

		string[] arrayvector = (string[])vectorstring.Split(new string[] { "," }, StringSplitOptions.None);

		Vector4 vectorfinal = new Vector4(float.Parse(arrayvector[0]), float.Parse(arrayvector[1]), float.Parse(arrayvector[3]), float.Parse(arrayvector[2]));

		return vectorfinal;
	}

}