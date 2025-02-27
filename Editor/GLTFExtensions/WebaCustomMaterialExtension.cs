﻿using GLTF.Schema;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WebaCustomMaterialExtension : Extension
{
    // global
    public List<string> matScripts = null;

    // material
    public bool isComponent = false;
    public UnityEngine.Material material = null;

    public string className = "";
    public string unityMaterialName = "";

    public int renderOrder = 0;
    public bool cloneForInst = false;
    public bool transparent = false;

    public WebaMaterialCustomOption[] customOptions = { };

    public WebaMaterialUniformTexture[] uniformsTexture = { };
    public WebaMaterialUniformCubeTexture[] uniformsCubeTexture = { };
    public WebaMaterialUniformFloat[] uniformsFloat = { };
    public WebaMaterialUniformFloatVec2[] uniformsFloatVec2 = { };
    public WebaMaterialUniformFloatVec3[] uniformsFloatVec3 = { };
    public WebaMaterialUniformFloatVec4[] uniformsFloatVec4 = { };
    public WebaMaterialUniformColor[] uniformsColor = { };
    public WebaMaterialUniformFloatMat2[] uniformsFloatMat2 = { };
    public WebaMaterialUniformFloatMat3[] uniformsFloatMat3 = { };
    public WebaMaterialUniformFloatMat4[] uniformsFloatMat4 = { };
    public WebaMaterialUniformInt[] uniformsInt = { };
    public WebaMaterialUniformIntVec2[] uniformsIntVec2 = { };
    public WebaMaterialUniformIntVec3[] uniformsIntVec3 = { };
    public WebaMaterialUniformIntVec4[] uniformsIntVec4 = { };

    public JProperty Serialize()
    {
        if (matScripts != null)
        {
            var array = new JArray();
            foreach (var script in matScripts)
            {
                array.Add(new JObject(new JProperty("uri", script)));
            }
            var value = new JObject(new JProperty("scripts", array));
            return new JProperty(ExtensionManager.GetExtensionName(typeof(WebaCustomMaterialExtensionFactory)), value);
        }

        var res = new JObject();
        res.Add("className", className);
        res.Add("renderOrder", renderOrder);
        res.Add("cloneForInst", cloneForInst);

        var options = new JObject();
        res.Add("options", options);
        foreach(WebaMaterialCustomOption option in customOptions)
        {
            options.Add(option.name, option.value);
        }

        var uniforms = new JObject();
        res.Add("uniforms", uniforms);
        WriteUiforms(uniforms, uniformsTexture);
        WriteUiforms(uniforms, uniformsCubeTexture);
        WriteUiforms(uniforms, uniformsFloat);
        WriteUiforms(uniforms, uniformsFloatVec2);
        WriteUiforms(uniforms, uniformsFloatVec3);
        WriteUiforms(uniforms, uniformsFloatVec4);
        WriteUiforms(uniforms, uniformsColor);
        WriteUiforms(uniforms, uniformsFloatMat2);
        WriteUiforms(uniforms, uniformsFloatMat3);
        WriteUiforms(uniforms, uniformsFloatMat4);
        WriteUiforms(uniforms, uniformsInt);
        WriteUiforms(uniforms, uniformsIntVec2);
        WriteUiforms(uniforms, uniformsIntVec3);
        WriteUiforms(uniforms, uniformsIntVec4);

        return new JProperty(ExtensionManager.GetExtensionName(typeof(WebaCustomMaterialExtensionFactory)), res);
    }

    private void WriteUiforms<TValue>(JObject container, WebaMaterialUniform<TValue>[] uniforms)
    {
        foreach (WebaMaterialUniform<TValue> uniform in uniforms)
        {
            var value = new JObject();
            container.Add(uniform.name, value);
            value.Add(new JProperty("type", (int)uniform.type));

            switch (uniform.type)
            {
                case (EWebaMaterialUniformType.FLOAT):
                case (EWebaMaterialUniformType.INT):
                    value.Add(new JProperty("value", uniform.value));
                    break;
                case (EWebaMaterialUniformType.SAMPLER_2D):
                    value.Add(new JProperty("value", new JObject(new JProperty("index", (uniform as WebaMaterialUniformTexture).id.Id))));
                    break;
                // todo: support cubemap
                case (EWebaMaterialUniformType.SAMPLER_CUBE):
                    break;
                case (EWebaMaterialUniformType.INT_VEC2):
                    var iv2 = (uniform as WebaMaterialUniformIntVec2).value;
                    value.Add(new JProperty("value", new JArray { (int)iv2.x, (int)iv2.y }));
                    break;
                case (EWebaMaterialUniformType.INT_VEC3):
                    var iv3 = (uniform as WebaMaterialUniformIntVec3).value;
                    value.Add(new JProperty("value", new JArray { (int)iv3.x, (int)iv3.y, (int)iv3.z }));
                    break;
                case (EWebaMaterialUniformType.INT_VEC4):
                    var iv4 = (uniform as WebaMaterialUniformIntVec4).value;
                    value.Add(new JProperty("value", new JArray { (int)iv4.x, (int)iv4.y, (int)iv4.z, (int)iv4.w }));
                    break;
                case (EWebaMaterialUniformType.FLOAT_VEC2):
                    var fv2 = (uniform as WebaMaterialUniformFloatVec2).value;
                    value.Add(new JProperty("value", new JArray { fv2.x, fv2.y }));
                    break;
                case (EWebaMaterialUniformType.FLOAT_VEC3):
                    var fv3 = (uniform as WebaMaterialUniformFloatVec3).value;
                    value.Add(new JProperty("value", new JArray { fv3.x, fv3.y, fv3.z }));
                    break;
                case (EWebaMaterialUniformType.FLOAT_VEC4):
                    if (uniform.GetType() == typeof(WebaMaterialUniformColor))
                    {
                        value.Add(new JProperty("isColor", true));
                        var fv4 = Utils.ExportColorVec4((uniform as WebaMaterialUniformColor).value);
                        value.Add(new JProperty("value", new JArray { fv4.x, fv4.y, fv4.z, fv4.w }));
                    }
                    else
                    {
                        var fv4 = (uniform as WebaMaterialUniformFloatVec4).value;
                        value.Add(new JProperty("value", new JArray { fv4.x, fv4.y, fv4.z, fv4.w }));
                    }
                    break;
                case (EWebaMaterialUniformType.FLOAT_MAT2):
                    var fm2 = (uniform as WebaMaterialUniformFloatMat2).value;
                    value.Add(new JProperty("value", new JArray { fm2.m00, fm2.m01, fm2.m10, fm2.m11 }));
                    break;
                case (EWebaMaterialUniformType.FLOAT_MAT3):
                    var fm3 = (uniform as WebaMaterialUniformFloatMat3).value;
                    value.Add(new JProperty("value", new JArray {
                        fm3.m00, fm3.m01, fm3.m02,
                        fm3.m10, fm3.m11, fm3.m12,
                        fm3.m20, fm3.m21, fm3.m22
                    }));
                    break;
                case (EWebaMaterialUniformType.FLOAT_MAT4):
                    var fm4 = (uniform as WebaMaterialUniformFloatMat4).value;
                    value.Add(new JProperty("value", new JArray {
                        fm4.m00, fm4.m01, fm4.m02, fm4.m03,
                        fm4.m10, fm4.m11, fm4.m12, fm4.m13,
                        fm4.m20, fm4.m21, fm4.m22, fm4.m23,
                        fm4.m30, fm4.m31, fm4.m32, fm4.m33
                    }));
                    break;
                default:
                    break;
            }
        }
    }

    public void AddComponentToGO(GameObject go)
    {
        var mat = go.AddComponent<WebaCustomMaterial>();

        mat.unityMaterialName = material.name;
        mat.className = className;
        mat.cloneForInst = cloneForInst;
        mat.renderOrder = renderOrder;
        mat.transparent = transparent;
        mat.customOptions = customOptions;
        mat.uniformsColor = uniformsColor;
        mat.uniformsTexture = uniformsTexture;
        mat.uniformsCubeTexture = uniformsCubeTexture;
        mat.uniformsFloat = uniformsFloat;
        mat.uniformsFloatVec2 = uniformsFloatVec2;
        mat.uniformsFloatVec3 = uniformsFloatVec3;
        mat.uniformsFloatVec4 = uniformsFloatVec4;
        mat.uniformsFloatMat2 = uniformsFloatMat2;
        mat.uniformsFloatMat3 = uniformsFloatMat3;
        mat.uniformsFloatMat4 = uniformsFloatMat4;
        mat.uniformsInt = uniformsInt;
        mat.uniformsIntVec2 = uniformsIntVec2;
        mat.uniformsIntVec3 = uniformsIntVec3;
        mat.uniformsIntVec4 = uniformsIntVec4;
    }
}