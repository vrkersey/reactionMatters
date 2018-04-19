/* ----------------------------------------------
 * 
 * Flickering Lights * (C)2010 Rouhee - Games
 * 
 * timo.anttila@rouheegames.com
 * 
 * http://www.rouheegames.com
 * 
 * - Provided as is.
 * - You can change and distribute as you like.
 * 
 * ------------------------------------------ */

using UnityEngine;
using System.Collections;

public class flickeringLight1 : MonoBehaviour
{
    // Flickering Styles
    public enum flickerinLightStyles { CampFire = 0, Fluorescent = 1 };
    public flickerinLightStyles flickeringLightStyle = flickerinLightStyles.CampFire;

    // Campfire Methods
    public enum campfireMethods { Intensity = 0, Range = 1, Both = 2 };
    public campfireMethods campfireMethod = campfireMethods.Intensity;

    // Intensity Styles
    public enum campfireIntesityStyles { Sine = 0, Random = 1 };
    public campfireIntesityStyles campfireIntesityStyle = campfireIntesityStyles.Random;

    // Range Styles
    public enum campfireRangeStyles { Sine = 0, Random = 1 };
    public campfireRangeStyles campfireRangeStyle = campfireRangeStyles.Random;

    // Base Intensity Value
    public float CampfireIntensityBaseValue = 0.5f;
    // Intensity Flickering Power
    public float CampfireIntensityFlickerValue = 0.1f;

    // Base Range Value
    public float CampfireRangeBaseValue = 10.0f;
    // Range Flickering Power
    public float CampfireRangeFlickerValue = 2.0f;

    private Light light;

    // Use this for initialization
    void Start () {
        light = GetComponent<Light>();
    }
	
	// Update is called once per frame
	void Update () {

        // If campfire method is Intesity OR Both
        if( campfireMethod == campfireMethods.Intensity || campfireMethod == campfireMethods.Both )
        {
            light.intensity = CampfireIntensityBaseValue + Random.Range( 0.0f, CampfireIntensityFlickerValue );
        }

        // If campfire method is Range OR Both
        if( campfireMethod == campfireMethods.Range || campfireMethod == campfireMethods.Both )
        {
            light.range = CampfireRangeBaseValue + Random.Range( 0.0f, CampfireRangeFlickerValue );
        }
	}
}
