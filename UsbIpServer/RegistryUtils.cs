﻿// SPDX-FileCopyrightText: Microsoft Corporation
//
// SPDX-License-Identifier: GPL-2.0-only

using System.Linq;
using Microsoft.Win32;
using System.Security.Principal;

namespace UsbIpServer
{
    static class RegistryUtils
    {
        const string devicesRegistryPath = @"SOFTWARE\USBIPD-WIN";

        public static bool IsDeviceAvailable(string busId)
        {
            return Registry.LocalMachine.CreateSubKey(devicesRegistryPath).GetSubKeyNames().Any(x => x == busId);
        }

        public static void SetDeviceAvailability(string busId, bool enable)
        {
            if (enable)
            { 
                Registry.LocalMachine.CreateSubKey($@"{devicesRegistryPath}\{busId}");
            } else if (IsDeviceAvailable(busId))
            {
                Registry.LocalMachine.DeleteSubKey($@"{devicesRegistryPath}\{busId}");
            }
        }

        public static string[] GetRegistryDevices()
        {
            return Registry.LocalMachine.CreateSubKey(devicesRegistryPath).GetSubKeyNames();
        }

        public static void InitializeRegistry()
        {
            Registry.LocalMachine.DeleteSubKeyTree(devicesRegistryPath);
            Registry.LocalMachine.CreateSubKey(devicesRegistryPath);
        }

        public static bool HasRegistryAccess()
        {
            bool isElevated;
            using (var identity = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(identity);
                isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }

            return isElevated;
        }
    }
}