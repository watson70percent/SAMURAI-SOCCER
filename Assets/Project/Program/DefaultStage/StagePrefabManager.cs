using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagePrefabManager : MonoBehaviour
{
    public Texture flagTexture;
    public Shader flagShader;
    public MeshRenderer[] flagRenderers;
    public Texture[] crowdTextures;
    public Renderer[] crowdRenderers;
    public Material crowdMaterial;
    public Texture groundTexture;
    public Texture groundNormalMap;
    public Renderer groundRenderer;
    public Material groundMaterial;
    public RefereeArea refereeArea;
    public int refereeMaxAng, refereeAreaSize;
    public bool useObstacles;
    public RefereeMove refereeMove;
    public float lookAtSpeed, runningSpeed;
    // Start is called before the first frame update
    void Start()
    {

        //旗
        Material material = new Material(flagShader);
        material.SetTexture("_Texture", flagTexture);

        for (int i = 0; i < 4; i++)
        {
            flagRenderers[i].material = material;
        }

        //観客席
        var _crowdMaterial = new Material(crowdMaterial);
        for (int i = 0; i < 10; i++)
        {
            string texturename = "_Person" + (i + 1).ToString();
            _crowdMaterial.SetTexture(texturename, crowdTextures[i % (crowdTextures.Length)]);
        }

        for (int i = 0; i < crowdRenderers.Length; i++)
        {
            crowdRenderers[i].material = _crowdMaterial;
        }

        //サッカーグラウンド//テクスチャがなければ何もしない
        if (groundTexture != null)
        {
            var _groundMaterial = new Material(groundMaterial);
            _groundMaterial.mainTexture = groundTexture;
            if (groundNormalMap != null) _groundMaterial.SetTexture("_BumpMap", groundNormalMap);
            groundRenderer.material = _groundMaterial;
        }

        //審判の挙動
        refereeArea.SerMaxAngle( refereeMaxAng);
        refereeArea.SerAreaSize(refereeAreaSize);
        refereeArea.useObstacles = useObstacles;
        refereeMove.runningspeed = runningSpeed;
        refereeMove.lookatspeed = lookAtSpeed;
        refereeArea.MeshMaker();
    }

}
