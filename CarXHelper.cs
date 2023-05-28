using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using BepInEx.Configuration;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

public class CarXHelper : MonoBehaviour
{
    public static bool ready = false;
    public static RaceCar raceCar;
    public static CARXCar CARX;
    public static bool CR_running = false;
    //private static CarX.CarDesc desc;
    //private static CarX.CarDesc originalDesc;
    public static IEnumerator FindLocalCar()
    {
        CR_running = true;
        while( !(raceCar && raceCar.isLocalPlayer) )
        {
            yield return new WaitForSeconds( 0.5f );

            RaceCar[] raceCars = GameObject.FindObjectsOfType<RaceCar>();
            for (int i = 0; i < raceCars.Length; i++)
            {
                if (raceCars[i].isLocalPlayer) raceCar = raceCars[i];
            }

            if (raceCar) {
                //Logger.LogInfo("BaseCar Found!");
                CARX = raceCar.GetComponent<CARXCar>();
                //CARX.GetCarDesc(ref desc);
                //CARX.GetCarDesc(ref originalDesc);
            } else {
                //Logger.LogInfo("Waiting for RaceCar...");
            }
        }
        CR_running = false;
    }

    public static void updateDesc(ref CarX.CarDesc desc) {
        CARX.SetCarDesc(desc, true);
        //raceCar = null;
        //Logger.LogInfo("CarX Desc Updated!");
    }

    public static object TextField (Rect screenRect, ref Dictionary<string, Dictionary<string, object>> engineTune, string Key = "", bool hasName = false , string Key2 = "", Dictionary<string, object> dict = null) {
        var n = 0;
        Dictionary<string, object> Alt = new Dictionary<string, object>();
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        style.contentOffset = new Vector2(0, -15);
        style.fontSize = 10;
        style.normal.textColor = Color.white;
        //screenRect.width = 100;
        //screenRect.x += screenRect.width;
        //screenRect.width = 100;
        if (dict != null) {
            screenRect.x += screenRect.width + 15;
            screenRect.width = 90;
            foreach(KeyValuePair<string, object> e in dict)
            {
                object Field = (object) float.Parse(GUI.TextField (screenRect, e.Value.ToString()));
                if (hasName) {
                    GUI.Label(screenRect, e.Key, style);
                }
                screenRect.x += screenRect.width + 15;
                //Event ev = Event.current;
                //if (ev.keyCode == KeyCode.Return) {
                Alt.Add(e.Key, Field);
                //}
                n++;
            }
            engineTune[Key]["Args"] = Alt;
        } else {
            screenRect.x += screenRect.width + 15;
            screenRect.width = 90;

            Dictionary<string, Dictionary<string, object>> Properties = null;

            object original = null;
            object current = null;

            if ( engineTune[Key].ContainsKey("Properties") ) {
                Properties = ((Dictionary<string, Dictionary<string, object>>) engineTune[Key]["Properties"]);
                if ( !Properties[Key2].ContainsKey("Original")) {
                    Properties[Key2].Add("Original", Properties[Key2]["Current"]);
                }
                original = (object) Properties[Key2]["Original"];
                current = (object) Properties[Key2]["Current"];
            } else {
                if ( !engineTune[Key].ContainsKey("Original") ) {
                    engineTune[Key].Add("Original", (object) engineTune[Key]["Current"]);
                }
                original = (object) engineTune[Key]["Original"];
                current = (object) engineTune[Key]["Current"];
            }

            Event e = Event.current;

            if (Properties != null) {
                //Logger.LogInfo("Properties[Key2][\"Current\"] = " + Properties[Key2]["Current"]);
                Properties[Key2]["Current"] = (object) float.Parse( GUI.TextField (screenRect, current.ToString()));
            } else {
                engineTune[Key]["Current"] = (object) float.Parse(GUI.TextField (screenRect, current.ToString()));
            }

            if (hasName) {
                GUI.Label(screenRect, Key, style);
            }
            return null;
        }

        return null;
    }

    public static object Slider (Rect screenRect, object sliderValue, object min, object max) {
        screenRect.x += screenRect.width + 15;
        screenRect.y += screenRect.height / 4;
        screenRect.width = 90;
        //screenRect.width = 100;

        //sliderValue = (object) GUI.HorizontalSlider (screenRect, (float) sliderValue, (float) min, (float) max);
        Event e = Event.current;
        //Logger.LogInfo("KeyCode:" + e.keyCode + " Type:" + e.type  );

        //if (e.type == EventType.MouseDrag || e.type == EventType.MouseDown || e.type == EventType.MouseUp) {
            //UpdateDesc();
        //}

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter;

        Rect r = screenRect;
        r.y -= r.height/1.45f;

        GUI.Label(r, sliderValue.ToString(), style);

        GUI.skin.horizontalSliderThumb.fixedWidth = 10;
        GUI.skin.horizontalSliderThumb.fixedHeight= 10;
        GUI.skin.horizontalSlider.fixedHeight = 12;

        sliderValue = (object) GUI.HorizontalSlider (screenRect, (float) sliderValue, (float) min, (float) max, GUI.skin.horizontalSlider, GUI.skin.horizontalSliderThumb);
        if (GUI.changed) {
            //CarXHelper.updateDesc(ref desc);
        }
        //Logger.LogInfo("Slider: " + sliderValue);
        return (float) Math.Round((float)sliderValue, 2);
    }

     public static void TunerWindow( ref Dictionary<string, Dictionary<string, object>> engineTune, ref CarX.CarDesc desc, ref CarX.CarDesc originalDesc, Action setDefaultProperties, ref Rect winTuner)
        {
            //GUIHandler.DrawBoxOutline(new Vector2(0,0), winTuner.width, winTuner.height, new Color(0.259f, 0.282f, 0.349f, 0.7f));
            GUIHandler.DrawBox(new Vector2(0, 0), new Vector2(winTuner.width, 18), new Color(0.159f, 0.182f, 0.249f, 0.7f), false);
            GUIHandler.DrawBox(new Vector2(0, 18), new Vector2(winTuner.width, winTuner.height-18), new Color(0.259f, 0.282f, 0.349f, 0.7f), false);
            GUI.color = Color.white;
            int n = 1;

            Rect r = new Rect (10, 30 * n, 150, 20);
            foreach(KeyValuePair<string, Dictionary<string, object>> entry in engineTune)
            {
                r.y = 30 * n;


                if ( (object) entry.Value["Type"] == "Method" && entry.Value["Object"].GetType().GetMethod(entry.Key) != null ) {
                    
                } else if ( (object) entry.Value["Type"] == "Property" && entry.Value["Object"].GetType().GetProperty(entry.Key) != null ) {
                    
                } else if ( (object) entry.Value["Type"] == "classProperty") {
                    object Info = engineTune[entry.Key]["Object"];
                    int Offset = (int) engineTune[entry.Key]["Offset"];

                    FieldInfo[] Fields = Info.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
                    FieldInfo Field = Info.GetType().GetField(entry.Key);

                    r.x += (int) Offset;

                    GUIStyle guiStyle = new GUIStyle();
                    guiStyle.fontSize = 15;
                    guiStyle.normal.textColor = Color.white;
                    guiStyle.fontStyle = FontStyle.Bold;

                    n = 1;
                    r.y = 30 * n;

                    GUI.Label(r, entry.Key, guiStyle);

                    r.x += 25;
                    n++;

                    foreach(KeyValuePair<string, Dictionary<string, object>> e in (Dictionary<string, Dictionary<string, object>>) engineTune[entry.Key]["Properties"])
                    {
                        var textDimensions = GUI.skin.label.CalcSize(new GUIContent(e.Key));
                        r.width = textDimensions.x;
                        if ( (object) e.Value["fieldType"]== "Toggle" ) {
                            r.y = 30 * n;
                            Rect r2 = new Rect (r.x + (r.width + 15), r.y, 20, 20);
                            GUI.Label(r, e.Key);
                            e.Value["Current"] = GUI.Toggle(r2, (bool) e.Value["Current"], "");
                            Field.GetValue(Info).GetType().GetField(e.Key).SetValue(Field.GetValue(Info), (object) e.Value["Current"]);
                        } else if ( (object) e.Value["fieldType"] == "TextField" ) {
                            r.y = 30 * n;
                            GUI.Label(r, e.Key);
                            CarXHelper.TextField (r, ref engineTune, entry.Key, false, e.Key);
                            Field.GetValue(Info).GetType().GetField(e.Key).SetValue(Field.GetValue(Info), (object) e.Value["Current"]);
                        } else if ( (object) e.Value["fieldType"] == "Slider" ) {
                            r.y = 30 * n;
                            GUI.Label(r, e.Key);
                            e.Value["Current"] = CarXHelper.Slider (r, e.Value["Current"], e.Value["Min"], e.Value["Max"]);
                            Field.GetValue(Info).GetType().GetField(e.Key).SetValue(Field.GetValue(Info), (object) e.Value["Current"]);
                        } else if ( (object) e.Value["fieldType"] == "DropDown" ) {
                            //DropDown (r, entry.Key);
                        }
                
                        if (GUI.changed) {
                            CarXHelper.updateDesc(ref desc);
                        }
                        n++;
                    }
                }
            }

            if (GUI.Button(new Rect(10, 30 * 11, 75, 30), "RESET"))
            {
                CARX.SetCarDesc(originalDesc, true);
                CARX.GetCarDesc(ref desc);
                setDefaultProperties();
            }

            /*if (GUI.Button(new Rect((75 * 1) + 10, 30 * 11, 100, 30), "Save Config"))
            {
                CarXTuner.Plugin.WriteToJsonFile("D:\\Steam\\steamapps\\common\\CarX Drift Racing Online\\BepInEx\\scripts\\Config.json", engineTune);
                
                //CarXTuner.Plugin.SaveConfig("D:\\Steam\\steamapps\\common\\CarX Drift Racing Online\\BepInEx\\scripts\\Config.json");
            }

            if (GUI.Button(new Rect((75 * 2) + 10, 30 * 11, 100, 30), "Load Config"))
            {
                CarXTuner.Plugin.ReadFromJsonFile("D:\\Steam\\steamapps\\common\\CarX Drift Racing Online\\BepInEx\\scripts\\Config.json", ref engineTune);
                //CarXTuner.Plugin.LoadConfig("D:\\Steam\\steamapps\\common\\CarX Drift Racing Online\\BepInEx\\scripts\\Config.json");
            }*/

            winTuner.width = r.x + (r.width + 185);
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }
}