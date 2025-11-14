using System.Reflection;
using UnityEngine.Rendering.Universal;

namespace CursedOnion.Extensions
{
    public static class UniversalRenderPipelineAssetExtensions
    {
        public static ScriptableRendererFeature EnableRenderFeature<T>
        (
            this UniversalRenderPipelineAsset asset,
            bool enable,
            string featureName = ""
        ) where T : ScriptableRendererFeature
        {
            var type = asset.GetType();
            var propertyInfo = type.GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);

            if (propertyInfo == null)
                return null;

            var renderDatas = (ScriptableRendererData[])propertyInfo.GetValue(asset);
            if (renderDatas == null) return null;

            foreach (var data in renderDatas)
            {
                foreach (var feature in data.rendererFeatures)
                {
                    if (feature is T && (string.IsNullOrEmpty(featureName) || feature.name == featureName))
                    {
                        feature.SetActive(enable);
                        data.SetDirty();
                        return feature;
                    }
                }
            }

            return null;
        }
    }
}