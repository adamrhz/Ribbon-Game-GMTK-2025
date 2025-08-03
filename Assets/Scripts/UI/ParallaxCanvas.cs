using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ribbon
{



    public class ParallaxCanvas : MonoBehaviour
    {
        public RawImage[] parallaxImages;
        //public float parallaxFactor = 0.1f;
        public float[] parallaxFactor;
        public float YClamp = 10;
        public float YMinClamp = -10;
        

        public void LateUpdate()
        {
            int index = 0;
            foreach(RawImage parallaxImage in parallaxImages)
            {
                Vector2 offset = new Vector2(transform.position.x, Mathf.Clamp(transform.position.y, YMinClamp, YClamp)) * parallaxFactor[index]/10f;
                parallaxImage.uvRect = new Rect(offset, parallaxImage.uvRect.size);
                index++;
            }
        }


    }

}