using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
[AddComponentMenu("Audio/Karaoke (Center-Cut) Filter")]
public class KaraokeCenterCut : MonoBehaviour
{
    // Vocal volume (dB): 0 = no removal, -70 = strong removal (like LADSPA)
    [Range(-70f, 0f)]
    [Tooltip("Attenuation applied to the center (mid) channel in dB. 0 dB = no removal.")]
    public float gainDb = -24f;

    // Process audio on Unity's audio thread
    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (channels < 2 || data == null || data.Length < 2)
            return; // needs stereo; otherwise do nothing

        // LADSPA: coef = pow(10, gain*0.05) * 0.5
        float coef = Mathf.Pow(10f, gainDb * 0.05f) * 0.5f;

        int frames = data.Length / channels;
        for (int f = 0; f < frames; f++)
        {
            int i = f * channels;

            float L = data[i];
            float R = data[i + 1];

            // Mid/Side
            float M = L + R;   // center (vocals, typically)
            float S = L - R;   // sides (stereo stuff)

            // Recombine per LADSPA:
            // lout = M*coef + S*0.5
            // rout = M*coef - S*0.5
            float Lout = M * coef + S * 0.5f;
            float Rout = M * coef - S * 0.5f;

            data[i] = Lout;
            data[i + 1] = Rout;
            // For channels > 2 (e.g., surround), we leave the others untouched.
        }
    }
}
