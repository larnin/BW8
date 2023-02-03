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
class GetUICameraEvent
{
    public Camera camera;

    public GetUICameraEvent()
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

class SetCustomScreenColorEvent
{
    public Color redColor;
    public Color yellowColor;
    public Color greenColor;
    public Color cyanColor;
    public Color blueColor;
    public Color pinkColor;

    public SetCustomScreenColorEvent(Color _redColor, Color _yellowColor, Color _greenColor, Color _cyanColor, Color _blueColor, Color _pinkColor)
    {
        redColor = _redColor;
        yellowColor = _yellowColor;
        greenColor = _greenColor;
        cyanColor = _cyanColor;
        blueColor = _blueColor;
        pinkColor = _pinkColor;
    }
}
