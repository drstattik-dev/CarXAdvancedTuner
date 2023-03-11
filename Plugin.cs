using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

using System;

using System.Reflection;

using System.Linq;
using System.Text;

using System.Collections;
using System.Collections.Generic;

//using ImGuiNET;

namespace CarXTuner
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {

        public bool init= false;
        public bool DebugLogger = true;

        public Rect winTuner = new Rect(20, 20, 900, (30 * 12)+10);

        public static Dictionary<string, Dictionary<string, object>> engineTune;

        public static CarX.CarDesc desc;
        public static CarX.CarDesc originalDesc;

        private static RaceCar raceCar = null;
        private static CARXCar CARX = null;

        private void Awake()
        {
            
        }

        public void Update() {
            if (!CarXHelper.raceCar && !CarXHelper.CR_running)
            {
                StartCoroutine(setupVariables ());
            }
        }

        public IEnumerator setupVariables ()
        {
            yield return StartCoroutine(CarXHelper.FindLocalCar());
            raceCar = CarXHelper.raceCar;
            CARX = CarXHelper.CARX;

            LogDebug("RaceCar Found: " + raceCar);
            LogDebug("storing original/modified desc...");

            CARX.GetCarDesc(ref desc);
            CARX.GetCarDesc(ref originalDesc);

            LogDebug("Stored!");
            
            setDefaultProperties(); //Set default properties
        }

        public void LogDebug(string message) {
            if (DebugLogger) Logger.LogInfo(message);
        }

        public void setDefaultProperties()
        {
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
                                { "useTC", new Dictionary<string, object> {{ "fieldType", "Toggle" }, { "Current", true } } },
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
                                { "springLength", new Dictionary<string, object> {{ "fieldType", "Slider" }, { "Current", 0.08f }, { "Min", 0.01f }, { "Max", 1f } } },
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
                                { "discDiam", new Dictionary<string, object> {{ "fieldType", "Slider" }, { "Current", 21f }, { "Min", 0.01f }, { "Max", 50f } } },
                                { "width", new Dictionary<string, object> {{ "fieldType", "Slider" }, { "Current", 305 }, { "Min", 1f }, { "Max", 450f } } },
                                { "mass", new Dictionary<string, object> {{ "fieldType", "Slider" }, { "Current", 1 }, { "Min", 0f }, { "Max", 1000f } } },
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
                                { "springLength", new Dictionary<string, object> {{ "fieldType", "Slider" }, { "Current", 0.08f }, { "Min", 0.01f }, { "Max", 1f } } },
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
                                { "discDiam", new Dictionary<string, object> {{ "fieldType", "Slider" }, { "Current", 21f }, { "Min", 0.01f }, { "Max", 50f } } },
                                { "width", new Dictionary<string, object> {{ "fieldType", "Slider" }, { "Current", 305 }, { "Min", 1f }, { "Max", 450f } } },
                                { "mass", new Dictionary<string, object> {{ "fieldType", "Slider" }, { "Current", 1 }, { "Min", 0f }, { "Max", 1000f } } },
                            }
                        },
                        { "Type", "classProperty" },
                        { "Object", desc },
                        { "Offset", 165 }
                    }
                },
            };

            //if (reset) {
                //engineTune = engineTuneDefault;
            //}

            foreach(KeyValuePair<string, Dictionary<string, object>> entry in engineTune)
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
                    LogDebug("___________ Field1: " + entry.Key + " ___________");

                    foreach(KeyValuePair<string, Dictionary<string, object>> e in (Dictionary<string, Dictionary<string, object>>) engineTune[entry.Key]["Properties"])
                    {
                        LogDebug("Field2: " + e.Key + " | Value: " + Field.GetValue(Info).GetType().GetField(e.Key).GetValue(Field.GetValue(Info)));

                        object val = Field.GetValue(Info).GetType().GetField(e.Key).GetValue(Field.GetValue(Info));
                        e.Value["Current"] = (object) val;
                    }

                    LogDebug("_________________________________________________");
                }
            }
        }

        void OnGUI()
        {
            if (raceCar) {
                GUI.backgroundColor = Color.clear;
                GUI.color = new Color(0.0f, 0.0f, 1.0f, 1f);
                winTuner = GUI.Window(0, winTuner, (int id) => { CarXHelper.TunerWindow(ref engineTune, ref desc, ref originalDesc, () => {setDefaultProperties();}, ref winTuner ); }, "CarX Advanced Engine Tuner!"); 
                //winTuner = GUI.Window(1, winTuner , TunerWindow, "CarX Advanced Suspension Tuner!"); 
            }
        }

    }

}