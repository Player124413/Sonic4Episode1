using System;
using System.Globalization;
#if WASM
using System.Text.Json.Serialization;
#endif

#if !AOT
using System.Xml.Serialization;
#endif

namespace gs
{
#if !AOT
    [XmlRoot("Sonic4Save")]
#endif
    internal class Sonic4Save
    {
        public Sonic4Save()
        {
            System = new System();
            Options = new gsOptions();
            Remaster = new Remaster();
            Debug = new Debug();

            Stages = new Stage[17];
            for (int i = 0; i < 17; i++)
            {
                Stages[i] = new Stage();
            }

            SpecialStages = new SpecialStage[7];
            for (int i = 0; i < 7; i++)
            {
                SpecialStages[i] = new SpecialStage();
            }
        }
#if !AOT
        [XmlElement("System")]
#endif
        public System System { get; set; }
#if !AOT
        [XmlElement("Options")]
#endif
        public gsOptions Options { get; set; }
#if !AOT
        [XmlElement("Remaster")]
#endif
        public Remaster Remaster { get; set; }
#if !AOT
        [XmlElement("Debug")]
#endif
        public Debug Debug { get; set; }
#if !AOT
        [XmlElement("Stage")]
#endif
        public Stage[] Stages { get; set; }
#if !AOT
        [XmlElement("SpecialStage")]
#endif
        public SpecialStage[] SpecialStages { get; set; }
    }

    internal class System
    {
#if !AOT
        [XmlElement("Lives")]
#endif
        public uint Lives { get; set; } = 3;
#if !AOT
        [XmlElement("Killed")] 
#endif
        public uint Killed { get; set; } = 0;
#if !AOT
        [XmlElement("ClearCount")]
#endif
        public uint ClearCount { get; set; } = 0;
#if !AOT
        [XmlElement("Announce")] 
#endif
        public Announce Announce { get; set; } = new Announce();
    }

    internal class Announce
    {
#if !AOT
        [XmlElement("ZoneSelect")]
#endif
        public bool ZoneSelect { get; set; }
#if !AOT
        [XmlElement("Zone1Boss")]
#endif
        public bool Zone1Boss { get; set; }
#if !AOT
        [XmlElement("Zone2Boss")] 
#endif
        public bool Zone2Boss { get; set; }
#if !AOT
        [XmlElement("Zone3Boss")] 
#endif
        public bool Zone3Boss { get; set; }
#if !AOT
        [XmlElement("Zone4Boss")]
#endif
        public bool Zone4Boss { get; set; }
#if !AOT
        [XmlElement("FinalZone")] 
#endif
        public bool FinalZone { get; set; }
#if !AOT
        [XmlElement("SpecialStage")] 
#endif
        public bool SpecialStage { get; set; }
#if !AOT
        [XmlElement("SuperSonic")] 
#endif
        public bool SuperSonic { get; set; }
#if !AOT
        [XmlElement("SpecialStageTilt")]
#endif
        public bool SpecialStageTiltControls { get; set; }
#if !AOT
        [XmlElement("SpecialStageFlick")] 
#endif
        public bool SpecialStageFlickControls { get; set; }
#if !AOT
        [XmlElement("MinecartTiltControls")]
#endif
        public bool MinecartTiltControls { get; set; }
#if !AOT
        [XmlElement("MinecartFlickControls")] 
#endif
        public bool MinecartFlickControls { get; set; }
    }

    internal class Stage
    {
        public Stage()
        {
            New = true;
            StageRecords = new StageRecord[2];
            for (int i = 0; i < 2; i++)
            {
                StageRecords[i] = new StageRecord();
            }
        }
        
#if !AOT
        [XmlAttribute("New")]
#endif
        public bool New { get; set; }
#if !AOT
        [XmlAttribute("HighScoreSuperSonic")] 
#endif
        public bool HighScoreSuperSonic { get; set; }
#if !AOT
        [XmlAttribute("BestTimeSuperSonic")] 
#endif
        public bool BestTimeSuperSonic { get; set; }
#if !AOT
        [XmlAttribute("UsedSuperSonic")] 
#endif
        public bool UsedSuperSonic { get; set; }
#if !AOT
        [XmlElement("StageRecord")] 
#endif
        public StageRecord[] StageRecords { get; set; }
    }

    internal class StageRecord
    {
#if !AOT
        [XmlAttribute("HighScore")] 
#endif
        public uint HighScore { get; set; }
#if !AOT
        [XmlIgnore]
#endif
        public TimeSpan BestTime { get; set; }

        //[Browsable(false)]
#if WASM
        [JsonIgnore]
#endif
#if !AOT
        [XmlAttribute("BestTime")]
#endif
        public string BestTimeString
        {
            get => BestTime.ToString("c");
            set => BestTime = string.IsNullOrEmpty(value)
                    ? TimeSpan.Zero
                    : TimeSpan.Parse(value, CultureInfo.InvariantCulture);
        }
    }

    internal class SpecialStage
    {
#if !AOT
        [XmlAttribute("EmeraldStage")] 
#endif
        public uint EmeraldStage { get; set; }
#if !AOT
        [XmlAttribute("HighScore")]
#endif
        public uint HighScore { get; set; }
#if !AOT
        [XmlIgnore] 
#endif
        public TimeSpan BestTime { get; set; }

        //[Browsable(false)]
#if WASM
        [JsonIgnore]
#endif
#if !AOT
        [XmlAttribute("BestTime")]
#endif
        public string BestTimeString
        {
            get => BestTime.ToString("c");
            set => BestTime = string.IsNullOrEmpty(value)
                    ? TimeSpan.Zero
                    : TimeSpan.Parse(value, CultureInfo.InvariantCulture);
        }
    }

    internal class gsOptions
    {
#if !AOT
        [XmlElement("Vibration")] 
#endif
        public bool Vibration { get; set; } = true;
#if !AOT
        [XmlElement("BGMVolume")] 
#endif
        public float BGMVolume { get; set; } = 1;
#if !AOT
        [XmlElement("SEVolume")]
#endif
        public float SEVolume { get; set; } = 1;
#if !AOT
        [XmlElement("ControlStyle")] 
#endif
        public ControlType ControlType { get; set; } = ControlType.VirtualPadUp;
    }

    internal class Remaster
    {
#if !AOT
        [XmlElement("LoopCamera")] 
#endif
        public bool LoopCamera { get; set; } = false;
#if !AOT
        [XmlElement("BetterSoundEffects")] 
#endif
        public bool BetterSoundEffects { get; set; } = true;
#if !AOT
        [XmlElement("ModernSoundEffects")] 
#endif
        public bool ModernSoundEffects { get; set; } = false;
#if !AOT
        [XmlElement("BetterGears")] 
#endif
        public bool BetterGears { get; set; } = true;
#if !AOT
        [XmlElement("BetterMusic")]
#endif
        public bool BetterMusic { get; set; } = true;
#if !AOT
        [XmlElement("BugFixes")] 
#endif
        public bool BugFixes { get; set; } = true;
#if !AOT
        [XmlElement("Misc")]
#endif
        public bool Misc { get; set; } = true;
#if !AOT
        [XmlElement("iPhoneMenus")]
#endif
        public bool iPhoneMenus { get; set; } = false;
    }

    internal class Debug
    {
#if !AOT
        [XmlElement("GodMode")] 
#endif
        public bool GodMode { get; set; } = false;
    }

    public enum ControlType
    {
        Tilt, VirtualPadDown, VirtualPadUp,
        Max, None,
    }
}