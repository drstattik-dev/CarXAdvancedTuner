<!-- GETTING STARTED -->
## Getting Started

### Installation

This is only compatible with moddable branch of CarX.

1. Clone the repo
   ```sh
   git clone https://github.com/drstattik-dev/CarXAdvancedTuner.git
   ```
2. Edit to your liking.

4. ```sh
   dotnet build
   ```


<!-- USAGE EXAMPLES -->
## Usage

Edit these properties to your liking.

```cs
engineTune = new Dictionary<string, Dictionary<string, object>>
{
    { "engineTurboPressure", new Dictionary<string, object>         { { "Type", "Property" },       { "Object", CARX }, { "fieldType", "TextField" }, { "Current", 3.3f } } },
    { "engineTurboCharged", new Dictionary<string, object>          { { "Type", "Property" },       { "Object", CARX }, { "fieldType", "Toggle" },    { "Current", true } } },
    { "engineRevLimiter", new Dictionary<string, object>            { { "Type", "Property" },       { "Object", CARX }, { "fieldType", "TextField" }, { "Current", 9800f } } },
    { "rearTyreFrictionMultiplier", new Dictionary<string, object>  { { "Type", "Property" },       { "Object", CARX }, { "fieldType", "TextField" }, { "Current", 1.2f } } },
    { "frontTyreFrictionMultiplier", new Dictionary<string, object> { { "Type", "Property" },       { "Object", CARX }, { "fieldType", "TextField" }, { "Current", 1.2f } } },
    { "SetEngineMaxTorque", new Dictionary<string, object>          { { "Type", "Method" },         { "Object", CARX }, { "fieldType", "TextField" }, { "Args", new Dictionary<string, object> { { "engineMaxTorque", 700f }, { "engineRPMAtMaxTorque", 3500f } }  } } },
    { "finaldrive", new Dictionary<string, object>                  { { "Type", "Property" },       { "Object", CARX }, { "fieldType", "TextField" }, { "Current", 3.5f } } },
    { "engineRevLimiterStep", new Dictionary<string, object>        { { "Type", "Property" },       { "Object", CARX }, { "fieldType", "TextField" }, { "Current", 250f } } },
    { "frontSuspension", 

        new Dictionary<string, object> { 
            { "Properties", new Dictionary<string, Dictionary<string, object> > {
                    { "frontLock", new Dictionary<string, object> {{ "fieldType", "TextField" }, { "Current", 120f } } },
                    { "springLength", new Dictionary<string, object> {{ "fieldType", "TextField" }, { "Current", 0.08f } } },
                    { "stiffness", new Dictionary<string, object> {{ "fieldType", "TextField" }, { "Current", 75000f } } },
                    { "camber", new Dictionary<string, object> {{ "fieldType", "TextField" }, { "Current", -2f } } },
                }
            },
            { "Type", "classProperty" },
            { "Object", desc }
        }
    },
    { "rearSuspension", 

        new Dictionary<string, object> { 
            { "Properties", new Dictionary<string, Dictionary<string, object> > {
                    { "springLength", new Dictionary<string, object> {{ "fieldType", "TextField" }, { "Current", 0.08f } } },
                    { "stiffness", new Dictionary<string, object> {{ "fieldType", "TextField" }, { "Current", 63000f } } },
                    { "camber", new Dictionary<string, object> {{ "fieldType", "TextField" }, { "Current", -0.6f } } },
                }
            },
            { "Type", "classProperty" },
            { "Object", desc }
        }
    }
};
```


<!-- ROADMAP -->
## Roadmap

- [x] Add properties dictionary.
- [x] Add Method support.
- [x] Add Class Field support.
- [x] Add auto adjusting UI
- [ ] Add saving properties.
- [ ] Add ability to easily copy others suspension.
- [ ] Other
    - [ ] Everything else



<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.
