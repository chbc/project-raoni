using DragonBones;
using UnityEngine;

namespace ProjectRaoni
{
    public class DragonBonesAnimationLoader : MonoBehaviour
    {
        [SerializeField] private string _skeletonPath;
        [SerializeField] private string _atlasPath;
        [SerializeField] private string _armatureName = "Armature";
        [SerializeField] private bool _playIdleOnStart = true;

        private void Start()
        {
            UnityFactory.factory.LoadDragonBonesData(_skeletonPath);
            UnityFactory.factory.LoadTextureAtlasData(_atlasPath);
            
            UnityArmatureComponent armatureComponent = UnityFactory.factory.BuildArmatureComponent(_armatureName);

            if (armatureComponent != null)
            {
                UnityEngine.Transform armatureTransform = armatureComponent.transform;
                
                armatureTransform.SetParent(base.transform, false);
                /*
                armatureTransform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                armatureTransform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
                */
                
                if (_playIdleOnStart)
                    armatureComponent.animation.Play("idle");
            }
            else
            {
                Debug.LogError("Erro ao carregar a armadura!", base.gameObject);
            }
        }
    }
}
