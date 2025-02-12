using HTCCL.Content;
using HTCCL.Patches;
using HTCCL.Saves;
using HTCCL.Updates;

namespace HTCCL.Utils;

internal class SaveRemapper
{
    internal static void PatchCustomContent(ref SaveData saveData)
    {
        ContentMappings newMap = ContentMappings.ContentMap;
        ContentMappings savedMap = SaveFilePatch.LoadPreviousMap();

        if (!VanillaCounts.Data.IsInitialized)
        {
            LogError("Vanilla counts not initialized. Skipping custom content patch.");
            return;
        }

        int oldVersion = Mathf.RoundToInt(savedMap.GameVersion * 100);
        int newVersion = Mathf.RoundToInt(Plugin.GameVersion * 100);

        VersionDiff versionDiff = null;

        if (oldVersion != newVersion)
        {
            LogInfo($"Game version changed from {oldVersion} to {newVersion}. Updating custom content map.");

            if (savedMap != null)
            {
                versionDiff = VersionDiff.GetVersionDiff(savedMap.VanillaCounts);
            }
        }
        ContentMappings.ContentMap.VanillaCounts = VanillaCounts.Data;
        try
        {
            foreach (Character character in saveData.savedChars)
            {
                if (character == null)
                {
                    continue;
                }

                foreach (Costume costume in character.costume)
                {
                    if (costume == null)
                    {
                        continue;
                    }

                    for (int i = 0; i < costume.texture.Length; i++)
                    {
                        if (VanillaCounts.Data.MaterialCounts[i] == 0)
                        {
                            continue;
                        }

                        if (versionDiff != null)
                        {
                            if (versionDiff.MaterialCountsDiff[i] != 0 && costume.texture[i] >
                                VanillaCounts.Data.MaterialCounts[i] - versionDiff.MaterialCountsDiff[i])
                            {
                                costume.texture[i] += versionDiff.MaterialCountsDiff[i];
                            }
                            else if (i == 3 && versionDiff.FaceFemaleCountDiff != 0 && costume.texture[i] <
                                     -VanillaCounts.Data.FaceFemaleCount + versionDiff.FaceFemaleCountDiff)
                            {
                                costume.texture[i] -= versionDiff.FaceFemaleCountDiff;
                            }
                            else if (i == 14 && versionDiff.SpecialFootwearCountDiff != 0 && costume.texture[i] <
                                     -VanillaCounts.Data.SpecialFootwearCount + versionDiff.SpecialFootwearCountDiff)
                            {
                                costume.texture[i] -= versionDiff.SpecialFootwearCountDiff;
                            }
                            else if (i == 15 && versionDiff.SpecialFootwearCountDiff != 0 && costume.texture[i] <
                                     -VanillaCounts.Data.SpecialFootwearCount + versionDiff.SpecialFootwearCountDiff)
                            {
                                costume.texture[i] -= versionDiff.SpecialFootwearCountDiff;
                            }
                            else if (i == 17 && versionDiff.TransparentHairMaterialCountDiff != 0 &&
                                     costume.texture[i] < -VanillaCounts.Data.TransparentHairMaterialCount +
                                     versionDiff.TransparentHairMaterialCountDiff)
                            {
                                costume.texture[i] -= versionDiff.TransparentHairMaterialCountDiff;
                            }
                            else if (i == 24 && versionDiff.KneepadCountDiff != 0 && costume.texture[i] <
                                     -VanillaCounts.Data.KneepadCount + versionDiff.KneepadCountDiff)
                            {
                                costume.texture[i] -= versionDiff.KneepadCountDiff;
                            }
                            else if (i == 25 && versionDiff.KneepadCountDiff != 0 && costume.texture[i] <
                                     -VanillaCounts.Data.KneepadCount + versionDiff.KneepadCountDiff)
                            {
                                costume.texture[i] -= versionDiff.KneepadCountDiff;
                            }
                        }

                        if (costume.texture[i] > VanillaCounts.Data.MaterialCounts[i])
                        {
                            int oldIndex = costume.texture[i] - VanillaCounts.Data.MaterialCounts[i] - 1;
                            if (oldIndex >= savedMap.MaterialNameMap[i].Count)
                            {
                                LogWarning(
                                    $"Custom material {i} index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.texture[i] = VanillaCounts.Data.MaterialCounts[i];
                                continue;
                            }

                            string oldName = savedMap.MaterialNameMap[i][oldIndex];
                            int newIndex = newMap.MaterialNameMap[i].IndexOf(oldName);
                            int internalIndex = newIndex + VanillaCounts.Data.MaterialCounts[i] + 1;
                            if (costume.texture[i] != internalIndex)
                            {
                                LogInfo(
                                    $"Custom material {oldName} at index {oldIndex} was remapped to index {newIndex} (internal index {internalIndex}) for material {i} of character {character.name} ({character.id}).");
                                costume.texture[i] = internalIndex;
                            }
                        }

                        else if (i == 3 && costume.texture[i] < -VanillaCounts.Data.FaceFemaleCount)
                        {
                            int oldIndex = -costume.texture[i] - VanillaCounts.Data.FaceFemaleCount - 1;
                            if (oldIndex >= savedMap.FaceFemaleNameMap.Count)
                            {
                                LogWarning(
                                    $"Custom material {i} index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.texture[i] = VanillaCounts.Data.MaterialCounts[i];
                                continue;
                            }

                            string oldName = savedMap.FaceFemaleNameMap[oldIndex];
                            int newIndex = newMap.FaceFemaleNameMap.IndexOf(oldName);
                            int internalIndex = -newIndex - VanillaCounts.Data.FaceFemaleCount - 1;
                            if (costume.texture[i] != internalIndex)
                            {
                                LogInfo(
                                    $"Custom material {oldName} at index {oldIndex} was remapped to index {newIndex} (internal index {internalIndex}) for material {i} of character {character.name} ({character.id}).");
                                costume.texture[i] = internalIndex;
                            }
                        }

                        else if (i == 14 && costume.texture[i] < -VanillaCounts.Data.SpecialFootwearCount)
                        {
                            int oldIndex = -costume.texture[i] - VanillaCounts.Data.SpecialFootwearCount - 1;
                            if (oldIndex >= savedMap.SpecialFootwearNameMap.Count)
                            {
                                LogWarning(
                                    $"Custom material {i} index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.texture[i] = VanillaCounts.Data.MaterialCounts[i];
                                continue;
                            }

                            string oldName = savedMap.SpecialFootwearNameMap[oldIndex];
                            int newIndex = newMap.SpecialFootwearNameMap.IndexOf(oldName);
                            int internalIndex = -newIndex - VanillaCounts.Data.SpecialFootwearCount - 1;
                            if (costume.texture[i] != internalIndex)
                            {
                                LogInfo(
                                    $"Custom material {oldName} at index {oldIndex} was remapped to index {newIndex} (internal index {internalIndex}) for material {i} of character {character.name} ({character.id}).");
                                costume.texture[i] = internalIndex;
                            }
                        }

                        else if (i == 15 && costume.texture[i] < -VanillaCounts.Data.SpecialFootwearCount)
                        {
                            int oldIndex = -costume.texture[i] - VanillaCounts.Data.SpecialFootwearCount - 1;
                            if (oldIndex >= savedMap.SpecialFootwearNameMap.Count)
                            {
                                LogWarning(
                                    $"Custom material {i} index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.texture[i] = VanillaCounts.Data.MaterialCounts[i];
                                continue;
                            }

                            string oldName = savedMap.SpecialFootwearNameMap[oldIndex];
                            int newIndex = newMap.SpecialFootwearNameMap.IndexOf(oldName);
                            int internalIndex = -newIndex - VanillaCounts.Data.SpecialFootwearCount - 1;
                            if (costume.texture[i] != internalIndex)
                            {
                                LogInfo(
                                    $"Custom material {oldName} at index {oldIndex} was remapped to index {newIndex} (internal index {internalIndex}) for material {i} of character {character.name} ({character.id}).");
                                costume.texture[i] = internalIndex;
                            }
                        }

                        else if (i == 17 && costume.texture[i] < -VanillaCounts.Data.TransparentHairMaterialCount)
                        {
                            int oldIndex = -costume.texture[i] - VanillaCounts.Data.TransparentHairMaterialCount - 1;
                            if (oldIndex >= savedMap.TransparentHairMaterialNameMap.Count)
                            {
                                LogWarning(
                                    $"Custom material {i} index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.texture[i] = VanillaCounts.Data.MaterialCounts[i];
                                continue;
                            }

                            string oldName = savedMap.TransparentHairMaterialNameMap[oldIndex];
                            int newIndex = newMap.TransparentHairMaterialNameMap.IndexOf(oldName);
                            int internalIndex = -newIndex - VanillaCounts.Data.TransparentHairMaterialCount - 1;
                            if (costume.texture[i] != internalIndex)
                            {
                                LogInfo(
                                    $"Custom material {oldName} at index {oldIndex} was remapped to index {newIndex} (internal index {internalIndex}) for material {i} of character {character.name} ({character.id}).");
                                costume.texture[i] = internalIndex;
                            }
                        }

                        else if (i == 24 && costume.texture[i] < -VanillaCounts.Data.KneepadCount)
                        {
                            int oldIndex = -costume.texture[i] - VanillaCounts.Data.KneepadCount - 1;
                            if (oldIndex >= savedMap.KneepadNameMap.Count)
                            {
                                LogWarning(
                                    $"Custom material {i} index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.texture[i] = VanillaCounts.Data.MaterialCounts[i];
                                continue;
                            }

                            string oldName = savedMap.KneepadNameMap[oldIndex];
                            int newIndex = newMap.KneepadNameMap.IndexOf(oldName);
                            int internalIndex = -newIndex - VanillaCounts.Data.KneepadCount - 1;
                            if (costume.texture[i] != internalIndex)
                            {
                                LogInfo(
                                    $"Custom material {oldName} at index {oldIndex} was remapped to index {newIndex} (internal index {internalIndex}) for material {i} of character {character.name} ({character.id}).");
                                costume.texture[i] = internalIndex;
                            }
                        }

                        else if (i == 25 && costume.texture[i] < -VanillaCounts.Data.KneepadCount)
                        {
                            int oldIndex = -costume.texture[i] - VanillaCounts.Data.KneepadCount - 1;
                            if (oldIndex >= savedMap.KneepadNameMap.Count)
                            {
                                LogWarning(
                                    $"Custom material {i} index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.texture[i] = VanillaCounts.Data.MaterialCounts[i];
                                continue;
                            }

                            string oldName = savedMap.KneepadNameMap[oldIndex];
                            int newIndex = newMap.KneepadNameMap.IndexOf(oldName);
                            int internalIndex = -newIndex - VanillaCounts.Data.KneepadCount - 1;
                            if (costume.texture[i] != internalIndex)
                            {
                                LogInfo(
                                    $"Custom material {oldName} at index {oldIndex} was remapped to index {newIndex} (internal index {internalIndex}) for material {i} of character {character.name} ({character.id}).");
                                costume.texture[i] = internalIndex;
                            }
                        }
                    }

                    for (int i = 0; i < costume.flesh.Length; i++)
                    {
                        if (VanillaCounts.Data.FleshCounts[i] == 0)
                        {
                            continue;
                        }

                        if (versionDiff != null)
                        {
                            if (versionDiff.FleshCountsDiff[i] != 0 && costume.flesh[i] >
                                VanillaCounts.Data.FleshCounts[i] - versionDiff.FleshCountsDiff[i])
                            {
                                costume.flesh[i] += versionDiff.FleshCountsDiff[i];
                            }
                            else if (i == 2 && versionDiff.BodyFemaleCountDiff != 0 && costume.flesh[i] <
                                     -VanillaCounts.Data.BodyFemaleCount + versionDiff.BodyFemaleCountDiff)
                            {
                                costume.flesh[i] -= versionDiff.BodyFemaleCountDiff;
                            }
                        }

                        if (costume.flesh[i] > VanillaCounts.Data.FleshCounts[i])
                        {
                            int oldIndex = costume.flesh[i] - VanillaCounts.Data.FleshCounts[i] - 1;
                            if (oldIndex >= savedMap.FleshNameMap[i].Count)
                            {
                                LogWarning(
                                    $"Custom flesh index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.flesh[i] = VanillaCounts.Data.FleshCounts[i];
                                continue;
                            }

                            string oldName = savedMap.FleshNameMap[i][oldIndex];
                            int newIndex = newMap.FleshNameMap[i].IndexOf(oldName);
                            int internalIndex = newIndex + VanillaCounts.Data.FleshCounts[i] + 1;
                            if (costume.flesh[i] != internalIndex)
                            {
                                LogInfo(
                                    $"Custom flesh {oldName} at index {oldIndex} was remapped to index {newIndex} (internal index {internalIndex}) for flesh {i} of character {character.name} ({character.id}).");
                                costume.flesh[i] = internalIndex;
                            }
                        }

                        else if (i == 2 && costume.flesh[i] < -VanillaCounts.Data.BodyFemaleCount)
                        {
                            int oldIndex = -costume.flesh[i] - VanillaCounts.Data.BodyFemaleCount - 1;
                            if (oldIndex >= savedMap.BodyFemaleNameMap.Count)
                            {
                                LogWarning(
                                    $"Custom flesh index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.flesh[i] = VanillaCounts.Data.FleshCounts[i];
                                continue;
                            }

                            string oldName = savedMap.BodyFemaleNameMap[oldIndex];
                            int newIndex = newMap.BodyFemaleNameMap.IndexOf(oldName);
                            int internalIndex = -newIndex - VanillaCounts.Data.BodyFemaleCount - 1;
                            if (costume.flesh[i] != internalIndex)
                            {
                                LogInfo(
                                    $"Custom flesh {oldName} at index {oldIndex} was remapped to index {newIndex} (internal index {internalIndex}) for flesh {i} of character {character.name} ({character.id}).");
                                costume.flesh[i] = internalIndex;
                            }
                        }
                    }

                    for (int i = 0; i < costume.shape.Length; i++)
                    {
                        if (VanillaCounts.Data.ShapeCounts[i] == 0)
                        {
                            continue;
                        }

                        if (versionDiff != null)
                        {
                            if (versionDiff.ShapeCountsDiff[i] != 0 && costume.shape[i] >
                                VanillaCounts.Data.ShapeCounts[i] - versionDiff.ShapeCountsDiff[i])
                            {
                                costume.shape[i] += versionDiff.ShapeCountsDiff[i];
                            }
                            else if (i == 17 && versionDiff.TransparentHairHairstyleCountDiff != 0 && costume.shape[i] <
                                     -VanillaCounts.Data.TransparentHairHairstyleCount +
                                     versionDiff.TransparentHairHairstyleCountDiff)
                            {
                                costume.shape[i] -= versionDiff.TransparentHairHairstyleCountDiff;
                            }
                        }

                        if (costume.shape[i] > VanillaCounts.Data.ShapeCounts[i])
                        {
                            int oldIndex = costume.shape[i] - VanillaCounts.Data.ShapeCounts[i] - 1;
                            if (oldIndex >= savedMap.ShapeNameMap[i].Count)
                            {
                                LogWarning(
                                    $"Custom shape index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.shape[i] = VanillaCounts.Data.ShapeCounts[i];
                                continue;
                            }

                            string oldName = savedMap.ShapeNameMap[i][oldIndex];
                            int newIndex = newMap.ShapeNameMap[i].IndexOf(oldName);
                            int internalIndex = newIndex + VanillaCounts.Data.ShapeCounts[i] + 1;
                            if (costume.shape[i] != internalIndex)
                            {
                                LogInfo(
                                    $"Custom shape {oldName} at index {oldIndex} was remapped to index {newIndex} (internal index {internalIndex}) for shape {i} of character {character.name} ({character.id}).");
                                costume.shape[i] = internalIndex;
                            }
                        }

                        else if (i == 17 && costume.shape[i] < -VanillaCounts.Data.TransparentHairHairstyleCount)
                        {
                            int oldIndex = -costume.shape[i] - VanillaCounts.Data.TransparentHairHairstyleCount - 1;
                            if (oldIndex >= savedMap.TransparentHairHairstyleNameMap.Count)
                            {
                                LogWarning(
                                    $"Custom shape index {oldIndex} is out of bounds for character {character.name} ({character.id}). Resetting.");
                                costume.shape[i] = VanillaCounts.Data.ShapeCounts[i];
                                continue;
                            }

                            string oldName = savedMap.TransparentHairHairstyleNameMap[oldIndex];
                            int newIndex = newMap.TransparentHairHairstyleNameMap.IndexOf(oldName);
                            int internalIndex = -newIndex - VanillaCounts.Data.TransparentHairHairstyleCount - 1;
                            if (costume.shape[i] != internalIndex)
                            {
                                LogInfo(
                                    $"Custom shape {oldName} at index {oldIndex} was remapped to index {newIndex} (internal index {internalIndex}) for shape {i} of character {character.name} ({character.id}).");
                                costume.shape[i] = internalIndex;
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            LogError("Failed to remap custom content!");
            LogError(e);
        }
        
        var oldAnims = savedMap.AnimationNameMap;
        var newAnims = newMap.AnimationNameMap;
        for (int i = 0; i < oldAnims.Count; i++)
        {
            if (i < newAnims.Count && oldAnims[i] == newAnims[i])
            {
                continue;
            }
            int index = newAnims.IndexOf(oldAnims[i]);
            if (index == -1)
            {
                LogWarning($"Animation {oldAnims[i]} not found in new map. Resetting to default.");
            }
            else
            {
                LogInfo($"Animation {oldAnims[i]} remapped to {newAnims[index]}.");
            }
            
            for (int j = 1; j < saveData.savedChars.Length; j++)
            {
                if (saveData.savedChars[j] == null)
                {
                    continue;
                }
                for (int k = 0; k < saveData.savedChars[j].moveAttack.Length; k++)
                {
                    if (saveData.savedChars[j].moveAttack[k] == i + 1000000)
                    {
                        saveData.savedChars[j].moveAttack[k] = index == -1 ? DefaultMove('a', k) : index + 1000000;
                    }
                }
                for (int k = 0; k < saveData.savedChars[j].moveCrush.Length; k++)
                {
                    if (saveData.savedChars[j].moveCrush[k] == i + 1000000)
                    {
                        saveData.savedChars[j].moveCrush[k] = index == -1 ? DefaultMove('c', k) : index + 1000000;
                    }
                }
                for (int k = 0; k < saveData.savedChars[j].moveFront.Length; k++)
                {
                    if (saveData.savedChars[j].moveFront[k] == i + 1000000)
                    {
                        saveData.savedChars[j].moveFront[k] = index == -1 ? DefaultMove('f', k) : index + 1000000;
                    }
                }
                for (int k = 0; k < saveData.savedChars[j].moveBack.Length; k++)
                {
                    if (saveData.savedChars[j].moveBack[k] == i + 1000000)
                    {
                        saveData.savedChars[j].moveBack[k] = index == -1 ? DefaultMove('b', k) : index + 1000000;
                    }
                }
                for (int k = 0; k < saveData.savedChars[j].moveGround.Length; k++)
                {
                    if (saveData.savedChars[j].moveGround[k] == i + 1000000)
                    {
                        saveData.savedChars[j].moveGround[k] = index == -1 ? DefaultMove('g', k) : index + 1000000;
                    }
                }
                for (int k = 0; k < saveData.savedChars[j].taunt.Length; k++)
                {
                    if (saveData.savedChars[j].taunt[k] == i + 1000000)
                    {
                        saveData.savedChars[j].taunt[k] = index == -1 ? DefaultMove('t', k) : index + 1000000;
                    }
                }
            }
        }
    }
    
    private static int DefaultMove(char type, int index)
    {
        switch (type)
        {
            case 'a':
                switch (index)
                {
                    case 0:
                        return 0;
                    case 1:
                        return 1002;
                    case 2:
                        return 1061;
                    case 3:
                        return 1124;
                    case 4:
                        return 1111;
                    case 5:
                        return 1306;
                    case 6:
                        return 1304;
                    case 7:
                        return 1302;
                    case 8:
                        return 1350;
                }
                break;
            case 'c':
                switch (index)
                {
                    case 0:
                        return 0;
                    case 1:
                        return 1205;
                    case 2:
                        return 1201;
                    case 3:
                        return 1211;
                    case 4:
                        return 1211;
                    case 5:
                        return 1350;
                    case 6:
                        return 1353;
                    case 7:
                        return 1350;
                    case 8:
                        return 1350;
                }
                break;
            case 'f':
                switch (index)
                {
                    case 0:
                        return 291;
                    case 1:
                        return 278;
                    case 2:
                        return 603;
                    case 3:
                        return 272;
                    case 4:
                        return 257;
                    case 5:
                        return 281;
                    case 6:
                        return 235;
                    case 7:
                        return 253;
                    case 8:
                        return 298;
                    case 9:
                        return 280;
                    case 10:
                        return 299;
                    case 11:
                        return 604;
                    case 12:
                        return 219;
                    case 13:
                        return 299;
                    case 14:
                        return 280;
                    case 15:
                        return 280;
                    case 16:
                        return 512;
                }
                break;
            case 'b':
                switch (index)
                {
                    case 0:
                        return 328;
                    case 1:
                        return 325;
                    case 2:
                        return 320;
                    case 3:
                        return 313;
                    case 4:
                        return 311;
                    case 5:
                        return 337;
                    case 6:
                        return 327;
                    case 7:
                        return 334;
                    case 8:
                        return 334;
                }
                break;
            case 'g':
                switch (index)
                {
                    case 0:
                        return 0;
                    case 1:
                        return 412;
                    case 2:
                        return 417;
                    case 3:
                        return 413;
                    case 4:
                        return 471;
                    case 5:
                        return 464;
                    case 6:
                        return 456;
                }
                break;
            case 't':
                switch (index)
                {
                    case 0:
                        return 15;
                    case 1:
                        return 5;
                    case 2:
                        return 94;
                    case 3:
                        return 15;
                }
                break;
        }
        return 0;
    }

    public static void FixBrokenSaveData()
    {
        LogInfo("Validating save data...");
        var saveData = GLPGLJAJJOP.APPDIBENDAH;
        var numChars = saveData.savedChars.Length;

        for (int index = saveData.savedChars.Length - 1; index >= 1; index--)
        {
            if (saveData.savedChars[index].id != index)
            {
                LogWarning(
                    $"Character index {index} does not match ID {saveData.savedChars[index].id}. Fixing.");
                saveData.savedChars[index].id = index;
            }

            for (int index2 = 1; index2 < saveData.savedChars[index].costume.Length; index2++)
            {
                if (saveData.savedChars[index].costume[index2].charID > numChars)
                {
                    LogWarning(
                        $"Costume index {index2} of character index {index} has character ID {saveData.savedChars[index].costume[index2].charID} out of bounds. Resetting to id.");
                    saveData.savedChars[index].costume[index2].charID = index;
                }
            }
        }
        
        if (saveData.star > numChars)
        {
            LogWarning(
                $"Star index {saveData.star} is out of bounds. Resetting to 1.");
            saveData.star = 1;
        }
        if (saveData.missionClient > numChars)
        {
            LogWarning(
                $"Mission client index {saveData.missionClient} is out of bounds. Resetting to 1.");
            saveData.missionClient = 1;
        }
        if (saveData.missionTarget > numChars)
        {
            LogWarning(
                $"Mission target index {saveData.missionTarget} is out of bounds. Resetting to 1.");
            saveData.missionTarget = 1;
        }
        if (saveData.charUnlock.Length != numChars)
        {
            LogWarning(
                $"Character unlock array length {saveData.charUnlock.Length} does not match number of characters {numChars}. Fixing.");
            int oldLength = saveData.charUnlock.Length;
            Array.Resize(ref saveData.charUnlock, numChars);
            if (oldLength < numChars)
            {
                for (int i = oldLength; i < numChars; i++)
                {
                    saveData.charUnlock[i] = 1;
                }
            }
        }
        for (int index = 1; index < saveData.hiChar.Length; index++)
        {
            int hiChar = saveData.hiChar[index];
            if (hiChar > numChars)
            {
                LogWarning(
                    $"Hi character index {hiChar} is out of bounds. Resetting to 1.");
                saveData.hiChar[index] = 1;
            }
        }
        if (saveData.stockFurniture != null)
        {
            foreach (var stockFurniture in saveData.stockFurniture)
            {
                if (stockFurniture == null)
                {
                    continue;
                }
                if (stockFurniture.owner > numChars)
                {
                    LogWarning(
                        $"Furniture owner index {stockFurniture.owner} is out of bounds. Resetting to 1.");
                    stockFurniture.owner = 1;
                }
            }
        }
        if (saveData.stockWeapons != null)
        {
            foreach (var stockWeapon in saveData.stockWeapons)
            {
                if (stockWeapon == null)
                {
                    continue;
                }
                if (stockWeapon.owner > numChars)
                {
                    LogWarning(
                        $"Weapon owner index {stockWeapon.owner} is out of bounds. Resetting to 1.");
                    stockWeapon.owner = 1;
                }
                if (stockWeapon.holder > numChars)
                {
                    LogWarning(
                        $"Weapon holder index {stockWeapon.holder} is out of bounds. Resetting to 1.");
                    stockWeapon.holder = 1;
                }
            }
        }
        foreach (var character in saveData.savedChars)
        {
            if (character == null)
            {
                continue;
            }
            for (int i = 1; i < character.relation.Length; i++)
            {
                if (character.relation[i] > numChars)
                {
                    LogWarning(
                        $"Relationship character index {character.relation[i]} is out of bounds. Resetting to 1.");
                    character.relation[i] = 1;
                }
            }
            if (character.grudge > numChars)
            {
                LogWarning(
                    $"Grudge character index {character.grudge} is out of bounds. Resetting to 1.");
                character.grudge = 1;
            }
            if (character.team > numChars)
            {
                LogWarning(
                    $"Team character index {character.team} is out of bounds. Resetting to 1.");
                character.team = 1;
            }
        }
        LogInfo("Save data validation complete.");
    }
}