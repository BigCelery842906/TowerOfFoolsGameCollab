using UnityEngine;
using System.Collections.Generic;
using static PG_RoomGenerator;

public class PlatformGenerator : MonoBehaviour
{


    public PLATFORM_GENERATION_METHOD m_platformGenMethod = PLATFORM_GENERATION_METHOD.NONE;

    public void GeneratePlatforms(List<PG_GridMap> room)
    {
        switch (m_platformGenMethod)
        {
            case PLATFORM_GENERATION_METHOD.NONE:
                break;
            case PLATFORM_GENERATION_METHOD.RANDOM:
                break;
            case PLATFORM_GENERATION_METHOD.LAYER:
                break;
            case PLATFORM_GENERATION_METHOD.CRITICAL_PATH:
                break;
        }
    }

    void RandomPlatforms(List<PG_GridMap> room)
    {

    }
    void LayerPlatforms(List<PG_GridMap> room)
    {

    }
    void CriticalPath(List<PG_GridMap> room)
    {

    }
    public enum PLATFORM_GENERATION_METHOD
    {
        NONE,
        RANDOM,
        LAYER,
        CRITICAL_PATH
    }
}
