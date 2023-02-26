using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using BepInEx.Configuration;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


namespace CarXTuner
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {

        public void Init() {
            Logger.LogInfo("CarXTuner Loaded");
        }
        public void Update() {
            
            if (!raceCar && !CR_running)
            {
                StartCoroutine(FindLocalCar());
            }
        }

        //Load the plugin
        public void Awake() {
            Logger.LogInfo("CarXTuner Loaded");
        }

        //Base variable setup
        public bool init;
        public bool CR_running;
        public Rect winTuner;

        private static RaceCar raceCar;
        private static CARXCar CARX;
        private static CarX.CarDesc desc = null;

        public static Dictionary<string, Dictionary<string, object>> engineTune;
        public static Dictionary<string, Dictionary<string, object>> suspensionTune;
        IEnumerator FindLocalCar()
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
                    Logger.LogInfo("BaseCar Found!");
                    CARX = raceCar.GetComponent<CARXCar>();
                    CARX.GetCarDesc(ref desc);

                    init = false;

                    engineTune = new Dictionary<string, Dictionary<string, object>>
                    {
                        { "engineTurboPressure", new Dictionary<string, object>         { { "Type", "Property" }, { "Object", CARX }, { "fieldType", "TextField" }, { "Current", 3.3f } } },
                        { "engineTurboCharged", new Dictionary<string, object>          { { "Type", "Property" }, { "Object", CARX }, { "fieldType", "Toggle" },    { "Current", true } } },
                        { "engineRevLimiter", new Dictionary<string, object>            { { "Type", "Property" }, { "Object", CARX }, { "fieldType", "TextField" }, { "Current", 9800f } } },
                        { "rearTyreFrictionMultiplier", new Dictionary<string, object>  { { "Type", "Property" }, { "Object", CARX }, { "fieldType", "TextField" }, { "Current", 1.2f } } },
                        { "frontTyreFrictionMultiplier", new Dictionary<string, object> { { "Type", "Property" }, { "Object", CARX }, { "fieldType", "TextField" }, { "Current", 1.2f } } },
                        { "SetEngineMaxTorque", new Dictionary<string, object>          { { "Type", "Method" },   { "Object", CARX }, { "fieldType", "TextField" }, { "Args", new Dictionary<string, object> { { "engineMaxTorque", 700f }, { "engineRPMAtMaxTorque", 3500f } }  } } },
                        { "finaldrive", new Dictionary<string, object>                  { { "Type", "Property" }, { "Object", CARX }, { "fieldType", "TextField" }, { "Current", 3.5f } } },
                        { "engineRevLimiterStep", new Dictionary<string, object>        { { "Type", "Property" }, { "Object", CARX }, { "fieldType", "TextField" }, { "Current", 250f } } },
                    };

                    suspensionTune = new Dictionary<string, Dictionary<string, object>>
                    {
                    };
                } else {
                    Logger.LogInfo("Waiting for RaceCar...");
                }
            }
            CR_running = false;
        }

        //tuning variables
        void OnGUI()
        {
            if (raceCar) {
                winTuner = new Rect(20, 20, 500, 350);
                winTuner = GUI.Window(0, winTuner, TunerWindow, "CarX Advanced Tuner"); 
            }
        }

        char[] newLine = "\n\r".ToCharArray();

        void TunerWindow(int windowID)
        {
            Dictionary<string, Dictionary<string, object>> alt = engineTune.ToDictionary(entry => entry.Key, entry => entry.Value);
            if (!init) {
                foreach(KeyValuePair<string, Dictionary<string, object>> entry in alt.ToList())
                {
                    if ( (object) entry.Value["Type"] == "Method" && entry.Value["Object"].GetType().GetMethod(entry.Key) != null ) {
                        Dictionary<string, object> args = (Dictionary<string, object>) entry.Value["Args"];

                        foreach(KeyValuePair<string, object> e in args.ToList())
                        {
                            if (entry.Value["Object"].GetType().GetProperty(e.Key) != null)
                            {
                                ((Dictionary<string, object>)engineTune[entry.Key]["Args"])[e.Key] = (object) entry.Value["Object"].GetType().GetProperty(e.Key).GetValue(entry.Value["Object"], null);
                            }
                        }
                    } else if ( (object) entry.Value["Type"] == "Property" && entry.Value["Object"].GetType().GetProperty(entry.Key) != null ) {
                        entry.Value["Current"] = (object) entry.Value["Object"].GetType().GetProperty(entry.Key).GetValue(entry.Value["Object"], null);
                    }
                }

                init = true;
            }

            int n = 1;
            if (init) {

                foreach(KeyValuePair<string, Dictionary<string, object>> entry in alt.ToList())
                {
                    Rect r = new Rect (10, 30 * n, 150, 20);

                    if ( (object) entry.Value["Type"] == "Method" && entry.Value["Object"].GetType().GetMethod(entry.Key) != null ) {
                        GUI.Label(r, entry.Key);

                        TextField (r, entry.Key, true, (Dictionary<string, object>)engineTune[entry.Key]["Args"]);

                        Dictionary<string, object> args = (Dictionary<string, object>) entry.Value["Args"];
                        object[] parameters = new object[args.Count];
                        int i = 0;

                        foreach(KeyValuePair<string, object> e in args.ToList())
                        {
                            parameters[i] = (object) e.Value;
                            i++;
                        }

                        entry.Value["Object"].GetType().GetMethod(entry.Key).Invoke(entry.Value["Object"], parameters);
                    } else if ( (object) entry.Value["Type"] == "Property" && entry.Value["Object"].GetType().GetProperty(entry.Key) != null ) {
                        GUI.Label(r, entry.Key);

                        if ( (object) entry.Value["fieldType"]== "Toggle" ) {
                            r.x += (r.width + 15);
                            entry.Value["Current"] = GUI.Toggle(r, (bool) entry.Value["Current"], "");
                        } else if ( (object) entry.Value["fieldType"] == "TextField" ) {
                            TextField (r, entry.Key, false);
                        }

                        entry.Value["Object"].GetType().GetProperty(entry.Key).SetValue(entry.Value["Object"], (object) entry.Value["Current"]);
                    }

                    n++;
                }
                //if (GUI.Button(new Rect(10, 30 * n, 75, 30), "Click"))
                //{
                    CarX.CarDesc desc = null;
                    CARX.GetCarDesc(ref desc);
                    desc.frontSuspension.springLength = 1 + Mathf.Sin(Time.time)/2;
                    desc.rearSuspension.springLength = 1 + Mathf.Sin(Time.time)/2;
                    CARX.SetCarDesc(desc, true);
                    //Logger.LogInfo("CarX.CarDesc: " + desc.);
                //}
            }
        }

        object TextField (Rect screenRect, string Key = "", bool hasName = false, Dictionary<string, object> dict = null) {
            var n = 0;
            Dictionary<string, object> Alt = new Dictionary<string, object>();
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
            style.contentOffset = new Vector2(0, -15);
            style.fontSize = 10;
            screenRect.width /= 2;
            screenRect.x += screenRect.width;
            if (dict != null) {
                foreach(KeyValuePair<string, object> e in dict)
                {
                    screenRect.x += screenRect.width + 15;
                    object Field = (object) float.Parse(GUI.TextField (screenRect, e.Value.ToString()));
                    if (hasName) {
                        GUI.Label(screenRect, e.Key, style);
                    }
                    //Event ev = Event.current;
                    //if (ev.keyCode == KeyCode.Return) {
                    Alt.Add(e.Key, Field);
                    //}
                    n++;
                }
                engineTune[Key]["Args"] = Alt;
            } else {
                screenRect.x += screenRect.width + 15;

                if (engineTune[Key].ContainsKey("Original") == false)
                    engineTune[Key]["Original"] = (object) engineTune[Key]["Current"].ToString();

                Event e = Event.current;
                if (e.keyCode == KeyCode.Return) {
                    engineTune[Key]["Current"] = (object) float.Parse(engineTune[Key]["Original"].ToString());
                } else {
                    engineTune[Key]["Original"] = (object) GUI.TextField (screenRect, engineTune[Key]["Original"].ToString());
                }

                if (hasName) {
                    GUI.Label(screenRect, Key, style);
                }
                return engineTune[Key]["Current"];
            }

            return null;
        }

        float LabelSlider (Rect screenRect, float sliderValue, float sliderMaxValue, string labelText) {
            GUI.Label (screenRect, labelText);

            screenRect.x += screenRect.width; 
        
            sliderValue = GUI.HorizontalSlider (screenRect, sliderValue, 0.0f, sliderMaxValue);
            return sliderValue;
        }
    }

}