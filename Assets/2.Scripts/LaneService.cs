using UnityEngine;
using System.Collections.Generic;

static class LaneLayers
{
    public static readonly int[] laneLayer = {
        LayerMask.NameToLayer("Lane0"),
        LayerMask.NameToLayer("Lane1"),
        LayerMask.NameToLayer("Lane2"),
    };

    public static readonly int[] groundLayer = {
        LayerMask.NameToLayer("GroundLane0"),
        LayerMask.NameToLayer("GroundLane1"),
        LayerMask.NameToLayer("GroundLane2"),
    };
}

public static class LaneService
{
    public static float[] laneY = { 0.25f, 0.0f, -0.25f };
    static readonly List<MovementComponent>[] lanes = { new(), new(), new() };

    public static int LaneCount => laneY.Length;

    public static void LaneRegister(int lane, MovementComponent mc)
    {
        if ((uint)lane >= lanes.Length) return;
        if (!lanes[lane].Contains(mc))
            lanes[lane].Add(mc);
    }

    public static void LaneUnregister(int lane, MovementComponent mc)
    {
        if ((uint)lane >= lanes.Length) return;
        if (lanes[lane].Contains(mc))
            lanes[lane].Remove(mc);
    }

    // 같은 Lane에서 앞에 있는 가장 가까운 좀비 하나 반환
    public static MovementComponent GetMovementComponentInFrontLane(int lane, MovementComponent mc)
    {
        if ((uint)lane >= lanes.Length) return null;

        // x 포지션 위치를 통해서 앞인지 뒤인지 체크
        float x = mc.transform.position.x;
        MovementComponent front = null;
        float frontDx = float.MaxValue;
        foreach (var m in lanes[lane])
        {
            // 본인이면 건너뛰기
            if (mc == m) continue;
            float dx = x - m.transform.position.x;
            if (dx > 0f && dx < frontDx)
            {
                frontDx = dx;
                front = m;
            }
        }
        return front;
    }

    public static int GetRandomLane()
    {
        return Random.Range(0, LaneCount);
    }
}