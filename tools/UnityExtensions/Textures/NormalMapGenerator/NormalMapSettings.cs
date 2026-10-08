// Normal map generation math ported from NormalMap-Online (https://github.com/cpetry/NormalMap-Online)
// Copyright (c) 2014 Christian Petry, MIT License.
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software
// and associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;

namespace SilkTouch.Tools.Textures
{
    public enum NormalMapFilterType
    {
        Sobel = 0,
        Scharr = 1,
    }

    [Serializable]
    public class NormalMapSettings
    {
        public float strength = 2.5f;
        public float level = 7f;
        public float blurSharp = 0f;
        public NormalMapFilterType filterType = NormalMapFilterType.Sobel;
        public bool invertHeight = false;
        public bool invertRed = false;
        public bool invertGreen = false;
        public bool seamless = true;
        public bool useAlphaAsHeight = false;
    }
}
