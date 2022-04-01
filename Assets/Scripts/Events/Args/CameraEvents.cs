using UnityEngine;
using System.Collections;

class GetCameraEvent
{
    public Camera camera;

    public GetCameraEvent()
    {
        camera = null;
    }
}

class AddScreenShakeEvent
{
    public ScreenShakeBase screenShake;
    public int resultID;

    public AddScreenShakeEvent(ScreenShakeBase _screenShake)
    {
        screenShake = _screenShake;
    }
}

class StopScreenShakeEvent
{
    public int ID;

    public StopScreenShakeEvent(int id)
    {
        ID = id;
    }
}

class StopAllScreenShakeEvent { }

class SetScreenColorEvent
{
    public Color darkColor;
    public Color lightColor;

    public SetScreenColorEvent(Color _darkColor, Color _lightColor)
    {
        darkColor = _darkColor;
        lightColor = _lightColor;
    }
}
