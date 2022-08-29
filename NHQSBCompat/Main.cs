﻿using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using QSB.Messaging;
using System.Reflection;
using QSB;
using QSB.Menus;
using Mirror;

namespace NHQSBCompat
{
    public class Main : ModBehaviour
    {
        public static Main Instance;

        public INewHorizons NewHorizonsAPI;


        public void Start()
        {
			Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            var kcpTransport = FindObjectOfType<kcp2k.KcpTransport>();
            if (kcpTransport) kcpTransport.Timeout = int.MaxValue;

			Instance = this;

            NewHorizonsAPI = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");

            gameObject.AddComponent<WarpManager>();
        }

        public static void Log(string msg) => Instance.ModHelper.Console.WriteLine(msg);
	}
}
