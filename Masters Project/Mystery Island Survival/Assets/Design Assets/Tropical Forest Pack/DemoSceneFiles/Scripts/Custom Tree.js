var Wind : Vector4 = Vector4(0.85,0.075,0.4,0.5);
var WindFrequency = 0.75;
 
function Start ()
{
    Shader.SetGlobalColor("_Wind", Wind);
}
function Update () {
    // simple wind animation
    var WindRGBA : Color = Wind *  ( (Mathf.Sin(Time.realtimeSinceStartup * WindFrequency)));
    WindRGBA.a = Wind.w;
    Shader.SetGlobalColor("_Wind", WindRGBA);
}