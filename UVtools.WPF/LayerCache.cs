﻿/*
 *                     GNU AFFERO GENERAL PUBLIC LICENSE
 *                       Version 3, 19 November 2007
 *  Copyright (C) 2007 Free Software Foundation, Inc. <https://fsf.org/>
 *  Everyone is permitted to copy and distribute verbatim copies
 *  of this license document, but changing it is not allowed.
 */

using System;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using UVtools.Core;

namespace UVtools.WPF
{
    public sealed class LayerCache
    {
        private Layer _layer;
        private Mat _image;
        private Array _layerHierarchyJagged;
        private VectorOfVectorOfPoint _layerContours;
        private Mat _layerHierarchy;

        public bool IsCached => !ReferenceEquals(_layer, null);

        public Layer Layer
        {
            get => _layer;
            set
            {
                Clear();
                _layer = value;
                _image = _layer.LayerMat;
                ImageBgr = new Mat();
                CvInvoke.CvtColor(_image, ImageBgr, ColorConversion.Gray2Bgr);
            }
        }

        public Mat Image => _image;

        public Mat ImageBgr { get; set; }

        public VectorOfVectorOfPoint LayerContours
        {
            get
            {
                if (_layerContours is null) CacheContours();
                return _layerContours;
            }
            private set => _layerContours = value;
        }

        public Mat LayerHierarchy
        {
            get
            {
                if (_layerHierarchy is null) CacheContours();
                return _layerHierarchy;
            }
            private set => _layerHierarchy = value;
        }

        public Array LayerHierarchyJagged
        {
            get
            {
                if(_layerHierarchyJagged is null) CacheContours();
                return _layerHierarchyJagged;
            }
            private set => _layerHierarchyJagged = value;
        }

        public void CacheContours(bool refresh = false)
        {
            if(refresh) Clear();
            if (!ReferenceEquals(_layerContours, null)) return;
            _layerContours = new VectorOfVectorOfPoint();
            _layerHierarchy = new Mat();
            CvInvoke.FindContours(Image, _layerContours, _layerHierarchy, RetrType.Ccomp,
                ChainApproxMethod.ChainApproxSimple);
            _layerHierarchyJagged = _layerHierarchy.GetData();
        }
        

        /// <summary>
        /// Clears the cache
        /// </summary>
        public void Clear()
        {
            _layer = null;
            _image?.Dispose();
            ImageBgr?.Dispose();
            _layerContours?.Dispose();
            _layerContours = null;
            _layerHierarchy?.Dispose();
            _layerHierarchy = null;
            _layerHierarchyJagged = null;
        }
    }
}
