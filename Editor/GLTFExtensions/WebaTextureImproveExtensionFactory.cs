﻿using System;
using System.Collections.Generic;
using GLTF.Schema;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

public class WebaTextureImproveExtensionFactory : WebaExtensionFactory
{
    public override string GetExtensionName() { return "WebaTextureImprove"; }

    public override void Serialize(ExporterEntry entry, Dictionary<string, Extension> extensions, UnityEngine.Object component = null, object options = null)
    {
        var texture = component as Texture2D;

        var extension = new WebaTextureImproveExtension();

        extension.anisotropic = texture.anisoLevel;
        // for 3d modle's textures, it always be true
        extension.isImageCanRelease = texture.isReadable;

        if (ExporterSettings.NormalTexture.pngFormat == EPNGTextureFormat.RGBA4444)
        {
            extension.textureType = 32819;
        } else
        {
            extension.textureType = 5121;
        }

        TextureImporter im = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture)) as TextureImporter;

        if (im)
        {
            extension.useMipmaps = im.mipmapEnabled;
        }

        AddExtension(extensions, extension);
    }

    public override Extension Deserialize(GLTFRoot root, JProperty extensionToken)
    {
        var extension = new WebaTextureImproveExtension();

        if (extensionToken == null)
        {
            return null;
        }

        extension.isImageCanRelease = (bool)extensionToken.Value["isImageCanRelease"];
        extension.anisotropic = (int)extensionToken.Value["anisotropic"];
        extension.textureType = (int)extensionToken.Value["textureType"];

        return extension;
    }
}