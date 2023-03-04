using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using BepInEx.Configuration;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;


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

        public void Unload() {
            Logger.LogInfo("CarXTuner Unloaded");
            //Object.Destroy(this);
        }

        //Base variable setup
        public bool init;
        public bool CR_running;
        public Rect winTuner = new Rect(20, 20, 900, (30 * 11)+10);

        private static RaceCar raceCar;
        private static CARXCar CARX;
        private static CarX.CarDesc desc;
        private static CarX.CarDesc originalDesc;

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
                    CARX.GetCarDesc(ref originalDesc);
                    //Logger.LogInfo(desc.frontSuspension.frontLock);
                    //Logger.LogInfo(desc);
                    //CARX.GetCarDesc(ref desc);

                    init = false;

                    engineTune = new Dictionary<string, Dictionary<string, object>>
                    {
                        //{ "engineTurboPressure", new Dictionary<string, object>         { { "Type", "Property" },       { "Object", CARX }, { "fieldType", "TextField" }, { "Current", 3.3f } } },
                        //{ "engineTurboCharged", new Dictionary<string, object>          { { "Type", "Property" },       { "Object", CARX }, { "fieldType", "Toggle" },    { "Current", true } } },
                        //{ "engineRevLimiter", new Dictionary<string, object>            { { "Type", "Property" },       { "Object", CARX }, { "fieldType", "TextField" }, { "Current", 9800f } } },
                        //{ "rearTyreFrictionMultiplier", new Dictionary<string, object>  { { "Type", "Property" },       { "Object", CARX }, { "fieldType", "TextField" }, { "Current", 1.2f } } },
                        //{ "frontTyreFrictionMultiplier", new Dictionary<string, object> { { "Type", "Property" },       { "Object", CARX }, { "fieldType", "TextField" }, { "Current", 1.2f } } },
                        //{ "SetEngineMaxTorque", new Dictionary<string, object>          { { "Type", "Method" },         { "Object", CARX }, { "fieldType", "TextField" }, { "Args", new Dictionary<string, object> { { "engineMaxTorque", 700f }, { "engineRPMAtMaxTorque", 3500f } }  } } },
                        //{ "finaldrive", new Dictionary<string, object>                  { { "Type", "Property" },       { "Object", CARX }, { "fieldType", "TextField" }, { "Current", 3.5f } } },
                        //{ "engineRevLimiterStep", new Dictionary<string, object>        { { "Type", "Property" },       { "Object", CARX }, { "fieldType", "TextField" }, { "Current", 250f } } },
                        { "engine", 
                        
                            new Dictionary<string, object> { 
                                { "Properties", new Dictionary<string, Dictionary<string, object> > {
                                        { "cutRPM", new Dictionary<string, object> {{ "fieldType", "TextField" }, { "Current", 220f } } },
                                        { "idleRPM", new Dictionary<string, object> {{ "fieldType", "Slider" }, { "Current", 1000f }, { "Min", 1f }, { "Max", 2000f } } },
                                        { "maxTorque", new Dictionary<string, object> {{ "fieldType", "TextField" }, { "Current", 850f } } },
                                        { "maxTorqueRPM", new Dictionary<string, object> {{ "fieldType", "TextField" }, { "Current", 6000f } } },
                                        { "revLimiter", new Dictionary<string, object> {{ "fieldType", "TextField" }, { "Current", 8200f } } },
                                        { "revLimiterStep", new Dictionary<string, object> {{ "fieldType", "TextField" }, { "Current", 300f } } },
                                        { "turboCharged", new Dictionary<string, object> {{ "fieldType", "Toggle" }, { "Current", true } } },
                                        { "turboPressure", new Dictionary<string, object> {{ "fieldType", "TextField" }, { "Current", 2f } } },
                                    }  
                                },
                                { "Type", "classProperty" },
                                { "Object", desc },
                                { "Offset", 0 }
                            }
                        },
                        { "frontSuspension", 
                        
                            new Dictionary<string, object> { 
                                { "Properties", new Dictionary<string, Dictionary<string, object> > {
                                        { "frontLock", new Dictionary<string, object> {{ "fieldType", "TextField" }, { "Current", 120f } } },
                                        { "springLength", new Dictionary<string, object> {{ "fieldType", "Slider" }, { "Current", 0.08f }, { "Min", 0.001f }, { "Max", 1f } } },
                                        { "stiffness", new Dictionary<string, object> {{ "fieldType", "TextField" }, { "Current", 75000f } } },
                                        { "camber", new Dictionary<string, object> {{ "fieldType", "Slider" }, { "Current", -2f }, { "Min", -20f }, { "Max", 20f } } },
                                    }
                                },
                                { "Type", "classProperty" },
                                { "Object", desc },
                                { "Offset", 185 }
                            }
                        },
                        { "frontTyre", 
                        
                            new Dictionary<string, object> { 
                                { "Properties", new Dictionary<string, Dictionary<string, object> > {
                                        { "frictionMultiplier", new Dictionary<string, object> {{ "fieldType", "Slider" }, { "Current", 1.1f }, { "Min", 0.01f }, { "Max", 5f } } },
                                        { "profile", new Dictionary<string, object> {{ "fieldType", "Slider" }, { "Current", 25 }, { "Min", 1f }, { "Max", 50f } } },
                                        { "discDiam", new Dictionary<string, object> {{ "fieldType", "Slider" }, { "Current", 21f }, { "Min", 0.001f }, { "Max", 50f } } },
                                        { "width", new Dictionary<string, object> {{ "fieldType", "Slider" }, { "Current", 305 }, { "Min", 1f }, { "Max", 450f } } },
                                        { "mass", new Dictionary<string, object> {{ "fieldType", "Slider" }, { "Current", 1 }, { "Min", 0f }, { "Max", 10f } } },
                                    }
                                },
                                { "Type", "classProperty" },
                                { "Object", desc },
                                { "Offset", 165 }
                            }
                        },
                        { "rearSuspension", 
                        
                            new Dictionary<string, object> { 
                                { "Properties", new Dictionary<string, Dictionary<string, object> > {
                                        { "springLength", new Dictionary<string, object> {{ "fieldType", "Slider" }, { "Current", 0.08f }, { "Min", 0.001f }, { "Max", 1f } } },
                                        { "stiffness", new Dictionary<string, object> {{ "fieldType", "TextField" }, { "Current", 63000f } } },
                                        { "camber", new Dictionary<string, object> {{ "fieldType", "Slider" }, { "Current", -0.6f }, { "Min", -20f }, { "Max", 20f } } },
                                    }
                                },
                                { "Type", "classProperty" },
                                { "Object", desc },
                                { "Offset", 180 }
                            }
                        },
                        { "rearTyre", 
                        
                            new Dictionary<string, object> { 
                                { "Properties", new Dictionary<string, Dictionary<string, object> > {
                                        { "frictionMultiplier", new Dictionary<string, object> {{ "fieldType", "Slider" }, { "Current", 1.1f }, { "Min", 0.01f }, { "Max", 5f } } },
                                        { "profile", new Dictionary<string, object> {{ "fieldType", "Slider" }, { "Current", 25 }, { "Min", 1f }, { "Max", 50f } } },
                                        { "discDiam", new Dictionary<string, object> {{ "fieldType", "Slider" }, { "Current", 21f }, { "Min", 0.001f }, { "Max", 50f } } },
                                        { "width", new Dictionary<string, object> {{ "fieldType", "Slider" }, { "Current", 305 }, { "Min", 1f }, { "Max", 450f } } },
                                        { "mass", new Dictionary<string, object> {{ "fieldType", "Slider" }, { "Current", 1 }, { "Min", 0f }, { "Max", 10f } } },
                                    }
                                },
                                { "Type", "classProperty" },
                                { "Object", desc },
                                { "Offset", 165 }
                            }
                        },
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
                //GUIStyle style = new GUIStyle(GUI.skin.window);
                
                //GUI.skin.window = style;
                GUI.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 1f);
                GUI.color = new Color(0.0f, 0.0f, 1.0f, 1f);
                winTuner = GUI.Window(0, winTuner, TunerWindow, "CarX Advanced Engine Tuner!"); 
                //winTuner = GUI.Window(1, winTuner , TunerWindow, "CarX Advanced Suspension Tuner!"); 
            }
        }

        char[] newLine = "\n\r".ToCharArray();

        void UpdateDesc() {
            CARX.SetCarDesc(desc, true);
            //raceCar = null;
            //Logger.LogInfo("CarX Desc Updated!");
        }

        void TunerWindow(int windowID)
        {
            Dictionary<string, Dictionary<string, object>> alt = engineTune.ToDictionary(entry => entry.Key, entry => entry.Value);
            if (!init) {
                CARX.GetCarDesc(ref desc);
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
                    } else if ( (object) entry.Value["Type"] == "classProperty") {
                        object Info = engineTune[entry.Key]["Object"];
                        FieldInfo[] Fields = Info.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
                        FieldInfo Field = Info.GetType().GetField(entry.Key);

                        foreach(KeyValuePair<string, Dictionary<string, object>> e in (Dictionary<string, Dictionary<string, object>>) engineTune[entry.Key]["Properties"])
                        {
                            object val = Field.GetValue(Info).GetType().GetField(e.Key).GetValue(Field.GetValue(Info));
                            e.Value["Current"] = (object) val;
                        }
                    }
                }

                init = true;
            }

            int n = 1;
            if (init) {
                Rect r = new Rect (10, 30 * n, 150, 20);
                foreach(KeyValuePair<string, Dictionary<string, object>> entry in alt.ToList())
                {
                    //Rect r = new Rect (10, 30 * n, 150, 20);
                    r.y = 30 * n;


                    if ( (object) entry.Value["Type"] == "Method" && entry.Value["Object"].GetType().GetMethod(entry.Key) != null ) {
                        var textDimensions = GUI.skin.label.CalcSize(new GUIContent(entry.Key));
                        r.width = textDimensions.x;
                        GUI.Label(r, entry.Key);

                        TextField (r, entry.Key, true, "" ,(Dictionary<string, object>)engineTune[entry.Key]["Args"]);

                        Dictionary<string, object> args = (Dictionary<string, object>) entry.Value["Args"];
                        object[] parameters = new object[args.Count];
                        int i = 0;

                        foreach(KeyValuePair<string, object> e in args.ToList())
                        {
                            parameters[i] = (object) e.Value;
                            i++;
                        }

                        entry.Value["Object"].GetType().GetMethod(entry.Key).Invoke(entry.Value["Object"], parameters);
                        n++;
                    } else if ( (object) entry.Value["Type"] == "Property" && entry.Value["Object"].GetType().GetProperty(entry.Key) != null ) {
                        var textDimensions = GUI.skin.label.CalcSize(new GUIContent(entry.Key));
                        r.width = textDimensions.x;
                        GUI.Label(r, entry.Key);

                        if ( (object) entry.Value["fieldType"]== "Toggle" ) {
                            //r.x += (r.width + 15);
                            //r.width = 20;
                            //r.height = 20;
                            Rect r2 = new Rect (r.x + (r.width + 15), r.y, 20, 20);
                            entry.Value["Current"] = GUI.Toggle(r2, (bool) entry.Value["Current"], "");
                            //r.width = 150;
                        } else if ( (object) entry.Value["fieldType"] == "TextField" ) {
                            //r.x = 10;
                            TextField (r, entry.Key, false);
                        } else if ( (object) entry.Value["fieldType"] == "Slider" ) {
                            //Slider (r, entry.Value["Current"], entry.Value["Min"], entry.Value["Max"]);
                            r.y = 30 * n;
                            //r.x = 10;
                            //r.width = 150;
                            GUI.Label(r, entry.Key);
                            entry.Value["Current"] = Slider (r, entry.Value["Current"], entry.Value["Min"], entry.Value["Max"]);
                            //Field.GetValue(Info).GetType().GetField(e.Key).SetValue(Field.GetValue(Info), (object) e.Value["Current"]);
                        } else if ( (object) entry.Value["fieldType"] == "DropDown" ) {
                            //DropDown (r, entry.Key);
                        }

                        entry.Value["Object"].GetType().GetProperty(entry.Key).SetValue(entry.Value["Object"], (object) entry.Value["Current"]);
                        n++;
                    } else if ( (object) entry.Value["Type"] == "classProperty") {
                        object Info = engineTune[entry.Key]["Object"];
                        int Offset = (int) engineTune[entry.Key]["Offset"];
                        FieldInfo[] Fields = Info.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
                        FieldInfo Field = Info.GetType().GetField(entry.Key);
                        //Logger.LogInfo("Offset: " + Offset);
                        r.x += (int) Offset;
                        //Logger.LogInfo("Offset: " + r.x);
                        
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
                                TextField (r, entry.Key, false, e.Key);
                                Field.GetValue(Info).GetType().GetField(e.Key).SetValue(Field.GetValue(Info), (object) e.Value["Current"]);
                            } else if ( (object) e.Value["fieldType"] == "Slider" ) {
                                r.y = 30 * n;
                                GUI.Label(r, e.Key);
                                e.Value["Current"] = Slider (r, e.Value["Current"], e.Value["Min"], e.Value["Max"]);
                                Field.GetValue(Info).GetType().GetField(e.Key).SetValue(Field.GetValue(Info), (object) e.Value["Current"]);
                            } else if ( (object) e.Value["fieldType"] == "DropDown" ) {
                                //DropDown (r, entry.Key);
                            }


                            //r = new Rect (10, 30 * n, 150, 20);
                            //GUI.Label(r, e.Key);

                            //TextField (r, entry.Key, false, e.Key);

                            //Field.GetValue(Info).GetType().GetField(e.Key).SetValue(Field.GetValue(Info), (object) e.Value["Current"]);
                            n++;
                        }
                    }

                    //n++;
                }
                if (GUI.Button(new Rect(10, 30 * 10, 75, 30), "RESET"))
                {
                    //desc.frontSuspension.springLength = 0.3f;// + Mathf.Sin(Time.time)/2;
                    //desc.rearSuspension.springLength = 0.3f;// + Mathf.Sin(Time.time)/2;
                    //desc.frontSuspension.frontLock = 120f;
                    //desc.frontSuspension.stiffness = 65000f;
                    //desc.frontSuspension.slowBump = 6500f;
                    //desc.rearSuspension.slowBump = 6000f;
                    //desc.rearSuspension.stiffness = 60000f;
                    //desc = originalDesc;
                    CARX.SetCarDesc(originalDesc, true);
                    //UpdateDesc();
                    init = false;
                    //CARX.SetCarDesc(desc, true);
                    //Logger.LogInfo("CarX.CarDesc: " + desc.);
                }
                winTuner.width = r.x + (r.width + 185);
            }
            //Logger.LogInfo("Init Dragger");

            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        object TextField (Rect screenRect, string Key = "", bool hasName = false , string Key2 = "", Dictionary<string, object> dict = null) {
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

                if (e.keyCode == KeyCode.Return && e.type == EventType.KeyDown) {
                    if (Properties != null) {
                        Properties[Key2]["Current"] = (object) float.Parse(original.ToString());
                    } else {
                        engineTune[Key]["Current"] = (object) float.Parse(original.ToString());
                    }
                    UpdateDesc();
                    engineTune[Key].Remove("Original");
                    //current = (object) float.Parse(original.ToString());
                } else {
                    if (Properties != null) {
                        //Logger.LogInfo("Properties[Key2][\"Current\"] = " + Properties[Key2]["Current"]);
                        Properties[Key2]["Original"] = (object) float.Parse( GUI.TextField (screenRect, original.ToString()));
                    } else {
                        engineTune[Key]["Original"] = (object) float.Parse(GUI.TextField (screenRect, original.ToString()));
                    }
                    //original = (object) GUI.TextField (screenRect, original.ToString());
                }

                if (hasName) {
                    GUI.Label(screenRect, Key, style);
                }
                return null;
            }

            return null;
        }

        object Slider (Rect screenRect, object sliderValue, object min, object max) {
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
                UpdateDesc();
            }
            //Logger.LogInfo("Slider: " + sliderValue);
            return (float) Math.Round((float)sliderValue, 2);
        }
    }

}