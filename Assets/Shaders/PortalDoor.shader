Shader "Portal/PortalDoor"
{
    Properties
    {

    }
    SubShader
    {
        ColorMask 0
        Zwrite Off

        Stencil{
            Ref 1
            Pass replace
        }
        Pass{

        }
    }

}
