using BBI.Unity.Game;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShiftRecorder
{
    public class Settings
    {
        public bool enabled = false;
        public bool debugLog = true;
        public bool autoRecordShifts = true;

        public string savePath = @".\BepInEx\plugins\ShiftRecorder\Videos\";
        public string saveFileName = "{SessionType}-{ShipName}-{ShipArchetype}-{Hash}";

        public bool hideHUDInRecording = true;

        public int recordingWidth = 1280;
        public int recordingHeight = 720;
        public int recordingTargetFPS = 15;
        public int recordingBitrate = 1500;
        public int recordingAntiAliasing = 1;

        public int garbageCollectionIntervalMS = 60000;

        public bool showInGamePreview = true;
        public int previewHeight = 200;
        public int previewVerticalPosition = 1;
        public int previewHorizontalPosition = 1;

        public string keybindNextCamera = "[+]";
        public string keybindPreviousCamera = "[-]";
        public string keybindCamera1 = "[1]";
        public string keybindCamera2 = "[2]";
        public string keybindCamera3 = "[3]";
        public string keybindCamera4 = "[4]";
        public string keybindCamera5 = "[5]";
        public string keybindCamera6 = "[6]";
        public string keybindCamera7 = "[7]";
        public string keybindCamera8 = "[8]";
        public string keybindCamera9 = "[9]";

        public string selectedCameraPosition = "BayHigh";

        public List<CameraPosition> cameraPositions = new List<CameraPosition>
        {
            new CameraPosition { name = "MasterJack", position = new Vector3(-6.3001f, 14.4832f, 97.9479f), rotation = new Vector3(13.8578f, 180f, 0f) },
            new CameraPosition { name = "BackBay", position = new Vector3(-6.3001f, 21.5f, -16.2832f), rotation = new Vector3(17.5f, 0f, 0f) },
            new CameraPosition { name = "BayHigh", position = new Vector3(-6.3001f, 30.4832f, 97.9479f), rotation = new Vector3(33.8578f, 180f, 0f) },
            new CameraPosition { name = "LeftJack", position = new Vector3(12.7864f, 21.7884f, 42.1802f), rotation = new Vector3(30f, 219.6062f, 0f) },
            new CameraPosition { name = "RightJack", position = new Vector3(-20.4643f, 21.1488f, 42.4983f), rotation = new Vector3(30f, 147.261f, 0f) },
        };

        public static Settings settings;

        public static void Load()
        {
            var settingsPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "settings.json");
            var defaults = JObject.FromObject(new Settings());
            var loaded = System.IO.File.Exists(settingsPath) ? JObject.Parse(System.IO.File.ReadAllText(settingsPath)) : new JObject();

            var addedFields = false;
            foreach (var property in defaults.Properties())
            {
                if (loaded.Property(property.Name) == null)
                {
                    loaded.Add(property.Name, property.Value);
                    addedFields = true;
                }
            }

            if (addedFields || !System.IO.File.Exists(settingsPath))
                System.IO.File.WriteAllText(settingsPath, loaded.ToString(Formatting.Indented));

            settings = loaded.ToObject<Settings>();
            RockVR.Video.PathConfig.saveFolder = System.IO.Path.GetFullPath(settings.savePath);
            RockVR.Video.PathConfig.ffmpegPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "ffmpeg.exe");

            MoveCamera(settings.selectedCameraPosition);
            MovePreview();
        }

        public static void MoveCamera(string newPositionName = null)
        {
            if (GameSession.SessionCount > 0 && ShiftRecorder.VideoCaptureSession.captureCamera != null)
            {
                var pos = settings.cameraPositions.FirstOrDefault(pos => pos.name == newPositionName) ?? settings.cameraPositions.First();
                ShiftRecorder.VideoCaptureSession.captureCamera.transform.position = pos.position;
                ShiftRecorder.VideoCaptureSession.captureCamera.transform.rotation = Quaternion.Euler(pos.rotation.x, pos.rotation.y, pos.rotation.z);
            }
        }

        public static void MovePreview()
        {
            var transform = ShiftRecorder.VideoCaptureSession.imgObject?.GetComponent<RectTransform>();
            if (transform != null)
            {
                transform.anchoredPosition = new Vector2(780 * Settings.settings.previewHorizontalPosition, 498 * Settings.settings.previewVerticalPosition * -1); // setting position, will be on center
            }
        }

        public class CameraPosition
        {
            public string name;
            public Vector3 position;
            public Vector3 rotation;
        }
    }
}
