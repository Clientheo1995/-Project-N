using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
using System;

public static class SpineManager
{
    public static void SetSkeleton(SkeletonAnimation spine, int index)
    {
        spine.skeletonDataAsset = Resources.Load<SkeletonDataAsset>($"_GlenSpine/type{index % 10 + 1}_SkeletonData");
        spine.Initialize(true);
        spine.AnimationState.SetAnimation(0, "2walk", true);
    }

    public static void SetSkeletonUI(SkeletonGraphic spine, int index)
    {
        SkeletonDataAsset temp= Resources.Load<SkeletonDataAsset>($"_GlenSpine/type{index % 10 + 1}_SkeletonData");
        spine.skeletonDataAsset = temp;
        spine.Initialize(true);
        spine.AnimationState.SetAnimation(0, "0idle", true);
    }

    public static void SetSlots(List<PartData> partList, Skeleton skeleton)
    {
        for (int i = 0; i < partList.Count; ++i)
        {
            if (partList[i].parts_type == SpineParts.background) continue;

            string partsName = partList[i].parts_type.ToString();
            if (skeleton.FindSlot(partsName) != null)
            {
                if (skeleton.GetAttachment(partsName, partList[i].parts_spine) == null)
                {
                    Debug.Log($"DIDNT EXIST {partList[i].parts_spine}");
                }
                else
                {
                    skeleton.SetAttachment(partsName, partList[i].parts_spine);
                    Debug.Log($"스파인 파츠 재생:  {partList[i].parts_spine}");
                }
            }

            if (skeleton.FindSlot($"{partList[i].parts_spine}_color") != null)
            {
                if (skeleton.GetAttachment($"{partsName}_color", $"{partList[i].parts_spine}_color") == null)
                {
                    Debug.Log($"NO EXIST {partList[i].parts_spine}");
                }
                else
                {
                    skeleton.SetAttachment($"{partsName}_color", $"{partList[i].parts_spine}_color");
                    Debug.Log($"스파인 컬러 재생:  {partList[i].parts_spine}");
                }
            }
        }
    }

    public static void SetColor(Color color, Skeleton skeleton)
    {
        for (int i = 0; i < (int)SpinePartsColor.Length; ++i)
        {
            Slot slot = skeleton.FindSlot(((SpinePartsColor)i).ToString());
            slot?.SetColor(color);
        }
    }
}
